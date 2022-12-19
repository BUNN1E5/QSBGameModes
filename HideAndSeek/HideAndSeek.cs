using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HideAndSeek.HidersAndSeekersSelection;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Menus;
using QSB.Messaging;
using QSB.Player;
using QSB.Player.TransformSync;
using QSB.Utility;
using QSB.WorldSync;
using UnityEngine;

namespace HideAndSeek
{
    public class HideAndSeek : ModBehaviour{
        public static HideAndSeek instance;
        
        private void Start(){
            instance = this;
            ModHelper.Console.WriteLine($"{nameof(HideAndSeek)} is loaded!", MessageType.Success);
            
            QSBPlayerManager.OnAddPlayer += PlayerManager.SetupPlayer;
            QSBPlayerManager.OnRemovePlayer += PlayerManager.RemovePlayer;

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
                PlayerManager.SetupPlayer(playerInfo);
            });


            ModHelper.Console.WriteLine("Setting all return platforms to active", MessageType.Info);
            foreach (NomaiWarpTransmitter transmitter in QSBWorldSync.GetUnityObjects<NomaiWarpTransmitter>()){
                transmitter.CloseBlackHole();
                transmitter._targetReceiver._returnPlatform = transmitter;
                transmitter._targetReceiver._returnOnEntry = true;
                transmitter._targetReceiver._returnGlowFadeController.FadeTo(0.5f, 5f);
            }
            
            //TODO :: Change to Pause menu button
            if (QSBCore.IsHost)
            {
                StartCoroutine(SelectRoles());
            }
        }

        IEnumerator SelectRoles()
        {
            while (QSBPlayerManager.PlayerList.Count <= 1)
            {
                yield return new WaitForSeconds(5f);
                ModHelper.Console.WriteLine("Waiting For Players . . .", MessageType.Info);
            }
            ModHelper.Console.WriteLine("Waiting For Players To Be Ready . . .", MessageType.Info);

            ModHelper.Events.Unity.RunWhen(() => QSBPlayerManager.PlayerList.TrueForAll(playerInfo => playerInfo.Body != null), () => {
                ModHelper.Console.WriteLine("Choosing the Roles", MessageType.Success);
                var seekers = RoleSelector.SelectRoles(1);
                new RolesSelectionMessage(seekers.ToArray()).Send();
            });
        }
    }
}
