---
name: ai-toolkit-gaia-state
description: "origin/main still serves v13; the entire v14 line sits unpushed — breaking, ahead-only, no deletions — and carries the still-unexplained MCP re-wiring with it. Lockstep intact at 14.3.0 across every version site. Never tagged past v8."
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
  reports stale until it is regenerated once. `14.1.0` through `14.3.0` harden the regenerator
  and widen the audit — `--scope`, a created-not-ignored missing index, blocked regeneration on
  bad frontmatter, the instruction-layer scan reaching skills and agent definitions, a remote
  proved by `ls-remote` rather than trusted, and then the round that made those checks actually
  fire: topics validated before an index is created, a future stamp blocking creation as well as
  regeneration, `SCOPE-EMPTY` per scope, the size cap measured over the body, a state word no
  longer exempted for sharing a line with `MEMORY.md`, and `UPKEEP-UNSCOPED`. One push ships all
  of it at once.
- **The `foundation` MCP re-wiring**, from 2026-08-24 and still carrying **no recorded
  rationale**: an `mcpServers` block wiring `fa-gaia-remote` to
  `https://gaia.frostaura.net/mcp`, reversing the v13.0.0 decision that no plugin declares
  the server ([`decisions.md`](decisions.md)). Once an isolated commit, it is now buried in
  an unpushed breaking release — **whoever authorises the v14 push authorises the re-wiring
  with it**, and makes the README's "the plugins send nothing anywhere" disclosure untrue for
  `foundation` in the same instant.

Pushing is the founder's call — and now one call over all of it, not three.

**Version lockstep is intact at 14.3.0.** Verified 2026-09-11 across all six `plugin.json`
files, all three plugin rows in both manifests and the README badge. `metadata.version` tracks
the catalog's shape and deliberately does not move ([`decisions-distribution.md`](decisions-distribution.md)).

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
