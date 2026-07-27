# ADR-0001: Record architecture decisions

- Status: Accepted
- Date: 2026-07-27

## Context

EMIBurn is small, but several of its choices are non-obvious and were arrived at
by decompiling vanilla RimWorld and ruling out approaches that silently do
nothing (fire that won't attach to buildings, patches on the wrong method, a
GameCondition nothing ever spawns). Without a record of *why*, the next change is
likely to re-derive — or re-break — the same reasoning.

## Decision

Keep Architecture Decision Records in `docs/adr/`, one file per decision,
Nygard-style (Status / Context / Decision / Consequences / Alternatives), numbered
from `0001` and listed in `docs/adr/README.md`. ADRs are immutable once Accepted;
a changed decision is a new ADR that supersedes the old one. The rules live in the
index README.

## Consequences

- A small per-decision writing cost, in exchange for a durable, searchable "why".
- `CLAUDE.md` points here so future sessions find the rationale.
- ADR markdown is excluded from the shipped/Workshop build (dev-only docs).

## Alternatives considered

- **Rationale only in `CLAUDE.md`** — mixes standing rules with historical
  decisions, and has no per-decision structure or explicit "alternatives rejected".
- **Rely on git commit messages** — not discoverable, and commits record *what*
  changed, rarely the options weighed and why they lost.
- **No records** — reintroduces exactly the re-derivation problem above.
