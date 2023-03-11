using System;
using System.Collections.Generic;
using OWML.Common;
using QSB;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement{
    //These are all settings that are configurable but are set by the host on connection
    //Things in the config.json
    public struct SettingsPayload{
        public string GameType;
        public int StartingSeekers;
        public float SeekerVolumeHeight;
        public float SeekerVolumeRadius;
        public bool AllowRepeatSeekers;
        public float PreroundTime;
        public bool AddPlayerSignals;
        public bool Disable6thLocation;
        public bool ActivateAllReturnPlatforms;
        public bool AllowJoinWhileGameInProgress;
        public bool KillHidersOnCatch;
    }

    public struct Setting{
        public Setting(Type type, Object o){
            this.type = type;
            this.o = o;
        }

        public Type type;
        public Object o;
    }

    public static class SharedSettings{

        public static bool receivedSettings = false; //Gets set to true the first time settings have changed

        public static Dictionary<string, Setting> sharedSettings = new(){ };

        private const string GameTypeKey = "GameType";
        private const string StartingSeekersKey = "Starting Seekers";
        private const string SeekerVolumeHeightKey = "Seeker Volume Height";
        private const string SeekerVolumeRadiusKey = "Seeker Volume Radius";
        private const string AllowRepeatSeekersKey = "Allow Repeat Seekers";
        private const string PreRoundTimeKey = "Pre-Round Time";
        private const string AddPlayerSignalsKey =  "Players Have Signals";
        private const string AllowJoinWhileGameInProgressKey = "Allow Join While Game in Progress";
        private const string Disable6ThLocationKey = "Disable 6th Location";
        private const string ActivateAllReturnPlatformsKey = "Activate All Return Platforms";
        private const string KillHidersOnCatchKey = "Kill Hiders on Catch";
            
        public static SettingsPayload settingsToShare = new(){
            GameType = "INFECTION",
            StartingSeekers = 1,
            SeekerVolumeHeight = 2,
            SeekerVolumeRadius = 1,
            AllowRepeatSeekers = true,
            PreroundTime = 30,
            AddPlayerSignals = true,
            Disable6thLocation = true,
            ActivateAllReturnPlatforms = true,
            AllowJoinWhileGameInProgress = false,
            KillHidersOnCatch = false
        };

        public static void Init(){
            settingsToShare = new SettingsPayload(){
                GameType = Utils.ModHelper.Config.GetSettingsValue<string>(GameTypeKey),
                StartingSeekers = Utils.ModHelper.Config.GetSettingsValue<int>(StartingSeekersKey),
                SeekerVolumeHeight = Utils.ModHelper.Config.GetSettingsValue<int>(SeekerVolumeHeightKey),
                SeekerVolumeRadius = Utils.ModHelper.Config.GetSettingsValue<int>(SeekerVolumeRadiusKey),
                AllowRepeatSeekers = Utils.ModHelper.Config.GetSettingsValue<bool>(AllowRepeatSeekersKey),
                PreroundTime = Utils.ModHelper.Config.GetSettingsValue<float>(PreRoundTimeKey),
                AddPlayerSignals =  Utils.ModHelper.Config.GetSettingsValue<bool>(AddPlayerSignalsKey),
                AllowJoinWhileGameInProgress = Utils.ModHelper.Config.GetSettingsValue<bool>(AllowJoinWhileGameInProgressKey),
                Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>(Disable6ThLocationKey),
                ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>(ActivateAllReturnPlatformsKey),
                KillHidersOnCatch = Utils.ModHelper.Config.GetSettingsValue<bool>(KillHidersOnCatchKey)
            };

            QSBPlayerManager.OnAddPlayer += SendSettings; //Make sure new people get sent the settings
            LoadSettings();
        }

        public static void LoadSettings(){
            LoadSettings(Utils.ModHelper.Config);
        }
        
        public static void LoadSettings(IModConfig config){
            Utils.WriteLine("Loading Shared Settings");
            
            //Put the shared settings here
            settingsToShare = new SettingsPayload(){
                GameType = Utils.ModHelper.Config.GetSettingsValue<string>(GameTypeKey),
                StartingSeekers = Utils.ModHelper.Config.GetSettingsValue<int>(StartingSeekersKey),
                SeekerVolumeHeight = Utils.ModHelper.Config.GetSettingsValue<int>(SeekerVolumeHeightKey),
                SeekerVolumeRadius = Utils.ModHelper.Config.GetSettingsValue<int>(SeekerVolumeRadiusKey),
                AllowRepeatSeekers = Utils.ModHelper.Config.GetSettingsValue<bool>(AllowRepeatSeekersKey),
                PreroundTime = Utils.ModHelper.Config.GetSettingsValue<float>(PreRoundTimeKey),
                AddPlayerSignals =  Utils.ModHelper.Config.GetSettingsValue<bool>(AddPlayerSignalsKey),
                AllowJoinWhileGameInProgress = Utils.ModHelper.Config.GetSettingsValue<bool>(AllowJoinWhileGameInProgressKey),
                Disable6thLocation = Utils.ModHelper.Config.GetSettingsValue<bool>(Disable6ThLocationKey),
                ActivateAllReturnPlatforms = Utils.ModHelper.Config.GetSettingsValue<bool>(ActivateAllReturnPlatformsKey),
                KillHidersOnCatch = Utils.ModHelper.Config.GetSettingsValue<bool>(KillHidersOnCatchKey)
            };
            
            
        }

        public static void SendSettings(PlayerInfo info) {
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    LoadSettings();
                    new SharedSettingsMessage(settingsToShare){To = info.PlayerId}.Send();
                }
            }
        }

        public static void SendSettings() {
            if (QSBCore.IsInMultiplayer){
                if (QSBCore.IsHost){
                    LoadSettings();
                    new SharedSettingsMessage(settingsToShare).Send();
                }
            }
        }

        public static void UpdateSettings(){
            Utils.ModHelper.Config.SetSettingsValue(GameTypeKey, settingsToShare.GameType);
            Utils.ModHelper.Config.SetSettingsValue(StartingSeekersKey, settingsToShare.StartingSeekers);
            Utils.ModHelper.Config.SetSettingsValue(SeekerVolumeHeightKey, settingsToShare.SeekerVolumeHeight);
            Utils.ModHelper.Config.SetSettingsValue(SeekerVolumeRadiusKey, settingsToShare.SeekerVolumeRadius);
            Utils.ModHelper.Config.SetSettingsValue(AllowRepeatSeekersKey, settingsToShare.AllowRepeatSeekers);
            Utils.ModHelper.Config.SetSettingsValue(PreRoundTimeKey, settingsToShare.PreroundTime);
            Utils.ModHelper.Config.SetSettingsValue(AddPlayerSignalsKey,settingsToShare.AddPlayerSignals);
            Utils.ModHelper.Config.SetSettingsValue(Disable6ThLocationKey,settingsToShare.Disable6thLocation);
            Utils.ModHelper.Config.SetSettingsValue(ActivateAllReturnPlatformsKey,settingsToShare.ActivateAllReturnPlatforms);
            Utils.ModHelper.Config.SetSettingsValue(AllowJoinWhileGameInProgressKey,settingsToShare.AllowJoinWhileGameInProgress);
            Utils.ModHelper.Config.SetSettingsValue(KillHidersOnCatchKey,settingsToShare.KillHidersOnCatch);
            Utils.ModHelper.Menus.ModsMenu.GetModMenu(QSBGameModes.instance).UpdateUIValues();
        }
    }
}