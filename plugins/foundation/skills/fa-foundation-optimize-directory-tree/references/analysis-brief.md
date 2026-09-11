# The branch analysis brief

This file is the brief handed to every subagent dispatched by `fa-foundation-optimize-directory-tree`. The caller opens it, substitutes exactly three tokens, and **pastes the body below the horizontal rule verbatim** into each dispatch, alongside a `Parent rules in effect` block carrying the draft root rules.

**It is byte-identical across branches on purpose.** Differences between reports are then real signal about the tree rather than noise from differently-worded instructions. A caller that summarises this brief instead of pasting it has re-introduced exactly the drift the single-source rule exists to remove, and nothing in the returned reports announces that it happened.

**It is also the compensating control for briefing generic subagents rather than shipping named agent definitions.** The method lives in one editable file without committing to a fixed roster of roles. The consequence is that a branch reads *this* and never reads `SKILL.md`: anything a branch must know lives here or nowhere. When the method changes, this file changes — never the wording of one dispatch.

## The three tokens

| Token | Substituted with |
|---|---|
| `<SCOPE_PATH>` | The absolute path of the one subtree this branch owns |
| `<MODE>` | Exactly one of `bootstrap`, `re-validate`, `verify-doc`, `sweep` |
| `<ROOT_PATH>` | The absolute path of the tree root this run was launched against — the scope whose `.tmp/` every branch drains its report into |

**The scope slug is derived from those, never substituted.** Where the pasted body says `<scope-slug>`, the branch writes `<SCOPE_PATH>` expressed relative to `<ROOT_PATH>` with every path separator replaced by `-`: a scope at `<ROOT_PATH>/packages/api` slugs as `packages-api`, and a branch whose scope *is* the root slugs as `root`. It is unique by construction — the partition is disjoint, so no two branches share a relative path, and no branch can overwrite another's drained report.

Nothing else is substituted — three tokens and no more. No plugin-root path, no per-branch preamble, no extra instruction. A caller that appends a special-case sentence for one branch has made that branch's report incomparable with its siblings.

## The version marker

The pasted body opens with a version marker, and every report must quote it back on its own first line. The marker is a **formality check** — cheap, and it catches a brief that never arrived or was truncated. It is not the compliance check. The fixed return schema, and the mandatory `files written` block inside it, are what actually prove a branch worked to this method; a non-compliant branch can quote a marker it read and then ignore everything under it.

Bump the marker whenever the pasted body changes, so a report quoting an older marker is visibly from a stale dispatch.

---

`ANALYSIS-BRIEF v2`

## Acknowledgement

Open your report with this exact line, and nothing else on that line:

```
ANALYSIS-BRIEF v2 acknowledged — scope <SCOPE_PATH>, mode <MODE>
```

## Your scope and your mode

You own the subtree at `<SCOPE_PATH>` and nothing else. You are running in mode `<MODE>`.

Other branches are working other subtrees at the same time. The partition that gave you this scope is frozen: **do not re-partition, do not follow a symlink or a reference out of your subtree, and do not act on anything you notice above or beside you.** Two agents inside one subtree is the failure this partition exists to prevent — the second silently overwrites the first, last writer wins, and neither report says so. Anything you notice outside your scope is a finding you return, never a change you make.

## Read first, in this order

1. **The instruction file and `MEMORY.md` at every level above `<SCOPE_PATH>`**, plus the `Parent rules in effect` block pasted with this brief. You cannot compute a delta against a parent you have not read.
2. **The root partition-map topic** in the root `memory/` directory. It records your scope's writer and its analysis depth. If it marks children of your scope as deferred, see *Deferred children* below.
3. **Every context file already inside `<SCOPE_PATH>`, end to end** — the pair at each level, every `memory/` topic, every local `SKILL.md`. Read them whole. **Search locates a scope; it never assesses one.** Nothing in a search result can tell you that a topic's concern has died, or that the same rule now sits at three depths and belongs at one.
4. **The project sources themselves**, ranked as below.

## Delta discipline

**Write only what your level adds. Never restate a parent; link to it.** Restatement is how the same paragraph ends up at four depths and then disagrees with itself, and the copies diverge quietly — nothing fires when they do.

Three things sharpen the rule, because authors violate it while believing they are complying:

- **Specificity is a delta; echoing is not.** If the parent says a default applies, naming which version, which flags, which entrypoint is new information only this level can supply. Repeating the parent's statement of what the default *is* creates a second copy that goes wrong the moment the parent is reworded.
- **A deviation is stated without its rule.** "No database and no ORM: this service is stateless by design — a deliberate departure from the parent default, recorded as a decision in the memory store" is the whole delta. Restating the default before departing from it adds nothing and rots.
- **The single sanctioned restatement** is the upkeep clause's statement that `MEMORY.md` is required at this level in its own right and cascades downward on identical terms. It is repeated deliberately, because an agent entering the tree at this depth may never read the level that states it. Restate rules about how to *read* the cascade; never restate rules the cascade already delivers.

Your report names, in its own block, **the parent rules you deliberately did not restate**. That block is how the caller distinguishes delta discipline from an omission.

## The six evidence rules

These govern every claim you write, in every mode. Slop here does not look like laziness — it looks like a well-structured document about a system that does not exist.

1. **Rank the evidence and say which rank each claim came from.** Build files, entrypoints, routes, schema and config are what the thing **is**. Tests are what it is **believed** to do. History is what **happened**, and it is optional: where there is no version control, report its absence — **never substitute file modification times**, which on a synced or restored volume record sync events rather than work — and treat "no version control" as itself a `watch`-worthy finding. README, roadmap and pitch material are what somebody **hoped**. **A README is a claim, not evidence.**
2. **Every architectural claim carries a file-path citation.** An uncited claim is downgraded to marked inference or deleted. The caller enforces this at reconciliation and rejects uncited claims wholesale, so citing as you go is cheaper than being sent back.
3. **Never write aspiration in the present tense.** "The service exposes a webhook endpoint" is a lie if the endpoint is a roadmap item. Planned work is named as planned, with its source.
4. **Mark inference inline.** Where you concluded rather than read, say so in the sentence: *(inferred from the route table; no test covers it)*. An unmarked inference is indistinguishable from a verified fact three months later, and that is when it gets acted on.
5. **A directory of loose files is not a system.** Say what it actually is, in one honest sentence. "No build step — this is a document set, not a system" is correct content when true. An invented command or a blank section is a defect, and nothing mechanical catches either.
6. **Confirm the project is alive before documenting it.** Dormancy is one paragraph and a dated `state` topic, not a document set. Writing a full architecture baseline for something abandoned two years ago spends the owner's attention and then misleads the next reader about what the tree contains.

**Standing prohibition, in every mode: never transcribe a credential, token, key or connection-string value.** Name the variable and where it is sourced — never its content. A secret you find committed is a `watch` topic describing the location and the exposure **without reproducing the value**; a report or a context file that quotes it has copied the secret into a second place and widened the exposure you were reporting.

## The memory format contract

You write memory files in almost every mode, and **this brief is the only place you will ever see their format.** You do not read the calling skill, and a link from here into the plugin's own reference layer does not resolve from inside the tree you are working in — so the contract is stated below in full and verbatim. It is exact rather than stylistic: a mechanical check enforces every constant here, and a file that misses one is a finding against the layer this run just built.

**`MEMORY.md` is a pure index, and it is *derived* from the topic files — you never write it by hand.** Its entire content is one `# ` heading plus one line per topic file in the `memory/` directory beside it, and every part of every line is computed: the heading from the scope's path, each title from the topic's `# H1`, each hook from the topic's `description:`, the order from the topic's `type:`. Write the topic files, then have the caller regenerate the index; a hand-written index is reported as `MEMORY-INDEX-STALE` the moment it differs by a byte.

```markdown
# MEMORY — <scope path relative to the tree root>

- [Red — there is no safe blanket push here](memory/alerts.md) — the description of alerts.md, verbatim
- [Current state](memory/state.md) — the description of state.md, verbatim
```

Entries are ordered by type — `alert`, `state`, `decision`, `gotcha`, `question`, `watch`, `kill-record`, `evidence`, `log`, `reference` — with a type's canonical file (`decisions.md`) before its split siblings (`decisions-sync.md`), then alphabetically. **The hook is the signal, not a label:** "deploy decided, unpushed range still local, kill clock void once live" is a hook; "notes about deployment" is a label. A reader who stops at the index must still leave informed — so that sentence belongs in the topic's `description:`, which is the only place it can be written.

**Every topic file opens with exactly this six-line frontmatter block** — the four keys, in this order, with **no extras** — so that `head -7` of any topic returns the complete relevance signal plus the first body line:

```markdown
---
name: <scope slug>-<file stem>
description: "one line, at most 240 characters — the index hook"
type: state
last_verified: YYYY-MM-DD
---

# <Topic heading — becomes the index title>
```

`name` is exactly `<scope slug>-<file stem>`, kebab-case: the scope's own directory name lowercased with dots, underscores and spaces as dashes, a *grouping* directory prefixed by its parent's slug, the tree root `root`. So `packages/ingest/memory/gotchas.md` is `ingest-gotchas`. Anything else is `MEMORY-TOPIC-NAME`.

`description` is one line and **at most 240 characters** — it *is* the index hook, so it carries the signal and never summarises the body; past the cap the topic is two topics, or the description is narrating (`MEMORY-DESCRIPTION-LONG`). `last_verified` must parse as a real `YYYY-MM-DD` date and must not be in the future. A fifth key, a reordered key, or anything other than the closing `---` on line six fails the check.

**The first body line is a `# Heading`** — the index title is taken from it (`MEMORY-TOPIC-NO-HEADING`) — and an `alert`'s heading begins `Red — `, so the hazard reads as one at a glance (`MEMORY-ALERT-HEADING`).

**`type:` is one of exactly ten values, always singular.** The *file* may be `decisions.md`; the *type* is `decision`. A plural defeats every filter that reads it:

`state` · `decision` · `gotcha` · `question` · `watch` · `kill-record` · `alert` · `log` · `evidence` · `reference`

Nothing outside that list is valid. Do not coin an eleventh — if no type fits, the concern is probably not a topic.

**Keep a topic file under 60 lines *and* 6,000 characters of body.** Past either it is almost always two topics wearing one filename — the line cap alone was met by files of paragraph-long lines, which is what the character cap closes. But **split on the real seam, never at the line count**: a topic split to satisfy the cap produces two halves nobody can name, and each half's index hook degrades into a label. A long file with genuinely one concern is one topic that needs *pruning*, not splitting. Where you do split, each half states its own remit and names its sibling, from both sides.

**Write protocol.** New concern → a new topic file. Changed concern → edit the topic and restamp its `last_verified`, re-cutting its `description:` if the signal moved. Dead concern → delete the file. In every case the index is regenerated afterwards, never edited; topic files carry no `## Upkeep` section of their own. Relative links inside a topic file resolve from `memory/`, one level below the scope — prefix them `../`.

**Every instruction file you write ends with the canonical `## Upkeep` clause, emitted in full at every level** — never a pointer to another level's copy, because the reader who most needs it entered at this depth and will never open the other file. `<children>` is the only token to substitute (`every project below`, `every package here`, `every child scope`); at a scope with no children, drop that clause:

```markdown
## Upkeep

This file, `MEMORY.md`, its `memory/` topic store, and the skills that fire at this level are kept current **as changes land**, not in a later cleanup pass. `MEMORY.md` is required here exactly as this file is, and it cascades downward on identical terms — <children> carries its own pair too, and an instruction file with no `MEMORY.md` beside it is a defect, not a shortcut: the rules are stated and reality is left unstated, so the next agent infers status instead of reading it. Both files, or neither.

- **Memory — always.** Update the topic files at the end of any session that shipped, decided, discovered, abandoned or unblocked something; re-cut the `description:` of any topic whose signal moved, then regenerate `MEMORY.md` from the topic files rather than editing it; restamp `last_verified:` only for what you actually re-inspected. Record decisions *with their why* — a decision without its rationale is reversed by the next agent, who sees only its cost.
- **This file — only when a rule changed.** A new convention, command or invariant earns an edit. A status change does not; it belongs in the memory store.
- **Skills — when *how* the work is done changed.** Fix or delete a skill whose commands or thresholds no longer exist; a stale skill is worse than a missing one because it fires with authority. A procedure that would be correct in a repository with nothing to do with this tree belongs upstream in a plugin, never forked into this tree.
- **Scratch — always.** Empty `.tmp/` before the session ends, promoting anything that still mattered first.

None of these four are assessed by search. Read the scope's files end to end before changing any of them. Drift is not caused by neglect over months; it is caused by good sessions that ended without a close-out.
```

Two notes on that clause. Its second sentence — that `MEMORY.md` is required here and cascades on identical terms — is **the single sanctioned restatement** named under *Delta discipline* above; keep it at every level, and do not delete it from a child while applying the delta rule. And the root's copy carries one extra line naming the mechanical check: **root is the caller's to write, never yours**, so do not add that line to a scope inside `<SCOPE_PATH>`.

## Mode contracts

Exactly one applies. Read yours; the others do not bind you.

### `bootstrap` — writes files, returns the full schema

Establish the context layer inside `<SCOPE_PATH>`: the pair at each level of your subtree that earns one, its `memory/` topic store, and — where both documentation gates below pass — its design documentation.

- **Depth is earned, never mechanical.** A directory gets a pair when it has working rules an agent would otherwise get wrong. Adding one to every directory taxes every future session forever.
- **Adopt, never overwrite.** Read any pre-existing instruction file whole. Split its state-bearing sentences into the new memory store, append only the sections that are missing, and leave its own wording alone. Anything you cannot confidently classify becomes a `question` topic, not a deletion. Never delete or rename an existing instruction file under either filename.
- **Match the tree's filename convention.** Whichever instruction filename the tree uses carries the content at every level; the other, where it exists, stays a thin pointer and never accumulates policy of its own.
- **State goes to memory; rules stay in the instruction file.** A sentence carrying a date, a status, a version, a count that moves, a "currently" or a "we decided" is memory. Navigation is instruction; condition is memory.

**Deferred children.** If the partition map marks children of your scope as deferred, write each of them a *minimal* pair now: an identity line, a pointer to the parent, an `## Upkeep` clause, and a `MEMORY.md` index over a single `state` topic recording that the scope has not yet been analysed against its source and was deferred by the staged run. Name every deferred child in your report. Without the minimal pair, the run ends by reporting a missing pair for every deferred child — against the layer it just built.

### `re-validate` — writes nothing, returns findings only

You are checking whether what the existing context files claim is still true of what the projects have actually become. **Write no files. Create no topics. Fix nothing.**

Return one finding per divergence, each with four parts:

| Part | Content |
|---|---|
| Claim | The assertion under test, in one sentence |
| Current file text | The exact sentence as it stands, with its file and line |
| Source citation | The path in the project that settles it |
| Verdict | `agree` or `drift` |

A finding with no source citation is not a finding — it is an opinion about a file, and the caller will discard it. Drift between context files and *each other*, rather than against source, is not yours to resolve: name it and hand it back.

### `verify-doc` — writes nothing, returns three defect classes only

You are the independent reader of design documentation another branch wrote from scratch. You did not write it, which is the entire point: an agent reviewing its own output defends what it just wrote.

Return **only** these, and nothing else — no praise, no restructuring proposals, no style notes:

1. **Citation failures** — an architectural claim with no file-path citation, or a citation that does not support the claim.
2. **Aspiration in the present tense** — described behaviour the source does not implement.
3. **Unmarked inference** — a conclusion written as a read fact.

Also flag any surviving `{{placeholder}}`, any Document Control status above `Draft`, and any invented owner name. The caller applies your findings; the branch that wrote the document never does.

### `sweep` — prunes and consolidates within your scope, proposes every deletion

> **Deletion and consolidation are the expected outcomes of a sweep. A pass that only adds has failed.**

> **Read whole files; search only locates a scope, it never assesses one.**

A sweep branch that greps is not sweeping. What you are hunting is invisible to search by construction — a search shows a wrong string, never what became redundant or quietly stopped being true:

- **Restatement across levels** — a child repeating what it inherits. The commonest rot, and it always ends with the two copies disagreeing. The `Parent rules in effect` block is what makes this checkable at all.
- **Restatement *within* one file** — the same rule stated twice at near-verbatim length in two sections, already lightly diverged in wording. That is what restatement looks like before it becomes a contradiction.
- **Leakage in both directions** — state that crept into an instruction file, and rules sitting in memory where the next agent will treat them as revocable. A mechanical check surfaces candidates but cannot judge them: a locked rule and a status line look identical to a regex, and driving that finding class to zero almost certainly means a rule was deleted.
- **Resolved items still open** — answered questions, cleared watch entries, contradictions fixed months ago. A watch list carrying resolved rows stops being read at all, which is worse than not having one.
- **Converged and overgrown topics** — two topics that are now one concern, and one that is genuinely two. **Split on the real seam, never at the line count**; a topic split to satisfy a length check produces two halves nobody can name. Where you split, make each half state its own remit and name its sibling, from both sides.
- **Index hooks whose signal moved.** A hook that no longer carries the actual signal is worse than no hook, because it is trusted and it is read first.
- **Pairs at directories that never earned one** — judged by the same test that creates one: has this directory working rules an agent would otherwise get wrong?
- **Local skills** now covered by an installed plugin, or whose description means they fire on nothing. A scope earns a local skill when a procedure there is repeated, ordered and trap-bearing. **A stale skill is worse than a missing one** — it fires with authority. **Anything that would be correct in a repository with nothing to do with this owner belongs upstream in a plugin, never forked into the tree.**
- **Dead pointers, phantom registry rows, and blanket restamps.** Restamp `last_verified` only on what you actually re-inspected. A pass that restamps everything has destroyed the freshness signal it exists to protect.

You may **edit only to consolidate or prune, and only within `<SCOPE_PATH>`.** You may **not delete on your own authority.** Return a `deletions proposed` block — path · what it was · why it is dead — and the caller applies it. A branch that deleted a file itself is indistinguishable from a branch that lost one.

Where a capability now exists that this scope should use but does not — an installed plugin capability a level should name, a machine-readable manifest the registry could defer to instead of duplicating, a reference layer that should be pointed at rather than inlined — **propose it with its rationale.** An adoption recorded without its why is reversed by the next agent who sees only its cost. **Propose only: never install a plugin, edit a settings file, add a hook, or enable a tool.** Those belong to the owner.

## Design documentation — two gates, both yours to apply

In `bootstrap` mode only, and in this order.

**Gate one — the executable surface.** A project earns design documentation only if it has one: a build file, an entrypoint, routes, a schema, or a deployable artifact. A research folder, a writing project or a runbook directory gets **none**. Its substitute is the instruction file's conditional section on how the work there is actually produced and checked, and the absence is recorded as a deliberate `decision` topic, not left looking like a gap.

**Gate two — per template, individually.** Never all-or-nothing:

| Template basename | Emit when |
|---|---|
| `system-components-template.md` | Always, once gate one passes |
| `use-cases-template.md` | There are identifiable actors |
| `class-diagrams-template.md` | There are types or modules worth diagramming |
| `user-interface-template.md` | **Only** where a human-facing interface exists in source — screens, components, view templates, a TUI |

A headless service, a CLI, a library or a data pipeline filled against `user-interface-template.md` returns a fully-formed specification for tokens, motion, moodboards and an accessibility target that do not exist. The citation rule cannot catch it, because there is no source to cite and the writer ends up citing the template's own `{{placeholders}}`. **A template you judge inapplicable is one line in the project's `decision` topic** — *"no interface specification: headless service, no human-facing interface in source"* — **and no file.**

Where documentation already exists, assess against it and **keep its convention**. A disagreement between an existing document and the source is **returned as a finding**, never silently applied.

**Document Control defaults for anything synthesized retrospectively:**

| Field | Value |
|---|---|
| Status | `Draft` — never `Reviewed`, never `Approved`; no human has read it |
| Owner | `unassigned`, unless a real name was supplied with this dispatch |
| Source of Truth | The repo-relative path the claims were read from |
| Last Updated | The date of this run |
| Version | Left at the template default |

**Never synthesize an owner name.** The same rule binds every `question` topic you write: where no human is named, the owner is written literally as `unassigned` and the index hook says so. An invented name reads as an assignment, and the person it names never learns they were assigned.

## Write allow-list

Phrased positively, so it stays correct as new capabilities are installed around it. **Write only:**

- your scope's pair — the instruction file and `MEMORY.md` — at each level of `<SCOPE_PATH>` that earns one;
- that scope's `memory/` topic files;
- `docs/architecture/` inside your scope, where both documentation gates pass.

**`fa-engineering-architecture` is the only engineering skill you may invoke**, and you never take its README-alignment step — return README-versus-architecture contradictions as findings for the caller to hand over. Where the engineering plugin is absent, record the gap as a `question` topic and continue: **never invent a substitute template.** A tree with a correct cascade and no design documents is one install away from complete; a tree full of documents written against a locally-invented template is a second standard that drifts forever.

You inherit the full installed skill roster, and several of those skills fire on exactly the gaps you are cataloguing — a missing linter configuration, an absent container definition, an unwired deploy chain. **Record each gap as a finding. Never remediate one.** Parallel branches acting on their own initiative rewrite a stranger's tooling and README files past the boundary the owner just approved, N times at once, and no single report shows the total.

## Prohibitions

- **Never commit, stage, push, or run any destructive version-control command.** Everything is left uncommitted for the owner to review.
- **Never edit product code.** Not a formatting fix, not a typo, not an obvious bug. Report it.
- **Never run a dependency-install or network-fetching command** — regardless of any consent granted for builds. Those execute third-party code, mutate lock files and reach the network.
- **Run build or test commands only where the `Parent rules in effect` block states the owner granted consent.** Without it, record the documented command as unverified inference plus a `question` topic. Never guess a command, and never present a guess as documented.
- **Never make a claim about a file outside `<SCOPE_PATH>`.**
- **Never write into a vendored tree or a submodule.** They are excluded from the analysis budget and they are somebody else's source.
- **Never transcribe a credential value.** See the standing prohibition above.
- **Never write a report file into the tree.** The drain path below is the only place a report goes.
- **Never record, in a memory topic, a fact about another context file's contents.** A topic may record a fact about the world it verified. "The parent still says X" is false the moment a concurrent branch rewrites the parent — which is exactly what is happening while you write it. Those observations go in the report.
- **Never re-partition, and never write above your scope.**

## Report contract

**Write your full report to `<ROOT_PATH>/.tmp/reports/<scope-slug>.md`**, where `<scope-slug>` is `<SCOPE_PATH>` relative to `<ROOT_PATH>` with every path separator replaced by `-` — `<ROOT_PATH>/packages/api` drains to `<ROOT_PATH>/.tmp/reports/packages-api.md`, and a branch whose scope is the root drains to `root.md`. That is scratch at the run's own scope, emptied when the run closes. That is a deliberate, narrow exception to the rule that a subagent never writes a report into the tree: the caller cannot hold every full report in context and still have room to reconcile the root, and `.tmp/` is not the tree proper.

**Return to your caller, inline:** the acknowledgement line, the drain path, your registry row, and the complete `files written` block. That block is returned inline as well as drained because it is the caller's only rollback source, and rollback must remain possible even if the drained file cannot be read.

The drained report carries these blocks, in this order, every one present even when its body is `none`:

| Block | Content |
|---|---|
| Acknowledgement | The exact line specified above |
| Scope | `<SCOPE_PATH>`, and the levels inside it that earned a pair |
| Registry row | Identifier · owning scope · one-line identity. No status — status is the roster's |
| Cross-scope dependencies | Shared infrastructure, identity, internal APIs, upstream data. **An explicit `none` is a required answer.** Declared non-couplings belong here too — a deliberate mirroring of names is a convention, not a coupling, and saying so prevents a well-meaning refactor |
| Documentation status | Which templates were emitted, which were judged inapplicable and why, or that gate one failed |
| Open questions | Each with an owner, or the literal `unassigned` |
| Parent rules deliberately not restated | The delta you chose not to write, so an omission is distinguishable from discipline |
| Findings | Your mode's payload: divergences, defect classes, or observations outside your remit |
| Deletions proposed | `sweep` only — path · what it was · why it is dead |
| Files written | **Mandatory in every mode.** See below |

### The `files written` block

One row per file you touched:

| Field | Content |
|---|---|
| Path | Absolute |
| Kind | `new` or `modified` |
| Why | One line |
| Backup taken | Whether a pre-run byte-copy of the prior content was made, and where |

**This block is mandatory in every mode.** In `re-validate` and `verify-doc` its only permitted content is the single word `none`; those modes write nothing, and a block listing files is itself a compliance failure the caller must act on.

**A missing or malformed `files written` block is discard-triggering** — the same as a missing version marker, and for a harder reason. It is the only record of what this branch changed, so without it a discarded report cannot be rolled back, and a scope the caller believes is untouched silently contains unverified files. That is worse than a scope that was never analysed at all, because nothing downstream knows to look.
