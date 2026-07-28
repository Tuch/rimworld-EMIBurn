# CLAUDE.md

Guidance for working in this repo. See `README.md` for the user-facing description.

## What this mod does

RimWorld C# mod (net472). A solar flare ("EMI" event) normally shuts off all
power. EMIBurn instead **keeps power on during a flare and makes powered
buildings catch fire**, so the player must manually flick off electricity.
Targets RimWorld 1.6, depends on Harmony.

## Commands

```bash
./build.sh      # dotnet build Source/EMIBurn.csproj -> Assemblies/net472/EMIBurn.dll
./install.sh    # clean-copy the mod into the local Steam RimWorld Mods/ folder
```

`Source/EMIBurn.csproj` hard-codes local paths to the Steam RimWorld install and
Harmony (`Assembly-CSharp.dll`, `Verse.dll`, `0Harmony.dll`). Update the
`<HintPath>`s if the install location changes.

## Dev loop & publishing (see ADR-0007)

The local `Mods/EMIBurn` copy is the working copy AND the source of Workshop
updates. Loop: edit → `./build.sh && ./install.sh` → test in-game.

A tracked pre-commit hook (`.githooks/pre-commit`) rebuilds `EMIBurn.dll` and
re-stages it whenever a commit touches `Source/*.cs`/`.csproj`, aborting the commit
if the build fails — so the committed binary always matches source. Enable it once
per clone: `git config core.hooksPath .githooks`. (A remote CI build isn't viable:
the `.csproj` references local RimWorld/Harmony DLLs a CI runner doesn't have.)

- **One active copy at a time.** Publishing + subscribing gives two mods with the
  same `packageId` (`Tuch.EMIBurn`) → duplicate-load conflict. Stay **unsubscribed**
  from your own Workshop item while developing.
- **Publish updates from the local copy**: in-game Mods → select EMIBurn → the
  upload button acts as **Update** (because `About/PublishedFileId.txt`, item
  `3772681865`, is tracked in the repo). No subscription needed to publish.
- **Never delete `About/PublishedFileId.txt`** — it is the Workshop link; it's
  tracked so `install.sh`'s wipe-and-copy always restores it.

## Why it's built this way

The non-obvious decisions (patch targets, no XML Defs, flame-explosion ignition,
single-assembly packaging) are recorded as ADRs in [`docs/adr/`](docs/adr/) —
read those before changing the mechanic, and add a new ADR when you change one.

**Rule: every decision updates the docs in the same change.** After any
architectural decision, write/supersede its ADR, add the index row, and sync every
doc it touches (`CLAUDE.md`, `README.md`, language keys, …). A change isn't done
until code and docs agree. See [`docs/adr/README.md`](docs/adr/README.md).

## Architecture (whole mod is 4 files under `Source/`)

- `EMIBurn.cs` — `Mod` entry point; runs `harmony.PatchAll()`.
- `EMIBurnHarmony.cs` — `Patch_GameConditionManager_ElectricityDisabled`: postfix
  on `GameConditionManager.ElectricityDisabled(Map)` forcing `false`. That method
  is the single gate `PowerNet.PowerNetTick` checks before cutting power, so this
  is what keeps electricity running during a flare.
- `MapComponent_EMIBurn.cs` — the fire mechanic. Auto-instantiated on every map
  (no `Def` needed — `Map.FillComponents` reflects over all `MapComponent`
  subclasses). Fires on a random schedule (`nextFireTick`, a random point in the
  settings' `[intervalMin, intervalMax]` range, persisted via ExposeData); when it
  fires during a `GameCondition_DisableElectricity`, it rolls `fireChance` per
  powered colonist consumer and triggers a small `DamageDefOf.Flame` explosion.
  Power **generators** (`CompPowerPlant`, or any `CompPowerTrader` with
  `PowerOutput > 0`) are skipped so the power source is never damaged.
  NOTE: fire can't "attach" to buildings (`CanEverAttachFire` requires a Pawn) and
  steel devices have Flammability 0, so `TryAttachFire`/`TryStartFireIn` are no-ops
  on them — hence the flame explosion.
- `EMIBurnSettings.cs` — a `GameComponent` (also auto-instantiated), so settings
  are **per-save**, editable only after a game is loaded. That is why
  `DoSettingsWindowContents` has a "not available yet" fallback.

There is intentionally **no `Defs/` folder** — everything is driven off the
vanilla solar-flare condition via Harmony + reflection-registered components.

## RimWorld internals this relies on (verified against decompiled 1.6)

- Solar flare uses `GameCondition_DisableElectricity` (`ElectricityDisabled => true`).
- `PowerNet.PowerNetTick` cuts power only when
  `Map.gameConditionManager.ElectricityDisabled(Map)` is true — the one method we patch.
- `GameConditionManager.ActiveConditions` exposes the active conditions list.
- `FireUtility.TryAttachFire(this Thing, float fireSize, Thing instigator)` — 3 args in 1.6; instigator may be null.

## Gotchas (learned the hard way)

- **RimWorld loads `Assemblies/` recursively** (`GetFiles(..., SearchOption.AllDirectories)`).
  So do NOT ship the DLL in two places (e.g. `Assemblies/EMIBurn.dll` *and*
  `Assemblies/net472/EMIBurn.dll`) — it loads twice → duplicate Harmony patches
  and type clashes. `install.sh` ships exactly one copy at `Assemblies/EMIBurn.dll`.
- A `[HarmonyPatch]` class using `TargetMethod()` that returns `null` crashes mod
  init with `TargetMethod() returned an unexpected result: null`. This was the
  original startup crash (a dead reflection-scanning patch, since removed).

## Reading vanilla source (decompiler)

`ilspycmd` is installed as a global dotnet tool but needs env overrides on this
machine (only .NET 9 is present; the tool targets .NET 6):

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
export DOTNET_ROOT="/opt/homebrew/Cellar/dotnet/9.0.7/libexec"
export DOTNET_ROLL_FORWARD=Major
ASM="/Users/tuch/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Contents/Resources/Data/Managed/Assembly-CSharp.dll"
ilspycmd -p "$ASM" -o /tmp/rwproj    # whole assembly to a project; then grep
ilspycmd -t RimWorld.CompPowerTrader "$ASM"   # single type to stdout
```

## Testing

No automated tests (RimWorld mod). Verify in-game: dev mode → trigger the
**Solar Flare** incident → power should stay ON and powered buildings should
start catching fire.
