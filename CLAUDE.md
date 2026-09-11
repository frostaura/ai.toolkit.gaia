# ai.toolkit.gaia — Gaia

## Research question
**Answered, and the program knows it.** The original question — can a spec-driven, role-decomposed agent workflow with an enforced completion contract produce more reliable software delivery than ad-hoc prompting — has been settled affirmatively in practice: Gaia is the workflow FrostAura actually builds with. What remains is distribution and durability, not research. Treat any new "research question" framing here with suspicion; the honest read is that this graduated and the paperwork lagged.

## What this program is
FrostAura's spec-driven AI development toolkit, shipped as **three installable plugins** — `foundation` (the context layer: establishing it across a whole tree, authoring, memory, session close-out, auditing, repo durability), `engineering` (delivery) and `product` — for **both** Claude Code and GitHub Copilot. The plugins carry the skills and agent definitions that encode FrostAura's delivery method (intake → architecture → planning → engineering → testing → release), and they are **self-contained**: the plan is the session's native todo list, and everything durable is files in the consuming repository. Gaia is simultaneously the tool FrostAura uses and a public developer product.

The repository also holds a small .NET 10 MCP server, hosted at `https://gaia.frostaura.net/mcp`, which exposes **third-party consumer integrations** as agent tools and a REST API. **The server and the plugins are separate concerns and neither requires the other.** Its only integration is Woolworths South Africa grocery, migrated whole from the retired `fa.integrations`. Whether any shipped plugin currently wires it is state, and belongs to [`MEMORY.md`](MEMORY.md) — check there before assuming either way. That the two live in one repository under one brand is a real coherence question, recorded as open in `MEMORY.md`, not settled here.

## Stack & repo
`github.com/frostaura/ai.toolkit.gaia` (public). **.NET 10 only.** There is **no EF Core, no PostgreSQL, no React, no frontend of any kind, and no persistence at all.** The server is stateless by design: no database, no credential store, no data directory. This is a deliberate deviation from the mandated stack, justified by the size and shape of the service, and it is worth naming plainly: this repo ships the `fa-engineering-default-tech-stack` skill telling every other FrostAura project to use EF Core + PostgreSQL + React + Tailwind + shadcn, while using none of them itself. That irony is acceptable; leaving it undocumented is not.

Layout: `plugins/{foundation,engineering,product}/` hold the shipped skill and agent definitions; `src/Gaia.Mcp.Server/` holds the MCP server (`Integrations/` for the provider-neutral seam and each provider, `Tools/` for the `[McpServerToolType]` classes); `src/Gaia.Mcp.Tests/` holds the xUnit suite; `.claude-plugin/` and `.github/plugin/` hold the two marketplace manifests. `foundation` additionally carries `references/` (shared doctrine), `agents/` and `scripts/`.

## Owner & key people
Dean Martin (founder). Gaia is the delivery substrate for every other FrostAura program, which makes its blast radius larger than its LOC suggests.

## Milestones
1. **Two-ecosystem distribution** — installable and working in both Claude Code and GitHub Copilot from a published marketplace.
2. **Hosted MCP reliability** — the remote server is a live integration gateway handling other people's provider credentials in transit, and must behave like a service, not a prototype.
3. **A real test suite** — see kill criteria; a delivery-discipline product with no tests of its own is a credibility problem, not just a quality one.
4. **Placement resolved** — moved to `Technologies/` or explicitly retained in Labs with a reason.

## Kill criteria
- The workflow stops being what FrostAura actually builds with. Gaia's whole justification is that it is dogfooded; the day it is not, it is dead weight.
- The hosted MCP cannot be operated to service standards and no one is willing to move it to a division that can.
- Maintaining plugin parity across two ecosystems costs more than the second ecosystem returns.

## Graduation pathway
`Technologies/`, and overdue. This is a versioned, publicly distributed, multi-ecosystem developer product with a live hosted service, CI/CD and a support address. It has no research character left, and it is the only Labs program with a granted stealth exception — which is precisely the signature of something that already graduated. Keep the OSS posture; move the home. Placement is parent-controlled: recommend, escalate, do not move it from inside Labs.

## Conventions & invariants
- **Every skill and agent is named `fa-<plugin>-<name>`, and a skill's directory name must equal its frontmatter `name`.** The loader matches on the directory; a mismatch silently produces a skill that cannot be invoked. Renaming one therefore means moving the directory, editing the frontmatter and repairing every cross-reference in the same pass.
- **A skill's `description` is a trigger, not a summary.** It is the only thing that decides whether the skill ever fires. This is not stylistic: a generation of these files shipped with frontmatter whose opening `---` was missing, which made every description parse as the literal string `name: <skill>` — inert, while still looking present in a listing.
- **Markdown links never escape the plugin root, but scripts are referenced with `${CLAUDE_PLUGIN_ROOT}`.** Both rules hold at once: the first governs relative links between skills and references, the second is how a skill invokes a file under `plugins/<name>/scripts/`.
- **The plugins are the product.** A change to a skill or agent definition ships to every installed user on the next resolve — treat plugin content edits with production seriousness. `src/` is not supporting infrastructure for them; it is an independent service that happens to share the repository.
- **Plugin sources are pinned to `ref: main`, not to a tag.** Installs therefore float with `main`. Any breaking change to a skill or agent contract lands on users immediately. Either respect that or move to tags deliberately.
- **The completion contract is still the point, but nothing enforces it mechanically any more.** Tasks cannot complete with unresolved blockers, missing proof, or unsatisfied gates. The server's task tools once *refused* a completion that broke it; they are gone, so the contract rests entirely on the skills and the roles that apply them. Do not add an escape hatch to make a workflow feel smoother — the friction is the feature, and weaker enforcement is a reason to write it more literally, not less.
- **The plan is the session's native todo list; durable state is files in the consuming repository.** Never reintroduce a hosted store for plans, memory or lessons — that was removed deliberately, and the plugins' independence from any server is now a property worth protecting.
- **The two marketplace manifests must agree.** `.claude-plugin/marketplace.json` and `.github/plugin/marketplace.json` describe the same product to two ecosystems, and every plugin additionally has a byte-identical duplicate `plugin.json` at two paths. Nothing in CI checks any of this; check it by hand on every change.
- **Hosted-service disclosure is a commitment.** The README states that the plugins send nothing anywhere and the server stores nothing at all — no credentials, no data, no session reuse. Do not add persistence of any kind, or credential caching of any kind, without changing that statement in the same change.
- **The integration invariants are inherited, load-bearing, and non-obvious.** Add cart items one at a time — the upstream batch endpoint discards the whole batch if any one product is unavailable, and returns HTTP 200 while doing it. Never trust an upstream status code; inspect the body. Search and preview must stay anonymous. MCP errors must be `McpException` or the SDK replaces the text with a generic message. Full reasoning and the captured endpoint contract: [`docs/integrations/`](docs/integrations/architecture.md).
- **Shared reference docs are duplicated by design of the plugin format**, not by accident — the same file exists in several plugin trees. When editing one, edit all copies or the plugins disagree with each other.

## Build / test / run
| Task | Command |
| --- | --- |
| Build | `dotnet build Gaia.slnx` |
| Test | `dotnet test Gaia.slnx` — a real gate — `src/Gaia.Mcp.Tests`, listed in `Gaia.slnx`. It was vacuous for a long time, so treat any older green as meaningless (`memory/gotchas.md`) |
| Run (HTTP) | `dotnet run --project src/Gaia.Mcp.Server` — the web host is the default; serves `/mcp` and `/api` |
| Run (stdio) | requires `MCP_TRANSPORT=stdio`; without it `Program.cs` starts the HTTP web host on stdio and misbehaves |

Container image publishing goes to Docker Hub under a namespace derived from a CI secret, so the published image name is not discoverable from the repo alone.

## Where docs live
The README is the entry point and is deliberately an **install-and-first-run document, not a catalog** — a hand-maintained roster in it goes stale faster than anyone maintains it, so skills are listed only in `docs/catalog.md`, which opens by dating itself and pointing at `/plugin` as the live roster. `docs/development.md` is the contributor layer: releases and the version sites, manifest agreement, local-clone installs, authoring invariants and the pre-ship checks. `AGENTS.md` is an **interop pointer at this file and carries no policy of its own** — if it starts explaining rules rather than pointing at them, delete the explanation. Any status, version or count claimed in any of them is `MEMORY.md`'s to confirm; read it before trusting one.

## Never do this here
- Never change a skill or agent definition without checking every duplicated copy across plugin trees.
- Never let the two marketplace manifests or the duplicated `plugin.json` files drift.
- Never store, log, cache or echo back a caller's provider credential, and never expose the server over plain HTTP — TLS is the only thing protecting credentials in transit.
- Never batch cart additions, and never put a checkout or payment step behind an automated call; this service fills a cart and stops, deliberately.
- Never weaken the completion contract to unblock a stuck workflow — nothing will stop you now, which is exactly why.
- Never change the MCP session mode without re-verifying that header credentials still resolve; it is a two-minute check and the failure mode looks like a caller-side configuration problem.
- Never add persistence of user code, specs or docs to the hosted service without amending the README's disclosure.

## Upkeep
This `CLAUDE.md`, [`MEMORY.md`](MEMORY.md) and the `memory/` topic store are kept current **as changes land**, not in a later cleanup pass. `MEMORY.md` is required at this level exactly as `CLAUDE.md` is, and cascades downward in the same way — a `CLAUDE.md` with no `MEMORY.md` beside it is a defect. `MEMORY.md` here is a **derived index, never hand-edited** — every line of it is computed from the `memory/` topic files. At the end of any session that shipped, decided, discovered or abandoned something, edit the topic files and then regenerate, from the tree root: `python3 .claude/scripts/audit-tree.py --no-git --fix-index --scope Labs/projects/ai.toolkit.gaia`. The same run regenerates the grouping index above this program, so no row is ever updated by hand. A hand-cut hook is the next `MEMORY-INDEX-STALE`, and the next regeneration silently discards it. Procedure itself comes from the installed Gaia plugins rather than a copy kept here; a stale procedure is fixed upstream and reinstalled, never forked into this tree. The standard: [`/docs/operating/context-cascade.md`](../../../docs/operating/context-cascade.md).
