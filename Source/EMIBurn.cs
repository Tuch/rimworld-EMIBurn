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

            list.Label("EMIBurn_IntervalMinLabel".Translate(settings.intervalMinTicks));
            settings.intervalMinTicks = (int)list.Slider(settings.intervalMinTicks, 1000, 10000);

            list.Label("EMIBurn_IntervalMaxLabel".Translate(settings.intervalMaxTicks));
            settings.intervalMaxTicks = (int)list.Slider(settings.intervalMaxTicks, 1000, 10000);

            // Keep the range valid: min must not exceed max.
            if (settings.intervalMinTicks > settings.intervalMaxTicks)
            {
                settings.intervalMaxTicks = settings.intervalMinTicks;
            }

            list.CheckboxLabeled("EMIBurn_NotifyLabel".Translate(), ref settings.enableNotifications);

            list.Gap(12f);
            if (list.ButtonText("EMIBurn_ResetButton".Translate()))
            {
                settings.Reset();
                Messages.Message("EMIBurn_ResetMessage".Translate(), MessageTypeDefOf.RejectInput);
            }

            // Dev-only: fire an EMI event (vanilla solar flare) on the current map for testing.
            if (Prefs.DevMode)
            {
                list.GapLine();
                list.Label("EMIBurn_DevSectionLabel".Translate());
                if (list.ButtonText("EMIBurn_DevTriggerButton".Translate()))
                {
                    TriggerEMIEvent();
                }
            }

            list.End();
        }

        private static void TriggerEMIEvent()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                Messages.Message("EMIBurn_DevNoMap".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }

            IncidentDef incident = IncidentDefOf.SolarFlare;
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(incident.category, map);
            if (incident.Worker.TryExecute(parms))
            {
                Messages.Message("EMIBurn_DevTriggered".Translate(), MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message("EMIBurn_DevFailed".Translate(), MessageTypeDefOf.RejectInput);
            }
        }

        public override string SettingsCategory()
        {
            return "EMIBurn_SettingsCategory".Translate();
        }
    }
}