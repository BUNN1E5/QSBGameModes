using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HideAndSeek.GameManagement;
using HideAndSeek.HidersAndSeekersSelection;
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

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                
                //This runs every loop IF we have started Hide and Seek
                Utils.RunWhen(() => QSBWorldSync.AllObjectsReady && GameManager.state != GameState.Stopped, GameManager.SetupHideAndSeek);
                
                //Setup the Host button 
                //for some reason this is currently not working
                if (QSBCore.IsHost){ //TODO :: CHANGE ORDER OF HIDE AND SEEK INTERACT BUTTON
                    Button menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("START HIDE AND SEEK"); //HIDE AND SEEK INTERACT BUTTON
                    Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                    c_event.AddListener(StartHideAndSeek);
                
                    menuButton.onClick = c_event;                
                }
            };
        }

        static void StartHideAndSeek(){
            GameManager.SetupHideAndSeek();
            GameManager.SelectRoles();
        }

        #region DEBUG

        private void Update(){
            if (GetKeyDown(Key.M)){
                PlayerManager.SetPlayerState(QSBPlayerManager.LocalPlayer, PlayerState.Hiding);
            }
            
            if (GetKeyDown(Key.Comma)){
                PlayerManager.SetPlayerState(QSBPlayerManager.LocalPlayer, PlayerState.Seeking);
            }
            
            if (GetKeyDown(Key.Period)){
                PlayerManager.SetPlayerState(QSBPlayerManager.LocalPlayer, PlayerState.Spectating);
            }
            
            if (GetKeyDown(Key.Slash)){
                PlayerManager.SetPlayerState(QSBPlayerManager.LocalPlayer, PlayerState.None);
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
