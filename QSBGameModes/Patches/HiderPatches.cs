using System;
using System.Linq;
using HarmonyLib;
using OWML.Common;
using QSB.DeathSync;
using QSB.Player;
using QSB.ShipSync;
using QSBGameModes.GameManagement.PlayerManagement;

namespace QSBGameModes.Patches{
    
    [HarmonyPatch]
    public static class HiderPatches{
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Necronomicon), nameof(Necronomicon.GetPhrase))]
        public static bool PlayerCaughtDeathMessage(DeathType deathType, int index, ref string __result){
            if (PlayerManager.PlayerDeathTypes.ContainsValue(deathType)){
                __result = "{0} got caught by " + Enum.GetName(typeof(DeathType), deathType);
                return false;
            }
            return true;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSignal), nameof(AudioSignal.SignalNameToString))]
        public static bool SignalNameToStringPatch(SignalName name, ref string __result){
            bool rval = true;
            string result = "";
            PlayerManager.PlayerInfos.DoIf((kvp) => 
                kvp.Value.GetType() == typeof(RemoteInfo), (kvp) => {
                result = kvp.Value.Info.Name;
                rval = false;
            });
            __result = result;
            return rval; // This cause dumb
        }
    }
}