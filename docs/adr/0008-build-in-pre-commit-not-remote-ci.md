# ADR-0008: Build in a pre-commit hook, not remote CI

- Status: Accepted
- Date: 2026-07-27

## Context

We want two things from automation: the committed `EMIBurn.dll` should always match
its source (the repo tracks the binary so users can clone-and-play), and a broken
build should be caught before it lands. The obvious tool — a GitHub Actions
workflow — can't compile this project: `Source/EMIBurn.csproj` references local,
non-redistributable RimWorld assemblies (`Assembly-CSharp.dll`, `Verse.dll`,
`UnityEngine*.dll`) and Harmony (`0Harmony.dll`) via absolute `HintPath`s. A CI
runner has none of them, and committing them would be a licensing problem.

## Decision

Use a tracked `.githooks/pre-commit` hook (enabled per clone with
`git config core.hooksPath .githooks`) that, when a commit stages `Source/*.cs` or
the `.csproj`, runs `./build.sh`, re-stages the rebuilt DLL/PDB, and **aborts the
commit if the build fails**. No remote CI.

## Consequences

- Build breaks are caught locally at commit time; the committed binary tracks
  source.
- The hook is **opt-in per clone** (`core.hooksPath` isn't versioned) and only the
  committer's machine validates — there is no server-side PR gate.
- .NET assemblies aren't byte-reproducible, so the tracked DLL churns on every
  source-touching commit. Accepted: it always reflects the latest build.

## Alternatives considered

- **GitHub Actions building the `.csproj`** — fails without the RimWorld/Harmony
  reference DLLs; shipping those to CI is a redistribution/licensing issue.
- **Vendor public reference assemblies** (e.g. the `Krafs.Rimworld.Ref` NuGet
  package) to make remote CI possible — the right escalation if the mod grows or
  gains contributors, but heavier setup than a solo project needs today.
- **No automation** — stale committed DLLs and build breaks slip into history.
- **`pre-push` instead of `pre-commit`** — catches problems later and still local;
  pre-commit keeps every commit buildable.
