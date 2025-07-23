using HarmonyLib;
using Verse;
using RimWorld;

namespace EMIBurn
{
    [HarmonyPatch(typeof(Building), "Tick")]
    public static class Patch_EMIDynamo_Tick
    {
        public static bool Prefix(Building __instance)
        {
            // Disable standard EMI Dynamo behavior (power shutdown)
            // Check if this is specifically an EMI Dynamo
            if (__instance.def.defName == "EMIDynamo")
            {
                return false; // Disable standard behavior
            }
            return true; // Keep standard behavior for other buildings
        }
    }

    [HarmonyPatch(typeof(GameCondition), "Tick")]
    public static class Patch_EMIField_Tick
    {
        public static bool Prefix(GameCondition __instance)
        {
            // Disable standard EMI Field behavior (power shutdown)
            // Check if this is specifically an EMI Field
            if (__instance.def.defName == "EMIField")
            {
                return false; // Disable standard behavior
            }
            return true; // Keep standard behavior for other conditions
        }
    }
} 