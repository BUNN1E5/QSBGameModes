using HideAndSeek.HidersAndSeekersSelection;
using QSB.Player;
using QSB.Messaging;
using UnityEngine;

namespace HideAndSeek{
    public class SeekerTrigger : MonoBehaviour{

        private OWTriggerVolume triggerVolume;

        public void Start(){
            CapsuleShape shapeTrigger = gameObject.AddComponent<CapsuleShape>();
            shapeTrigger.radius = 0.5f;
            shapeTrigger.height = 2f;
            shapeTrigger.SetCollisionMode(Shape.CollisionMode.Volume);

            triggerVolume = gameObject.AddComponent<OWTriggerVolume>();
            triggerVolume.OnEntry += ShapeTrigger_OnEntry;
        }

        private void ShapeTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector") && PlayerManager.LocalPlayerState == PlayerState.Hiding)
            {
                Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);

                new RoleChangeMessage(QSBPlayerManager.LocalPlayerId,PlayerState.Seeking).Send();
            }
        }
    }
}