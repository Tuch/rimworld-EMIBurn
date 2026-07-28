# Architecture Decision Records

This folder records the **why** behind EMIBurn's non-obvious design choices, so a
future reader (or a future us) doesn't have to re-derive them. An ADR captures a
*decision* — its context, what was chosen, the consequences, and the alternatives
rejected. It is not a how-to; operational docs live in [`README.md`](../../README.md)
and [`CLAUDE.md`](../../CLAUDE.md).

## Rules

- **One decision per file**, named `NNNN-kebab-title.md`, numbered from `0001`
  (4-digit, zero-padded). Next number = highest existing + 1.
- Each ADR uses these sections: **Status · Context · Decision · Consequences ·
  Alternatives considered**.
- **Status vocabulary:** `Proposed` → `Accepted` → `Superseded by ADR-NNNN`.
- ADRs are **immutable once Accepted**. To change a decision, write a new ADR and
  mark the old one `Superseded by ADR-NNNN` (and link back with `Supersedes`).
- Every ADR is listed in the index table below.
- When a decision changes a convention, sync the affected rules file
  (`CLAUDE.md`, this repo's docs).

## When to write one

Write an ADR when a choice is architectural and non-obvious: a Harmony patch
target, why a whole approach was avoided, a packaging/distribution constraint.
Skip trivial or purely mechanical changes.

## Index

| #    | Title                                                                 | Status   |
| ---- | --------------------------------------------------------------------- | -------- |
| 0001 | [Record architecture decisions](0001-record-architecture-decisions.md) | Accepted |
| 0002 | [Keep electricity on during solar flares](0002-keep-electricity-on-during-solar-flares.md) | Accepted |
| 0003 | [Drive the mechanic from an auto-registered MapComponent](0003-mapcomponent-no-xml-defs.md) | Accepted |
| 0004 | [Ignite powered devices with a flame explosion](0004-flame-explosion-ignition.md) | Accepted |
| 0005 | [Store settings in a per-save GameComponent](0005-settings-in-gamecomponent.md) | Accepted |
| 0006 | [Single-assembly packaging and clean-copy install](0006-single-assembly-packaging.md) | Accepted |
| 0007 | [Develop against the local copy; one active copy at a time](0007-dev-loop-and-publishing.md) | Accepted |
