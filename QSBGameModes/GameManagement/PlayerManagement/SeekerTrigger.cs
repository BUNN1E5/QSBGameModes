using QSB.Player;
using QSB.Messaging;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWML.Common;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Messages;

namespace QSBGameModes{
    public class SeekerTrigger : MonoBehaviour{
        private OWTriggerVolume triggerVolume;
        public PlayerInfo seekerInfo;
        
        public void Start(){

            if (GetComponent<CapsuleShape>() == null){
                CapsuleShape shapeTrigger = gameObject.AddComponent<CapsuleShape>();
                shapeTrigger.radius = 0.5f;
                shapeTrigger.height = 2f;
                shapeTrigger.SetCollisionMode(Shape.CollisionMode.Volume);
            }
            triggerVolume = gameObject.GetAddComponent<OWTriggerVolume>();
            triggerVolume.OnEntry += ShapeTrigger_OnEntry;
        }

        private void ShapeTrigger_OnEntry(GameObject hitObj){
            //Only kill the player if they are hiding
            if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != GameManagement.PlayerManagement.PlayerState.Hiding)
                return;

            new RoleChangeMessage(QSBPlayerManager.LocalPlayer, GameManagement.PlayerManagement.PlayerState.Seeking).Send();
            /*
            if (hitObj.CompareTag("PlayerDetector"))
            {
                Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Instant);
                StartCoroutine(AutoRespawnWithDelay(5f));
                //TODO :: DECIDE ON IF WE WANT TO KILL THE PLAYER OR NOT
                if (PlayerManager.PlayerDeathTypes.ContainsKey(seekerInfo)){
                    Locator.GetDeathManager().KillPlayer(PlayerManager.PlayerDeathTypes[seekerInfo]);
                    return;
                }
                
                Locator.GetDeathManager().KillPlayer(DeathType.Impact);
                Utils.WriteLine("DeathType not found for " + seekerInfo);
            }
            */
        }

        private void OnDestroy(){
            triggerVolume.OnEntry -= ShapeTrigger_OnEntry;
        }

        IEnumerator AutoRespawnWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            new LocationRespawnMessage(QSBPlayerManager.LocalPlayerId, SpawnLocation.TimberHearth).Send();
            new RoleChangeMessage(seekerInfo, GameManagement.PlayerManagement.PlayerState.Hiding).Send();
        }

    }
}