using System.Linq;
using OWML.Common;
using QSB.Player;

namespace HideAndSeek{
    public class LocalInfo : HideAndSeekInfo{
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

            return true;
        }
    }
}