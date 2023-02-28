using System.Collections;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Messages;
using UnityEngine;

namespace QSBGameModes.GameManagement.GameTypes;

public class Tag : GameBase{
    private ImmunePlayer immuneFromPlayer;
    private float immunityCooldown = 30f; //30 seconds

    public override void OnCatch(GameModeInfo seekerPlayer){
        //We only run if we are a not "IT" and we got hit by someone who is "IT"
        if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != GameManagement.PlayerManagement.PlayerState.Hiding)
            return;
        
        //This is a case that can actually happen if there are multiple IT
        if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State == seekerPlayer.State){
            Utils.WriteLine("Tag :: How are you getting caught if you are both the same team!? Ignoring");
            return;
        }
        
        //TODO :: Implement Immunity
        //if this function is running that means the local player got caught
        //which means in order for the other player to know that they caught someone a message must be sent

        new RoleChangeMessage(QSBPlayerManager.LocalPlayer, GameManagement.PlayerManagement.PlayerState.Seeking).Send();
        new RoleChangeMessage(seekerPlayer.Info, GameManagement.PlayerManagement.PlayerState.Hiding).Send();
        ImmunityCooldown(seekerPlayer, immunityCooldown); //This is handled locally

        if (SharedSettings.settingsToShare.KillHidersOnCatch){
            Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Instant);
            Utils.StartCoroutine(AutoRespawnWithDelay(5f));
            
            if (PlayerManager.PlayerDeathTypes.ContainsKey(seekerPlayer.Info)){
                Locator.GetDeathManager().KillPlayer(PlayerManager.PlayerDeathTypes[seekerPlayer.Info]);
                return;
            }
            
            Locator.GetDeathManager().KillPlayer(DeathType.Impact);
            Utils.WriteLine("Tag :: DeathType not found for " + seekerPlayer.Info);
        }
    }

    IEnumerator AutoRespawnWithDelay(float delay){
        yield return new WaitForSeconds(delay);
        new LocationRespawnMessage(QSBPlayerManager.LocalPlayerId, SpawnLocation.TimberHearth).Send();
    }
    
    IEnumerator ImmunityCooldown(GameModeInfo seekerInfo, float delay){
        immuneFromPlayer = new ImmunePlayer(seekerInfo, delay); //we are now immune from getting caught from this person
        while (immuneFromPlayer.immunityTimeLeft >= 0){
            yield return new WaitForEndOfFrame();
            immuneFromPlayer.immunityTimeLeft -= Time.deltaTime;
        }
        immuneFromPlayer = null; //We are no longer immune from getting caught by this person
    }
    
    public class ImmunePlayer{
        public ImmunePlayer(GameModeInfo info, float ImmunityTime){
            this.info = info;
            this.immunityTimeLeft = ImmunityTime;
        }
        public GameModeInfo info{ get; set; }
        public float immunityTimeLeft{ get; set; }
    }
}