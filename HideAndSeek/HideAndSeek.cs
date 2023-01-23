using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HideAndSeek.GameManagement;
using HideAndSeek.GameManagement.RoleSelection;
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

        private static Button menuButton;
        private static Text menuText;

        private static Button spectateButton;
        private static Text spectateText;

        
        private void Start(){
            instance = this;
            HarmonyLib.Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Utils.WriteLine($"{nameof(HideAndSeek)} is loaded!", MessageType.Success);
            
            PlayerManager.Init();
            GameManager.Init();

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                
                
                ModHelper.Events.Unity.FireOnNextUpdate(() => {
                    Utils.WriteLine("Adding button to menu");
                    //Setup the Host button 
                    //TODO :: MAKE BETTER GUI FOR SETTING UP GAME
                    if (QSBCore.IsHost){ //TODO :: CHANGE ORDER OF HIDE AND SEEK INTERACT BUTTON
                        menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("START " + SharedSettings.settingsToShare.GameType); //HIDE AND SEEK INTERACT BUTTON
                        menuText = menuButton.GetComponentInChildren<Text>();
                        Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                        c_event.AddListener(StartHideAndSeek);
                
                        menuButton.onClick = c_event;                
                    } else {
                        menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN " + SharedSettings.settingsToShare.GameType); //HIDE AND SEEK INTERACT BUTTON
                        menuText = menuButton.GetComponentInChildren<Text>();
                        Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                        c_event.AddListener(JoinHideAndSeek);
                        
                        menuButton.onClick = c_event;       
                    }
                    
                    UpdateGUI();
                });
                
                
                
                
                //This runs every loop IF we have started Hide and Seek
                Utils.RunWhen(() => QSBWorldSync.AllObjectsReady && GameManager.state != GameState.Stopped, GameManager.SetupHideAndSeek);
            };
        }

        static void StartHideAndSeek(){
            Utils.RunWhen(() => QSBWorldSync.AllObjectsReady, () => {
                GameManager.SetupHideAndSeek();
                GameManager.SelectRoles();
            });
        }

        static void UpdateGUI(){
            if (QSBCore.IsHost){
                if (GameManager.state == GameState.Stopped){
                    menuText.text = "Start " + SharedSettings.settingsToShare.GameType;
                } else{
                    menuText.text = "Stop " + SharedSettings.settingsToShare.GameType;
                }
            } else {
                if (GameManager.state != GameState.Stopped){
                    if (PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State == PlayerState.None){
                        menuText.text = "Join " + SharedSettings.settingsToShare.GameType;
                    } else if(PlayerManager.playerInfo[QSBPlayerManager.LocalPlayer].State != PlayerState.None){
                        menuText.text = "Leave " + SharedSettings.settingsToShare.GameType;
                    }
                }
            }
        }

        static void JoinHideAndSeek(){
            
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
