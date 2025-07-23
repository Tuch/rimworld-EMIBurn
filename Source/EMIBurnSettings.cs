using Verse;

namespace EMIBurn
{
    public class EMIBurnSettings : GameComponent
    {
        private const float DEFAULT_FIRE_CHANCE = 0.05f;
        private const int DEFAULT_INTERVAL_TICKS = 3000;
        private const bool DEFAULT_ENABLE_NOTIFICATIONS = true;

        public float fireChance = DEFAULT_FIRE_CHANCE;
        public int intervalTicks = DEFAULT_INTERVAL_TICKS;
        public bool enableNotifications = DEFAULT_ENABLE_NOTIFICATIONS;

        public EMIBurnSettings(Game game) { }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref fireChance, "fireChance", DEFAULT_FIRE_CHANCE);
            Scribe_Values.Look(ref intervalTicks, "intervalTicks", DEFAULT_INTERVAL_TICKS);
            Scribe_Values.Look(ref enableNotifications, "enableNotifications", DEFAULT_ENABLE_NOTIFICATIONS);
        }

        public void Reset()
        {
            fireChance = DEFAULT_FIRE_CHANCE;
            intervalTicks = DEFAULT_INTERVAL_TICKS;
            enableNotifications = DEFAULT_ENABLE_NOTIFICATIONS;
        }
    }
}
