using System;
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
            Utils.WriteLine("How are you getting caught if you are both the same team!? Ignoring");
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
            Utils.WriteLine("DeathType not found for " + seekerPlayer.Info);
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
    public override void OnWaiting(){
        //Wait X amount of time
        //then move to inProgress
        float waitRemaining = (Time.time - gameStartTime) + SharedSettings.settingsToShare.PreroundTime;
        Utils.WriteLine(String.Format("Waiting for {0:0.##} seconds", waitRemaining), MessageType.Info);
        preroundTimer = Utils.StartCoroutine(PreRoundTimer(waitRemaining));
        //Utils.WaitFor(waitRemaining, () => GameManager.state = GameState.InProgress);
    }

    public IEnumerator PreRoundTimer(float time){
        string formattedString = "Seekers selected in {0:0} seconds";
        
        NotificationData preroundNotification = new(NotificationTarget.All, String.Format(formattedString, time), 1f, false);
        foreach(var notifiable in NotificationManager.SharedInstance._notifiableElements){
            Utils.WriteLine(notifiable.ToString());
        }
        NotificationManager.SharedInstance.PostNotification(preroundNotification, true);
        while (time > 0){
            preroundNotification.displayMessage = String.Format(formattedString, time);
            Utils.WriteLine(String.Format(formattedString, time), MessageType.Info);
            NotificationManager.SharedInstance.PostNotification(preroundNotification);
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
        endGameCheck = Utils.RunWhen(() => PlayerManager.hiders.Count == 0, () => { GameManager.state = GameState.Ending; });
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