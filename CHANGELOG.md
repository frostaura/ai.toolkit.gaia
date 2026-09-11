# Changelog

All notable changes to the Gaia plugins are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and versions are lockstep across all plugins. Plugin sources track `ref: main`; the `version` field in each `plugin.json` is the update trigger, and changes reach users when pushed to GitHub.

## [14.3.1] - 2026-09-11

Two checks that were reporting on the wrong text. No new finding codes, no contract change.

### Fixed

- **The instruction-layer scan no longer skips table rows and blockquotes.** Any line starting `|` or `>` was dropped before the date and state-word tests — which exempted exactly the two shapes policy prose most often puts a status in: a rule table whose cells carry "shipped 2026-01-02", and a callout quoting a current condition. Only a table's *separator* row is skipped now, because it is punctuation rather than prose. Expect this to surface findings in files that were previously reported clean; they were never scanned.
- **`UPKEEP-UNSCOPED` accepts every spelling of the same command.** It required `--fix-index` immediately followed by `--scope <own>` with no trailing slash, so a section reading `--scope Labs/projects --fix-index`, or `--scope Labs/projects/`, was reported as unscoped while being correctly scoped. `--scope <own>` is now matched anywhere in the Upkeep section, with an optional trailing slash.

## [14.3.0] - 2026-09-11

A fourth pass over the audit, and every item in it is a check that existed but could not fire, or fired on the wrong text. Nothing here is a new idea; all of it is the previous three releases actually working.

### Added

- **`UPKEEP-UNSCOPED`** — an instruction file whose directory carries a `memory/` store must state the regeneration command *scoped to its own directory* in its `## Upkeep` section (`--fix-index --scope .` at the root). `--scope` existed to stop a fan-out agent rewriting a sibling's index mid-flight, but the command the next agent actually copies is the one written in that section, so an unscoped one there outlived every brief that got it right. The expected scope moves with `--root`, exactly like the memory-topic `name` slug.

### Changed

- **A missing index now runs every per-topic check before one is created.** The missing-index branch returned before the topic loop, so a store with no `MEMORY.md` got exactly one finding and none of the checks over its topics — and `--fix-index` then derived an index from files nothing had validated, carrying a non-kebab-case filename straight into every link built from it. The store is now checked first and the index decided last: defects are reported on the same run that creates the index, and a store that cannot pass them keeps `MEMORY-INDEX-MISSING`.
- **A future `last_verified` blocks index *creation* as well as regeneration.** It is raised by the topic parser, which is what a `--fix-index` write is gated on, rather than in the per-topic loop that runs after. Raised later, the one defect that reads as freshly verified for as long as its date says was blocked from repairing an existing index and sailed straight into a brand-new one.
- **`SCOPE-EMPTY` is reported per `--scope`**, not once for the whole run. A run naming three scopes where only the third is a typo reported nothing at all, because the total was non-zero — the mis-aimed argument was invisible in exactly the run it mattered in.
- **`MEMORY-TOPIC-LONG` is measured over the body**, as the spec says ("60 lines and 6,000 characters of body"). Counting the six frontmatter lines and the blank after them made the real ceiling 53 lines of content, so a topic that obeyed the written rule was reported for breaking it.
- **`--scope` is written as required, not optional, in `fa-foundation-context-authoring`, `fa-foundation-memory-maintenance` and `fa-foundation-session-close`**, and the authoring skill now says to put that same scoped command in the instruction file's `## Upkeep` section.

### Fixed

- **A loose artefact at a repository root is no longer reported twice.** The stray-artifact surfaces are deduplicated by path: `--scope .` at a root — now the form the Upkeep rule asks for — made the root its own scope, so every check ran over it once as the root and again as a scope.
- **`POSSIBLE-STATE-IN-INSTRUCTIONS` no longer exempts a whole line for mentioning `MEMORY.md`.** The state-word branch tested `"MEMORY.md" not in line`, so `**Status:** currently blocked — see MEMORY.md` — a status assertion that happens to route — was silently exempt, which is the exact defect the check exists to catch. Both exemptions now work on the *residue*: the routing reference and every quoted or italic span are stripped, and what is left is tested, so a rule that *names* a state word ("anything with a 'currently' goes in the memory store") stays exempt. The date branch runs on the full residue with no quote stripping, because a date is state under any reading.

## [14.2.0] - 2026-09-11

A third pass over the same machinery, all of it additive: the audit now proves a remote exists instead of trusting the URL string, sweeps the *whole* instruction layer rather than only the instruction file, and refuses three more ways a memory store can silently stop being derivable.

### Added

- **`GIT-REMOTE-MISSING` and `GIT-REMOTE-UNREACHABLE`** — a configured remote is a local string that nothing validates, and repositories have been recorded as "ahead-only, safe to push" against remotes that did not exist. Each remote is now probed with `git ls-remote --exit-code <remote> HEAD` on a 20-second timeout, and the result is *classified*: a credential refusal, an unreachable host and a "repository not found" are the same non-zero exit, and conflating them either invents a lost repository or hides one. An SSH GitHub remote that refuses this machine's key is probed again over its HTTPS twin before anything is concluded, so "the key is missing but the repository is there" and "there is nothing to push to" are reported as the different facts they are.
- **`--no-remote-probe`** — skips that probe, the script's only network call, while keeping every other repo-durability check. For offline runs and for CI that must not depend on a remote answering.
- **`REGISTRY-DUPLICATE-ID`** — the on-disk map is now keyed by `(owning group, name)` rather than by name alone. Keyed by name, a project id repeated under two grouping directories silently overwrote itself: the audit reported one row where two directories existed, and the collision — which is itself a finding, because a memory topic's `name` slug is derived from the directory name — was the one thing that could never surface.
- **`MEMORY-INDEX-MISSING`** — a `memory/` store with no `MEMORY.md` beside it. The store was invisible to a reader entering the scope and, because the memory checks keyed on the index file, invisible to the audit too: deleting the index also deleted every check over the topics it was derived from. `--fix-index` now creates a missing index as well as repairing a stale one.
- **`SCOPE-EMPTY`** — every `--scope` given matched no memory store. A scope that matches nothing is indistinguishable in the output from a scope that is clean, so a typo, a renamed directory or a stale fan-out brief made the run print CLEAN over a subtree it never opened. A `--scope` outside `--root` is now an argument error rather than a finding, because every path in the output and every `--fix-index` write is root-relative.
- **`MEMORY-TOPIC-FILENAME`** — a topic *filename* that is not kebab-case, reported separately from `MEMORY-TOPIC-NAME`. Folded together, the message read as "your `name:` key disagrees with the slug", and the obvious fix was to bend the key to match the broken filename — backwards, since the filename is what every derived index link is built from.

### Changed

- **`POSSIBLE-STATE-IN-INSTRUCTIONS` now sweeps the whole instruction layer**, not only the instruction file: every `SKILL.md`, every skills-directory `README.md` and every agent definition under a `.claude/` or `.github/` root, with the same date and state-word tokens. A skill fires with the same authority as the instruction file and rots at the same speed — a `SKILL.md` that says "currently on v3" misleads every session it triggers in, and the third context artifact was exempt from this check entirely.
- **A UTF-8 BOM is now named as a BOM**, and the six-line-block recital is no longer appended to the BOM, CRLF and unreadable reasons. A BOM makes line 1 read as `﻿---`, so the parser reported "no opening '---' on line 1" against a file whose first line is visibly `---` in every editor — an unfalsifiable finding decorated with advice to rewrite a block that was already correct.
- **A future `last_verified` blocks regeneration**, like every other malformed-frontmatter case. It was the one defect that survived a `--fix-index` run and then read as freshly verified for as long as the date said, which is exactly backwards for a store whose purpose is to be trusted about its own age.
- **The index title escapes `(` and `)` as well as `[` and `]`.** An unescaped `)` closes the link target early, so the rendered index points at a truncated path and the rest of the title leaks out as literal text.
- **The type vocabulary is written in index order everywhere** — `alert`, `state`, `decision`, `gotcha`, `question`, `watch`, `kill-record`, `evidence`, `log`, `reference` — in `references/context-cascade.md`, `fa-foundation-context-authoring` and the `fa-foundation-optimize-directory-tree` branch brief, which listed it in three different orders while the regenerator sorted by one.
- **`fa-engineering-deploy-chain` records activation state in a `memory/` topic and regenerates the index**, instead of writing a row into `MEMORY.md`. Since 14.0.0 a hand-added row is discarded by the next regeneration, so the skill was teaching users to write a record that disappears.

### Fixed

- **`MEMORY-VOLATILE-COUNT` no longer fires on a version string.** `\d[\d,]*` matched the "0," in "14.1.0, unpushed" and reported a *version* as a volatile count — a false positive in the one sentence shape the rule most wants written. The pattern now matches a count with thousands separators and nothing else.
- **Running on a subtree root is documented as supported and guarded.** Point `--root` at one repository inside a larger tree and only that repository is read or written; the registry check is opt-in, and no `--scope` outside the root can be named. The one thing that moves with `--root` is the memory-topic `name` slug, which is derived from the scope's path relative to it.

## [14.1.0] - 2026-09-11

Hardening for the derived-index machinery shipped in 14.0.0, all of it additive: a regenerator that refuses the cases where it would destroy an index rather than repair one, and four new findings for the ways a store silently stops being derivable.

### Added

- **`--scope` now narrows the memory-store checks and `--fix-index`**, not just scope discovery. In a fan-out, every agent passes its own scope, so no agent regenerates a sibling's `MEMORY.md` while that sibling is still editing its topic files. With no `--scope` the whole tree is in scope, exactly as before.
- **`MEMORY-STORE-EMPTY`** — a `memory/` directory holding no topic file. `--fix-index` refuses it rather than truncating the existing index to a bare heading, which is what it used to do: an empty store silently deleted the only copy of a hand-written index.
- **`MEMORY-STORE-DEBRIS`** — anything in `memory/` that is not a `*.md` topic: a `.bak`, a stray note, a subdirectory. It is a second source of truth that nothing checks and no index points at.
- **`MEMORY-DESCRIPTION-LINK`** — a markdown link inside a `description:`. The description is copied verbatim into the index, one directory above `memory/`, so the link resolves from the wrong place. Name the file in backticks instead.
- **`MEMORY-DESCRIPTION-THIN`** — a `description:` under 40 characters, or one that simply repeats the topic's heading. The 240-character ceiling was being defeated from the other side: a one-word hook passes every check and tells a reader nothing.
- **`MEMORY-TOPIC-UPKEEP`** — an `## Upkeep` section inside a topic file. The write protocol *is* a topic's upkeep; a "delete this once X" condition is the topic's last sentence.

### Fixed

- **CRLF topic files are now a `MEMORY-FRONTMATTER` finding and block regeneration.** Files are read as bytes before decoding. A CRLF topic parses perfectly as text and then renders an index nothing can ever match, so the store sat permanently `MEMORY-INDEX-STALE` while every individual file looked correct.
- **The index is compared CRLF-normalised.** A `\r\n` index is a line-ending defect, not a content defect, and reporting it as `MEMORY-INDEX-STALE` sent the reader hunting for a hook that was in fact identical.
- **A `--fix-index` run that rewrote an index no longer also reports `MEMORY-ORPHAN-TOPIC` and `MEMORY-LINK-BROKEN` for that store.** Those describe the index as it was *before* the rewrite, and they sent agents to fix an index that was already correct.
- **The index title escapes `[` and `]` rather than stripping them.** A heading like `Red — [ingest] do not push` is about a thing actually called `[ingest]`; deleting the brackets made the index title disagree with the topic's own `# H1`.
- **`MEMORY-VOLATILE-COUNT` now scans the `description:` as well as the body.** The hook is the most-read line in a store and was the one line exempt from the rule.
- **Vendored third-party libraries are skipped again.** `.pio/` and `libdeps/` by name, and any directory carrying a `library.properties` or `library.json` manifest — an Arduino or PlatformIO library tree. Its README's link debt is upstream's, and forty vendored libraries drown every real finding.

### Changed

- **`--registry` is documented where the script is offered as a registry check.** `fa-foundation-registry-audit` told the reader to run the script and read `REGISTRY-*`; the check is opt-in, so without `--registry PATH` it printed a clean run over a registry it never opened. `fa-foundation-context-audit` now names `--registry` and `--instruction-file` at the same step.
- **`head -7` is `head -8` everywhere.** Line 7 of a topic file is the mandatory blank; the `# H1` every index title is derived from is line 8, so the documented command stopped one line short of the heading it was meant to show. Corrected in `references/context-cascade.md`, `fa-foundation-context-authoring`, `fa-foundation-memory-maintenance` and the `fa-foundation-optimize-directory-tree` branch brief.
- **The cascade reference now states the four rules the tooling enforces**: never hand-edit the index; a description may not contain a link; only `*.md` topics live in `memory/`; pass `--scope` in a fan-out.

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
