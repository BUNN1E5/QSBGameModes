using HideAndSeek.HidersAndSeekersSelection;
using QSB.Player;
using QSB.Messaging;
using UnityEngine;
using System.Collections;
using HideAndSeek.ArbitraryLocaltionRespawnMessage;

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
            if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != PlayerState.Hiding)
                return;
            
            if (hitObj.CompareTag("PlayerDetector"))
            {
                //TODO :: ADD CUSTOM DEATHTYPES
                Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);
                Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Crushed);
                StartCoroutine(AutoRespawnWithDelay(5f));
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