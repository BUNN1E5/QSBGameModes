using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using QSB;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement.PlayerManagement
{
    public static class PlayerManager
    {

        public static HashSet<PlayerInfo> hiders = new();
        public static HashSet<PlayerInfo> seekers = new();
        public static HashSet<PlayerInfo> spectators = new();

        public static Dictionary<PlayerInfo, GameModeInfo> PlayerInfos = new(); //All HideAndSeekInfos have playerInfo
        public static Dictionary<PlayerInfo, DeathType> PlayerDeathTypes = new(); //This gets setup by the HideAndSeekInfo

        public static GameModeInfo LocalPlayer => QSBPlayerManager.LocalPlayer == null ? null : PlayerInfos[QSBPlayerManager.LocalPlayer];

        public static void Init()
        {
            QSBPlayerManager.OnAddPlayer += (PlayerInfo info) =>
            {
                SetupPlayer(info);
                if (QSBCore.IsHost)
                    new PlayerManagerUpdateMessage(PlayerInfos.Values.ToArray()) { To = info.PlayerId }.Send();
            };

            QSBPlayerManager.OnRemovePlayer += RemovePlayer;
        }

        public static void Reset()
        {
            PlayerInfos.Clear();
            hiders.Clear();
            seekers.Clear();
            spectators.Clear();
        }

        public static void RemovePlayer(PlayerInfo playerInfo)
        {
            PlayerManager.CleanUpPlayer(PlayerManager.PlayerInfos[playerInfo]);
            PlayerManager.PlayerInfos.Remove(playerInfo);
            PlayerManager.hiders.Remove(playerInfo);
            PlayerManager.seekers.Remove(playerInfo);
            PlayerManager.spectators.Remove(playerInfo);
        }

        public static void ResetAllPlayers()
        {
            foreach (var info in PlayerInfos.Values)
            {
                info.Reset();
            }
        }

        public static void SetupAllPlayers()
        {
            QSBPlayerManager.PlayerList.ForEach(PlayerManager.SetupPlayer);
        }

        public static void SetAllPlayerStates(PlayerState state)
        {
            QSBPlayerManager.PlayerList.ForEach((playerInfo) =>
            {
                PlayerManager.SetPlayerState(playerInfo, state);
            });
        }

        public static void SetPlayerState(PlayerInfo playerInfo, PlayerState state)
        {
            Utils.WriteLine($"Changing player {playerInfo} state to {state.ToString()} [Client ID: {QSBPlayerManager.LocalPlayerId}]", MessageType.Success);
            switch (state)
            {
                case PlayerState.Hiding:
                    hiders.Add(playerInfo);
                    seekers.Remove(playerInfo);
                    spectators.Remove(playerInfo);
                    SetupHider(PlayerManager.PlayerInfos[playerInfo]);
                    break;
                case PlayerState.Seeking:
                    hiders.Remove(playerInfo);
                    seekers.Add(playerInfo);
                    spectators.Remove(playerInfo);
                    SetupSeeker(PlayerManager.PlayerInfos[playerInfo]);
                    break;
                case PlayerState.Spectating:
                    hiders.Remove(playerInfo);
                    seekers.Remove(playerInfo);
                    spectators.Add(playerInfo);
                    SetupSpectator(PlayerManager.PlayerInfos[playerInfo]);
                    break;
                case PlayerState.None:
                    hiders.Remove(playerInfo);
                    seekers.Remove(playerInfo);
                    spectators.Remove(playerInfo);
                    PlayerManager.PlayerInfos[playerInfo].State = PlayerState.None;
                    Reset(PlayerManager.PlayerInfos[playerInfo]);
                    break;
                case PlayerState.Ready:
                    PlayerManager.PlayerInfos[playerInfo].State = PlayerState.Ready;
                    break;
            }
        }

        //This should run once every loop to initialize everything needed for Hide and Seek
        public static void SetupPlayer(PlayerInfo playerInfo, PlayerState state)
        {
            Utils.RunWhen(() => playerInfo.Body != null, () =>
            {
                Utils.WriteLine($"Setting up {playerInfo.Name}({playerInfo.PlayerId}):", MessageType.Debug);

                if (!PlayerManager.PlayerInfos.ContainsKey(playerInfo))
                {
                    GameModeInfo info = playerInfo.IsLocalPlayer ? new LocalInfo() : new RemoteInfo();
                    PlayerManager.PlayerInfos[playerInfo] = info;
                }

                if (!PlayerManager.PlayerInfos[playerInfo].isSetup())
                    PlayerManager.PlayerInfos[playerInfo].SetupInfo(playerInfo);

                SetPlayerState(playerInfo, state);
            });
        }

        public static void SetupPlayer(PlayerInfo playerInfo)
        {
            if (PlayerManager.PlayerInfos.TryGetValue(playerInfo, out GameModeInfo info))
            {
                SetupPlayer(playerInfo, info.State is PlayerState.None or PlayerState.Spectating
                    ? PlayerState.None : PlayerState.Ready);
                return;
            }
            SetupPlayer(playerInfo, PlayerState.None);
        }


        public static void SetupHider(GameModeInfo info)
        {
            info.SetupHider();
        }

        public static void SetupSeeker(GameModeInfo info)
        {
            info.SetupSeeker();
        }

        public static void SetupSpectator(GameModeInfo info)
        {
            info.SetupSpectator();
        }

        public static void Reset(GameModeInfo info)
        {
            info.Reset();
        }

        public static void CleanUpPlayer(GameModeInfo info)
        {
            info.CleanUp();
        }

        public static void OnSettingsChange()
        {
            QSBPlayerManager.PlayerList.ForEach((playerInfo) =>
            {
                if (PlayerManager.PlayerInfos.TryGetValue(playerInfo, out var info)) info.OnSettingChange();
            });
        }

        public static void SetPlayerSignalSize(PlayerInfo info, float size)
        {
            //PlayerTransformSync.LocalInstance?.ReferenceSector?.AttachedObject.GetRootSector();
            //TODO :: WHEN ADDED TO QSB
        }
    }

    public enum PlayerState
    {
        Hiding,
        Seeking,
        Spectating,
        Ready,
        None
    }
}