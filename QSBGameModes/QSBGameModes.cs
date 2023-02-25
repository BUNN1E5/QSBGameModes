﻿using System.Reflection;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Messaging;
using QSB.Player;
using QSB.WorldSync;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.GameManagement.SharedSettings;
using QSBGameModes.Menu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QSBGameModes
{
    public class QSBGameModes : ModBehaviour{
        public static QSBGameModes instance;
        
        private void Start(){
            instance = this;
            HarmonyLib.Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Utils.WriteLine($"{nameof(QSBGameModes)} is loaded!", MessageType.Success);
            
            AssetBundlesLoader.LoadBundles(ModHelper);
            PlayerManager.Init();
            GameManager.Init();
            SharedSettings.Init();
            
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                Utils.ModHelper.Events.Unity.FireOnNextUpdate(() => {
                    GameModeMenu.SetupPauseButton();
                    GameModeMenu.UpdateGUI();
                });
                //This runs every loop IF we have started Hide and Seek
                Utils.RunWhen(() => GameManager.state != GameState.Stopped, StartHideAndSeek);
            };
        }
        
        public static void StartHideAndSeek(){
            Utils.RunWhen(() => QSBWorldSync.AllObjectsReady, () => {
                GameManager.SetupGame();
                GameManager.SelectRoles();
            });
        }

        public static void JoinHideAndSeek(){
            new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready).Send();
        }

        public static void LeaveHideAndSeek() {
            new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Spectating).Send();
        }

        //TODO :: CONFIRM THAT THIS WORKS
        //Ambiguous on if the settings are changed before or after this function
        public override void Configure(IModConfig config){
            SharedSettings.SendSettings(); //This only sends if host of session 
        }

        #region DEBUG

        private void Update(){
            if (GetKey(Key.Quote)){
                new SharedSettingsMessage(SharedSettings.settingsToShare);
            }

            if (GetKey(Key.Semicolon)){
                ModHelper.Console.WriteLine("Update UI", MessageType.Debug);
                GameModeMenu.UpdateGUI();
            }


            if (GetKeyDown(Key.M)){
                ModHelper.Console.WriteLine("Changing role to Hiding", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Hiding).Send();
            }
            
            if (GetKeyDown(Key.Comma)){
                ModHelper.Console.WriteLine("Changing role to Seeking", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Seeking).Send();
            }
            
            if (GetKeyDown(Key.Period)){
                ModHelper.Console.WriteLine("Changing role to Spectating", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Spectating).Send();
            }
            
            if (GetKeyDown(Key.Slash)){
                ModHelper.Console.WriteLine("Changing role to Ready", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready).Send();
            }
            
            if (GetKeyDown(Key.N)){
                ModHelper.Console.WriteLine("Changing role to None", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.None).Send();
            }            
        }

        private bool GetKeyDown(Key keyCode) {
            return Keyboard.current[keyCode].wasPressedThisFrame;
        }
        
        private bool GetKey(Key keyCode) {
            return Keyboard.current[keyCode].isPressed;
        }

        #endregion
    }
}