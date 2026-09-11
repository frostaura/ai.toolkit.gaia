# Changelog

All notable changes to the Gaia plugins are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and versions are lockstep across all plugins. Plugin sources track `ref: main`; the `version` field in each `plugin.json` is the update trigger, and changes reach users when pushed to GitHub.

## [14.0.0] - 2026-09-11

### Changed — BREAKING

- **`MEMORY.md` is now derived from the topic files, not written by hand.** Every line of an index is computed: the heading from the scope's path relative to the audited root, each entry's title from the topic's `# H1`, its hook from the topic's `description:`, and the order from the topic's `type:` — `alert`, `state`, `decision`, `gotcha`, `question`, `watch`, `kill-record`, `evidence`, `log`, `reference`, with a type's canonical file (`decisions.md`) before its split siblings (`decisions-sync.md`), then alphabetically. `context-audit.py` reports any byte of difference as **`MEMORY-INDEX-STALE`**, and the new **`--fix-index`** flag rewrites every stale index in place (**`MEMORY-INDEX-REWRITTEN`**). This is why the release is major: an existing store whose index was authored by hand almost certainly fires the new finding until `--fix-index` is run once, and a hook that disagrees with the body it points at will be replaced by the body's own `description:`. The failure it closes is a real one — a hand-cut hook and its topic drift apart on the next edit, and nothing mechanical could see it before.
- **`MEMORY-INDEX-DRIFT` is retired**, replaced by `MEMORY-INDEX-STALE`. Prose in an index was only ever a symptom of the index being hand-written. `MEMORY-LINK-BROKEN` and `MEMORY-ORPHAN-TOPIC` remain, now reported as symptoms of a stale index rather than as defects to fix by editing it.
- **Topic `name:` must equal `<scope slug>-<file stem>`** (**`MEMORY-TOPIC-NAME`**), where the slug is the scope's own directory name lowercased with dots, underscores and spaces turned into dashes, a *grouping* directory (`--group-dir`, default `projects`) prefixed by its parent's slug, and the audited root `root`. Names are then unique by construction; a duplicate is reported too. Note that the slug follows `--root`: a repository nested inside a larger tree is `my-repo-state` when the tree is audited and `root-state` when that repository is audited alone.
- **A topic's first body line must be a `# Heading`** (**`MEMORY-TOPIC-NO-HEADING`**) — the index title is taken from it — and an `alert`'s heading must begin `Red — ` (**`MEMORY-ALERT-HEADING`**) so the hazard reads as one in the index.
- **`description:` is capped at 240 characters** (**`MEMORY-DESCRIPTION-LONG`**). It *is* the index hook now, so it must carry the signal rather than summarise the body; past the cap the topic is two topics, or the description is narrating.

### Added

- **A character cap on topic bodies: 6,000** (**`MEMORY-TOPIC-HEAVY`**), alongside the existing 60-line cap. The line rule alone was being met by files of paragraph-long lines — one 57-line topic carried 22 KB.
- **`MEMORY-VOLATILE-COUNT`** — advisory, for git-shaped integers in a topic's prose (`3 commits ahead`, `12 uncommitted`). These go stale inside the session that writes them, because the closing context commit moves them. Advisory rather than blocking, because a count that is *itself* the hazard ("the unpushed range deletes 21 tracked files") is legitimate and stays.
- **`last_verified` in the future** is now a `MEMORY-FRONTMATTER` finding.

### Changed

- **Every skill, reference and agent that touched the index now says the same thing**: edit the topic files — `description` is the hook, at most 240 characters; body under 60 lines and 6,000 characters — then regenerate with `--fix-index`, and never hand-edit `MEMORY.md`. `fa-foundation-memory-maintenance`, `fa-foundation-session-close`, `fa-foundation-context-authoring`, `fa-foundation-context-audit`, `fa-foundation-optimize-directory-tree` (with `context-templates.md` and the branch `analysis-brief.md`), `fa-foundation-context-auditor`, `references/context-cascade.md`, `references/context-audit-findings.md` and the foundation ownership table. No skill's `description:` trigger changed, because none of the trigger conditions did.
- **Prose rules stated where the tooling cannot enforce them**: one home per fact, with an upward `alert` written as a pointer of at most ~15 lines rather than a copy; never pad a small store to a canonical set of topic files; topic files carry no `## Upkeep` section, because the write protocol is their upkeep; a `log` topic is a pointer index, never a changelog.

### Migration

Run `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --fix-index` once per tree. It regenerates every index from the topic files and reports what it rewrote; it refuses to regenerate a store holding malformed frontmatter, so fix `MEMORY-FRONTMATTER` first. Everything else is a rename or a trim: `MEMORY-TOPIC-NAME` wants the scope slug in front of the file stem, `MEMORY-TOPIC-NO-HEADING` wants an `# H1` as the first body line, and `MEMORY-DESCRIPTION-LONG` wants the hook cut to its signal.

## [13.0.0] - 2026-08-21

### Removed — BREAKING

- **The MCP server's `tasks_*`, `memory_*` and `evolve_*` tools are gone** — all 15 of them (`tasks_create` / `list` / `update` / `complete` / `request_input` / `delete` / `clear`, `memory_remember` / `recall` / `forget` / `clear`, `evolve_log` / `list` / `apply` / `clear`), along with the flat-JSON stores, the completion validator, the `GAIA_*` error-code namespace and the task schema behind them. Anything calling those tool names now fails. The server keeps no data directory at all, so the container's `/app/data` volume is gone too.
- **`foundation` no longer declares the remote MCP server.** Its `plugin.json` had wired `fa-gaia-remote` into every install to persist tasks, memory and evolution lessons; there is nothing left there to persist to, and the server's remaining tools have nothing to do with the plugin. Installing `foundation` no longer adds an MCP server to your client.

### Changed — BREAKING

- **The plan is now your assistant's own todo list, and memory is files in your repository.** Nine plugin files that instructed agents to call the removed tools are retargeted: both `delivery-policy.md` references, `ownership-and-conventions.md`, `fa-engineering-planning` (skill and `plan-template.md`), `fa-engineering-process`, `fa-engineering-testing`, `fa-engineering-implementation`, `fa-product-process` and `product-discovery-team.md`. Durable facts, decisions and lessons go to `MEMORY.md` and its `memory/` topic files through `fa-foundation-memory-maintenance`, swept by `fa-foundation-session-close`. Improvements that used to be `evolve_log` entries are now logged as memories, with the reasoning that produced them.
- **The completion contract survives, but is enforced by the roles rather than by the server.** A task still cannot be closed with unresolved blockers, missing proof, or unsatisfied gates — that friction is still the feature. What changed is that nothing refuses it mechanically any more; the discipline rests on the agents applying the policy, principally QA's veto. Two skill `description` fields changed as part of this, so routing behaviour changed with them.
- **Two of the three plugins are now fully self-contained** — no network, no account, no hosted state. `engineering` still wires Playwright for browser testing.

### Added

- **The Woolworths South Africa integration**, migrated whole from the retired `fa.integrations` repository, which this server replaces. Four MCP tools — `woolworths_search_products`, `woolworths_preview_shopping_list`, `woolworths_add_shopping_list_to_cart`, `woolworths_add_product_to_cart` — search the grocery catalogue and fill the signed-in customer's cart from a shared shopping list (the text the Cookidoo/Thermomix app puts on the share sheet). Search and preview need no credentials and must stay that way: they are what makes the server safe to expose to an agent that holds none.
- **The REST surface came with it.** `/api/health`, `/api/integrations`, and `/api/woolworths/*` map the identical services the MCP tools call, so a Siri Shortcut or a curl script reaches the same behaviour without speaking MCP. A behaviour added to one surface must not diverge from the other.
- **Credentials over stdio.** The migrated service resolved credentials only from HTTP headers; Gaia also serves stdio, where there is no request to read. Each credential field now derives an environment variable from its header so the two cannot drift — `X-Woolworths-Password` becomes `GAIA_WOOLWORTHS_PASSWORD` — and `GET /api/integrations` reports both names. The environment is consulted **only** when the transport supplied no headers, so an HTTP caller who omits one is told so rather than being silently served the host's own environment.
- **A real test suite.** `src/Gaia.Mcp.Tests` is an actual xUnit project wired into `Gaia.slnx`, carrying the 12 migrated `ShoppingListParser` tests. `dotnet test` was vacuous in every prior version — it asserted nothing, and CI ran it as a gate anyway. Any note claiming a green run proved something before 13.0.0 is out of date.

### Fixed

- **A rejected password now says so.** The MCP SDK replaces arbitrary exception text with a generic "an error occurred", and only the *missing*-credential path was wrapped in `McpException` — so a caller whose password was simply wrong got told nothing at all, which is at least as common a case. Provider rejections are now translated on every credentialed tool. Inherited from `fa.integrations` and found by probing the migrated server, not by reading it.
- The missing-credential message no longer repeats itself over stdio (`GAIA_WOOLWORTHS_USERNAME (environment variable)` in a sentence already saying "environment variable(s)").

### Migration

If you called the removed tools directly, there is no drop-in replacement by design — that is the point of the change. Track work in your assistant's todo list, and write anything that must outlive the session to `MEMORY.md` and `memory/` in the repository it concerns. `fa-foundation-memory-maintenance` and `fa-foundation-session-close` do both jobs, and neither needs a server.

## [12.1.0] - 2026-08-21

### Added

- **`fa-foundation-optimize-directory-tree`** (foundation, 9 → 10 skills) — the entry point for putting the Gaia context layer onto a directory tree that has none, and for keeping an existing one true. It covers three jobs and picks between them from disk: **establish** the pair at every scope that earns one plus a root registry and the retrospective design documentation the tree's projects never had; **maintain** it by re-deriving every claim from each project's own source; **sweep** it, where deletion and consolidation are the expected outcome and a pass that only adds has failed. Nothing in the ecosystem did the first job — `fa-foundation-context-audit` explicitly refuses a tree with no context pair, and named no skill because none existed.
  Method: interview once, freeze a partition map as a root `memory/` topic, fan out one **generic** subagent per disjoint subtree against a byte-identical pasted brief, reconcile root last from drained reports, run the audit script, and leave everything uncommitted for the owner. Deliberately no new agent definitions — the method lives in an editable brief rather than in a role roster that ages out.
- Three references under that skill: `context-templates.md` (structure-neutral root / scope / grouping / project templates, the memory archetype bodies, and the fan-out block pasted into a consuming tree's root file), `analysis-brief.md` (the branch brief — version marker, four modes with their own return contracts, evidence-ranking rules, the write allow-list and the prohibitions), and `orientation-interview.md`.

### Changed

- **Routing boundaries sharpened between the four skills that now collide.** `fa-foundation-context-audit` states that it checks claims against disk rather than re-deriving them from source; `fa-foundation-context-authoring` narrows from "at any scope" to a single scope inside an existing cascade; `fa-engineering-default-tech-stack` binds "bootstrapping" to a new application's *stack*; `fa-engineering-process` states that it coordinates delivery inside a repository that already carries a context layer; `fa-engineering-architecture` now advertises documenting an implemented system from source where no baseline exists — a capability that previously lived only in its body. `fa-foundation-context-auditor` gains the matching negative. A description is the only layer a router reads, so these are behaviour changes, not wording.
- **`scripts/context-audit.py` genericized** — five fixes, each of which was a transcription of one organization's directory shape: a repeatable `--group-dir` (default `projects`) so `packages/`, `apps/` and `services/` trees are actually walked; `--all` now honoured for the vendored-directory skip; the registry check seeds leaf scopes so a **flat** tree stops firing one false `REGISTRY-PHANTOM` per project; all three registry name patterns widened to accept PascalCase, snake_case and underscores; and `--instruction-file NAME`, so an `AGENTS.md`-convention tree is audited on its real root file instead of on its interop pointer.
- **`references/ownership-and-conventions.md`** registers `fa-foundation-optimize-directory-tree` and the previously unregistered `fa-foundation-session-close` in both the ownership table and the routing list, and amends the two entries that claimed instruction files "at any scope" for `fa-foundation-context-authoring`.
- **README rewritten as an install-and-first-run document** (211 → ~145 lines): a "Start here" section framing the new skill as three recurring jobs rather than a one-off, install coverage for Claude Code, Copilot CLI and Claude Desktop / claude.ai / Cowork, and an explicit surface caveat — skills load everywhere, sub-agents and hooks run in Claude Code. The version badge now carries full semver and links the changelog.
- **Catalog and contributor material moved out of the README** into `docs/catalog.md` (a dated snapshot that says so, pointing at `/plugin` as the live roster) and `docs/development.md` (releases and the 12 version sites, manifest agreement, local-clone installs, authoring invariants, the pre-ship checks). The README no longer lists skills at all — a hand-maintained roster is what let the previous one go stale.
- `AGENTS.md` is a pointer to `CLAUDE.md`, not a workflow contract; the README no longer calls it one.

### Fixed

- The 12.0.0 entry below said "Two new agents"; three shipped — `fa-foundation-context-auditor` was omitted.

## [12.0.0] - 2026-08-21

### Changed — BREAKING

- **Every skill and agent is renamed to `fa-<plugin>-<name>`.** A skill's directory name must equal its frontmatter `name`, so this moved directories, frontmatter and every cross-reference in one pass: 11 skill directories and 6 agent files, with 194 reference rewrites across 53 files. `fa-engineering` became `fa-engineering-implementation`; `fa-ui-engineering` became `fa-engineering-ui`; `fa-unit-economics-model` became `fa-product-unit-economics-model`. Product's other 13 skills and all 11 of its agents already conformed. Anything naming a skill by its old name — notes, prompts, agent bodies — must be updated; the names are the contract.

### Added

- **`foundation` grows from 3 skills to 9, and gains `agents/`, `references/` and `scripts/`** (none of which existed). The six new skills carry the context-layer discipline: `fa-foundation-session-close`, `fa-foundation-memory-maintenance`, `fa-foundation-context-authoring`, `fa-foundation-repo-durability`, `fa-foundation-context-audit`, `fa-foundation-registry-audit`. Three new agents: `fa-foundation-context-auditor`, `fa-foundation-repo-durability-auditor` and `fa-foundation-skills-auditor`.
- **`plugins/foundation/references/`** — `context-cascade.md` (the instruction-vs-state doctrine and the memory topic-store shape), `ownership-and-conventions.md`, and `context-audit-findings.md` (the finding-code catalogue, so skills can name codes without inlining them).
- **`plugins/foundation/scripts/context-audit.py`** — a mechanical context-layer checker, invoked as `${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py`. Scopes are supplied by repeatable `--scope` rather than hardcoded, and the registry check is opt-in via `--registry`. **Two real bugs are fixed relative to the internal script it derives from:** it now opens each `SKILL.md` and validates the frontmatter (previously it only checked the file existed, so corrupt frontmatter was undetectable by construction), and its skill-index walk reaches `.github/skills/` under `--all`. Verified against a repository holding 16 skills whose frontmatter was missing its opening `---`: the old check reported 0, the new one reports all 16.
- **`engineering` grows from 7 skills to 16**: `fa-engineering-deploy-chain` (the credential set that does not inherit, the deployment target that must exist before its identifier has a value, and the namespace mismatch that makes every job green while nothing redeploys), five per-language baselines (`fa-engineering-stack-{web-ts,dotnet-api,dotnet-maui,flutter,python}`), plus `fa-engineering-e2e-testing`, `fa-engineering-containerization` and `fa-engineering-manual-regression`.

### Changed

- `fa-foundation-create-skill` and `fa-foundation-create-agent` absorbed a skill-maintenance discipline: the four-part bar for a new skill (repeated · procedural · trap-bearing · uncovered), the rule that a `description` is a **trigger** and not a summary of the body, delete-without-ceremony for stale skills, and the honest-limits note that a shell grant makes a "read-only" agent read-only *by instruction*, not by tool grant.
- `fa-engineering-default-tech-stack` gained the **declared-deviation** rule: an undeclared deviation from the baseline is a finding regardless of how good the technical argument is.

## [11.0.0] - 2026-08-21

### MCP server (`src/Gaia.Mcp.Server`)

#### Changed

- **SDK upgraded `ModelContextProtocol` 1.4.0 → 2.2.0** (`Microsoft.Extensions.AI.Abstractions` 10.7.0 → 10.9.0). The server now negotiates spec revision **2026-07-28** (with automatic back-compat down to legacy `initialize` clients) and runs the HTTP transport in `StatefulForInitializeClients` hybrid session mode — stateless for discovery-based clients, stateful sessions for legacy ones.
- `ServerInfo` version aligned to the product (`11.0.0`, was hardcoded `1.0.0`); server now also carries a `Title`.
- All domain tools carry spec tool annotations (`readOnlyHint`, `destructiveHint`, `idempotentHint`, `openWorldHint`) and typed-return tools (`tasks_create`, `tasks_list`, `memory_*`, `evolve_log`/`evolve_list`) emit `structuredContent` with auto-inferred `outputSchema`.
- stdio mode now logs to stderr instead of muting all diagnostics; stdout stays reserved for the protocol.
- Dockerfile hardened: non-root `USER`, `VOLUME /app/data` (+ `GAIA_DATA_DIR` default), and a verified MCP-initialize `HEALTHCHECK` (`ping` was removed in spec 2026-07-28 and the server rejects it).

#### Added

- **`sampling_summarize`** — example tool that borrows the *client's* LLM via MCP sampling: MRTR-native (`input_required` / retry-with-responses, stateless-safe) with a legacy `sampling/createMessage` fallback for stateful down-level clients, and a graceful message when the client supports neither.
- **`example_echo` / `example_echo_by_letter`** — the example tools are now actually registered (they existed but were never wired into the server). `example_echo_by_letter` emits one `notifications/progress` per character (progress/total/message) for end-to-end progress verification.
- Verified by a comprehensive throwaway MCP test client (SDK 2.2.0, Streamable HTTP): 36/36 checks green across all 18 tools — tool listing + annotations + output schemas, progress notification ordering and payloads, the sampling round-trip via a mock client LLM, the full completion-contract error ladder, memory upsert/prefix/forget semantics, evolve filters, and unknown-tool errors.

### All plugins

#### Changed

- **Agent schema migrated to the Claude Code sub-agents spec.** All agent definitions previously used GitHub Copilot's frontmatter schema (`tools: ["gaia/*", "read", ...]`, `disable-model-invocation`, `user-invocable`), which made every agent un-launchable in Claude Code — invalid tool names cause a launch refusal. Agents now carry `name` + `description`, omit `tools:` to inherit everything including MCP, and use `disallowedTools:` where a role is deliberately restricted (reviewers, planners, testers).
- Reference docs consolidated to one canonical copy per plugin at `plugins/<name>/references/`, with skill links repaired to plain plugin-internal relative paths (no path escapes the plugin root).
- The MCP task graph (`tasks_create` / `tasks_update` / `tasks_complete` with gates, proof, and blockers) is now the only authoritative planning model; all references to a `gaia_plan.md` file (which never existed) are gone.
- Skill frontmatter normalized to the portable subset (`name`, `description`, `license`, `compatibility`, `metadata`, `allowed-tools`); every skill carries `license: MIT`.
- Marketplace owner unified as "FrostAura Technologies" across both manifests; `engineering` and `product` now declare `"dependencies": ["foundation"]`.
- Documentation links updated to `code.claude.com/docs/en/...`.

#### Removed

- **`personal` plugin removed** from the repo and both marketplace manifests. Its content was a byte-identical clone of `foundation`'s three skills; it is recoverable from git history if ever needed.

### foundation

- `fa-foundation-create-plan` rewritten as a native Claude Code skill (it was a lifted VS Code prompt referencing `#tool:vscode/...` mechanics): alignment via AskUserQuestion, three identical parallel proposal subagents via the Task tool, critique, synthesis, and an on-disk plan file as the handoff artifact.
- `fa-foundation-create-agent`'s vendored agents specification replaced — it was a GitHub Copilot doc scrape — with the current Claude Code sub-agents schema, including tool-name rules, `disallowedTools`, `model`, and validation guidance.
- `fa-foundation-create-skill`'s vendored skills specification cleaned of doc-scrape artifacts and extended with Claude Code-specific frontmatter notes.
- Mirror-tree mandates (`.claude/` ↔ `.github/` duplicated trees) removed from both authoring skills; single tree, Claude Code schema canonical.

### engineering

- All 6 agents migrated to the Claude Code schema (see above); phantom skill references (`fa-agents`, `fa-skills`) corrected to `fa-foundation-create-agent` / `fa-foundation-create-skill` in the foundation plugin.
- Dead `docs/architecture` references reworded to target the consuming repository's tree.
- `repo-structure.md` repaired into one coherent `apps/*` + `packages/*` document (it was two spliced incompatible generations with a literal truncation); legacy `src/` layout kept as explicit migration guidance.
- `fa-delivery-policy.md` (4 copies) and `fa-ownership-and-conventions.md` (7 copies) consolidated and rewritten at `plugins/engineering/references/`.
- Tech-stack baseline refreshed (Tailwind v4 CSS-first configuration noted alongside the v3 path) and extended with an **MCP surface baseline** verified against the live 2026-07-28 spec and SDK 2.x: hybrid session mode, tool annotations, structured output, `IProgress` progress reporting, MRTR-over-deprecated-sampling guidance, initialize-based health probes, and stdio/stderr logging discipline.

### product

- All 11 agents migrated to the Claude Code schema (see above).
- 12 skills' broken links to a deleted team doc repaired via a new plugin-local `references/product-discovery-team.md`; ownership/conventions and delivery-policy references consolidated at `plugins/product/references/`.
- Market facts refreshed: data.ai (retired into Sensor Tower) dropped from live tool rosters; the 15%/30% store-fee binary replaced with channel-aware net-of-fee math (EU DMA terms, US external-purchase links, Play external offers); Apple IAP guideline citations corrected from 4.5.4 to the 3.1.1 family; compliance facts updated (Belgium loot-box status, Japan kompu-gacha, FTC 2025 Cognosphere benchmark, PEGI 16 floor from June 2026).
- Unit-economics model schema extended with `store_commission_by_channel[]` / `net_by_channel[]`.
- `plugin.json` keywords, tags, and category corrected from foundation copy-paste debris to product-appropriate values.

## [10.0.1] and earlier

No changelog was kept before 11.0.0.
