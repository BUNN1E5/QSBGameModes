using System;
using HideAndSeek.GameManagement.PlayerManagement;
using OWML.Common;
using OWML.Utils;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public class HideAndSeekInfo{
        public PlayerInfo Info;
        public GameManagement.PlayerManagement.PlayerState State;
        private Coroutine waitCoroutine;

        private bool isReady{
            get { return Info.Body != null; }
        }

        public virtual bool SetupInfo(PlayerInfo playerInfo){
            this.Info = playerInfo;
            State = GameManagement.PlayerManagement.PlayerState.None;
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
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => Reset(), waitCoroutine);
                Utils.WriteLine("Player not ready, Waiting", MessageType.Debug);
                return false;
            }
            Utils.WriteLine("Resetting Player", MessageType.Debug);
            State = GameManagement.PlayerManagement.PlayerState.None;
            return true;
        }

        public virtual bool SetupHider(){
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => SetupHider(), waitCoroutine);
                return false;
            }

            if (this.State == GameManagement.PlayerManagement.PlayerState.Hiding){
                Utils.WriteLine(this.Info + " is already a Hider", MessageType.Info);
                return false;
            }
            Reset();
            State = GameManagement.PlayerManagement.PlayerState.Hiding;

            return true;
        }

        public virtual bool SetupSeeker(){
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => SetupSeeker(), waitCoroutine);
                return false;
            }

            if (this.State == GameManagement.PlayerManagement.PlayerState.Seeking){
                Utils.WriteLine(this.Info + " is already a Seeker", MessageType.Info);
                return false;
            }
            Reset();
            State = GameManagement.PlayerManagement.PlayerState.Seeking;

            return true;
        }

        public virtual bool SetupSpectator(){
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => SetupSpectator(), waitCoroutine);
                return false;
            }

            if (this.State == GameManagement.PlayerManagement.PlayerState.Spectating){
                Utils.WriteLine(this.Info + " is already a Spectator", MessageType.Info);
                return false;
            }
            
            Reset();
            State = GameManagement.PlayerManagement.PlayerState.Spectating;

            return true;
        }
    }
}