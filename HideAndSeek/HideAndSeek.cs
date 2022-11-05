using System;
using System.Collections.Generic;
using OWML.Common;
using OWML.ModHelper;
using QSB.Menus;
using QSB.Player;
using QSB.Player.TransformSync;
using QSB.Utility;
using QSB.WorldSync;
using UnityEngine;

namespace HideAndSeek
{
    public class HideAndSeek : ModBehaviour{
        public static HideAndSeek instance;

        public PlayerManager playerManager;

        private void Start(){
            instance = this;
            ModHelper.Console.WriteLine($"{nameof(HideAndSeek)} is loaded!", MessageType.Success);
            playerManager = new PlayerManager();

            QSBPlayerManager.OnAddPlayer += playerManager.SetupPlayer;
            QSBPlayerManager.OnRemovePlayer += playerManager.RemovePlayer;
            
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                ModHelper.Events.Unity.RunWhen(() => QSBWorldSync.AllObjectsReady, () => {
                    SetupHideAndSeek(scene, loadScene);
                });
            };
        }

        public void SetupHideAndSeek(OWScene scene, OWScene loadScene){
            if (loadScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Added the Hide and Seek Frequency");
            PlayerData.LearnFrequency(SignalFrequency.HideAndSeek);
            
            ModHelper.Console.WriteLine("Preventing the Quantum Moon from going to the 6th location", MessageType.Info);
            QuantumMoon qm = QSBWorldSync.GetUnityObject<QuantumMoon>();
            if (qm != null){
                qm._collapseToIndex = 0;
                qm.ChangeQuantumState(true);
                Array.Resize(ref qm._orbits, 5);
                Array.Resize(ref qm._states, 5);
                Array.Resize(ref qm._stateSkipCounts, 5);
            }

            // QSBWorldSync.GetUnityObjects<SandLevelController>().ForEach(controller => {});

            ModHelper.Console.WriteLine("Setting Up Settings for players", MessageType.Info);
            QSBPlayerManager.PlayerList.ForEach((playerInfo) => {
                ModHelper.Events.Unity.RunWhen(() => playerInfo.Body != null, () => {
                    ModHelper.Console.WriteLine("Setting up " + playerInfo.Name + ": ", MessageType.Debug);
                    playerManager.SetupPlayer(playerInfo);
                });
            });

            ModHelper.Console.WriteLine("Setting all return platforms to active", MessageType.Info);
            foreach (NomaiWarpTransmitter transmitter in QSBWorldSync.GetUnityObjects<NomaiWarpTransmitter>()){
                transmitter.CloseBlackHole();
                transmitter._targetReceiver._returnPlatform = transmitter;
                transmitter._targetReceiver._returnOnEntry = true;
                transmitter._targetReceiver._returnGlowFadeController.FadeTo(0.5f, 5f);
            }
        }
    }
}
