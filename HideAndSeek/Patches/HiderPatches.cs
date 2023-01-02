using HarmonyLib;
using OWML.Common;
using QSB.DeathSync;
using QSB.Player;
using QSB.ShipSync;

namespace HideAndSeek.Patches{
    
    [HarmonyPatch]
    public static class HiderPatches{
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipManager), nameof(ShipManager.CurrentFlyer), MethodType.Setter)]
        public static bool PreventHidersFromUsingShip(uint value){
            if (PlayerManager.hiders.Contains(QSBPlayerManager.GetPlayer(value))){
                Utils.WriteLine("A Hider just tried to fly the ship", MessageType.Warning);
                return false;
            }
            return true;
        }

        [Harmony]
        [HarmonyPatch(typeof(Necronomicon), nameof(Necronomicon.GetPhrase))]
        public static bool PlayerCaughtDeathMessage(DeathType deathType, int index, ref string __result){
            return false;
        }
    }
}