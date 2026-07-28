# ADR-0007: Develop against the local copy; one active copy at a time

- Status: Accepted
- Date: 2026-07-27

## Context

The mod is published to the Steam Workshop (item `3772681865`, see ADR-0006). Once
you also *subscribe* to it, two copies exist with the same `packageId`
(`Tuch.EMIBurn`): the local dev copy in `RimWorldMac.app/Mods/EMIBurn` and the
Workshop copy in `steamapps/workshop/content/294100/3772681865/`. RimWorld loads
both, which means a duplicate `packageId` and — because assemblies load
(ADR-0006) — duplicate Harmony patches and clashing types. We need a way to both
develop and ship without that conflict.

## Decision

The **local `Mods/EMIBurn` copy is the working copy** and the source of Workshop
updates. The loop is: edit → `./build.sh && ./install.sh` → test in-game. Publish
updates from that same local copy via the in-game Mods screen — with
`About/PublishedFileId.txt` tracked (ADR-0006), the upload button acts as **Update**
and targets the existing item. **Stay unsubscribed** from your own Workshop item
during development so only one copy is ever active.

## Consequences

- No duplicate-`packageId` conflict; the dev copy is the single source of truth.
- Updates ship straight from what you just tested — no separate release checkout.
- You don't need a subscription to publish; subscribing is only for *playing* the
  released version, and then you must disable/remove the local copy instead of
  running both.
- Deleting `About/PublishedFileId.txt` would sever the update link — it is tracked
  precisely to prevent that.

## Alternatives considered

- **Stay subscribed and keep the local copy** — the exact conflict this ADR
  avoids (two mods, same `packageId`, double-loaded assembly).
- **A separate dev `packageId`/name** (e.g. `Tuch.EMIBurn.Dev`) so the release and
  dev copies coexist — works, but is overkill for solo development and doubles the
  metadata to keep in sync.
- **Edit the Workshop content folder directly** — Steam re-syncs and overwrites it;
  not a source of truth.
