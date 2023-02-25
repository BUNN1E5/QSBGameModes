using System.Collections;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Messages;
using UnityEngine;

namespace QSBGameModes.GameManagement.GameTypes;

public class HideAndSeek : GameBase {
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
            //TODO :: DECIDE ON IF WE WANT TO KILL THE PLAYER OR NOT
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
}