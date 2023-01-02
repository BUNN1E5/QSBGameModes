using System;
using OWML.Common;
using OWML.Utils;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class HideAndSeekInfo{
        public PlayerInfo Info;
        public AudioSignal Signal;
        public PlayerState State;

        private Coroutine waitCoroutine;

        private bool isReady{
            get { return Info.Body != null; }
        }

        public virtual bool SetupInfo(PlayerInfo playerInfo){
            this.Info = playerInfo;
            State = PlayerState.None;
            if (!EnumUtils.IsDefined<DeathType>(playerInfo.Name)){
                PlayerManager.PlayerDeathTypes.Add(this.Info, EnumUtils.Create<DeathType>(playerInfo.Name));
            }
            return true;
        }

        public virtual bool CleanUp(){
            EnumUtils.Remove<DeathType>(this.Info);
            return false;
        }

        public virtual bool Reset(){
            State = PlayerState.None;
            return true;
        }

        public virtual bool SetupHider(){
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => SetupHider(), waitCoroutine);
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
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => SetupSeeker(), waitCoroutine);
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
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => SetupSpectator(), waitCoroutine);
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