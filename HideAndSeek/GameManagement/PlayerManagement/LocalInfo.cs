using System.Linq;
using HideAndSeek.Patches;
using OWML.Common;
using QSB.Player;

namespace HideAndSeek{
    public class LocalInfo : HideAndSeekInfo{

        NotificationData youAreAHiderNotification = new(NotificationTarget.All, "You are a HIDER");
        NotificationData youAreASeekerNotification = new(NotificationTarget.All, "You are a SEEKER");
        NotificationData youAreASpectatorNotification = new(NotificationTarget.All, "You are a SPECTATOR");

        protected float DefaultRunSpeed = 6f;
        public override bool SetupHider() {
            if (!base.SetupHider()) //If the base func snagged out
                return false;

            Utils.WriteLine("Local Player Is Now Hider", MessageType.Info);
            Utils.WriteLine("Removing the All Markers", MessageType.Success);
            foreach (PlayerInfo info in QSBPlayerManager.PlayerList) {
                if (info.IsLocalPlayer)
                    continue;
                
                if (info.Body != null){
                    info.MapMarker.enabled = false;
                    info.HudMarker.enabled = false;
                }
            }

            Locator.GetPlayerSuit().SuitUp(false, false, true);
            NotificationManager.SharedInstance.UnpinNotification(youAreASeekerNotification);
            NotificationManager.SharedInstance.UnpinNotification(youAreASpectatorNotification);
            NotificationManager.SharedInstance.PostNotification(youAreAHiderNotification, true);
            
            Locator.GetPlayerController()._runSpeed = DefaultRunSpeed * 1.5f;

            playerResources._currentFuel = ChangePlayerResources.DefaultMaxFuel * 1.2f;
            playerResources._currentOxygen = ChangePlayerResources.DefaultMaxOxygen * 1.2f;

            ChangePlayerResources.ChangeValues = true;
            ChangePlayerResources.MaxFuel = ChangePlayerResources.DefaultMaxFuel * 1.2f;
            ChangePlayerResources.MaxOxygen = ChangePlayerResources.DefaultMaxOxygen * 1.2f;

            return false;
        }
        
        public override bool SetupSeeker() {
            if (!base.SetupSeeker()) //If the base func snagged out
                return false;
            Utils.WriteLine("Local Player Is Now Seeker", MessageType.Info);
            
            Utils.WriteLine("Removing the Hider Markers", MessageType.Success);
            foreach (PlayerInfo info in PlayerManager.playerInfo.Keys.Except(PlayerManager.seekers)) {
                if (info.IsLocalPlayer)
                    continue;
                
                info.HudMarker.enabled = false;
                info.MapMarker.enabled = false;

                if (PlayerManager.spectators.Contains(info)) {
                    //Turn off spectator just in case
                    info.SetVisible(false);
                }
            } //Turn off markers for everyone excluding seekers
            
            Utils.WriteLine("Adding the Seeker Markers", MessageType.Success);
            foreach (PlayerInfo info in PlayerManager.seekers) {
                if (info.IsLocalPlayer)
                    continue;

                info.HudMarker.enabled = true;
                info.MapMarker.enabled = true;
            }

            Locator.GetPlayerSuit().SuitUp(false, false, true);
            NotificationManager.SharedInstance.UnpinNotification(youAreAHiderNotification);
            NotificationManager.SharedInstance.UnpinNotification(youAreASpectatorNotification);
            NotificationManager.SharedInstance.PostNotification(youAreASeekerNotification, true);
            Locator.GetPlayerController()._runSpeed = DefaultRunSpeed * 1.6f;

            playerResources._currentFuel = ChangePlayerResources.DefaultMaxFuel * 1.2f;
            playerResources._currentOxygen = ChangePlayerResources.DefaultMaxOxygen * 1.2f;

            ChangePlayerResources.ChangeValues = true;
            ChangePlayerResources.MaxFuel = ChangePlayerResources.DefaultMaxFuel * 1.2f;
            ChangePlayerResources.MaxOxygen = ChangePlayerResources.DefaultMaxOxygen * 1.2f;

            return true;
        }

        public override bool SetupSpectator() {
            if (!base.SetupSpectator()) //If the base func snagged out
                return false;
            Utils.WriteLine("Local Player Is now Spectator", MessageType.Info);

            //For now make spectators able to see all
            foreach (PlayerInfo info in PlayerManager.playerInfo.Keys) {
                if (info.IsLocalPlayer)
                    continue;
                info.HudMarker.enabled = true;
                info.MapMarker.enabled = true;
                info.SetVisible(true);
            }
            
            NotificationManager.SharedInstance.UnpinNotification(youAreAHiderNotification);
            NotificationManager.SharedInstance.UnpinNotification(youAreASeekerNotification);
            NotificationManager.SharedInstance.PostNotification(youAreASpectatorNotification, true);

            return true;
        }

        private static PlayerResources _playerResources;
            private static PlayerResources playerResources{
            get{
                if (_playerResources != null)
                    return _playerResources;

                var playerBody = Locator.GetPlayerBody();
                if (playerBody != null)
                    _playerResources = playerBody.GetComponent<PlayerResources>();
            
                return _playerResources;
            }
        }
    }
}