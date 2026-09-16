---
name: ai-toolkit-gaia-state
description: "origin/main still serves v13; the whole v14 line sits unpushed — breaking, ahead-only, no deletions — carrying the unexplained MCP re-wiring. A release moves thirteen version sites by hand, and a fourteenth nobody enumerated."
type: state
last_verified: 2026-09-16
---

# Current state

**Published — and behind the working tree.** `origin/main` still carries v13.0.0: the removal
of the `tasks_*` / `memory_*` / `evolve_*` tools and the whole Woolworths capability migrated
in from the retired `fa.integrations`. Plugin sources float on `ref: main`, so `origin/main`
is what a fetching install resolves — **no installed user has v14**, now confirmed against the
one channel that has an install at all ([`watch.md`](watch.md)), and every one of them takes
it in a single step the moment someone pushes.

**Local `main` is ahead-only against `origin/main`, no deletions in the range** — the safe
topology; a plain `git push` fast-forwards. Re-inspected 2026-09-16 with no fetch permitted, so
this is the remote-tracking ref as it stands, not as the remote does. Waiting in that range:

- **The whole v14 line.** `14.0.0` is breaking: `MEMORY.md` becomes a *derived* index
  regenerated from its `memory/` topic files, with new `MEMORY-*` findings and `--fix-index`
  in `context-audit.py`, and every skill and reference saying "re-cut the index hook" rewritten
  to say edit the topics and regenerate — a consuming repo's hand-authored index reports stale
  until regenerated once. `14.1.0` through `14.3.1` harden the regenerator and widen the audit
  (`--scope`, blocked regeneration on bad frontmatter, the instruction scan reaching skills and
  agents, a remote proved by `ls-remote`), make those checks actually fire, and fix two that
  were reading the wrong text. One push ships all of it at once.
- **The `foundation` MCP re-wiring**, from 2026-08-24 and still carrying **no recorded
  rationale**: an `mcpServers` block wiring `fa-gaia-remote` to
  `https://gaia.frostaura.net/mcp`, reversing the v13.0.0 decision that no plugin declares the
  server ([`decisions.md`](decisions.md)). Once an isolated commit, it is now buried in an
  unpushed breaking release — **whoever authorises the v14 push authorises the re-wiring with
  it**, and makes the README's "the plugins send nothing anywhere" disclosure untrue for
  `foundation` in the same instant.

Pushing is the founder's call — and now one call over all of it, not three.

**Lockstep is an invariant nothing enforces, and the enumeration was itself short.** A release
moves twelve JSON version sites plus the README badge by hand with no CI job comparing them —
the topology of the hazard, not its current value ([`gotchas.md`](gotchas.md),
[`decisions-distribution.md`](decisions-distribution.md)); `metadata.version` in the two
manifests tracks the catalog's shape and deliberately does not move. **A fourteenth
hand-maintained site sits outside that loop entirely:** `src/Gaia.Mcp.Server/Program.cs` sets
the version the MCP server advertises on `initialize`, it reads `13.0.0`, and the v13.0.0
commit was the last to move it ([`watch.md`](watch.md)).
*Dated reading 2026-09-16: those thirteen agreed at `14.3.1`, the server site read `13.0.0`. Re-measure, never quote.*

**Nothing past v8 was ever tagged.** The newest tag is `v8.0.0`; v9 onward ship by moving
`main` alone, and no step in the release path creates a tag.

## Verified on disk, 2026-09-16

- Three plugins ship — `foundation`, `engineering`, `product` — each with its own `skills/`
  and `agents/` trees, all three `plugin.json` pairs byte-identical (`cmp`), and `foundation`
  carrying every `fa-foundation-*` capability name this tree's context files reach for.
  Authored here; *installed* anywhere is [`watch.md`](watch.md).
- `src/` is the .NET 10 MCP server plus `src/Gaia.Mcp.Tests`, a real xUnit project in
  `Gaia.slnx`, so CI's `dotnet test` gate asserts something — before v13.0.0 it did not. The
  server's only integration is Woolworths (`Integrations/Woolworths`, `Tools/WoolworthsTools.cs`),
  exposed on both MCP and REST.

## What this repository cannot tell you

**Which image `gaia.frostaura.net/mcp` is running.** `build-gaia-mcp.yml` tests and pushes
`gaia-mcp:{latest,<sha>}` to Docker Hub on every push to `main` and **contains no deploy step of
any kind**, so the running tool set is unknowable from here: [`questions.md`](questions.md) §1.
