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
            Utils.WriteLine("Loading Shared Settings");
            //settingsToShare.Add("Setting Here");
            var localSettings = Utils.ModHelper.Config.Settings;
            //settingsToShare.Add("Test", localSettings["test"]);
        }

        public static void UpdateSettings(Dictionary<string, object> sharedSettings){
            foreach (var setting in settingsToShare){
                Utils.ModHelper.Config.Settings[setting.Key] = sharedSettings[setting.Key];
            }
        }
    }
    
    public class SharedSettingsMessage : QSBMessage{
        private Dictionary<string, object> settings;
        
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