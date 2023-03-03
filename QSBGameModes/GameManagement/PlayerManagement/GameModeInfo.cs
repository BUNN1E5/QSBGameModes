using OWML.Common;
using OWML.Utils;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine;

namespace QSBGameModes{
    public class GameModeInfo{
        public PlayerInfo Info;
        public GameManagement.PlayerManagement.PlayerState State;
        private Coroutine waitCoroutine;

        private GameObject playerBody = null;

        public bool isSetup(){
            return playerBody != null;
        }

        private bool isReady{
            get { return Info.Body != null; }
        }

        public virtual bool SetupInfo(PlayerInfo playerInfo){
            this.Info = playerInfo;
            playerBody = playerInfo._body; //So we know if we got setup this loop
            State = GameManagement.PlayerManagement.PlayerState.None;
            if (!EnumUtils.IsDefined<DeathType>(playerInfo.Name)){
                PlayerManager.PlayerDeathTypes.Add(this.Info, EnumUtils.Create<DeathType>(playerInfo.Name));
            }
            
            
            return true;
        }

        public virtual bool CleanUp(){
            EnumUtils.Remove<DeathType>(this.Info.Name);
            return false;
        }

        public virtual bool Reset(){
            if (!isReady){
                waitCoroutine = Utils.RunWhen(() => isReady, () => Reset(), waitCoroutine);
                Utils.WriteLine("GameModeInfo :: Player not ready, Waiting", MessageType.Debug);
                return false;
            }
            Utils.WriteLine("Resetting Player", MessageType.Debug);
            
            if (State is GameManagement.PlayerManagement.PlayerState.None 
                or GameManagement.PlayerManagement.PlayerState.Spectating){
                return true;
            }

            State = GameManagement.PlayerManagement.PlayerState.Ready;
            return true;
        }

        public virtual void OnSettingChange(){}

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
            //new LocationWarpMessage(Info.PlayerId, SpawnLocation.HourglassTwin_2).Send();
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
            //new LocationWarpMessage(Info.PlayerId, SpawnLocation.TimberHearth).Send();

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