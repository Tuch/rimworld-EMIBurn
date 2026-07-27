# ADR-0002: Keep electricity on during solar flares

- Status: Accepted
- Date: 2026-07-27

## Context

EMIBurn's premise is that an "EMI" event (the vanilla solar flare) should burn
powered devices instead of cutting power. Decompiling 1.6 showed the exact
mechanism: the solar flare uses `GameCondition_DisableElectricity`
(`ElectricityDisabled => true`), and `PowerNet.PowerNetTick` cuts power only when
`GameConditionManager.ElectricityDisabled(Map)` returns true. That manager method
is the **single gate** every power net consults.

## Decision

Apply a Harmony **postfix** to `GameConditionManager.ElectricityDisabled(Map)` that
forces the result to `false`. Power then keeps flowing during a solar flare (and
any other electricity-disabling condition), leaving the fire risk (ADR-0004) as the
flare's new effect.

## Consequences

- One tiny, robust patch on the one method that matters — no reimplementation of
  power logic.
- Everything that keys off `ElectricityDisabled` (comms console,
  `CompFacilityInactiveWhenElectricityDisabled`) also keeps working during a flare,
  which is consistent with "EMI no longer disables electricity".
- The method runs every power-net tick; the postfix is O(1) so the cost is
  negligible.

## Alternatives considered

- **Transpile / patch `PowerNet.PowerNetTick`** — a hot path, and IL-fragile
  across versions; the gate call is the clean seam.
- **Suppress the solar-flare incident entirely** — removes the event, its alerts
  and visuals; the mod wants the flare to still happen, just do something else.
- **Patch each powered comp** — unbounded surface, easy to miss modded comps.
- **Remove/replace `GameCondition_DisableElectricity`** — would break any other
  content that relies on it.
