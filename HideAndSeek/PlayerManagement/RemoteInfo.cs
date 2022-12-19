using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class RemoteInfo : HideAndSeekInfo{

        public override bool SetupInfo(PlayerInfo playerInfo) {
            if (!base.SetupInfo(playerInfo)) //If the base func snagged out
                return false;
            
            Utils.WriteLine("Adding Audio Signal", MessageType.Success);
            signal = this.playerInfo.Body.AddComponent<AudioSignal>();
                
            Utils.WriteLine("Add the known signal for the local player", MessageType.Success);
            signal._name = SignalName.RadioTower; //TODO :: CHANGE THIS NAME (Without losing prox chat support)
            signal._frequency = SignalFrequency.HideAndSeek; 
            
            return true;
        }

        public override bool SetupHider(){
            if (!base.SetupHider()) //If the base func snagged out
                return false;
            this.signal._sourceRadius = 500; //Magic OoOOooOh (Around Timber Hearth Radius)
            Utils.WriteLine("Removing the Markers for " + playerInfo, MessageType.Success);
            this.playerInfo.MapMarker.enabled = false;
            this.playerInfo.HudMarker.enabled = false;
            base.SetupHider();

            return true;
        }

        public override bool SetupSeeker(){
            if (!base.SetupSeeker()) //If the base func snagged out
                return false;
            GameObject seekerVolume = new("seeker_volume");
            seekerVolume.transform.parent = this.playerInfo.Body.transform;
            seekerVolume.transform.localPosition = Vector3.zero;
            seekerVolume.transform.localRotation = Quaternion.identity;
            seekerVolume.AddComponent<SeekerTrigger>();

            Utils.WriteLine("Adding the HUD Marker", MessageType.Success);
            playerInfo.HudMarker.enabled = true;
            playerInfo.MapMarker.enabled = true;
            return true;
        }

        public override bool SetupSpectator() {
            if (!base.SetupSpectator()) //If the base func snagged out
                return false;
            return true;
        }
    }
}