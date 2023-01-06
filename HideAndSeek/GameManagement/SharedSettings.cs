using System.Collections.Generic;
using Mirror;
using OWML.Common;
using QSB.Messaging;

namespace HideAndSeek.GameManagement{
    //These are all settings that are configurable but are set by the host on connection
    //Things in the config.json
    public static class SharedSettings{

        public static Dictionary<string, object> settingsToShare = new();

        public static void LoadSettings(){
            LoadSettings(Utils.ModHelper.Config);
        }

        public static void LoadSettings(IModConfig config){
            Utils.WriteLine("Loading Shared Settings");
            
            //Put the shared settings here
            settingsToShare["GameType"] = config.Settings["GameType"];
            settingsToShare["Disable 6th Location"] = config.Settings["Disable 6th Location"];
            settingsToShare["Activate All Return Platforms"] = config.Settings["Activate All Return Platforms"];
        }

        public static void UpdateSettings(Dictionary<string, object> sharedSettings){
            foreach (var setting in settingsToShare){
                Utils.ModHelper.Config.Settings[setting.Key] = sharedSettings[setting.Key];
            }
        }
    }
    
    public class SharedSettingsMessage : QSBMessage{
        private Dictionary<string, object> settings;
        
        public SharedSettingsMessage(IModConfig config) : base(){
            SharedSettings.LoadSettings(config);
            this.settings = SharedSettings.settingsToShare;
        }

        public SharedSettingsMessage() : base(){
            SharedSettings.LoadSettings();
            this.settings = SharedSettings.settingsToShare;
        }

        public override void Serialize(NetworkWriter writer){
            base.Serialize(writer);
            writer.Write(settings);
        }

        public override void Deserialize(NetworkReader reader){
            base.Deserialize(reader);
            settings = reader.Read<Dictionary<string, object>>();
        }
        
        public override void OnReceiveRemote(){
            SharedSettings.UpdateSettings(settings);
        }

        public override void OnReceiveLocal(){ }
    }
}