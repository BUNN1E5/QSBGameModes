using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HideAndSeek.GameManagement;
using HideAndSeek.HidersAndSeekersSelection;
using HideAndSeek.RoleSelection;
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

        private Button menuButton;
        
        private void Start(){
            instance = this;
            HarmonyLib.Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Utils.WriteLine($"{nameof(HideAndSeek)} is loaded!", MessageType.Success);
            
            PlayerManager.Init();

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                
                //This runs every loop IF we have started Hide and Seek
                Utils.RunWhen(() => QSBWorldSync.AllObjectsReady && GameManager.state != GameState.Stopped, GameManager.SetupHideAndSeek);
            };
            
            Utils.WriteLine("Adding button to menu");
            //Setup the Host button 
            //TODO :: MAKE BETTER GUI FOR SETTING UP GAME
            if (QSBCore.IsHost){ //TODO :: CHANGE ORDER OF HIDE AND SEEK INTERACT BUTTON
                menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("START GAME"); //HIDE AND SEEK INTERACT BUTTON
                Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                c_event.AddListener(StartHideAndSeek);
                
                menuButton.onClick = c_event;                
            }
            else{
                menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN GAME"); //HIDE AND SEEK INTERACT BUTTON
                Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                c_event.AddListener(StartHideAndSeek);
                
                menuButton.onClick = c_event;       
            }
        }

        static void StartHideAndSeek(){
            Utils.RunWhen(() => QSBWorldSync.AllObjectsReady, () => {
                GameManager.SetupHideAndSeek();
                GameManager.SelectRoles();
            });
        }

        static void JoinHideAndSeek(){
            
        }

        //TODO :: CONFIRM THAT THIS WORKS
        //Ambiguous on if the settings are changed before or after this function
        public override void Configure(IModConfig config){
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    //Make sure everyone has the server settings
                    new SharedSettingsMessage(config);
                    base.Configure(config);
                    return;
                }

                foreach (var setting in config.Settings){
                    if (SharedSettings.settingsToShare.ContainsKey(setting.Key))
                        continue;
                    Utils.ModHelper.Config.Settings[setting.Key] = setting.Value;
                }
            }
            base.Configure(config);
        }

        #region DEBUG

        private void Update(){
            if (GetKeyDown(Key.M)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, PlayerState.Hiding);
            }
            
            if (GetKeyDown(Key.Comma)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, PlayerState.Seeking);
            }
            
            if (GetKeyDown(Key.Period)){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, PlayerState.Spectating);
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
