using OWML.Common;
using QSB.Player;

namespace HideAndSeek{
    public class LocalInfo : HideAndSeekInfo{
        public override bool SetupHider() {
            if (!base.SetupHider()) //If the base func snagged out
                return false;

            Utils.WriteLine("Local Player Is Now Hider", MessageType.Info);
            Utils.WriteLine("Removing the HUD Markers", MessageType.Success);
            foreach (PlayerInfo _info in QSBPlayerManager.PlayerList){
                if (_info.Body != null){
                    _info.MapMarker.enabled = false;
                    _info.HudMarker.enabled = false;
                }
            }

            return false;
        }
        
        public override bool SetupSeeker() {
            if (!base.SetupSeeker()) //If the base func snagged out
                return false;
            Utils.WriteLine("Local Player Is Now Seeker", MessageType.Info);
            Utils.WriteLine("Adding the Seeker Markers", MessageType.Success);
            foreach (PlayerInfo _info in PlayerManager.seekers) {
                _info.HudMarker.enabled = true;
                _info.MapMarker.enabled = true;
            }
            
            return true;
        }

        public override bool SetupSpectator() {
            if (!base.SetupSpectator()) //If the base func snagged out
                return false;
            Utils.WriteLine("Local Player Is now Spectator", MessageType.Info);
            return true;
        }
    }
}