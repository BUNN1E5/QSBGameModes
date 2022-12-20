using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class RemoteInfo : HideAndSeekInfo
    {
        public SeekerTrigger Trigger;
        public override bool Reset() {
            if (!base.Reset()) //If the base func snagged out
                return false;
            GameObject.Destroy(Trigger);
            return true;
        }

        public override bool SetupInfo(PlayerInfo playerInfo) {
            if (!base.SetupInfo(playerInfo)) //If the base func snagged out
                return false;
            
            Utils.WriteLine("Adding Audio Signal", MessageType.Success);
            Signal = this.Info.Body.AddComponent<AudioSignal>();
                
            Utils.WriteLine("Add the known signal for the local player", MessageType.Success);
            Signal._name = SignalName.RadioTower; //TODO :: CHANGE THIS NAME (Without losing prox chat support)
            Signal._frequency = SignalFrequency.HideAndSeek; 
            
            return true;
        }

        public override bool SetupHider(){
            if (!base.SetupHider()) //If the base func snagged out
                return false;
            Info.SetVisible(true);

            this.Signal._sourceRadius = 500; //Magic OoOOooOh (Around Timber Hearth Radius)
            Utils.WriteLine("Removing the Markers for " + Info, MessageType.Success);
            this.Info.MapMarker.enabled = false;
            this.Info.HudMarker.enabled = false;
            base.SetupHider();

            return true;
        }

        public override bool SetupSeeker(){
            if (!base.SetupSeeker()) //If the base func snagged out
                return false;
            Info.SetVisible(true);

            GameObject seekerVolume = new("seeker_volume");
            seekerVolume.transform.parent = this.Info.Body.transform;
            seekerVolume.transform.localPosition = Vector3.zero;
            seekerVolume.transform.localRotation = Quaternion.identity;
            Trigger = seekerVolume.AddComponent<SeekerTrigger>();
            Trigger.seekerInfo = Info;

            Utils.WriteLine("Adding the HUD Marker", MessageType.Success);
            Info.HudMarker.enabled = true;
            Info.MapMarker.enabled = true;
            return true;
        }

        public override bool SetupSpectator() {
            if (!base.SetupSpectator()) //If the base func snagged out
                return false;
            Info.SetVisible(false);
            return true;
        }
    }
}