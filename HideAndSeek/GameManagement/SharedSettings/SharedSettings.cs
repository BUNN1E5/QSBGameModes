using System.Collections.Generic;
using Mirror;
using OWML.Common;
using QSB.Messaging;

namespace HideAndSeek.GameManagement{
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
            GameType = Utils.ModHelper.Config.GetSettingsValue<string>("GameType"),
            Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>("Disable 6th Location"),
            ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>("Activate All Return Platforms")
        };

        public static void LoadSettings(){
            LoadSettings(Utils.ModHelper.Config);
        }
        
        public static void LoadSettings(IModConfig config){
            Utils.WriteLine("Loading Shared Settings");
            
            //Put the shared settings here
            settingsToShare.GameType = config.GetSettingsValue<string>("GameType");
            settingsToShare.Disable6thLocation = config.GetSettingsValue<bool>("Disable 6th Location");
            settingsToShare.ActivateAllReturnPlatforms = config.GetSettingsValue<bool>("Activate All Return Platforms");
            settingsToShare.AllowJoinWhileGameInProgress = config.GetSettingsValue<bool>("Allow Join While Game in Progress");
        }

        public static void UpdateSettings(){
            Utils.ModHelper.Config.Settings["GameType"] = settingsToShare.GameType;
            Utils.ModHelper.Config.Settings["Disable 6th Location"] = (object)settingsToShare.Disable6thLocation;
            Utils.ModHelper.Config.Settings["Activate All Return Platforms"] = (object)settingsToShare.ActivateAllReturnPlatforms;
            Utils.ModHelper.Config.Settings["Allow Join While Game in Progress"] = (object)settingsToShare.AllowJoinWhileGameInProgress;
        }
    }
}