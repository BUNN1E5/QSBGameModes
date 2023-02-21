using OWML.Common;
using QSB;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement.SharedSettings{
    //These are all settings that are configurable but are set by the host on connection
    //Things in the config.json
    public struct SettingsPayload{
        public string GameType;
        public bool Disable6thLocation;
        public bool ActivateAllReturnPlatforms;
        public bool AllowJoinWhileGameInProgress;
    }

    public static class SharedSettings{

        public static SettingsPayload settingsToShare = new(){
            GameType = "HIDE AND SEEK",
            Disable6thLocation = true,
            ActivateAllReturnPlatforms = true,
            AllowJoinWhileGameInProgress = false
        };

        public static void Init(){
            settingsToShare = new SettingsPayload(){
                GameType = Utils.ModHelper.Config.GetSettingsValue<string>("GameType"),
                AllowJoinWhileGameInProgress = Utils.ModHelper.Config.GetSettingsValue<bool>("Allow Join While Game in Progress"),
                Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>("Disable 6th Location"),
                ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>("Activate All Return Platforms")
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
                AllowJoinWhileGameInProgress = Utils.ModHelper.Config.GetSettingsValue<bool>("Allow Join While Game in Progress"),
                Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>("Disable 6th Location"),
                ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>("Activate All Return Platforms")
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
            Utils.ModHelper.Config.Settings["Disable 6th Location"] = (object)settingsToShare.Disable6thLocation;
            Utils.ModHelper.Config.Settings["Activate All Return Platforms"] = (object)settingsToShare.ActivateAllReturnPlatforms;
            Utils.ModHelper.Config.Settings["Allow Join While Game in Progress"] = (object)settingsToShare.AllowJoinWhileGameInProgress;
        }
    }
}