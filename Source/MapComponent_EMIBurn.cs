using RimWorld;
using UnityEngine;
using Verse;

namespace EMIBurn
{
    // Auto-instantiated on every map by Map.FillComponents (no Def required).
    // While a solar flare (GameCondition_DisableElectricity) is active, powered colonist
    // devices can overheat and burst into flame instead of just losing power. Combined with
    // the Harmony patch that keeps electricity ON, this is the whole "EMI burns instead of
    // shuts down" mechanic: the player must manually flick off power to stay safe.
    public class MapComponent_EMIBurn : MapComponent
    {
        // Absolute game tick of the next fire check. Randomized within the configured
        // [min, max] range each time so the timing isn't predictable. Persisted so a
        // reloaded save doesn't fire immediately.
        private int nextFireTick = -1;

        public MapComponent_EMIBurn(Map map) : base(map) { }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextFireTick, "nextFireTick", -1);
        }

        public override void MapComponentTick()
        {
            var settings = Current.Game?.GetComponent<EMIBurnSettings>();
            if (settings == null)
                return;

            int now = Find.TickManager.TicksGame;

            // First tick after load/spawn: just arm the timer, don't fire.
            if (nextFireTick < 0)
            {
                ScheduleNext(settings, now);
                return;
            }

            if (now < nextFireTick)
                return;

            ScheduleNext(settings, now);

            if (SolarFlareActive())
                TryIgnitePoweredThings(settings);
        }

        private void ScheduleNext(EMIBurnSettings settings, int now)
        {
            int min = Mathf.Max(1, Mathf.Min(settings.intervalMinTicks, settings.intervalMaxTicks));
            int max = Mathf.Max(1, Mathf.Max(settings.intervalMinTicks, settings.intervalMaxTicks));
            nextFireTick = now + Rand.RangeInclusive(min, max);
        }

        private bool SolarFlareActive()
        {
            var conditions = map.gameConditionManager.ActiveConditions;
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i] is GameCondition_DisableElectricity)
                    return true;
            }
            return false;
        }

        private void TryIgnitePoweredThings(EMIBurnSettings settings)
        {
            var buildings = map.listerBuildings.allBuildingsColonist;
            // Reverse loop: the explosion may destroy a building, mutating the list.
            for (int i = buildings.Count - 1; i >= 0; i--)
            {
                var building = buildings[i];
                var comp = building.GetComp<CompPowerTrader>();
                if (comp == null || !comp.PowerOn)
                    continue;

                // Never damage power sources. Generators (solar, geothermal, wind, ... including
                // modded ones) are producers you can't switch off to protect, so they are exempt.
                // CompPowerPlant covers virtually all of them; PowerOutput > 0 is a safety net for
                // any odd producer that doesn't derive from it.
                if (building.GetComp<CompPowerPlant>() != null || comp.PowerOutput > 0f)
                    continue;

                if (Rand.Value < settings.fireChance)
                {
                    IgniteBuilding(building);
                    if (settings.enableNotifications)
                    {
                        Messages.Message(
                            "EMIBurn_AlertFire".Translate(building.LabelCap),
                            building,
                            MessageTypeDefOf.ThreatBig);
                    }
                }
            }
        }

        // Vanilla fire can only "attach" to pawns, and most powered devices are steel
        // (Flammability 0), so a fire won't catch on them directly. Model the overheating
        // burnout as a small flame burst instead: it damages the device and ignites anything
        // flammable around it (wooden walls, furniture, chemfuel, etc.).
        private void IgniteBuilding(Building building)
        {
            GenExplosion.DoExplosion(
                center: building.Position,
                map: map,
                radius: 1.5f,
                damType: DamageDefOf.Flame,
                instigator: null,
                chanceToStartFire: 1f);
        }
    }
}
