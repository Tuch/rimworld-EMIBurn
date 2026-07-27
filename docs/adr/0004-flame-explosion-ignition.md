# ADR-0004: Ignite powered devices with a flame explosion

- Status: Accepted
- Date: 2026-07-27

## Context

"A powered device catches fire" turned out not to work the obvious way. In 1.6:

- `Thing.TryAttachFire` only attaches fire to **pawns** — `CanEverAttachFire`
  returns false unless `def.category == Pawn`. On a building it silently no-ops.
- `FireUtility.TryStartFireIn` only ignites a cell whose contents have
  `Flammability > 0`. Most powered devices (machining table, generators,
  crematorium) are **steel, Flammability 0**, so it no-ops too.

The result during testing: the "caught fire" alert fired but nothing burned. We
also want the power *sources* spared — a generator is the thing you can't switch
off to protect.

## Decision

On ignition, trigger a small `DamageDefOf.Flame` explosion at the device's cell
(`GenExplosion.DoExplosion`, radius ~1.5, `chanceToStartFire: 1`). **Exempt power
producers**: skip any building with `CompPowerPlant`, or whose `CompPowerTrader`
has `PowerOutput > 0`.

## Consequences

- Works uniformly on flammable *and* non-flammable devices: the device takes flame
  damage (overheat burnout) and flammable surroundings ignite and can spread.
- Generators (solar, geothermal, wind, incl. modded — anything `CompPowerPlant`)
  are never damaged; only powered consumers are at risk, matching the "switch it
  off to protect it" design.
- The effect is an instantaneous burst rather than a sustained fire on the device,
  and at high fire-chance it is destructive — mitigated by user-tunable settings.

## Alternatives considered

- **`TryAttachFire`** — the original bug; no-ops on every building.
- **`TryStartFireIn`** — no-ops on non-flammable steel devices (most of them).
- **Direct HP/Flame damage without an explosion** — less thematic and doesn't
  ignite the surroundings, losing the "fire spreads, act fast" pressure.
- **Force-spawn a `Fire` on the cell** (bypassing the flammability check) — fire
  with no fuel on a steel device fizzles immediately; inconsistent results.
- **Damage generators too** — punishes the player for something they can't turn
  off, which contradicts the mod's core loop.
