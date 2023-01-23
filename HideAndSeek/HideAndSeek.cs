using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HideAndSeek.GameManagement;
using HideAndSeek.GameManagement.PlayerManagement;
using HideAndSeek.GameManagement.RoleSelection;
using HideAndSeek.Menu;
using HideAndSeek.Messages;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper;
using OWML.ModHelper.Menus;
using QSB;
using QSB.Menus;
using QSB.Messaging;
using QSB.Player;
using QSB.Player.TransformSync;
using QSB.Utility;
using QSB.WorldSync;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class HideAndSeek : ModBehaviour{
        public static HideAndSeek instance;
        
        private void Start(){
            instance = this;
            HarmonyLib.Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Utils.WriteLine($"{nameof(HideAndSeek)} is loaded!", MessageType.Success);
            
            PlayerManager.Init();
            GameManager.Init();

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                HideAndSeekMenu.SetupPauseButton();
                HideAndSeekMenu.UpdateGUI();
                //This runs every loop IF we have started Hide and Seek
                Utils.RunWhen(() => GameManager.state != GameState.Stopped, StartHideAndSeek);
            };
        }
        
        public static void StartHideAndSeek(){
            Utils.RunWhen(() => QSBWorldSync.AllObjectsReady, () => {
                GameManager.SetupHideAndSeek();
                GameManager.SelectRoles();
            });
        }

        public static void JoinHideAndSeek(){
            new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready);
        }

        //TODO :: CONFIRM THAT THIS WORKS
        //Ambiguous on if the settings are changed before or after this function
        public override void Configure(IModConfig config){
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    base.Configure(config);
                    SharedSettings.LoadSettings();
                    new SharedSettingsMessage(SharedSettings.settingsToShare);
                    return;
                }
            }
            base.Configure(config);
        }

        #region DEBUG

        private void Update(){
            if (GetKeyDown(Key.M)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Hiding);
            }
            
            if (GetKeyDown(Key.Comma)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Seeking);
            }
            
            if (GetKeyDown(Key.Period)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Spectating);
            }
            
            if (GetKeyDown(Key.Slash)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready);
            }
            
            if (GetKeyDown(Key.Period)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.None);
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
