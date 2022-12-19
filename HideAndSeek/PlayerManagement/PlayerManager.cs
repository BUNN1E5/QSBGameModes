using System.Collections.Generic;
using OWML.Common;
using QSB.Player;
using UnityEngine;

namespace HideAndSeek{
    public static class PlayerManager{

        public static HashSet<PlayerInfo> hiders = new();
        public static HashSet<PlayerInfo> seekers = new();

        public static Dictionary<PlayerInfo, HideAndSeekInfo> playerInfo = new();

        public static PlayerState LocalPlayerState;

        public static void RemovePlayer(PlayerInfo playerInfo){
            PlayerManager.playerInfo.Remove(playerInfo);
            PlayerManager.hiders.Remove(playerInfo);
            PlayerManager.seekers.Remove(playerInfo);
        }

        public static void ResetAllPlayerStates() {
            foreach (var info in playerInfo.Values) {
                info.Reset();
            }
        }

        public static void SetPlayerState(PlayerInfo playerInfo, PlayerState state){
            switch (state){
                case PlayerState.Hiding:
                    hiders.Add(playerInfo);
                    seekers.Remove(playerInfo);
                    SetupHider(PlayerManager.playerInfo[playerInfo]);
                    break;
                case PlayerState.Seeking:
                    hiders.Remove(playerInfo);
                    seekers.Add(playerInfo);
                    SetupSeeker(PlayerManager.playerInfo[playerInfo]);
                    break;
                case  PlayerState.Spectating:
                    hiders.Remove(playerInfo);
                    seekers.Remove(playerInfo);
                    SetupSeeker(PlayerManager.playerInfo[playerInfo]);
                    break;
                case PlayerState.None:
                    Utils.WriteLine("Player Set to None State", MessageType.Error);
                    break;
            }
        }

        //This should run once every loop to initialize everything needed for Hide and Seek
        public static void SetupPlayer(PlayerInfo playerInfo){
            HideAndSeek.instance.ModHelper.Events.Unity.RunWhen(() => playerInfo.Body != null, () => {
                Utils.WriteLine("Setting up " + playerInfo.Name + ": ", MessageType.Debug);
                HideAndSeekInfo info = playerInfo.IsLocalPlayer ? new LocalInfo() : new RemoteInfo();
                info.SetupInfo(playerInfo);
                
                if (!PlayerManager.playerInfo.ContainsKey(playerInfo)){
                    PlayerManager.playerInfo[playerInfo] =  info;
                }
                
                SetPlayerState(playerInfo, PlayerManager.playerInfo[playerInfo].state);
            });
        }

        
        public static void SetupHider(HideAndSeekInfo info){
            info.SetupHider();
        }
        
        public static void SetupSeeker(HideAndSeekInfo info){
            info.SetupSeeker();
        }
        
        public static void SetupSpectator(HideAndSeekInfo info){
            info.SetupSpectator();
        }
        
        public static void SetPlayerSignalSize(HideAndSeekInfo info){
            //PlayerTransformSync.LocalInstance?.ReferenceSector?.AttachedObject.GetRootSector();
            //TODO :: WHEN ADDED TO QSB
        }
    }

    public enum PlayerState{
        Hiding,
        Seeking,
        Spectating,
        None
    }
}