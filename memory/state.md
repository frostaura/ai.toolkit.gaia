---
name: ai-toolkit-gaia-state
description: "origin/main still serves v13; the entire v14 line sits unpushed — breaking, ahead-only, no deletions — and carries the still-unexplained MCP re-wiring with it. A release bump touches 13 sites by hand and nothing in CI checks they agree."
type: state
last_verified: 2026-09-11
---

# Current state

**Published — and behind the working tree.** `origin/main` still carries v13.0.0: the removal
of the `tasks_*` / `memory_*` / `evolve_*` tools and the whole Woolworths capability migrated
in from the retired `fa.integrations`. Plugin sources float on `ref: main`, so `origin/main`
is what a fetching install resolves — **no installed user has v14**, and they all get it in
one step the moment someone pushes. What "installed" means here at all: [`watch.md`](watch.md).

**Local `main` is ahead-only against `origin/main`, no deletions in the range** — the safe
topology; a plain `git push` fast-forwards. Waiting in that range:

- **The whole v14 line.** `14.0.0` is breaking: `MEMORY.md` becomes a *derived* index
  regenerated from its `memory/` topic files, with new `MEMORY-*` findings and `--fix-index`
  in `context-audit.py`, and every skill and reference that said "re-cut the index hook"
  rewritten to say edit the topics and regenerate — a consuming repo's hand-authored index
  reports stale until it is regenerated once. `14.1.0` through `14.3.1` then harden the
  regenerator and widen the audit (`--scope`, blocked regeneration on bad frontmatter, the
  instruction scan reaching skills and agents, a remote proved by `ls-remote`), make those
  checks actually fire, and finally stop two of them reading the wrong text — the instruction
  scan had been skipping every table row and blockquote, and `UPKEEP-UNSCOPED` accepted only
  one spelling of its own command. One push ships all of it at once.
- **The `foundation` MCP re-wiring**, from 2026-08-24 and still carrying **no recorded
  rationale**: an `mcpServers` block wiring `fa-gaia-remote` to
  `https://gaia.frostaura.net/mcp`, reversing the v13.0.0 decision that no plugin declares
  the server ([`decisions.md`](decisions.md)). Once an isolated commit, it is now buried in
  an unpushed breaking release — **whoever authorises the v14 push authorises the re-wiring
  with it**, and makes the README's "the plugins send nothing anywhere" disclosure untrue for
  `foundation` in the same instant.

Pushing is the founder's call — and now one call over all of it, not three.

**Lockstep is an invariant nothing enforces.** A release moves twelve JSON version sites plus the
README badge by hand, and no CI job compares them — the topology of the hazard, not its current
value ([`gotchas.md`](gotchas.md), [`decisions-distribution.md`](decisions-distribution.md)).
`metadata.version` in the two manifests tracks the catalog's shape and deliberately does not move.
*Dated reading: all thirteen sites agreed at `14.3.1` on 2026-09-11 — re-measure, never quote.*

**Nothing past v8 was ever tagged.** The newest tag is `v8.0.0`; v9 onward ship by moving
`main` alone, and no step in the release path creates a tag.

## Verified on disk, 2026-09-11

- Three plugins ship — `foundation`, `engineering`, `product` — each with its own `skills/`
  and `agents/` trees, and all three `plugin.json` pairs byte-identical (`cmp`).
- `foundation` carries the capability names FrostAura context files reach for, each as an
  `fa-foundation-*` skill: session-close, repo-durability, context-audit, memory-maintenance,
  context-authoring, registry-audit. Authored here; *installed* anywhere is [`watch.md`](watch.md).
- `src/` is the .NET 10 MCP server plus `src/Gaia.Mcp.Tests`, a real xUnit project listed in
  `Gaia.slnx`, so CI's `dotnet test` gate asserts something. Before v13.0.0 it did not.
- The server's only integration is Woolworths (`Integrations/Woolworths`,
  `Tools/WoolworthsTools.cs`), exposed on both MCP and REST.

## What this repository cannot tell you

**Which image `gaia.frostaura.net/mcp` is running.** The one workflow, `build-gaia-mcp.yml`,
tests and then pushes `gaia-mcp:latest` and `gaia-mcp:<sha>` to Docker Hub on every push to `main`
— and **contains no deploy step of any kind**. Redeploy is a manual act outside this repo, so
the running tool set is unknowable from here: [`questions.md`](questions.md).
