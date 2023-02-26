using OWML.Common;
using QSB;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement{
    //These are all settings that are configurable but are set by the host on connection
    //Things in the config.json
    public struct SettingsPayload{
        public string GameType;
        public bool AddPlayerSignals;
        public bool Disable6thLocation;
        public bool ActivateAllReturnPlatforms;
        public bool AllowJoinWhileGameInProgress;
        public bool KillHidersOnCatch;
    }

    public static class SharedSettings{

        public static bool receivedSettings = false; //Gets set to true the first time settings have changed
        
        public static SettingsPayload settingsToShare = new(){
            GameType = "INFECTION",
            AddPlayerSignals = true,
            Disable6thLocation = true,
            ActivateAllReturnPlatforms = true,
            AllowJoinWhileGameInProgress = false,
            KillHidersOnCatch = false
        };

        public static void Init(){
            settingsToShare = new SettingsPayload(){
                GameType = Utils.ModHelper.Config.GetSettingsValue<string>("GameType"),
                AddPlayerSignals =  Utils.ModHelper.Config.GetSettingsValue<bool>("Players Have Signals"),
                AllowJoinWhileGameInProgress = Utils.ModHelper.Config.GetSettingsValue<bool>("Allow Join While Game in Progress"),
                Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>("Disable 6th Location"),
                ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>("Activate All Return Platforms"),
                KillHidersOnCatch = Utils.ModHelper.Config.GetSettingsValue<bool>("Kill Hiders on Catch")
            };

            QSBPlayerManager.OnAddPlayer += SendSettings; //Make sure new people get sent the settings
        }

        public static void LoadSettings(){
            LoadSettings(Utils.ModHelper.Config);
        }
        
        public static void LoadSettings(IModConfig config){
            Utils.WriteLine("Loading Shared Settings");
            
            //Put the shared settings here
            settingsToShare = new SettingsPayload(){
                GameType = Utils.ModHelper.Config.GetSettingsValue<string>("GameType"),
                AddPlayerSignals =  Utils.ModHelper.Config.GetSettingsValue<bool>("Players Have Signals"),
                AllowJoinWhileGameInProgress = Utils.ModHelper.Config.GetSettingsValue<bool>("Allow Join While Game in Progress"),
                Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>("Disable 6th Location"),
                ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>("Activate All Return Platforms"),
                KillHidersOnCatch = Utils.ModHelper.Config.GetSettingsValue<bool>("Kill Hiders on Catch")
            };
        }

        public static void SendSettings(PlayerInfo info) {
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    LoadSettings();
                    new SharedSettingsMessage(SharedSettings.settingsToShare){To = info.PlayerId}.Send();
                }
            }
        }

        public static void SendSettings() {
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    LoadSettings();
                    new SharedSettingsMessage(SharedSettings.settingsToShare).Send();
                }
            }
        }

        public static void UpdateSettings(){
            Utils.ModHelper.Config.Settings["GameType"] = settingsToShare.GameType;
            Utils.ModHelper.Config.Settings["Players Have Signals"] = (object)settingsToShare.AddPlayerSignals;
            Utils.ModHelper.Config.Settings["Disable 6th Location"] = (object)settingsToShare.Disable6thLocation;
            Utils.ModHelper.Config.Settings["Activate All Return Platforms"] = (object)settingsToShare.ActivateAllReturnPlatforms;
            Utils.ModHelper.Config.Settings["Allow Join While Game in Progress"] = (object)settingsToShare.AllowJoinWhileGameInProgress;
            Utils.ModHelper.Config.Settings["Kill Hiders on Catch"] = (object)settingsToShare.KillHidersOnCatch;
        }
    }
}