using System.Collections;
using System.Linq;
using OWML.Common;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Messages;
using UnityEngine;

namespace QSBGameModes.GameManagement.GameTypes;

public class Tag : GameBase
{
    private ImmunePlayer immuneFromPlayer;

    public Tag()
    {
        catcheeNotification = new(NotificationTarget.All, "NOT IT", 0, false);
        catcherNotification = new(NotificationTarget.All, "YOU ARE IT", 0, false);
        spectatorNotification = new(NotificationTarget.All, "SPECTATOR", 0, false);
    }

    public override void OnCatch(GameModeInfo seekerPlayer)
    {
        //We only run if we are a hider and we hit a seeker
        if (PlayerManager.PlayerInfos[QSBPlayerManager.LocalPlayer].State != GameManagement.PlayerManagement.PlayerState.Hiding)
            return;

        if (PlayerManager.PlayerInfos[QSBPlayerManager.LocalPlayer].State == seekerPlayer.State)
        {
            Utils.WriteLine("How are you getting caught if you are both the same team!? Ignoring");
            return;
        }
        new RoleChangeMessage(QSBPlayerManager.LocalPlayer, GameManagement.PlayerManagement.PlayerState.Seeking).Send();


        if (SharedSettings.settingsToShare.KillHidersOnCatch)
        {
            Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Instant);
            Utils.StartCoroutine(AutoRespawnWithDelay(5f));
            if (PlayerManager.PlayerDeathTypes.ContainsKey(seekerPlayer.Info))
            {
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
    }

    IEnumerator ImmunityCooldown(GameModeInfo seekerInfo, float delay)
    {
        immuneFromPlayer = new ImmunePlayer(seekerInfo, delay); //we are now immune from getting caught from this person
        while (immuneFromPlayer.immunityTimeLeft >= 0)
        {
            yield return new WaitForEndOfFrame();
            immuneFromPlayer.immunityTimeLeft -= Time.deltaTime;
        }
        immuneFromPlayer = null; //We are no longer immune from getting caught by this person
    }

    public class ImmunePlayer
    {
        public ImmunePlayer(GameModeInfo info, float ImmunityTime)
        {
            this.info = info;
            this.immunityTimeLeft = ImmunityTime;
        }
        public GameModeInfo info { get; set; }
        public float immunityTimeLeft { get; set; }
    }

    public override void OnStarting()
    {
        base.OnStarting();


        //TODO :: Make sure all player's eyes are open
        Utils.RunWhen(() => PlayerManager.PlayerInfos.Values.All(info => info.Info.SuitedUp), () => GameManager.state = GameState.Waiting);
    }

    private Coroutine preroundTimer;

    public override void OnWaiting()
    {
        //Wait X amount of time
        //then move to inProgress
        float waitRemaining = (System.DateTime.Now.Millisecond / 1000f - stateTime) + SharedSettings.settingsToShare.PreroundTime;
        Utils.WriteLine($"Waiting for {waitRemaining:0.00} seconds", MessageType.Info);

        if (preroundTimer != null)
            Utils.StopCoroutine(preroundTimer);

        preroundTimer = Utils.StartCoroutine(PreRoundTimer(waitRemaining));
        //Utils.WaitFor(waitRemaining, () => GameManager.state = GameState.InProgress);
    }

    public IEnumerator PreRoundTimer(float time)
    {
        string formattedString = "Seekers selected in ";

        var preroundNotification = new NotificationData(NotificationTarget.All, formattedString + $"{time:0.0}");
        NotificationManager.SharedInstance.PostNotification(preroundNotification, true);

        //Get all the strings for the notification
        var datas = Utils.getNotificationDisplayData(preroundNotification);

        while (time > 0)
        {
            datas.ForEach((display) => display.TextDisplay.text = formattedString + $"{time:0.0}");
            if (time % 1 <= Time.deltaTime)//Debug only on the whole numbers, dont wanna spam too much
                Utils.WriteLine(formattedString + $"{time:0}", MessageType.Info);
            yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
        }
        NotificationManager.SharedInstance.UnpinNotification(preroundNotification);
        GameManager.state = GameState.InProgress;
    }

    public override void OnInProgress()
    {
        base.OnInProgress();
        GameManager.SelectRoles();
        //We do not need an End Game Check I guess?
    }

    public override void OnStopped()
    {
        base.OnStopped();
        Utils.StopCoroutine(preroundTimer);
    }
}
