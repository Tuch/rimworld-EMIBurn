# ADR-0005: Store settings in a per-save GameComponent

- Status: Accepted
- Date: 2026-07-27

## Context

The mod needs configurable values (fire chance, the min/max interval range,
notifications toggle). RimWorld offers two common homes: `ModSettings` (one global
config, editable from the main menu) or a `GameComponent` (per-save state,
auto-instantiated and serialized with the save).

## Decision

Keep settings in `EMIBurnSettings : GameComponent`. The mod's settings window reads
`Current.Game.GetComponent<EMIBurnSettings>()`, and shows a "load a game first"
fallback when no game is active.

## Consequences

- Settings travel with the save; different colonies can have different EMI
  behaviour, and a shared save keeps its settings.
- Settings are **not** editable from the main menu — only after a game is loaded —
  hence the fallback label. This is the main trade-off.
- The random-interval scheduler state (`nextFireTick` in the MapComponent) is
  likewise serialized, so a reload doesn't fire immediately.

## Alternatives considered

- **`ModSettings`** — editable at the menu and global, but not per-save; sharing a
  save wouldn't carry the config, and we'd want per-colony variation for a
  gameplay tweak like this.
- **A static/config file** — no integration with RimWorld's save system or
  settings UI; more plumbing for less.
