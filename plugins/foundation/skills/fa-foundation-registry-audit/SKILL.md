---
name: fa-foundation-registry-audit
description: Provides a two-direction registry audit that reconciles a repository's registry of scopes against the directories that actually exist on disk. Use it by running the context-audit script for the mechanical forward and reverse passes, then cross-checking by hand every other registry that names the same scopes and grepping the whole tree for stale mentions of anything removed. Use it when adding, killing, renaming or moving a scope, when a named project cannot be located, before any decision that allocates people or budget across the registry, and periodically — this drift is silent, continuous, and invisible until something is planned against a directory that is not there. It reconciles a registry that already exists; establishing one for a tree that has none is `fa-foundation-optimize-directory-tree`'s.
license: MIT
---

# Gaia Registry Audit

## Scope and when to use

Use this skill when the repository's registry — the section of the root instruction file that names every scope — has to be trusted rather than assumed. The registry drifts in two directions at once, and both directions are dangerous.

Use this skill when:

- a scope is being added, killed, renamed, or moved between parents
- a named project or scope cannot be located on disk
- an allocation decision (people, budget, priority) is about to read the registry as fact
- periodically, on a schedule — nothing announces this drift

Do not use this skill when:

- the question is whether a scope is *healthy* or *active* rather than whether it exists
- the registry is fine and the defect is inside one scope's own context pair
- a new scope is being created and has simply not been registered yet — register it

## Required inputs

- the registry section of the root instruction file, read whole
- every other document in the repository that also enumerates scopes
- the real directory listing under each parent that holds scopes
- the kill records or retirement notes for anything the registry marks as gone

## Owned outputs

- a list of phantoms — registered, not on disk — each with a note on whether a kill record exists
- a list of orphans — on disk, registered nowhere — each registered or explicitly rejected
- a reconciliation across every registry that names the same scopes
- a completed stale-mention sweep for every identifier that was removed or renamed

## Decision tree

- If the script has not been run yet, run it first; steps it automates are not worth agent attention.
- If a registry entry has no directory and no kill record, surface it as a finding — do not quietly delete the line.
- If a directory exists that no registry names, register it, or record why it should not exist.
- If two registries disagree, the disk decides existence; the kill records decide intent.
- If an identifier was removed, the audit is not finished until the stale-mention sweep is finished.

## Core workflow

1. Run `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --scope <dir> --registry <path to the registry file> --no-git` and read `REGISTRY-PHANTOM`, `REGISTRY-UNLISTED`, `REGISTRY-CONTRADICTION` (registered as retired, yet present on disk) and `REGISTRY-DUPLICATE-ID` (the same id under two owners — ids are unique across a tree, and a memory topic's `name` slug is derived from the directory name, so the two stores collide as well). **`--registry` is not optional here:** the check is opt-in, because a repository with no central registry is not in violation of anything — omit the flag and the script prints a clean run over a registry it never opened.
2. Do the forward pass by hand only to interpret a finding: for every registry entry, confirm its directory exists.
3. Do the reverse pass the same way: list every directory under every parent that holds scopes, and flag anything unregistered.
4. Cross-check every *other* registry that names the same scopes — the script reads only the one it anchors on, so this step is manual by nature.
5. Sweep the whole tree for stale mentions of any identifier that was removed or renamed, and open every hit.
6. Report before fixing. Register the orphans; for phantoms, record the absence of a kill record as its own finding.
7. Record what you reconciled, and what you deliberately left alone, in the relevant `MEMORY.md`.

## Forward, reverse, and the stale-mention sweep

**The forward pass finds phantoms: registered, not on disk.** A phantom is dangerous because it is *authoritative and unfalsifiable from the outside* — it reads exactly like a real scope, and nothing on disk contradicts it. Agents plan against it, budget for it, assign owners to it, and cite it in documents, until someone finally tries to open the directory.

**The reverse pass finds orphans: on disk, registered nowhere.** An orphan is dangerous in the opposite direction — it is invisible to every process the registry drives. It is never reviewed, never audited, never assigned, and it is the thing that gets deleted by accident because no document claims it. A pass that finds only one of these two classes is half an audit.

**Cross-check every registry that names the same things.** Most repositories grow more than one: the root registry, a placement or rationale document, and the roster inside a parent scope's own `MEMORY.md`. They are written by different processes at different times, so they diverge. Mechanical checking covers only the registry it is anchored on; reconciling the rest is the genuinely manual step of this skill, and skipping it is the most common way an audit reports clean while the repository is not.

**Then sweep for stale mentions — and read the caveat, because the caveat is the step.** A removed identifier is still named in neighbouring instruction files, in reference documents, and in prose that no registry check ever reads. Grep the whole tree for the identifier before declaring it gone. **The grep finds the files; it does not tell you what each one should now say.** A mention inside a kill record is *correct* and must stay — that is the record doing its job. The same identifier in a live roster is the defect. And the fourth mention two paragraphs below the one you just fixed is exactly why this step exists: partial sweeps are the characteristic failure here, and they recur.

**Existence is not aliveness.** A directory proves the registry's claim, and nothing more. Whether the work inside it is alive, dormant, or abandoned is a question for that scope's `MEMORY.md`, and answering it from a directory listing is how a dead project keeps drawing attention for another year.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| phantom with no kill record | keep the line, mark it unexplained, report it | auditor | ask the human owner what happened |
| orphan directory | register it in every registry, or record why it must not exist | auditor | escalate if ownership is genuinely unknown |
| registries disagree | reconcile against disk for existence, kill records for intent | auditor | escalate a conflict the records cannot settle |
| partial stale-mention sweep | re-grep the identifier and open every hit to the end of the file | auditor | re-run the sweep before closing the audit |

## Anti-patterns

- do not delete a phantom entry without recording that it had no kill record
- do not treat a directory's existence as proof the work inside it is active
- do not rewrite placement rationale for a scope that was never placed — carry it forward and flag the gap
- do not stop the stale-mention sweep at the first hit in a file
- do not drive the finding count to zero as if it were a score

## Handoff and downstream impact

- give any allocation or prioritization work a registry it can read as fact
- give scope creation and kill procedures the reconciled list they must update
- give memory maintenance the aliveness questions this audit deliberately refused to answer
- give the human owner every phantom that vanished without a record

## Examples

- **Good fit:** a scope was renamed last month; sweep the tree for the old identifier and fix every live mention while leaving the kill record intact.
- **Good fit:** before an allocation review, reconcile the root registry, the placement document, and each parent roster against the directories on disk.
- **Not a fit:** deciding whether a registered, existing project is still worth continuing — that is an aliveness judgement, not a registry question.

## Completion checklist

- both passes ran: phantoms and orphans are each listed, not just one class
- every registry naming the same scopes has been read and reconciled by hand
- every removed identifier has been grepped tree-wide and every hit opened
- kill-record mentions were preserved; live-roster mentions were fixed
- phantoms without kill records are reported, not silently deleted
- what was reconciled, and what was deliberately left alone, is recorded in `MEMORY.md`

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit finding codes](../../references/context-audit-findings.md)
