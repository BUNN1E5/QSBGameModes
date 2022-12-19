using System;
using UnityEngine;
using Unity;
using QSB.Player;
using QSB.Player.TransformSync;

namespace HideAndSeek{
    public class SeekerTrigger : MonoBehaviour{

        private OWTriggerVolume triggerVolume;

        public void Start(){
            CapsuleShape shapeTrigger = GetComponent<CapsuleShape>();
            shapeTrigger.radius = 0.5f;
            shapeTrigger.height = 2f;
            shapeTrigger.SetCollisionMode(Shape.CollisionMode.Volume);

            triggerVolume = GetComponent<OWTriggerVolume>();
            triggerVolume.OnEntry += ShapeTrigger_OnEntry;
        }

        private void ShapeTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);
            }
        }
    }
}