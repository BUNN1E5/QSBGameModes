using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class HideAndSeekInfo{
        public PlayerInfo playerInfo;
        public AudioSignal signal;
        public PlayerState state;

        public bool isSetup;

        public virtual bool SetupInfo(PlayerInfo playerInfo) {
            this.playerInfo = playerInfo;
            state = PlayerState.None;
            return true;
        }

        public virtual bool Reset() {
            state = PlayerState.None;
            return true;
        }

        public virtual bool SetupHider(){
            if (this.state == PlayerState.Hiding){
                Utils.WriteLine(this.playerInfo + " is already a Hider", MessageType.Info);
                return false;
            }
            state = PlayerState.Hiding;
            
            return true;
        }
        
        public virtual bool SetupSeeker(){
            if (this.state == PlayerState.Seeking){
                Utils.WriteLine(this.playerInfo + " is already a Seeker", MessageType.Info);
                return false;
            }
            state = PlayerState.Seeking;
            
            return true;
        }

        public virtual bool SetupSpectator(){
            if (this.state == PlayerState.Spectating){
                Utils.WriteLine(this.playerInfo + " is already a Spectator", MessageType.Info);
                return false;
            }
            state = PlayerState.Spectating;
            
            return true;
        }
    }
}