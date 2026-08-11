# EMIBurn

A RimWorld mod that **changes what a solar flare does to your electricity**.

In vanilla, a solar flare (an "EMI" event) simply shuts all power off until it
passes. EMIBurn replaces that: **power keeps running, but your energized
buildings can overheat and catch fire.** To stay safe you now have to *manually*
flick off power to anything you don't want burning.

Supported RimWorld version: **1.6**. Requires [Harmony](https://github.com/pardeike/HarmonyRimWorld).

## How it works

Two pieces do all the work:

- **`Patch_GameConditionManager_ElectricityDisabled`** (`Source/EMIBurnHarmony.cs`) —
  a Harmony postfix on `GameConditionManager.ElectricityDisabled(Map)`, the single
  gate that `PowerNet.PowerNetTick` checks before cutting power. Forcing it to
  `false` keeps electricity flowing during a solar flare instead of shutting down.

- **`MapComponent_EMIBurn`** (`Source/MapComponent_EMIBurn.cs`) —
  auto-instantiated on every map (no XML `Def` needed). While a solar flare
  (`GameCondition_DisableElectricity`) is active, on a random schedule within the
  settings' `[min, max]` interval it rolls `fireChance` **once**; on a hit it picks
  **one random at-risk device** and triggers a small flame explosion — damaging the
  device and igniting flammable surroundings — then optionally posts an alert. (Rolling
  per-device meant near-certain mass death on a large base — see
  [ADR-0009](docs/adr/0009-one-ignition-per-interval.md).) An *at-risk* device is a
  powered colonist *consumer* drawing at least the configured minimum; power generators
  are exempt (see [ADR-0004](docs/adr/0004-flame-explosion-ignition.md)). While the flare
  lasts, every at-risk device also shows a warning marker so you can react (see
  [ADR-0010](docs/adr/0010-at-risk-overlay.md)).

Settings live in **`EMIBurnSettings`**, a per-save `GameComponent`, so they are
stored in the save file and are only editable once a game is loaded.

## Settings

Configurable in *Options → Mod settings → EMIBurn* (after starting a game):

| Setting | Default | Meaning |
| --- | --- | --- |
| Fire chance per interval (%) | 5% | Chance, each interval, that **one** random at-risk device bursts into flame during a flare |
| Min interval (ticks) | 2500 | Lower bound of the random gap between fire checks (2500 ticks ≈ 1 in-game hour) |
| Max interval (ticks) | 7500 | Upper bound — each check waits a random amount in `[min, max]` |
| Min. power draw to be at risk (W) | 100 | Devices drawing less than this are never targeted or marked (`0` = every powered consumer) |
| Show fire notifications | on | Post a threat alert when a device ignites |
| Show warning marker over at-risk devices | on | While a flare is active, mark every at-risk device so you can switch it off |

Power **generators** (solar, geothermal, wind, … including modded ones — anything
with `CompPowerPlant`) are exempt: they're the power source you can't switch off to
protect, so they never take EMI damage. Only powered *consumers* are at risk.

## Build

Requires the .NET SDK. Reference paths in `Source/EMIBurn.csproj` point at a
local Steam install of RimWorld and Harmony — adjust them if yours differs.

```bash
./build.sh
```

Output: `Assemblies/net472/EMIBurn.dll`.

## Install into the game

```bash
./build.sh && ./install.sh
```

`install.sh` does a clean copy of the mod (About, Assemblies, Languages) into
your RimWorld `Mods/EMIBurn` folder, excluding source and build junk. Restart
RimWorld and enable EMIBurn (below Harmony) in the mod list.

## Testing it

1. Load a save with a powered colony.
2. Open the dev console (dev mode) and trigger the **Solar Flare** incident.
3. Confirm power stays **on**, and that powered buildings start catching fire.
   Flick off anything flammable to survive the flare.
