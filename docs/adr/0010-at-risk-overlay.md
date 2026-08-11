# ADR-0010: Show an at-risk overlay on devices during a flare

- Status: Accepted
- Date: 2026-08-11

## Context

With ignition now bounded to one random at-risk device per interval (ADR-0009), the player
needs to *see* which devices are candidates so they can react — flick off the ones they
don't want burning — instead of guessing. There is no vanilla affordance that marks "this
powered thing might catch fire during the current flare".

## Decision

While a flare is active, draw a warning marker over every at-risk device (the same
`IsAtRisk` set that ignition draws from). Reuse the vanilla overlay pipeline rather than
shipping a texture: call `map.overlayDrawer.DrawOverlay(building, OverlayTypes.QuestionMark)`
from `MapComponent_EMIBurn.MapComponentUpdate()`. Gated by a new per-save setting
`showRiskOverlay` (default `true`).

Two engine constraints drive the details (verified against decompiled 1.6):

- `DrawOverlay` is **transient**: `OverlayDrawer.DrawAllOverlays()` renders then clears
  `overlaysToDraw` every frame, so the marker must be re-enqueued every frame from an
  Update hook, not set once.
- `MapComponentUpdate()` runs for **every** map and *after* `DrawAllOverlays()` in
  `Map.MapUpdate`. Enqueueing for a non-drawn map would never be flushed, so we guard with
  `map == Find.CurrentMap`. The one-frame lag (enqueue after this frame's flush → shown next
  frame) is imperceptible.

## Consequences

- The player gets a clear, zero-asset "watch/switch this off" signal on exactly the devices
  that can ignite; toggle it off via `showRiskOverlay` if it's visual clutter.
- The marker set and the ignition set can't drift — both call `IsAtRisk`.
- `QuestionMark` is a repurposed "pay attention here" icon, not a bespoke danger glyph. If a
  dedicated icon is wanted later it means shipping a `Textures/` asset + material — a new
  packaging concern (ADR-0006), deferred until asked for.

## Alternatives considered

- **Ship a custom flame/warning texture and draw it via a Material** — nicer glyph, but adds
  an asset + material and expands the single-DLL packaging story (ADR-0006) for a solo mod;
  not worth it yet.
- **Persistent handles (`OverlayDrawer.Enable`/`Disable`)** — avoids per-frame re-enqueue but
  needs per-thing handle bookkeeping and explicit teardown when a device stops being at risk
  or the flare ends; the transient per-frame call is simpler and self-clearing.
- **A letter/alert listing at-risk devices** — noisier than an in-world marker and doesn't
  point at the physical thing to switch off.
