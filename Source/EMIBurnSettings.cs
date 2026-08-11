using Verse;

namespace EMIBurn
{
    public class EMIBurnSettings : GameComponent
    {
        private const float DEFAULT_FIRE_CHANCE = 0.05f;
        private const int DEFAULT_INTERVAL_MIN_TICKS = 2500;
        private const int DEFAULT_INTERVAL_MAX_TICKS = 7500;
        private const float DEFAULT_MIN_POWER_CONSUMPTION = 100f;
        private const bool DEFAULT_ENABLE_NOTIFICATIONS = true;
        private const bool DEFAULT_SHOW_RISK_OVERLAY = true;

        // Per interval: chance that *one* device ignites (a single roll, not per-device — see ADR-0009).
        public float fireChance = DEFAULT_FIRE_CHANCE;
        // Fire checks fire at a random interval picked from [min, max] each time.
        public int intervalMinTicks = DEFAULT_INTERVAL_MIN_TICKS;
        public int intervalMaxTicks = DEFAULT_INTERVAL_MAX_TICKS;
        // Devices drawing less than this (in watts) are considered too weak to "overheat" and are
        // excluded from both ignition and the at-risk overlay.
        public float minPowerConsumption = DEFAULT_MIN_POWER_CONSUMPTION;
        public bool enableNotifications = DEFAULT_ENABLE_NOTIFICATIONS;
        // Draw a warning marker over every at-risk device while a flare is active (see ADR-0010).
        public bool showRiskOverlay = DEFAULT_SHOW_RISK_OVERLAY;

        public EMIBurnSettings(Game game) { }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref fireChance, "fireChance", DEFAULT_FIRE_CHANCE);
            Scribe_Values.Look(ref intervalMinTicks, "intervalMinTicks", DEFAULT_INTERVAL_MIN_TICKS);
            Scribe_Values.Look(ref intervalMaxTicks, "intervalMaxTicks", DEFAULT_INTERVAL_MAX_TICKS);
            Scribe_Values.Look(ref minPowerConsumption, "minPowerConsumption", DEFAULT_MIN_POWER_CONSUMPTION);
            Scribe_Values.Look(ref enableNotifications, "enableNotifications", DEFAULT_ENABLE_NOTIFICATIONS);
            Scribe_Values.Look(ref showRiskOverlay, "showRiskOverlay", DEFAULT_SHOW_RISK_OVERLAY);
        }

        public void Reset()
        {
            fireChance = DEFAULT_FIRE_CHANCE;
            intervalMinTicks = DEFAULT_INTERVAL_MIN_TICKS;
            intervalMaxTicks = DEFAULT_INTERVAL_MAX_TICKS;
            minPowerConsumption = DEFAULT_MIN_POWER_CONSUMPTION;
            enableNotifications = DEFAULT_ENABLE_NOTIFICATIONS;
            showRiskOverlay = DEFAULT_SHOW_RISK_OVERLAY;
        }
    }
}
