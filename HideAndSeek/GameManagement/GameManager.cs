using System;
using System.Collections.Generic;
using System.Linq;
using HideAndSeek.GameManagement.PlayerManagement;
using HideAndSeek.GameManagement.RoleSelection;
using HideAndSeek.Messages;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Messaging;
using QSB.Player;
using QSB.SaveSync.Messages;
using QSB.WorldSync;
using UnityEngine.UI;

namespace HideAndSeek.GameManagement{
    public static partial class GameManager{

        private static GameState _state = GameState.Stopped;
        public static GameState state{
            get{ return _state; }
            set{
                if (QSBCore.IsHost){ //So that when the host changes the game state a message gets sent
                    new GameStateMessage(value).Send();
                    _state = value; //we will also technically be set from the message
                }
                //We dont want the non hosts from changing the gamestate at any point
            }
        }

        public static void Init(){
            QSBPlayerManager.OnAddPlayer += (PlayerInfo info) => { new GameStateMessage(state){To = info.PlayerId}.Send(); };
        }

        public static void SetupHideAndSeek(){
            GameManager.state = GameState.Starting;
            
            Utils.WriteLine("Resetting All Player States");
            PlayerManager.ResetAllPlayerStates();

            Utils.WriteLine("Added the Hide and Seek Frequency");
            PlayerData.LearnFrequency(SignalFrequency.HideAndSeek);
            
            Utils.WriteLine("Setting Up Settings for players", MessageType.Info);
            
            
            
            if(SharedSettings.SharedSettings.settingsToShare.Disable6thLocation){
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

            if (SharedSettings.SharedSettings.settingsToShare.ActivateAllReturnPlatforms){
                Utils.WriteLine("Setting all return platforms to active", MessageType.Info);
                foreach (NomaiWarpTransmitter transmitter in QSBWorldSync.GetUnityObjects<NomaiWarpTransmitter>()){
                    transmitter.CloseBlackHole();
                    transmitter._targetReceiver._returnPlatform = transmitter;
                    transmitter._targetReceiver._returnOnEntry = true;
                    transmitter._targetReceiver._returnGlowFadeController.FadeTo(0.5f, 5f);
                }
            }
        }
        
        public static void SelectRoles(){
            if (!QSBCore.IsHost){
                Utils.WriteLine("Only the host may select the roles", MessageType.Info);
                return;
            }

            Utils.RunWhen(() => QSBPlayerManager.PlayerList.TrueForAll(playerInfo => playerInfo.Body != null), () => {
                Utils.WriteLine("Choosing the Roles", MessageType.Success);
                HashSet<HideAndSeekInfo> players = new();
                HashSet<uint> hiders = new();
                List<uint> spectators = new();
                
                foreach (HideAndSeekInfo info in PlayerManager.playerInfo.Values){
                    if (info.State == PlayerManagement.PlayerState.None){
                        new RoleChangeMessage(info.Info.PlayerId, PlayerManagement.PlayerState.Spectating).Send();
                        spectators.Add(info.Info.PlayerId);
                        continue;
                    }
                
                    if (info.State == PlayerManagement.PlayerState.Spectating){
                        spectators.Add(info.Info.PlayerId);
                        //new RoleChangeMessage(info.Info.PlayerId, PlayerManagement.PlayerState.Spectating).Send();
                        continue;
                    }
                
                    players.Add(info);
                    hiders.Add(info.Info.PlayerId);
                }
                
                var seekers = RoleSelector.SelectRoles(players, 1);
                hiders.ExceptWith(seekers);
                new RolesSelectionMessage(seekers.ToArray(), hiders.ToArray(), spectators.ToArray()).Send();
            });
        }

        public static void SeekersReleased(){
            GameManager.state = GameState.InProgress;
        }
    }

    //Player should only be able to join as a player while the state is:
    //  Starting, Ending, or Waiting
    
    //If game state is InProgress and Setting is false let player join as
    //spectator
    public enum GameState{
        Starting,
        InProgress,
        Ending, //Mark this at some point
        Waiting,
        Stopped //This is the default state until host starts the game
    }
}