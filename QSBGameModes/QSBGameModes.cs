using System.Reflection;
using OWML.Common;
using OWML.ModHelper;
using QSB;
using QSB.Messaging;
using QSB.Player;
using QSB.WorldSync;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.GameManagement;
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
            Utils.WriteLine("QSBGameModes :: " + $"{nameof(QSBGameModes)} is loaded!", MessageType.Success);
            
            AssetBundlesLoader.LoadBundles(ModHelper);
            PlayerManager.Init();
            GameManager.Init();
            SharedSettings.Init();
            
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                Utils.ModHelper.Events.Unity.FireOnNextUpdate(() => {
                    GameModeMenu.SetupPauseButton();
                    GameModeMenu.UpdateGUI();
                    
                    Utils.RunWhen(() => QSBWorldSync.AllObjectsReady && GameManager.state != GameState.Stopped, GameManager.SetupGame);
                });
                //This runs every loop IF we have started Hide and Seek
                
            };
        }

        public static void StopGameMode(){
            
            Utils.RunWhen(() => QSBWorldSync.AllObjectsReady, () => {
                Utils.WriteLine("QSBGameModes :: " + "Host is stopping game", MessageType.Debug);
                GameManager.StopGame();
                PlayerManager.SetAllPlayerStates(GameManagement.PlayerManagement.PlayerState.None);
                GameModeMenu.UpdateGUI();
            });
        }

        public static void StartGameMode(){
            if(QSBCore.IsHost)
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready).Send();
            
            GameModeMenu.UpdateGUI();
            if (GameManager.state != GameState.Stopped){
                Utils.WriteLine("QSBGameModes :: " + "How are you starting game? The game is already started", MessageType.Debug);
                return;
            }
            Utils.WriteLine("QSBGameModes :: " + "Host Started Game", MessageType.Debug);
            GameManager.SetupGame();
        }
        
        public static void JoinGameMode(){
            if (GameManager.state == GameState.Stopped){
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready).Send();
                return;
            }
            
            Utils.WriteLine("QSBGameModes :: " + "Client is joining game", MessageType.Debug);
            
            Utils.RunWhen(() => SharedSettings.receivedSettings, () => {
                if (SharedSettings.settingsToShare.AllowJoinWhileGameInProgress){
                    switch (GameManager.state){
                        case GameState.Starting:
                        case GameState.Waiting:
                            new RoleChangeMessage(PlayerManager.LocalPlayer.Info, GameManager.gameMode.StateOnJoinEarly()).Send();
                            break;
                        case GameState.InProgress:
                            new RoleChangeMessage(PlayerManager.LocalPlayer.Info, GameManager.gameMode.StateOnJoinLate()).Send();
                            break;
                        case GameState.Ending:
                            //What state should be here?
                            new RoleChangeMessage(PlayerManager.LocalPlayer.Info, GameManagement.PlayerManagement.PlayerState.Ready).Send();
                            break;
                    }
                    return;
                }
                new RoleChangeMessage(PlayerManager.LocalPlayer.Info, GameManagement.PlayerManagement.PlayerState.Spectating).Send();
            });
            
        }

        public static void LeaveGameMode() {
            new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Spectating).Send();
            GameModeMenu.UpdateGUI();
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
                ModHelper.Console.WriteLine("QSBGameModes :: " + "Update UI", MessageType.Debug);
                GameModeMenu.UpdateGUI();
            }


            if (GetKeyDown(Key.M)){
                ModHelper.Console.WriteLine("QSBGameModes :: " + "Changing role to Hiding", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Hiding).Send();
            }
            
            if (GetKeyDown(Key.Comma)){
                ModHelper.Console.WriteLine("QSBGameModes :: " + "Changing role to Seeking", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Seeking).Send();
            }
            
            if (GetKeyDown(Key.Period)){
                ModHelper.Console.WriteLine("QSBGameModes :: " + "Changing role to Spectating", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Spectating).Send();
            }
            
            if (GetKeyDown(Key.Slash)){
                ModHelper.Console.WriteLine("QSBGameModes :: " + "Changing role to Ready", MessageType.Debug);
                new RoleChangeMessage(QSBPlayerManager.LocalPlayer.PlayerId, GameManagement.PlayerManagement.PlayerState.Ready).Send();
            }
            
            if (GetKeyDown(Key.N)){
                ModHelper.Console.WriteLine("QSBGameModes :: " + "Changing role to None", MessageType.Debug);
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
