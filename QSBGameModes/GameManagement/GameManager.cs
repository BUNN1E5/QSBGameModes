using System;
using System.Collections.Generic;
using System.Linq;
using QSBGameModes.Messages;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Messaging;
using QSB.Player;
using QSB.SaveSync.Messages;
using QSB.WorldSync;
using QSBGameModes.GameManagement.GameTypes;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using UnityEngine.UI;

namespace QSBGameModes.GameManagement{
    public static partial class GameManager{

        public static GameBase gameMode = new HideAndSeek();
        
        private static GameState _state = GameState.Stopped;
        public static GameState state
        {
            get => _state;
            set{
                if (QSBCore.IsHost){ //So that when the host changes the game state a message gets sent
                    new GameStateMessage(value).Send();
                    Utils.WriteLine($"Current State {value}", MessageType.Debug);
                    _state = value; //we will also technically be set from the message
                } else {
                    Utils.WriteLine("Non-Host tried to set GameState", MessageType.Debug);
                }
                //We dont want the non hosts from changing the gamestate at any point
            }
        }

        public static void Init(){
            if(QSBCore.IsHost)
                QSBPlayerManager.OnAddPlayer += (PlayerInfo info) => { new GameStateMessage(state){To = info.PlayerId}.Send(); };
        }

        private static int timeRan = 0;
        
        public static void SetupGame(){
            GameManager.state = GameState.Starting;
            Utils.WriteLine("SetupGame ran " + ++timeRan + " times", MessageType.Error);

            Utils.WriteLine("Setting Up Game", MessageType.Info);

            Utils.WriteLine("Resetting All Player States");
            PlayerManager.ResetAllPlayerStates();
            
            Utils.WriteLine("Setting Up All Players");
            PlayerManager.SetupAllPlayers();


            if (SharedSettings.settingsToShare.AddPlayerSignals){
                Utils.WriteLine("Added the Hide and Seek Frequency");
                PlayerData.LearnFrequency(SignalFrequency.HideAndSeek);
            
                Utils.WriteLine("Setting Up Settings for players", MessageType.Info);
            }

            if(SharedSettings.settingsToShare.Disable6thLocation){
                Utils.WriteLine("Preventing the Quantum Moon from going to the 6th location", MessageType.Info);
                QuantumMoon qm = QSBWorldSync.GetUnityObject<QuantumMoon>();
                if (qm != null){
                    qm._collapseToIndex = 0;
                    qm.ChangeQuantumState(true);
                    Array.Resize(ref qm._orbits, 5);
                    Array.Resize(ref qm._states, 5);
                    Array.Resize(ref qm._stateSkipCounts, 5);
                }
            }
            
            // QSBWorldSync.GetUnityObjects<SandLevelController>().ForEach(controller => {});

            if (SharedSettings.settingsToShare.ActivateAllReturnPlatforms){
                Utils.WriteLine("Setting all return platforms to active", MessageType.Info);
                foreach (NomaiWarpTransmitter transmitter in QSBWorldSync.GetUnityObjects<NomaiWarpTransmitter>()){
                    transmitter.CloseBlackHole();
                    transmitter._targetReceiver._returnPlatform = transmitter;
                    transmitter._targetReceiver._returnOnEntry = true;
                    transmitter._targetReceiver._returnGlowFadeController.FadeTo(0.5f, 5f);
                }
            }
        }

        public static void StopGame(){
            //Make sun splode
            Utils.WriteLine("Stopping Game", MessageType.Info);
            GameManager.state = GameState.Stopped;
        }

        public static void SelectRoles(){
            if (!QSBCore.IsHost){
                Utils.WriteLine("Only the host may select the roles", MessageType.Info);
                return;
            }

            Utils.RunWhen(() => QSBPlayerManager.PlayerList.TrueForAll(playerInfo => playerInfo.Body != null), () => {
                Utils.WriteLine("Choosing the Roles", MessageType.Success);
                HashSet<GameModeInfo> players = new();
                HashSet<uint> hiders = new();
                HashSet<uint> spectators = new();
                
                foreach (GameModeInfo info in PlayerManager.playerInfo.Values){
                    if (info.State is PlayerManagement.PlayerState.None 
                                    or PlayerManagement.PlayerState.Spectating){
                        new RoleChangeMessage(info.Info, PlayerManagement.PlayerState.Spectating).Send();
                        spectators.Add(info.Info.PlayerId);
                        continue;
                    }

                    players.Add(info);
                    hiders.Add(info.Info.PlayerId);
                }
                
                var seekers = RoleSelector.SelectRoles(players, 1);
                hiders.ExceptWith(seekers);
                SendSelectedRoles(seekers, hiders, spectators);
            });
        }
        
        private static void SendSelectedRoles(HashSet<uint> seekers, HashSet<uint> hiders, HashSet<uint> spectators){
            Utils.WriteLine("Sending Roles!");
            Utils.WriteLine($"Seekers: {seekers.Count}");
            foreach (uint seeker in seekers){
                Utils.WriteLine($"Seeker: {seeker}");
                if (QSBPlayerManager.PlayerExists(seeker))
                    new RoleChangeMessage(seeker, PlayerManagement.PlayerState.Seeking).Send();
            }

            Utils.WriteLine($"Hiders: {hiders.Count}");
            foreach (uint hider in hiders){
                Utils.WriteLine($"Hider: {hider}");
                if (QSBPlayerManager.PlayerExists(hider))
                    new RoleChangeMessage(hider, PlayerManagement.PlayerState.Hiding).Send();
            }
			
            Utils.WriteLine($"Spectators: {spectators.Count}");
            foreach (uint spectator in spectators){
                Utils.WriteLine($"Spectator: {spectator}");
                if (QSBPlayerManager.PlayerExists(spectator))
                    new RoleChangeMessage(spectator, PlayerManagement.PlayerState.Spectating).Send();
            }
        }
    }

    //Player should only be able to join as a player while the state is:
    //  Starting, Ending, or Waiting
    
    //If game state is InProgress and Setting is false let player join as
    // spectator
    //if game state is InProgress and setting is true let player join as
    // spectator
    // Seeker
    public enum GameState{
        Starting,
        Waiting,
        InProgress,
        Ending,
        Stopped //This is the default state until host starts the game
    }
}