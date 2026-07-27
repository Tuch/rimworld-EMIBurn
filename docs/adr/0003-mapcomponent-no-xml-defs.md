# ADR-0003: Drive the mechanic from an auto-registered MapComponent

- Status: Accepted
- Date: 2026-07-27

## Context

The mod's first incarnation put the fire logic in a custom
`GameCondition_ElectricalBurnout` that looked for buildings/conditions named
`EMIDynamo` / `EMIField` — defNames that don't exist in vanilla — and there was no
`Defs/` folder to ever spawn that condition. So the logic never ran. What we
actually need is per-map ticking that activates while the vanilla solar flare is
present.

## Decision

Implement the mechanic in `MapComponent_EMIBurn`, which `Map.FillComponents`
auto-instantiates on every map by reflecting over all non-abstract `MapComponent`
subclasses — **no XML `Def` required**. Each scheduled tick it checks for an active
`GameCondition_DisableElectricity` and, if present, runs the ignition pass. The mod
ships **no `Defs/` folder**.

## Consequences

- Zero XML; the component appears on every map and every existing save
  automatically.
- Flare detection (the presence of `GameCondition_DisableElectricity`) is
  independent of the ADR-0002 patch, so the two concerns don't entangle.
- A small coupling to the vanilla condition class name — acceptable, and the same
  name ADR-0002 already depends on.

## Alternatives considered

- **Custom `GameConditionDef` + `IncidentDef`** — adds XML and a second event to
  balance, yet we'd *still* have to neutralize the vanilla flare's power-off; more
  moving parts for no gain.
- **Harmony-patch the flare condition's tick** — more fragile than owning a
  component, and harder to gate on settings.
- **A `GameComponent` instead of a `MapComponent`** — would have to enumerate maps
  and per-map state by hand; `MapComponent` gives that for free.
