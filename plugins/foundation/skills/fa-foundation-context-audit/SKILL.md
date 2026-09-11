---
name: fa-foundation-context-audit
description: Provides the whole-repository context-layer drift sweep — every instruction file, every MEMORY.md and topic store, and every skills directory reconciled against inspected reality rather than against what the files claim. Use it by running the mechanical audit script first, fanning out one agent per disjoint scope concurrently, re-running the script after the fan-out drains, and reconciling the root scope last from the branch reports. Use it when the periodic sweep is due, after a long dormancy, before any decision that depends on the context layer or the registry being literally true, when more than one scope looks untrustworthy at once, or when a previous sweep's findings need verifying rather than re-deriving. It checks what those files and registries claim against what is on disk; it never re-derives what they should say from a project's source — a tree with no context layer, or one to rebuild from source, is `fa-foundation-optimize-directory-tree`'s.
license: MIT
---

# Context Audit

## Scope and when to use

Use this skill to sweep an entire repository's context layer at once and
reconcile every claim in it against inspected reality. This skill is the
fan-out; refreshing one directory is a `fa-foundation-memory-maintenance` job,
and closing one working session is a `fa-foundation-session-close` job.

Use this skill when:

- the scheduled periodic drift sweep is due, or the repository has been dormant
- a decision depends on the registry or the context layer being literally true
- more than one scope looks untrustworthy at the same time
- a previous sweep's findings need verifying rather than re-deriving

Do not use this skill when:

- only one scope is in question — refresh that pair directly instead
- the defect is that one file mixes instruction and state — that is an authoring fix
- the repository has no context pair yet — author the pair before auditing it
- the real question is repository durability — that has its own dedicated pass

## Required inputs

- the scopes to sweep, as explicit `--scope` arguments, with their directory boundaries
- the root `MEMORY.md`, which records the previous pass's findings and its date
- the cascade standard and the finding-code reference this sweep enforces
- the standing agent definitions carrying the briefs, including their git posture
- the consuming repo's own rules for scratch files, registries and commit policy

## Owned outputs

- a mechanical findings run, taken twice: before the judgement pass and after it drains
- one report per scope, each from an agent whose scope was disjoint from every other's
- a reconciled root `MEMORY.md`, pruned of everything this pass resolved, not merely appended to
- an explicit list of claims carried forward unverified, each labelled with the pass that last inspected it

## Decision tree

- If the mechanical script has not run, run it before spending any agent attention.
- If a finding sits in a judgement-required class, open the whole file; never resolve it from the printed line.
- If two candidate scopes overlap, redraw the boundaries until they are disjoint, or merge them into one agent.
- If a branch report contradicts another, resolve it at the parent scope, last — never inside either branch.
- If a fact was not re-inspected this pass, carry it forward with its original date instead of restamping it.

## Core workflow

1. Run the mechanical pass first: `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --scope <dir> --scope <dir> …`, adding `--registry <path>` where the tree carries a central registry file (the registry check is opt-in and silently does nothing without it) and `--instruction-file AGENTS.md` where that, not `CLAUDE.md`, carries the rules. It takes seconds and removes the entire class of findings that need no judgement. `MEMORY-INDEX-STALE` is the cheapest of them: every `MEMORY.md` is derived from its `memory/` topic files, so the fix is `--fix-index`, never an edit to the index.
2. Read the previous pass's record and note its date. This sweep verifies those claims; it does not re-derive them from scratch.
3. Partition the repository into disjoint scopes and fan out one agent per scope, all launched concurrently, under the rules below.
4. While the branches run, resolve the judgement-required findings yourself, with whole files open.
5. Wait for the fan-out to fully drain, then re-run the script with `--fix-index` so every index the branches' topic edits invalidated is regenerated in one place — a mid-flight run reports the sweep's own noise, and a mid-flight `--fix-index` regenerates indexes the branches are still editing.
6. Reconcile the parent scope last, once, from the merged branch reports, pruning as you go.
7. Empty the sweep's scratch and confirm every branch emptied its own; the report goes to whoever asked for it, never into the repository.

## Fan-out rules for a disjoint-scope sweep

- **The script is a locator, not an assessor.** It hands you a file and a line number; what that line *should* say is decided with the whole file open. `POSSIBLE-STATE-IN-INSTRUCTIONS` is the sharp case — a locked rule ("releases are cut from `main`, always") and a status line ("currently on v3, dormant since March") are the same shape to a regex. **A pass that drives that class to zero has almost certainly deleted binding rules.** The count going down is not the goal; the count is not a score at all. Expect to close a healthy sweep with findings deliberately left standing, each with its reason recorded so the next sweep does not re-litigate it.
- **One agent per scope, and never two agents inside the same subtree.** Two agents editing one scope's topic files overwrite each other: last writer wins, the other's findings vanish, and nothing in either report says so. Draw the boundaries first, write them down, and confirm every pair is disjoint before launching. Batch several small or empty scopes into a single agent rather than splitting one busy scope across two.
- **Launch them concurrently.** A fan-out run sequentially costs what doing it yourself costs and returns none of the advantage; the parallelism *is* the reason to fan out at all.
- **Brief the two things the standing definitions deliberately leave open**: state that the job is *verification, not rewrite*, and name the previous pass's date. Without both, agents rewrite files that were already correct and you cannot tell a real change from churn.
- **Never hand agents a git posture that contradicts their own definitions.** If the definitions say auditors commit context edits locally, never push, and never rewrite history, then a blanket "no git write commands" line in the brief splits the sweep — some branches commit, others leave the human owner a dirty tree to untangle by hand.
- **Re-run the script only after the fan-out has drained.** Concurrent agents' own git calls manufacture lock-debris and dirty-tree findings that evaporate the moment they finish. Acting on a mid-flight run means chasing artifacts of the sweep itself.
- **Reconcile the parent scope last, once, from the branch reports.** Writing the root record while branches are still reporting guarantees rewriting it. Prune while reconciling: an audit that only appends is how a `MEMORY.md` becomes unreadable, and pruning is the part that proves the pass verified something rather than restating it.
- **Verify the previous pass's own claims in both directions.** Items recorded as broken that a later pass quietly fixed, and — the nastier direction — items the previous pass asserted it had fixed and had not. Both are stale; only one of them looks stale.
- **Never launder an unverified fact through a fresh timestamp.** If the root record asserts something sourced from a scope this pass did not re-inspect, say so inline: "git state re-inspected; internals carried from the earlier pass". Stamping today's date over yesterday's evidence is the same failure as editing an old file to look current, executed one level up where the date hides it.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| overlapping agent scopes | discard both reports, redraw disjoint boundaries, re-run those branches | sweep owner | pause the sweep before more writes land |
| judgement class cleared to zero | restore from history and re-read each file whole | sweep owner | involve the human owner if rules were lost |
| script findings from the sweep itself | wait for the fan-out to drain, then re-run before believing any of them | sweep owner | none — expected noise |
| branch left scratch or an uncommitted tree | re-run that branch's close-out; do not clean it from the parent | branch agent | human owner if history was rewritten |

## Anti-patterns

- do not trust a status that appears only in prose; inspect the thing it describes
- do not resolve a script finding from the line the script printed
- do not restamp a freshness date for anything you did not personally re-inspect
- do not let a sweep end without pruning; growth-only is how a memory store dies
- do not delete an open question because it is uncomfortable — move it up a scope if it is not yours
- do not describe a directory's file inventory from inside a file you are adding to that directory

## Handoff and downstream impact

- give the human owner the sweep report in the reply, plus the findings deliberately left standing and why
- give scope maintainers the specific claims their memory topics must now carry or drop
- give skill maintainers the list of skills the sweep found stale, for `fa-foundation-create-skill`
- give agent maintainers any brief-versus-definition contradiction the sweep exposed, for `fa-foundation-create-agent`

## Examples

- **Good fit:** a quarterly sweep of a monorepo whose scopes have each been edited by different sessions since the last pass.
- **Good fit:** verifying a six-week-old audit's claims before a decision that depends on the registry being accurate.
- **Not a fit:** one project's `MEMORY.md` is stale after today's work — close that session instead of sweeping the repository.

## Completion checklist

- the script ran before the fan-out and again after it fully drained
- every agent scope was disjoint, and every scope in the repository was covered exactly once
- the judgement-required findings were each read whole, and the ones kept have recorded reasons
- the root record was written once, at the end, and pruned of what this pass resolved
- every carried-forward claim names the pass that actually inspected it
- all scratch is empty and no report was written into the repository

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit finding codes](../../references/context-audit-findings.md)
- [Ownership and conventions](../../references/ownership-and-conventions.md)
