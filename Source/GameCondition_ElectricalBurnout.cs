using Verse;
using RimWorld;
using System.Linq;

namespace EMIBurn
{
    public class GameCondition_ElectricalBurnout : GameCondition
    {
        private int tickCounter = 0;

        public override void GameConditionTick()
        {
            base.GameConditionTick();
            tickCounter++;

            var settings = Current.Game?.GetComponent<EMIBurnSettings>();
            if (settings == null) return;

            if (tickCounter % settings.intervalTicks == 0)
            {
                foreach (Map map in AffectedMaps)
                {
                    if (HasAnyEMISource(map))
                    {
                        TryIgnitePoweredThings(map, settings);
                    }
                }
            }
        }

        private bool HasAnyEMISource(Map map)
        {
            // Check for EMI Dynamo (building)
            bool hasDynamo = map.listerBuildings.allBuildingsColonist
                .Any(b => b.def.defName == "EMIDynamo");

            // Check for EMI Event (GameCondition)
            bool hasEvent = map.gameConditionManager.ActiveConditions
                .Any(gc => gc.def.defName == "EMIField");

            return hasDynamo || hasEvent;
        }

        private void TryIgnitePoweredThings(Map map, EMIBurnSettings settings)
        {
            foreach (var building in map.listerBuildings.allBuildingsColonist)
            {
                var comp = building.GetComp<CompPowerTrader>();
                if (comp != null && comp.PowerOn)
                {
                    if (Rand.Value < settings.fireChance)
                    {
                        GenExplosion.DoExplosion(building.Position, map, 1.9f, DamageDefOf.Flame, null);
                        if (settings.enableNotifications)
                        {
                            Messages.Message("EMIBurn_AlertFire".Translate(building.LabelCap), building, MessageTypeDefOf.ThreatBig);
                        }
                    }
                }
            }
        }
    }
}
