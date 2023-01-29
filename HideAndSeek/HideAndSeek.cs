using System.Reflection;
using HideAndSeek.GameManagement;
using HideAndSeek.GameManagement.PlayerManagement;
using HideAndSeek.GameManagement.RoleSelection;
using HideAndSeek.GameManagement.SharedSettings;
using HideAndSeek.Menu;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Messaging;
using QSB.Player;
using QSB.WorldSync;
using UnityEngine;
using UnityEngine.InputSystem;

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
            SharedSettings.Init();
            
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                Utils.ModHelper.Events.Unity.FireOnNextUpdate(() => {
                    HideAndSeekMenu.SetupPauseButton();
                    HideAndSeekMenu.UpdateGUI();
                });
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
            new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready).Send();
        }

        //TODO :: CONFIRM THAT THIS WORKS
        //Ambiguous on if the settings are changed before or after this function
        public override void Configure(IModConfig config){
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    SharedSettings.LoadSettings();
                    new SharedSettingsMessage(SharedSettings.settingsToShare).Send();
                }
            }
        }

        #region DEBUG

        private void Update(){
            if (GetKey(Key.Quote)){
                new SharedSettingsMessage(SharedSettings.settingsToShare);
            }

            if (GetKey(Key.Semicolon)){
                ModHelper.Console.WriteLine("Update UI", MessageType.Debug);
                HideAndSeekMenu.UpdateGUI();
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
