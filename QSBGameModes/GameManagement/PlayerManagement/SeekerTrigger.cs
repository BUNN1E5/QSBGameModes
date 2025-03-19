using QSB.Player;
using QSB.Messaging;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWML.Common;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Messages;

namespace QSBGameModes
{
    public class SeekerTrigger : MonoBehaviour
    {
        private OWTriggerVolume triggerVolume;
        public GameModeInfo seekerInfo;

        public void Start()
        {

            if (GetComponent<CapsuleShape>() == null)
            {
                CapsuleShape shapeTrigger = gameObject.AddComponent<CapsuleShape>();
                shapeTrigger.radius = 0.5f;
                shapeTrigger.height = 2f;
                shapeTrigger.SetCollisionMode(Shape.CollisionMode.Volume);
            }
            triggerVolume = gameObject.GetAddComponent<OWTriggerVolume>();
            triggerVolume.OnEntry += ShapeTrigger_OnEntry;
        }

        private void ShapeTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                GameManager.gameMode.OnCatch(seekerInfo); //The local player will never be the one with the 
            }
        }

        private void OnDestroy()
        {
            if (triggerVolume != null) triggerVolume.OnEntry -= ShapeTrigger_OnEntry;
        }

    }
}