using HarmonyLib;
using RimWorld;
using Verse;

namespace EMIBurn
{
    // Core of the mod: a solar flare (an "EMI" event) uses GameCondition_DisableElectricity,
    // whose ElectricityDisabled flag makes PowerNet.PowerNetTick cut all power.
    // GameConditionManager.ElectricityDisabled(Map) is the single gate every power net checks.
    // We force it to false so power KEEPS running during a flare. The overheating/fire risk
    // that replaces the shutdown is applied by MapComponent_EMIBurn.
    [HarmonyPatch(typeof(GameConditionManager), nameof(GameConditionManager.ElectricityDisabled))]
    public static class Patch_GameConditionManager_ElectricityDisabled
    {
        static void Postfix(ref bool __result)
        {
            __result = false;
        }
    }
}
