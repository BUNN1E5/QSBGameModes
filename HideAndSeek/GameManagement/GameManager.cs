using System;
using System.Linq;
using HideAndSeek.HidersAndSeekersSelection;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Messaging;
using QSB.Player;
using QSB.WorldSync;
using UnityEngine.UI;

namespace HideAndSeek.GameManagement{
    public static class GameManager{

        public static GameState state = GameState.Stopped;

        public static void SetupHideAndSeek(){
            
            Utils.WriteLine("Resetting All Player States");
            PlayerManager.ResetAllPlayerStates();

            Utils.WriteLine("Added the Hide and Seek Frequency");
            PlayerData.LearnFrequency(SignalFrequency.HideAndSeek);
            
            Utils.WriteLine("Preventing the Quantum Moon from going to the 6th location", MessageType.Info);
            QuantumMoon qm = QSBWorldSync.GetUnityObject<QuantumMoon>();
            if (qm != null){
                qm._collapseToIndex = 0;
                qm.ChangeQuantumState(true);
                Array.Resize(ref qm._orbits, 5);
                Array.Resize(ref qm._states, 5);
                Array.Resize(ref qm._stateSkipCounts, 5);
            }

            // QSBWorldSync.GetUnityObjects<SandLevelController>().ForEach(controller => {});

            Utils.WriteLine("Setting Up Settings for players", MessageType.Info);
            QSBPlayerManager.PlayerList.ForEach((playerInfo) => {
                PlayerManager.SetupPlayer(playerInfo);
            });

            Utils.WriteLine("Setting all return platforms to active", MessageType.Info);
            foreach (NomaiWarpTransmitter transmitter in QSBWorldSync.GetUnityObjects<NomaiWarpTransmitter>()){
                transmitter.CloseBlackHole();
                transmitter._targetReceiver._returnPlatform = transmitter;
                transmitter._targetReceiver._returnOnEntry = true;
                transmitter._targetReceiver._returnGlowFadeController.FadeTo(0.5f, 5f);
            }
        }
        
        public static void SelectRoles(){
            Utils.RunWhen(() => QSBPlayerManager.PlayerList.TrueForAll(playerInfo => playerInfo.Body != null), () => {
                Utils.WriteLine("Choosing the Roles", MessageType.Success);
                var seekers = RoleSelector.SelectRoles(1);
                new RolesSelectionMessage(seekers.ToArray()).Send();
            });
        }
    }

    public enum GameState{
        Starting,
        InProgress,
        Ending,
        Stopped //This is the default state until host starts the game
    }
}