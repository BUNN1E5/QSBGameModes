using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class HideAndSeekInfo{
        public PlayerInfo Info;
        public AudioSignal Signal;
        public PlayerState State;

        private bool isReady {
            get { return Info.Body != null; }
        }

        public virtual bool SetupInfo(PlayerInfo playerInfo) {
            this.Info = playerInfo;
            State = PlayerState.None;
            return true;
        }

        public virtual bool Reset() {
            State = PlayerState.None;
            return true;
        }
        
        public virtual bool SetupHider(){
            if (!isReady) { 
                Utils.RunWhen(() => isReady, () => SetupHider());
                return false;
            }

            if (this.State == PlayerState.Hiding){
                Utils.WriteLine(this.Info + " is already a Hider", MessageType.Info);
                return false;
            }
            Reset();
            State = PlayerState.Hiding;
            
            return true;
        }
        
        public virtual bool SetupSeeker(){
            if (!isReady) { 
                Utils.RunWhen(() => isReady, () => SetupSeeker());
                return false;
            }
            
            if (this.State == PlayerState.Seeking){
                Utils.WriteLine(this.Info + " is already a Seeker", MessageType.Info);
                return false;
            }
            Reset();
            State = PlayerState.Seeking;
            
            return true;
        }

        public virtual bool SetupSpectator(){
            if (!isReady) { 
                Utils.RunWhen(() => isReady, () => SetupSpectator());
                return false;
            }
            
            if (this.State == PlayerState.Spectating){
                Utils.WriteLine(this.Info + " is already a Spectator", MessageType.Info);
                return false;
            }
            Reset();
            State = PlayerState.Spectating;
            
            return true;
        }
    }
}