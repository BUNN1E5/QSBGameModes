using OWML.Common;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine;

namespace QSBGameModes{
    public class RemoteInfo : GameModeInfo
    {
        public SeekerTrigger Trigger;
        public AudioSignal Signal;
        
        const string RemotePlayerMeshObject = "REMOTE_Traveller_HEA_Player_v2";
        public GameObject SeekerVisual;
        public static Material SeekerMaterial;
        
        public override bool Reset() {
            if (!base.Reset()) //If the base func snagged out
                return false;
            Info.MapMarker.enabled = true;
            Info.HudMarker.enabled = true;
            SeekerVisual.SetActive(false);
            return true;
        }

        public override bool CleanUp(){
            if (!base.CleanUp()) //If the base func snagged out
                return false;
            GameObject.Destroy(Trigger);
            GameObject.Destroy(Signal);
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

            if(SeekerMaterial == null){
                SeekerMaterial = GameObject.FindObjectOfType<TimelineObliterationEffect>().gameObject.GetComponent<MeshRenderer>().material;// new Material(Shader.Find("Outer Wilds/Effects/Reality Cracks"));
            }
            var remotePlayerSuitVisual = this.Info.Body.transform.Find(RemotePlayerMeshObject).GetChild(1);
            SeekerVisual = GameObject.Instantiate(remotePlayerSuitVisual.gameObject, remotePlayerSuitVisual.position,
                remotePlayerSuitVisual.rotation, remotePlayerSuitVisual.parent);
            SeekerVisual.transform.name = "Seeker_visual_geo";
            SeekerVisual.SetActive(true);
            var seekerVisualSkinnedRenderers = SeekerVisual.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(var renderer in seekerVisualSkinnedRenderers){
                renderer.material = SeekerMaterial;
            }
            SeekerVisual.SetActive(false);
            
            return true;
        }
        
        private void SetSeekerVisual(bool enable){
            if (!Utils.ModHelper.Config.GetSettingsValue<bool>("Seeker Visual Effect")){
                SeekerVisual.SetActive(false);
            }
            SeekerVisual.SetActive(enable);
        }
        
        public override bool SetupHider(){
            if (!base.SetupHider()) //If the base func snagged out
                return false;
            Info.SetVisible(true);

            this.Signal._sourceRadius = 500; //Magic OoOOooOh (Around Timber Hearth Radius)
            Utils.WriteLine("Removing the Markers for " + Info, MessageType.Success);
            this.Info.MapMarker.enabled = false;
            this.Info.HudMarker.enabled = false;

            SetSeekerVisual(false);

            return true;
        }

        public override bool SetupSeeker(){
            if (!base.SetupSeeker()) //If the base func snagged out
                return false;
            Info.SetVisible(true);
            this.Signal._sourceRadius = 1;


            GameObject seekerVolume = new("seeker_volume");
            seekerVolume.transform.parent = this.Info.Body.transform;
            seekerVolume.transform.localPosition = Vector3.zero;
            seekerVolume.transform.localRotation = Quaternion.identity;
            Trigger = seekerVolume.AddComponent<SeekerTrigger>();
            Trigger.seekerInfo = Info;

            Utils.WriteLine("Adding the HUD Marker", MessageType.Success);
            
            //Hiders shouldn't be able to see the seekers Map and Hud Markers
            bool state = PlayerManager.LocalPlayer.State == State;
            Info.HudMarker.enabled = state;
            Info.MapMarker.enabled = state;

            SetSeekerVisual(true);

            return true;
        }

        public override bool SetupSpectator() {
            if (!base.SetupSpectator()) //If the base func snagged out
                return false;
            
            //The visibility of the player should be the same as the lcoal Player if they are spectator already
            bool state = PlayerManager.LocalPlayer.State == State;
            Info.SetVisible(state);
            Info.HudMarker.enabled = state;
            Info.MapMarker.enabled = state;

            SetSeekerVisual(false);

            return true;
        }
    }
}