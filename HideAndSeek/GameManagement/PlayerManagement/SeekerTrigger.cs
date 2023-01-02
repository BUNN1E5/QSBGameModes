using HideAndSeek.HidersAndSeekersSelection;
using QSB.Player;
using QSB.Messaging;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HideAndSeek.Messages;
using OWML.Common;

namespace HideAndSeek{
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
            if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != PlayerState.Hiding)
                return;
            
            if (hitObj.CompareTag("PlayerDetector"))
            {
                Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Instant);
                StartCoroutine(AutoRespawnWithDelay(5f));
                
                if (PlayerManager.PlayerDeathTypes.ContainsKey(seekerInfo)){
                    Locator.GetDeathManager().KillPlayer(PlayerManager.PlayerDeathTypes[seekerInfo]);
                }
                
                Locator.GetDeathManager().KillPlayer(DeathType.Impact);
                Utils.WriteLine("DeathType not found for " + seekerInfo);
                
                
            }
        }

        private void OnDestroy(){
            triggerVolume.OnEntry -= ShapeTrigger_OnEntry;
        }

        IEnumerator AutoRespawnWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            new LocationRespawnMessage(QSBPlayerManager.LocalPlayerId, SpawnLocation.TimberHearth).Send();
            new RoleChangeMessage(QSBPlayerManager.LocalPlayerId, PlayerState.Seeking).Send();
            new RoleChangeMessage(seekerInfo.PlayerId, PlayerState.Hiding).Send();
        }

    }
}