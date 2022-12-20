using HideAndSeek.HidersAndSeekersSelection;
using QSB.Player;
using QSB.Messaging;
using UnityEngine;
using System.Collections;
using HideAndSeek.ArbitraryLocaltionRespawnMessage;

namespace HideAndSeek
{
    public class SeekerTrigger : MonoBehaviour
    {

        private OWTriggerVolume triggerVolume;

        public void Start()
        {
            CapsuleShape shapeTrigger = gameObject.AddComponent<CapsuleShape>();
            shapeTrigger.radius = 0.5f;
            shapeTrigger.height = 2f;
            shapeTrigger.SetCollisionMode(Shape.CollisionMode.Volume);

            triggerVolume = gameObject.AddComponent<OWTriggerVolume>();
            triggerVolume.OnEntry += ShapeTrigger_OnEntry;
        }

        private void ShapeTrigger_OnEntry(GameObject hitObj)
        {
            if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != PlayerState.Hiding)
                return;

            if (hitObj.CompareTag("PlayerDetector"))
            {
                //TODO :: ADD CUSTOM DEATHTYPES
                Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayerId, PlayerState.Seeking).Send();
                StartCoroutine(AutoRespawnWithDelay(5f));
            }
        }

        IEnumerator AutoRespawnWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            new LocationRespawnMessage(QSBPlayerManager.LocalPlayerId, SpawnLocation.TimberHearth).Send();
        }

    }
}