using System.Linq;
using OWML.Common;
using QSB.Player;
using QSB.ShipSync;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.Patches;

namespace QSBGameModes{
    public class LocalInfo : GameModeInfo{

        protected float DefaultRunSpeed = 6f;

        public override bool Reset(){
            if (!base.Reset()) //If the base func snagged out
                return false;
            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.catcherNotification);
            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.spectatorNotification);
            NotificationManager.SharedInstance.PostNotification(GameManager.gameMode.catcheeNotification);
            return true;
        }

        public override bool SetupHider() {
            if (!base.SetupHider()) //If the base func snagged out
                return false;

            Utils.WriteLine($"Local Player Is Now Hider ({Info.PlayerId})", MessageType.Info);
            Utils.WriteLine("Removing the All Markers", MessageType.Success);
            foreach (PlayerInfo info in PlayerManager.playerInfo.Keys) {
                if (info.IsLocalPlayer) continue;
                if (info.Body == null) continue;

                if (PlayerManager.spectators.Contains(info)) {
                    //Turn off spectator just in case
                    info.SetVisible(false);
                    info.MapMarker.enabled = false;
                    info.HudMarker.enabled = false;
                    continue;
                }
                
                info.MapMarker.enabled = false;
                info.HudMarker.enabled = true;
            }
            
            ShipManager.Instance.CockpitController._interactVolume.DisableInteraction();

            var playerSuit = Locator.GetPlayerSuit();
            if (!playerSuit.IsWearingSuit()){
                playerSuit.SuitUp(false, true, true);
            }

            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.catcherNotification);
            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.spectatorNotification);
            NotificationManager.SharedInstance.PostNotification(GameManager.gameMode.catcheeNotification, true);
            
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
            Utils.WriteLine($"Local Player Is Now Seeker ({Info.PlayerId})", MessageType.Info);
            
            Utils.WriteLine("Removing the Hider Markers", MessageType.Success);
            foreach (PlayerInfo info in PlayerManager.playerInfo.Keys) {
                if (info.IsLocalPlayer) continue;
                if (info.Body == null) continue;

                if (PlayerManager.seekers.Contains(info)){
                    info.HudMarker.enabled = true;
                    info.MapMarker.enabled = true;
                }
                
                info.HudMarker.enabled = false;
                info.MapMarker.enabled = false;

                if (PlayerManager.spectators.Contains(info)) {
                    //Turn off spectator just in case
                    info.SetVisible(false);
                }
            } //Turn off markers for everyone excluding seekers

            ShipManager.Instance.CockpitController._interactVolume.EnableInteraction();

            var playerSuit = Locator.GetPlayerSuit();
            if (!playerSuit.IsWearingSuit()){
                playerSuit.SuitUp(false, true, true);
            }

            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.catcheeNotification);
            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.spectatorNotification);
            NotificationManager.SharedInstance.PostNotification(GameManager.gameMode.catcherNotification, true);
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
            Utils.WriteLine($"Local Player Is now Spectator ({Info.PlayerId})", MessageType.Info);

            //For now make spectators able to see all
            foreach (PlayerInfo info in PlayerManager.playerInfo.Keys) {
                if (info.IsLocalPlayer)
                    continue;
                
                info.HudMarker.enabled = true;
                info.MapMarker.enabled = true;
                info.SetVisible(true);
            }
            
            //Spectators cannot use the ship
            ShipManager.Instance.CockpitController._interactVolume.DisableInteraction();
            
            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.catcheeNotification);
            NotificationManager.SharedInstance.UnpinNotification(GameManager.gameMode.catcherNotification);
            NotificationManager.SharedInstance.PostNotification(GameManager.gameMode.spectatorNotification, true);

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