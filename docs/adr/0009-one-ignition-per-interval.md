# ADR-0009: Ignite one random device per interval, not per-device rolls

- Status: Accepted
- Date: 2026-08-11

## Context

The original fire mechanic looped over every powered colonist consumer each interval
and rolled `fireChance` independently for each one. On a real colony there are often a
hundred or more powered devices, so even at a low 5% per-device chance the expected
number of ignitions per interval is `0.05 x deviceCount` — roughly five simultaneous
flame explosions on a 100-device base, every interval, for the whole flare. In testing
this was a near-guaranteed wipe: the flare didn't create a manageable hazard to react
to, it deleted the colony.

We also want trivially small loads (a standing lamp, a hidden conduit-level draw) to be
exempt — they read as "can't really overheat and explode".

## Decision

Change the probability model to **one roll per interval, then one victim**:

1. Roll `fireChance` a single time. If it misses, nothing burns this interval.
2. On a hit, collect the **at-risk** devices and ignite exactly one, chosen at random.

A device is *at risk* when it is a powered colonist consumer (`CompPowerTrader.PowerOn`)
that is **not** a generator (ADR-0004's exemption: `CompPowerPlant` or `PowerOutput > 0`)
**and** draws at least `minPowerConsumption` watts (`-CompPowerTrader.PowerOutput`, a new
per-save setting, default `100`). The eligibility test lives in one predicate
(`IsAtRisk`) shared by ignition and the at-risk overlay (ADR-0010).

`fireChance` now means "chance that **a** device ignites this interval", not "chance per
device". Default stays `0.05`.

## Consequences

- Flare lethality is bounded and tunable: at most one ignition per interval regardless of
  colony size, so a big base is no longer punished for being big.
- `minPowerConsumption` lets the player exclude low-draw devices from both ignition and
  the overlay; at `0` every powered consumer is eligible.
- The meaning of `fireChance` changed. Documented in `README.md`'s settings table and the
  reworded `EMIBurn_FireChanceLabel` ("per interval"); old saves keep their stored value,
  which is now interpreted under the new model.

## Alternatives considered

- **Keep per-device rolls but drop `fireChance` far** — still scales lethality with colony
  size (the exact problem), just with a different constant; a large base stays doomed.
- **Cap the number of per-interval ignitions to N** — more knobs, same idea as "one" with
  N=1; one victim is the simplest bound that makes the hazard readable.
- **Damage instead of ignite the picked device** — loses the "spreading fire you must
  contain" tension that is the point of the mod (see ADR-0004).
