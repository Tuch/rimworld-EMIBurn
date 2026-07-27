using Verse;

namespace EMIBurn
{
    public class EMIBurnSettings : GameComponent
    {
        private const float DEFAULT_FIRE_CHANCE = 0.05f;
        private const int DEFAULT_INTERVAL_MIN_TICKS = 2500;
        private const int DEFAULT_INTERVAL_MAX_TICKS = 7500;
        private const bool DEFAULT_ENABLE_NOTIFICATIONS = true;

        public float fireChance = DEFAULT_FIRE_CHANCE;
        // Fire checks fire at a random interval picked from [min, max] each time.
        public int intervalMinTicks = DEFAULT_INTERVAL_MIN_TICKS;
        public int intervalMaxTicks = DEFAULT_INTERVAL_MAX_TICKS;
        public bool enableNotifications = DEFAULT_ENABLE_NOTIFICATIONS;

        public EMIBurnSettings(Game game) { }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref fireChance, "fireChance", DEFAULT_FIRE_CHANCE);
            Scribe_Values.Look(ref intervalMinTicks, "intervalMinTicks", DEFAULT_INTERVAL_MIN_TICKS);
            Scribe_Values.Look(ref intervalMaxTicks, "intervalMaxTicks", DEFAULT_INTERVAL_MAX_TICKS);
            Scribe_Values.Look(ref enableNotifications, "enableNotifications", DEFAULT_ENABLE_NOTIFICATIONS);
        }

        public void Reset()
        {
            fireChance = DEFAULT_FIRE_CHANCE;
            intervalMinTicks = DEFAULT_INTERVAL_MIN_TICKS;
            intervalMaxTicks = DEFAULT_INTERVAL_MAX_TICKS;
            enableNotifications = DEFAULT_ENABLE_NOTIFICATIONS;
        }
    }
}
