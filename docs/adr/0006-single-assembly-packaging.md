# ADR-0006: Single-assembly packaging and clean-copy install

- Status: Accepted
- Date: 2026-07-27

## Context

RimWorld loads a mod's assemblies **recursively** — `ModContentPack` enumerates
`Assemblies/` with `SearchOption.AllDirectories`. The original `install.sh` both
`cp`-ed the DLL to `Assemblies/EMIBurn.dll` and rsync-ed the build tree containing
`Assemblies/net472/EMIBurn.dll`, so the installed mod held **two copies of the same
assembly** — loaded twice, producing duplicate Harmony patches and type clashes.
The game also generates `About/PublishedFileId.txt` (the Workshop link), which a
naive wipe-and-copy would destroy.

## Decision

Ship **exactly one** assembly at `Assemblies/EMIBurn.dll`. `install.sh` wipes the
target mod folder and clean-copies from the repo, excluding source, build output,
VCS and dev-only files (`Source/`, `obj/`, `.git`, `*.md`, `docs/`, cover-art
master, scripts). `About/PublishedFileId.txt` is tracked in the repo so the
clean-copy always restores the Workshop link (see the dev-loop notes in
`CLAUDE.md`).

## Consequences

- No duplicate-load; the mod initializes once.
- Reproducible installs — the target is a clean projection of the repo, never an
  accumulation of stale files.
- The Workshop link survives every reinstall, so updates keep targeting the same
  item.
- `install.sh` hard-codes the local macOS Steam paths — fine for solo dev, not a
  portable installer.

## Alternatives considered

- **Copy the build output tree as-is** (`Assemblies/net472/…`) plus a top-level
  copy — this *was* the double-load bug.
- **rsync without wiping the target** — stale/renamed files linger between builds.
- **Commit only source, build in place inside the Mods folder** — RimWorld needs
  the compiled DLL present; building in the live mod dir mixes `obj/`/`bin/` into
  the shipped mod.
