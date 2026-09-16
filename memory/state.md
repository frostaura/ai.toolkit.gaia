---
name: ai-toolkit-gaia-state
description: "The whole v14 line was published on 2026-09-16 — breaking, and carrying the unexplained MCP re-wiring with it. Installs float on ref:main and cross that boundary on their next fetch. Thirteen version sites moved; a fourteenth did not."
type: state
last_verified: 2026-09-16
---

# Current state

**Published, and current.** On the evening of 2026-09-16 the founder authorised a tree-wide commit
and push, and `main` fast-forwarded onto `origin/main`: re-read locally afterwards, branch `main`,
upstream `origin/main`, **zero ahead and zero behind**, clean tree. That is the tracking ref rather
than a live probe, so fetch before acting on a count.

**What that push released, all at once:**

- **The whole v14 line.** `14.0.0` is breaking: `MEMORY.md` becomes a *derived* index regenerated
  from its `memory/` topic files, with new `MEMORY-*` findings and `--fix-index` in
  `context-audit.py`, and every skill and reference saying "re-cut the index hook" rewritten to say
  edit the topics and regenerate — **a consuming repository's hand-authored index now reports stale
  until it is regenerated once**. `14.1.0`–`14.3.1` harden the regenerator and widen the audit
  (`--scope`, blocked regeneration on bad frontmatter, the instruction scan reaching skills and
  agents, a remote proved by `ls-remote`) and fix two checks that read the wrong text.
- **The `foundation` MCP re-wiring**, from 2026-08-24 and **still carrying no recorded rationale**:
  an `mcpServers` block wiring `fa-gaia-remote` to `https://gaia.frostaura.net/mcp`, reversing the
  v13.0.0 decision that no plugin declares the server ([`decisions.md`](decisions.md)). It shipped
  inside the release rather than on its own, so **it is published without ever having been argued**,
  and the README's "the plugins send nothing anywhere" disclosure is untrue for `foundation` as
  published. Ratifying or reverting it is a founder call — [`questions.md`](questions.md) §5.

**Plugin sources float on `ref: main`**, so what `origin/main` serves is what a fetching install
resolves: every install takes v14 in one step, across a breaking boundary, on its next re-fetch.
The one install this tree can see still carried **13.0.0** when it was last observed on 2026-09-16
— before the push — and **whether it has re-fetched since is not verifiable from this scope**
([`watch.md`](watch.md)).

**Lockstep is an invariant nothing enforces, and the enumeration was itself short.** A release
moves twelve JSON version sites plus the README badge by hand with no CI job comparing them — the
topology of the hazard, not its current value ([`gotchas.md`](gotchas.md),
[`decisions-distribution.md`](decisions-distribution.md)); `metadata.version` in the two manifests
tracks the catalog's shape and deliberately does not move. **A fourteenth hand-maintained site sits
outside that loop:** `src/Gaia.Mcp.Server/Program.cs` sets the version the server advertises on
`initialize`, it reads `13.0.0`, and v13.0.0 was the last commit to move it — so the *published*
repository now advertises a major version behind its own plugins ([`watch.md`](watch.md)).
*Dated reading 2026-09-16: the thirteen agreed at `14.3.1`, the server site read `13.0.0`. Re-measure, never quote.*

**Nothing past v8 was ever tagged.** The newest tag is `v8.0.0`; v9 onward ship by moving `main`
alone, so v14 is published with nothing to pin to and nothing to roll back to.

**Verified on disk, 2026-09-16.** Three plugins ship — `foundation`, `engineering`, `product` —
each with its own `skills/` and `agents/` trees, all three `plugin.json` pairs byte-identical
(`cmp`), and `foundation` carrying every `fa-foundation-*` capability name this tree's context
files reach for. Authored here; *installed* anywhere is [`watch.md`](watch.md). `src/` is the
.NET 10 MCP server plus `src/Gaia.Mcp.Tests`, a real xUnit project in `Gaia.slnx`, so CI's
`dotnet test` gate asserts something; the server's only integration is Woolworths, on MCP and REST.

**What this repository cannot tell you: which image `gaia.frostaura.net/mcp` is running.**
`build-gaia-mcp.yml` tests and pushes `gaia-mcp:{latest,<sha>}` to Docker Hub on every push to
`main` and **contains no deploy step of any kind** — so the 2026-09-16 push triggered a build, not
a deployment, and the running tool set stays unknowable: [`questions.md`](questions.md) §1.
