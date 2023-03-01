using System;
using System.Linq;
using System.Collections;
using OWML.Common;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Messages;
using UnityEngine;
using UnityEngine.Playables;

namespace QSBGameModes.GameManagement.GameTypes;

public class HideAndSeek : GameBase{
    public HideAndSeek(){ 
        catcheeNotification = new(NotificationTarget.All, "HIDER", 0, false);
        catcherNotification = new(NotificationTarget.All, "SEEKER", 0, false);
        spectatorNotification = new(NotificationTarget.All, "SPECTATOR",0, false);
    }
    public override PlayerManagement.PlayerState StateOnJoinLate() => PlayerManagement.PlayerState.Seeking;
    public override PlayerManagement.PlayerState StateOnJoinEarly()  => PlayerManagement.PlayerState.Hiding;
    public override void OnCatch(GameModeInfo seekerPlayer){
        //We only run if we are a hider and we hit a seeker
        if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != GameManagement.PlayerManagement.PlayerState.Hiding)
            return;

        if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State == seekerPlayer.State){
            Utils.WriteLine("HideAndSeek :: How are you getting caught if you are both the same team!? Ignoring");
            return;
        }
        new RoleChangeMessage(QSBPlayerManager.LocalPlayer, GameManagement.PlayerManagement.PlayerState.Seeking).Send();

        if (SharedSettings.settingsToShare.KillHidersOnCatch){
            Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Instant);
            Utils.StartCoroutine(AutoRespawnWithDelay(5f));
            if (PlayerManager.PlayerDeathTypes.ContainsKey(seekerPlayer.Info)){
                Locator.GetDeathManager().KillPlayer(PlayerManager.PlayerDeathTypes[seekerPlayer.Info]);
                return;
            }
            
            Locator.GetDeathManager().KillPlayer(DeathType.Impact);
            Utils.WriteLine("HideAndSeek :: DeathType not found for " + seekerPlayer.Info);
        }
    }
    
    IEnumerator AutoRespawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        new LocationRespawnMessage(QSBPlayerManager.LocalPlayerId, SpawnLocation.TimberHearth).Send();
        //new RoleChangeMessage(seekerInfo.Info, GameManagement.PlayerManagement.PlayerState.Hiding).Send();
    }

    public override void OnStarting(){
        base.OnStarting();
        GameManager.state = GameState.Waiting;
    }

    private Coroutine preroundTimer;
    private int runCount = 0;
    
    public override void OnWaiting(){
        //Wait X amount of time
        //then move to inProgress
        float waitRemaining = (System.DateTime.Now.Millisecond / 1000f - stateTime) + SharedSettings.settingsToShare.PreroundTime;
        Utils.WriteLine($"Waiting for {waitRemaining:0.##} seconds", MessageType.Info);
        
        Utils.WriteLine("Ran OnWaiting " + ++runCount + " times", MessageType.Error);
        if(preroundTimer == null)
            preroundTimer = Utils.StartCoroutine(PreRoundTimer(waitRemaining));
        //Utils.WaitFor(waitRemaining, () => GameManager.state = GameState.InProgress);
    }

    public IEnumerator PreRoundTimer(float time){
        string formattedString = "Seekers selected in";
        
        NotificationData preroundNotification = new(NotificationTarget.All, formattedString, 1f, false);
        NotificationData timeNotification = new(NotificationTarget.All, "" + time, 1f, false);
        NotificationManager.SharedInstance.PostNotification(preroundNotification, true);
        NotificationManager.SharedInstance.PostNotification(timeNotification);
        while (time > 0){
            timeNotification.displayMessage = "" + time;
            Utils.WriteLine("Seekers selected in " + time, MessageType.Info);
            NotificationManager.SharedInstance.RepostNotifcation(timeNotification);
            yield return new WaitForSeconds(1);
            time -= 1;
        }
        NotificationManager.SharedInstance.UnpinNotification(preroundNotification);
        GameManager.state = GameState.InProgress;
    }

    private Coroutine endGameCheck;
    public override void OnInProgress(){
        base.OnInProgress();
        GameManager.SelectRoles();
        endGameCheck = Utils.RunWhen(() => PlayerManager.hiders.Count == 0 
                                           && PlayerManager.playerInfo.Values.All(info=>info.State != PlayerManagement.PlayerState.Ready),
            () => { GameManager.state = GameState.InProgress; });
    }

    public override void OnEnding(){
        //TODO :: DO SOMETHING HERE
        base.OnEnding();
        
    }

    public override void OnStopped(){
        base.OnStopped();
        Utils.StopCoroutine(endGameCheck);
        Utils.StopCoroutine(preroundTimer);
    }
}