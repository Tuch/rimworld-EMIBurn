using System.Collections.Generic;
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

        // Reused scratch list for the at-risk candidates so the per-interval / per-frame scans
        // don't allocate.
        private readonly List<Building> atRiskScratch = new List<Building>();

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

        // Draw a warning marker over every at-risk device while a flare is active, so the player
        // can see what to switch off. DrawOverlay is transient (re-queued every frame) and cleared
        // by OverlayDrawer.DrawAllOverlays; only enqueue for the drawn map, otherwise the queue for
        // a non-current map never gets flushed. See ADR-0010.
        public override void MapComponentUpdate()
        {
            if (map != Find.CurrentMap)
                return;

            var settings = Current.Game?.GetComponent<EMIBurnSettings>();
            if (settings == null || !settings.showRiskOverlay || !SolarFlareActive())
                return;

            var buildings = map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < buildings.Count; i++)
            {
                var building = buildings[i];
                if (IsAtRisk(building, settings))
                    map.overlayDrawer.DrawOverlay(building, OverlayTypes.QuestionMark);
            }
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

        // One roll per interval decides whether *anything* burns; on a hit we ignite a single
        // random at-risk device. Rolling fireChance per-device meant ~fireChance x deviceCount
        // fires per interval — on a large colony that's near-guaranteed mass death. See ADR-0009.
        private void TryIgnitePoweredThings(EMIBurnSettings settings)
        {
            if (Rand.Value >= settings.fireChance)
                return;

            atRiskScratch.Clear();
            var buildings = map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < buildings.Count; i++)
            {
                var building = buildings[i];
                if (IsAtRisk(building, settings))
                    atRiskScratch.Add(building);
            }

            if (atRiskScratch.Count == 0)
                return;

            var target = atRiskScratch.RandomElement();
            IgniteBuilding(target);
            if (settings.enableNotifications)
            {
                Messages.Message(
                    "EMIBurn_AlertFire".Translate(target.LabelCap),
                    target,
                    MessageTypeDefOf.ThreatBig);
            }

            atRiskScratch.Clear();
        }

        // A device is at risk if it's a powered colonist consumer drawing at least the configured
        // minimum. Power sources (generators — solar, geothermal, wind, ... incl. modded ones) are
        // exempt: they're producers you can't switch off to protect. CompPowerPlant covers virtually
        // all of them; PowerOutput > 0 is a safety net for any odd producer that doesn't derive from it.
        private static bool IsAtRisk(Building building, EMIBurnSettings settings)
        {
            var comp = building.GetComp<CompPowerTrader>();
            if (comp == null || !comp.PowerOn)
                return false;

            if (building.GetComp<CompPowerPlant>() != null || comp.PowerOutput > 0f)
                return false;

            // Consumers draw power, so PowerOutput is negative; its magnitude is the draw.
            // Too little draw to "overheat" -> not a target.
            float consumption = -comp.PowerOutput;
            return consumption >= settings.minPowerConsumption;
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
