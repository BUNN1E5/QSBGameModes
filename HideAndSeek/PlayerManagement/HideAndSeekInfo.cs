using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class HideAndSeekInfo{
        public PlayerInfo playerInfo;
        public AudioSignal signal;
        public PlayerState state;

        public bool isSetup;

        public virtual void SetupSeeker(){
            if (this.state == PlayerState.Seeking){
                Utils.WriteLine(this.playerInfo + " is already a Seeker", MessageType.Info);
                return;
            }
            
            if (this.playerInfo.IsLocalPlayer){
                Utils.WriteLine("Local Player Is Seeker", MessageType.Info);
                return;
            }

            if (PlayerManager.hiders.Contains(QSBPlayerManager.LocalPlayer)){
                Utils.WriteLine("Local Player is a hider, dont add the HUD Markers", MessageType.Info);
                return;
            }

            HideAndSeek.instance.ModHelper.Events.Unity.RunWhen(() => this.playerInfo.Body != null && isSetup, () => {            
                GameObject seekerVolume = new("seeker_volume");
                seekerVolume.transform.parent = this.playerInfo.Body.transform;
                seekerVolume.transform.localPosition = Vector3.zero;
                seekerVolume.transform.localRotation = Quaternion.identity;
                seekerVolume.AddComponent<SeekerTrigger>();

                Utils.WriteLine("Adding the HUD Marker", MessageType.Success);

                //We are the local player at this point
                //We want to add all the HUD markers
                //of all the seekers
                //foreach (var playerInfo in seekers){
                this.playerInfo.HudMarker.enabled = true;
                this.playerInfo.MapMarker.enabled = true;
                //}
                //foreach (var playerInfo in hiders)
                //{
                //    playerInfo.HudMarker.enabled = false;
                //    playerInfo.MapMarker.enabled = false;
                //}
            });
        }

        public virtual void SetupHider(){
            if (this.state == PlayerState.Hiding){
                Utils.WriteLine(this.playerInfo + " is already a Hider", MessageType.Info);
                return;
            }

            if (this.playerInfo.IsLocalPlayer){
                Utils.WriteLine("Local Player Is Now Hider", MessageType.Info);
                Utils.WriteLine("Removing the HUD Markers", MessageType.Success);
                foreach (PlayerInfo _info in QSBPlayerManager.PlayerList){
                    if (_info.Body != null){
                        _info.MapMarker.enabled = false;
                        _info.HudMarker.enabled = false;
                    }
                }
                return;
            }

            HideAndSeek.instance.ModHelper.Events.Unity.RunWhen(() => this.playerInfo.Body != null && isSetup, () =>{
                this.signal._sourceRadius = 500; //Magic OoOOooOh (Around Timber Hearth Radius)
                Utils.WriteLine("Removing the HUD Marker", MessageType.Success);

                this.playerInfo.MapMarker.enabled = false;
                this.playerInfo.HudMarker.enabled = false;
            });            
        }

        public virtual void SetupSpectator(){
            if (this.state == PlayerState.Spectating){
                Utils.WriteLine(this.playerInfo + " is already a Spectator", MessageType.Info);
                return;
            }

            if (this.playerInfo.IsLocalPlayer){
                Utils.WriteLine("Local Player Is Spectator", MessageType.Info);
                return;
            }
            
            //Does Nothing Rn
            //Heard QSB Is gonna add Spectating
        }
    }
}