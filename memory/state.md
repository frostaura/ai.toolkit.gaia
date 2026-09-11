---
name: ai-toolkit-gaia-state
description: "v13 is live on origin/main and floats into installs via ref:main. One unpushed commit re-wires the hosted MCP into foundation and bumps foundation alone to 13.1.0 — ahead-only, no deletions. Never tagged past v8."
type: state
last_verified: 2026-09-11
---

# Current state

**Published.** `origin/main` carries v13.0.0 — the removal of the `tasks_*` / `memory_*` /
`evolve_*` tools and the whole Woolworths capability migrated in from the retired
`fa.integrations`. Because plugin sources float on `ref: main`, whatever sits on `origin/main`
is what a fetching install resolves. What "installed" actually means on this machine is a
separate matter — [`watch.md`](watch.md).

**Local `main` is ahead-only against `origin/main`, with zero deletions in the range** — the
safe topology; a plain `git push` fast-forwards. The unpushed commit (2026-08-24,
`chore(foundation): adding mcp server to foundation plugin set`) touches four files and does
exactly two things:

- re-adds an `mcpServers` block to `foundation` wiring `fa-gaia-remote` to
  `https://gaia.frostaura.net/mcp` — reversing the v13.0.0 decision that no plugin declares
  the server, with no recorded rationale ([`decisions.md`](decisions.md));
- bumps **`foundation` alone** to `13.1.0` across its four version sites, leaving
  `engineering` and `product` at `13.0.0` — so lockstep versioning is broken in the working
  tree ([`decisions-distribution.md`](decisions-distribution.md)).

Pushing is the founder's call. It would also make the README's "the plugins send nothing
anywhere" disclosure untrue for `foundation` the moment it lands.

**Nothing past v8 was ever tagged.** The newest tag here is `v8.0.0`; v9 through v13 shipped
by moving `main` alone, and no step in the release path creates a tag.

## Verified on disk, 2026-09-11

- Three plugins ship — `foundation`, `engineering`, `product`, each with its own `skills/` and
  `agents/` trees. All three `plugin.json` pairs are byte-identical (`cmp`).
- `foundation`'s authored skills include `fa-foundation-session-close`,
  `fa-foundation-repo-durability`, `fa-foundation-context-audit`,
  `fa-foundation-memory-maintenance`, `fa-foundation-context-authoring` and
  `fa-foundation-registry-audit` — the capability names FrostAura context files reach for.
  They exist here, authored. Whether they are *installed* anywhere is [`watch.md`](watch.md).
- `src/` is the .NET 10 MCP server plus `src/Gaia.Mcp.Tests`, a real xUnit project listed in
  `Gaia.slnx` — so CI's `dotnet test` gate asserts something. Before v13.0.0 it did not.
- The server's only integration is Woolworths (`Integrations/Woolworths`,
  `Tools/WoolworthsTools.cs`), exposed on both MCP and REST.

## What this repository cannot tell you

**Whether `gaia.frostaura.net/mcp` is running a v13 image.** The repo holds one workflow,
`build-gaia-mcp.yml`: it tests, then builds and pushes `gaia-mcp:latest` and `gaia-mcp:<sha>`
to Docker Hub on every push to `main`. **There is no deploy step of any kind** — no Portainer
webhook, no stack update. Redeploy is a manual act outside this repository, so the running
tool set is unknowable from here. Owned and open: [`questions.md`](questions.md).
