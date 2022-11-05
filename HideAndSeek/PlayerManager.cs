using System.Collections.Generic;
using OWML.Common;
using QSB.Player;

namespace HideAndSeek{
    public class PlayerManager{

        public HashSet<PlayerInfo> hiders;
        public HashSet<PlayerInfo> seekers;

        public Dictionary<PlayerInfo, HideAndSeekInfo> playerInfo;

        public PlayerManager(){
            playerInfo = new Dictionary<PlayerInfo, HideAndSeekInfo>();
            hiders = new HashSet<PlayerInfo>();
            seekers = new HashSet<PlayerInfo>();
        }
        public void RemovePlayer(PlayerInfo playerInfo){
            this.playerInfo.Remove(playerInfo);
        }

        public void SetPlayerState(PlayerInfo playerInfo, PlayerState state){
            switch (state){
                case PlayerState.Hiding:
                    hiders.Add(playerInfo);
                    seekers.Remove(playerInfo);
                    SetupHider(this.playerInfo[playerInfo]);
                    break;
                case PlayerState.Seeking:
                    hiders.Remove(playerInfo);
                    seekers.Add(playerInfo);
                    SetupSeeker(this.playerInfo[playerInfo]);
                    break;
                case  PlayerState.Spectating:
                    hiders.Remove(playerInfo);
                    seekers.Remove(playerInfo);
                    SetupSeeker(this.playerInfo[playerInfo]);
                    break;
            }
        }

        //This should run once every loop to initialize everything needed for Hide and Seek
        public void SetupPlayer(PlayerInfo playerInfo){
            HideAndSeekInfo info = new HideAndSeekInfo();
            info.playerInfo = playerInfo;

            if (!playerInfo.IsLocalPlayer){
                HideAndSeek.instance.ModHelper.Console.WriteLine("Adding Audio Signal", MessageType.Success);
                AudioSignal signal = playerInfo.Body.AddComponent<AudioSignal>();
                signal._frequency = SignalFrequency.HideAndSeek;
                info.signal = signal;
                HideAndSeek.instance.ModHelper.Console.WriteLine("Add the known signal for the local player", MessageType.Success);
            } else {
                HideAndSeek.instance.ModHelper.Console.WriteLine("Local Player! Skipping Adding Audio Signal", MessageType.Info);
            }
            
            if(!this.playerInfo.ContainsKey(playerInfo))
                this.playerInfo.Add(playerInfo, info);
        }

        
        private void SetupHider(HideAndSeekInfo info){
            info.signal._sourceRadius = 500; //Magic OoOOooOh (Around Timber Hearth Radius)
            HideAndSeek.instance.ModHelper.Console.WriteLine("Removing the HUD Marker", MessageType.Success);
            info.playerInfo.HudMarker.enabled = false;
        }
        
        private void SetupSeeker(HideAndSeekInfo info){
            if (hiders.Contains(QSBPlayerManager.LocalPlayer)){
                HideAndSeek.instance.ModHelper.Console.WriteLine("Local Player is a hider, dont add the HUD Markers", MessageType.Info);
                return;
            }
            
            //We are the local player at this point
            //We want to add all the HUD markers
            //of all the seekers
            
            HideAndSeek.instance.ModHelper.Console.WriteLine("Adding the HUD Marker", MessageType.Success);
            foreach (var playerInfo in seekers){
                playerInfo.HudMarker.enabled = true;
            }
        }
        
        private void SetupSpectator(HideAndSeekInfo info){
            //Does Nothing Rn
            //Heard QSB Is gonna add Spectating
        }
        
        public void SetPlayerSignalSize(HideAndSeekInfo info){
            //PlayerTransformSync.LocalInstance?.ReferenceSector?.AttachedObject.GetRootSector();
            //TODO :: WHEN ADDED TO QSB
        }
    }
    
    public struct HideAndSeekInfo{
        public PlayerInfo playerInfo;
        public AudioSignal signal;
    }
    
    public enum PlayerState{
        Hiding,
        Seeking,
        Spectating
    }
}