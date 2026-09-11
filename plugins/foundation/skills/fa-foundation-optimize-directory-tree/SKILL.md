---
name: fa-foundation-optimize-directory-tree
description: Provides the whole-tree bootstrap that puts the Gaia context layer onto an existing directory tree — an instruction file and MEMORY.md topic store at every scope that earns one, a root registry, and the retrospective design documentation its projects never had — and re-validates a bootstrapped tree against its projects' source. Use it by interviewing once, freezing a partition map, fanning out one briefed subagent per disjoint subtree to read each project against its own source, and reconciling root last. Use it when a tree of code, research or operational documentation has no context layer, or when one exists that was never derived from what the projects actually are. It never edits one scope's pair inside a cascade that already exists — that is `fa-foundation-context-authoring`; never audits an existing layer against itself — `fa-foundation-context-audit`; never writes one repository's design docs alone — `fa-engineering-architecture`; and never scaffolds application code.
license: MIT
---

# Optimize Directory Tree

## Scope and when to use

Use this skill to put the Gaia context layer onto a directory tree that has none, and to keep an existing one true. A **scope** is any directory carrying the **pair** — its instruction file (`CLAUDE.md` or `AGENTS.md`) plus a `memory/` topic store and the `MEMORY.md` index derived from it; a **grouping directory** is one whose only job is holding scopes (`projects/`, `packages/`, `apps/`, `services/`), a term with no industry usage and therefore defined here rather than assumed.
Depth is earned, never mechanical: a directory gets a pair when it has working rules an agent would otherwise get wrong — so a single repository with no sub-projects is a legitimate degenerate partition of exactly one scope, written by the caller rather than dispatched to a branch.

Use this skill when:

- a tree of code, research or operational documentation has no context layer anywhere in it
- a layer exists but was never derived from what the projects actually are, and its claims need re-deriving from source
- the design documentation the tree's projects never had has to be written from the code itself
- the layer is due a sweep — stale claims pruned, converged topics consolidated, scopes that never earned a pair removed

Do not use this skill when:

- one scope's pair is being written or refreshed inside a cascade that already exists — that is `fa-foundation-context-authoring`, or `fa-foundation-memory-maintenance` where the store is structurally sound and merely stale
- the question is only whether the context files, registries and tree agree with disk and with each other — that is `fa-foundation-context-audit`
- only the registry needs reconciling against what is on disk — that is `fa-foundation-registry-audit`
- one repository's `docs/architecture/` is the deliverable — that is `fa-engineering-architecture`, which owns those documents and their templates

## Required inputs

- the tree root, plus the owner's answers to the orientation interview — or the two testable conditions that waive it
- read access to every project's own source; `foundation` alone establishes the context layer, and retrospective design documentation additionally requires the `engineering` plugin installed
- a subagent-capable runtime, or an explicit acceptance of the single-agent fallback below
- every instruction, memory and `docs/` file the tree already carries, read end to end before any of them is touched

## Owned outputs

- the pair at every scope that earned one, plus a root registry, routing rules, naming conventions and the pasted fan-out block
- a frozen partition map, written as a root `memory/` topic and re-read by every later run to detect its own mode
- retrospective design documentation for every project with an executable surface, on the engineering plugin's four templates
- a change manifest — path, new-versus-modified, one-line why — grouped by repository, with everything left uncommitted for the owner

## Decision tree

- If deciding what a context file should say requires reading the projects' **source**, it is this skill; if it requires only checking context files against each other and against disk, it is `fa-foundation-context-audit`, and re-validation hands per-scope content drift to that skill rather than reimplementing it.
- If the cascade already exists and one scope's pair is wrong, it is `fa-foundation-context-authoring`; if that scope's store is sound and merely stale, it is `fa-foundation-memory-maintenance`. This skill **creates** memory stores where none exist and never deep-refreshes one.
- If the registry merely needs reconciling against disk, it is `fa-foundation-registry-audit`. This skill **establishes** the registry once from the frozen partition map; re-validation mode calls that skill for everything after.
- If a repository's `docs/architecture/` is the deliverable, that belongs to `fa-engineering-architecture` — this skill owns the **analysis** that makes writing them possible and hands it over. Where the engineering plugin is absent, record a `question` topic and continue; never invent a substitute template.
- If the root pair exists but no partition map is found, re-partition and diff against disk — never read a missing map as greenfield, because every scope is under standing instruction to prune topics whose concern died.

## Core workflow

1. **Orient shallowly, then interview — in that order, and once.** Ask only what the filesystem cannot answer; bring the answer and ask for confirmation.
   Skip it wholesale under exactly two testable conditions: the request carries an explicit autonomy instruction, or this session cannot reach a human. Anything ambiguous asks — asking wrongly costs one exchange, skipping wrongly writes a stranger's tree from guesses.
2. **Detect the mode from disk, partition the tree, and freeze the map before launching anything.** No root pair and no map → **greenfield**; a map naming scopes that still lack pairs → **resume**; a map holding `analysis-depth: pending` entries → **continue**; root pair present and every entry `deep` → **re-validation**; re-validation returning no source drift → **sweep**, the terminal steady state a healthy tree spends most of its life in.
   Enumerate candidates from cheap filesystem markers only and cap the list before applying judgement, or a large tree exhausts the caller's context before anything reaches disk; give every scope a `writer` and an `analysis-depth`; cap concurrent dispatch at 8 per wave and stage analysis depth above 25 analysable projects, giving each deferred scope a minimal pair so it does not fire `MISSING-PAIR` against the layer this run just built. Write the map as a root `memory/` topic — `type: reference`, opening with a do-not-delete line naming this skill's mode detection as its reader — and generate the `MEMORY.md` index above it in the same action with `--fix-index`, never by hand, splitting the map by grouping above roughly forty entries so each file stays inside the topic size caps.
3. **Capture the undo baseline, then draft root's *rules* — never its roster.** Byte-copy to `<ROOT_PATH>/.tmp/pre-bootstrap/<relpath>` any pre-existing instruction or memory file whose current content is **not recoverable from HEAD** — unversioned, untracked, or dirty — and record every repository as `clean-before` or `dirty-before`.
   Root's rules are held in the caller's context so every branch has a parent to compute its delta against; they land on disk at step 7, because a root file asserting rules no leaf implements reads as a completed bootstrap. The roster is written last, from what the branches found.
4. **State the write plan, then fan out.** Scope count, projects to be analysed, estimated file count, the per-repository `.gitignore` edits, the write allow-list, whether builds will run — stated in **every** run; only the blocking go-ahead is waived by step 1, because one misfiring predicate otherwise produces an unannounced multi-hundred-file write.
   Dispatch one **general-purpose** subagent per disjoint subtree, in a single dispatch, pasting the body of `references/analysis-brief.md` verbatim with exactly three substituted tokens — `<ROOT_PATH>`, `<SCOPE_PATH>` and `<MODE>` — plus a "parent rules in effect" block; never dispatch `fa-foundation-context-auditor`, which carries its own method and converts a briefed branch into an unbriefed one that looks identical. Give the judgement-heavy branches the strongest model and highest reasoning effort the session offers — **a tier, never a model name**. Each branch drains its report to `<ROOT_PATH>/.tmp/reports/` and returns a pointer plus its registry row. Where subagents do not exist, the partition, the one brief and the report contract still bind: run one scope at a time and reconcile root last.
5. **Assess each project against its own source, route its design docs, then verify what was written.** Where docs exist, keep their convention and return doc-versus-source disagreements as findings, never applying them in the branch; where they are absent, the branch invokes `fa-engineering-architecture` inside its own scope.
   Two gates, both the branch's: the project must have an **executable surface** — build file, entrypoint, routes, schema, deployable artifact — and the four templates are gated **individually**, `user-interface-template.md` only where a human-facing interface exists in source. A template judged inapplicable is one line in a `decision` topic, not a file. Where a branch wrote docs from scratch, a second subagent gets the same pasted brief with the `verify-doc` token and returns only citation failures, aspiration in the present tense and unmarked inference; the caller applies them at step 7, never the branch that wrote the doc.
6. **Write the pair at every scope the partition did not hand to a branch, leaves first — adopting, never overwriting.** That is the grouping directories whose children span more than one branch, and the batched small or empty scopes; the map's `writer` field makes the split checkable from either end.
   A pre-existing instruction file is read whole, its state-bearing sentences split into the new memory store, and only missing sections appended; anything not confidently classifiable becomes a `question` topic rather than a deletion, and an existing `AGENTS.md` or `CLAUDE.md` is never deleted or renamed. This skill owns *coverage*; `fa-foundation-context-authoring` owns each file's shape.
7. **Reconcile root last, once, from the drained reports; paste the fan-out block verbatim; run the audit script; leave everything uncommitted.** Pass every mapped scope as its own argument, at every depth — `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --scope <dir> --scope <dir> … --fix-index`, which also regenerates every index the branches' topic files derive to — with `--registry` only where the tree carries a full markdown registry, and `--instruction-file` where the tree chose `AGENTS.md`.
   Assemble the change manifest by concatenating the reports' `files written` blocks, never by re-walking the tree, which cannot tell what this run wrote from what was already there. Apply the sweep's proposed deletions and the verify pass's findings here. Where a hosted memory store holds entries for this tree, recall them, reconcile toward local, and record every contradiction as a `question` topic — **the tree's own memory is the source of truth; hosted tooling is used alongside it, never instead of it.** Propose capability adoptions — plugin capabilities a level should now name, a `docs/` layer to point at, a manifest the registry could defer to — as `decision` topics carrying their rationale, and **never install a plugin, edit a settings file, add a hook or enable a tool.**

## Reading a project into documentation it never had

This is the only content nobody else owns, and everything downstream inherits it. Slop here does not look like laziness — it looks like a well-structured document about a system that does not exist. **All six evidence rules live verbatim in `references/analysis-brief.md`, because the branch reads the brief and never reads this file.** Two of them are the caller's, enforced at reconciliation:

- **Rank the evidence and say which rank each claim came from.** Build files, entrypoints, routes, schema and config are what the project *is*; tests are what it is *believed* to do; history is what *happened*; README, roadmap and pitch are what somebody *hoped*. **A README is a claim, not evidence.**
- **History is optional and never substituted for.** Where there is no version control, report it as absent — mtimes record sync events, not work — and treat the absence itself as a `watch`-worthy finding.
- **Reject uncited architectural claims and downgrade unmarked inference** at step 7, from the verify pass's findings — the agent that wrote a document never gets to defend it.

The four the brief carries alone: never write aspiration in the present tense · mark inference inline · a directory of loose files is not a system, so say what it actually is in one honest sentence · confirm the project is alive first, because dormancy is one paragraph and a dated `state` topic, not a document set.

Plus the standing prohibition, which binds the caller exactly as it binds a branch: **never transcribe a credential, token or connection-string value.** Name the variable and where it is sourced; a secret found committed is a `watch` topic describing the location without reproducing the value.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| interview skipped under an autonomy instruction | take every filesystem default and record each inference as a `question` topic, owner `unassigned` | caller | owner reviews the questions before the layer is trusted |
| a branch fails, times out, or returns off-contract | roll its scope back from the discarded report's `files written` block, re-run once, then demote to `watch` | caller | never re-run a scope whose report has no `files written` block — flag it for human review first |
| engineering plugin absent | record the gap as a `question` topic and continue; never invent a substitute template | caller | owner installs `engineering` and re-runs in re-validation mode |
| pre-existing instruction file found | adopt it, split its state into the new store, append only what is missing | branch | caller, for anything not confidently classifiable |
| script reports findings this run itself created | classify as bootstrap residue against the expected-findings list, and do not chase them | caller | none — a dirty tree per repository is the intended terminal state |

## Anti-patterns

- do not let a pass only add — deletion and consolidation are the deliverable of a sweep, and a pass that only adds has failed
- do not sweep by grepping: read whole files, because search locates a scope and never assesses one
- do not put two agents in one subtree — the second silently overwrites the first, and neither report says so
- do not reword the pasted fan-out block or the branch brief; a paraphrase is a second standard the moment it lands
- do not give a directory a pair because it exists — mechanical depth taxes every future session forever
- do not install anything, run a build, or edit product code in a stranger's tree; propose it in the manifest and let the owner decide

## Handoff and downstream impact

- give the owner the change manifest **before** session close empties `.tmp/`, naming `.tmp/pre-bootstrap/` as the only undo path for content not recoverable from HEAD
- tell the owner that every touched repository is dirty by construction, and that this run's close-out is deliberately commit-free — the tree may not be a repository at all, so `fa-foundation-session-close`'s local-commit step is suppressed here, and the reason is stated rather than left for an auditor to file as a defect
- give `fa-engineering-architecture` the doc-versus-source disagreements and any README contradictions the branches returned, as follow-ups rather than side effects
- give `fa-foundation-context-audit` and `fa-foundation-registry-audit` a tree they can now run against, and `fa-foundation-create-skill` any local skill the sweep proposed adding, rewriting or deleting

## Examples

- **Good fit — establish:** a folder holding three unrelated side projects with no instruction file anywhere in it, one of them dormant and one not software at all.
- **Good fit — maintain, then sweep:** *"assess all projects and directories in this parent directory; most have instruction and memory files; create them where missing, and fix the drift between each project's real intent and state and what its files claim"* — re-derived from each project's source, then the sweep a healthy tree runs most often, where deletion is a normal outcome rather than data loss.
- **Not a fit:** one repository's `docs/architecture/` has gone stale against its code — that is `fa-engineering-architecture`, and a single scope's pair drifting is an authoring or an audit job.

## Completion checklist

- the partition map is on disk as a root `memory/` topic, and every scope in it carries a `writer` and an `analysis-depth`
- every scope was written by exactly one writer, and root was reconciled last, once, from drained reports
- every architectural claim carries a file-path citation, no template placeholder survives in any written document, and no owner name was invented
- the audit script ran with one scope argument per mapped scope at every depth, every `MEMORY.md` was generated from its topic files rather than hand-written, and the residue is listed as expected findings rather than chased
- the change manifest is in the owner's hands, all scratch is emptied, and nothing was committed or pushed

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit finding codes](../../references/context-audit-findings.md)
- [Context templates](references/context-templates.md)
- [The branch analysis brief](references/analysis-brief.md)
- [The orientation interview](references/orientation-interview.md)
