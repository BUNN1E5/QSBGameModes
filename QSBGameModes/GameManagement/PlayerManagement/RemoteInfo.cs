using OWML.Common;
using QSB.Player;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine;

namespace QSBGameModes{
    public class RemoteInfo : GameModeInfo{
        public SeekerTrigger Trigger;

        public AudioSignal Signal;

        const string RemotePlayerMeshObject = "REMOTE_Traveller_HEA_Player_v2";
        public GameObject SeekerVisual;
        public GameObject SuitVisual;
        public GameObject HeartianVisual;

        public override bool Reset(){
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
            return false;
        }

        public override void OnSettingChange(){
            if (SharedSettings.settingsToShare.AddPlayerSignals) SetupPlayerSignal();
            else GameObject.Destroy(Signal);
        }

        public override bool SetupInfo(PlayerInfo playerInfo){
            if (!base.SetupInfo(playerInfo)) //If the base func snagged out
                return false;

            if (SharedSettings.settingsToShare.AddPlayerSignals){
                SetupPlayerSignal();
            }

            var remoteVisuals = this.Info.Body.transform.Find(RemotePlayerMeshObject);
            var remotePlayerSuitVisual = remoteVisuals.GetChild(1);
            HeartianVisual = remoteVisuals.GetChild(0).gameObject;
            SuitVisual = remotePlayerSuitVisual.gameObject;
            SeekerVisual = Object.Instantiate(SuitVisual, remotePlayerSuitVisual.position,
                remotePlayerSuitVisual.rotation, remotePlayerSuitVisual.parent);
            SeekerVisual.transform.name = "Seeker_visual_geo";
            SeekerVisual.SetActive(true);
            var seekerVisualSkinnedRenderers = SeekerVisual.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in seekerVisualSkinnedRenderers){
                renderer.material = AssetBundlesLoader.SeekerMaterial;
            }

            SeekerVisual.SetActive(false);

            return true;
        }
        
        private void ReturnToDefaultVisual(){
            if (Info.SuitedUp){
                SuitVisual.SetActive(true);
                HeartianVisual.SetActive(false);
            }
            else{
                SuitVisual.SetActive(false);
                HeartianVisual.SetActive(true);
            }
        }

        private void SetupPlayerSignal(){
            Utils.WriteLine("Adding Audio Signal to " + Info, MessageType.Success);
            Signal = this.Info.Body.AddComponent<AudioSignal>();

            Utils.WriteLine("Add the known signal for the local player", MessageType.Success);
            Signal._name = SignalName.RadioTower; //TODO :: CHANGE THIS NAME (Without losing prox chat support)
            Signal._frequency = SignalFrequency.HideAndSeek;
        }
        
        private void SetSeekerVisual(bool enable){
            if (!Utils.ModHelper.Config.GetSettingsValue<bool>("Seeker Visual Effect")){
                SeekerVisual.SetActive(false);
                return;
            }
            if (!enable){
                SeekerVisual.SetActive(false);
                ReturnToDefaultVisual();
                return;
            }
            Utils.RunWhen( () => SuitVisual.activeSelf,() =>
            {
                SeekerVisual.SetActive(true);
                SuitVisual.SetActive(false);
                HeartianVisual.SetActive(true);
            });
        }
        
        public override bool SetupHider(){
            if (!base.SetupHider()) //If the base func snagged out
                return false;
            Info.SetVisible(true);

            if(Signal != null) Signal._sourceRadius = 500; //Magic OoOOooOh (Around Timber Hearth Radius)
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
            if(Signal != null) Signal._sourceRadius = 1;


            GameObject seekerVolume = new("seeker_volume")
            {
                transform =
                {
                    parent = this.Info.Body.transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity
                }
            };
            Trigger = seekerVolume.AddComponent<SeekerTrigger>();
            Trigger.seekerInfo = this;

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