using System;
using HarmonyLib;
using HideAndSeek.GameManagement.PlayerManagement;
using OWML.Common;
using QSB.DeathSync;
using QSB.Player;
using QSB.ShipSync;

namespace HideAndSeek.Patches{
    
    [HarmonyPatch]
    public static class HiderPatches{
        [Harmony]
        [HarmonyPatch(typeof(Necronomicon), nameof(Necronomicon.GetPhrase))]
        public static bool PlayerCaughtDeathMessage(DeathType deathType, int index, ref string __result){
            if (PlayerManager.PlayerDeathTypes.ContainsValue(deathType)){
                __result = "{0} got caught by " + Enum.GetName(typeof(DeathType), deathType);
                return false;
            }
            return true;
        }
    }
}