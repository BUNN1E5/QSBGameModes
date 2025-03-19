using System;
using System.Linq;
using HarmonyLib;
using OWML.Common;
using QSB.DeathSync;
using QSB.Player;
using QSB.ShipSync;
using QSBGameModes.GameManagement.PlayerManagement;

namespace QSBGameModes.Patches
{

    [HarmonyPatch]
    public static class HiderPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Necronomicon), nameof(Necronomicon.GetPhrase))]
        public static bool PlayerCaughtDeathMessage(DeathType deathType, int index, ref string __result)
        {
            if (PlayerManager.PlayerDeathTypes.ContainsValue(deathType))
            {
                __result = "{0} got caught by " + Enum.GetName(typeof(DeathType), deathType);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSignal), nameof(AudioSignal.SignalNameToString))]
        public static bool SignalNameToStringPatch(SignalName name, ref string __result)
        {
            if ((int)name < 101) //If we are larger than 101 we are out of the normal range so we are doing player name
                return true;
            __result = QSBPlayerManager.GetPlayer((uint)name + 101).Name;
            return false;
        }
    }
}