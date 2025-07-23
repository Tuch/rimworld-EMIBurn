using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace EMIBurn
{
    public class EMIBurn : Mod
    {
        public EMIBurn(ModContentPack content) : base(content) 
        {
            // Initialize Harmony for applying patches
            var harmony = new Harmony("com.yourname.EMIBurn");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var settings = Current.Game?.GetComponent<EMIBurnSettings>();
            if (settings == null)
            {
                Rect topRect = inRect.TopPart(0.2f);
                Widgets.Label(topRect, "EMIBurn_NotAvailable".Translate());
                return;
            }

            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);

            list.Label("EMIBurn_FireChanceLabel".Translate((settings.fireChance * 100f).ToString("F0")));
            settings.fireChance = list.Slider(settings.fireChance, 0f, 1f);

            list.Label("EMIBurn_IntervalLabel".Translate(settings.intervalTicks));
            settings.intervalTicks = (int)list.Slider(settings.intervalTicks, 1000, 10000);

            list.CheckboxLabeled("EMIBurn_NotifyLabel".Translate(), ref settings.enableNotifications);

            list.Gap(12f);
            if (list.ButtonText("EMIBurn_ResetButton".Translate()))
            {
                settings.Reset();
                Messages.Message("EMIBurn_ResetMessage".Translate(), MessageTypeDefOf.RejectInput);
            }

            list.End();
        }

        public override string SettingsCategory()
        {
            return "EMIBurn_SettingsCategory".Translate();
        }
    }
}