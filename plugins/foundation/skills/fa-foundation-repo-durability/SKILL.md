---
name: fa-foundation-repo-durability
description: Provides the six-probe git inspection that establishes whether a repository is genuinely durable and genuinely safe to push, by running commands rather than trusting what a context file claims. Use it by running each probe in order, classifying the upstream topology in words, reading what the unpushed range deletes, and recording an exact recovery sequence in the scope's MEMORY.md. Use it before writing or acting on any sentence containing "push", "durable", "backed up", "deploy-ready" or "committed", before handing the human owner a push runbook, before calling anything release-ready, after any pass that committed broadly across scopes, and at the start of any audit branch that owns repositories. It inspects and reports only — it never commits, pushes or rewrites history, and remediation stays with the human owner.
license: MIT
---

# Gaia Repository Durability

## Scope and when to use

Use this skill to find out, by inspection, which repositories a push runbook is safe for. A generic runbook applied to every repository is not safe, because repositories differ in exactly the way that decides whether a push saves work or destroys it.

Use this skill when:

- any sentence you are about to write or act on claims work is pushed, backed up, durable, committed or deploy-ready
- a `MEMORY.md` asserts a git state that nobody re-verified this session
- a pass has just committed broadly across several scopes
- an audit branch owns one or more repositories, or the context-audit script reported a `GIT-` finding

Do not use this skill when:

- the question is what to build, not whether what exists survives a disk failure
- the repository has no remote *and* the human owner has already recorded that as intended
- you are being asked to perform the push rather than to establish whether one is safe

## Required inputs

- read access to the repository working tree and its `.git` directory
- the branch under examination and the scope that owns it
- the scope's `MEMORY.md`, read as a set of hypotheses to test, never as facts to repeat
- any recorded decision about the remote's visibility and what it is allowed to carry

## Owned outputs

- a per-repository verdict: durable / undurable, safe to push / not safe to push
- the upstream topology stated in words, not integers
- for every failing check, the exact recovery sequence, written into the scope's `MEMORY.md`
- an escalation to the human owner wherever the next command is theirs to run

## Decision tree

- If a probe contradicts a context file, the probe wins and the context file is rewritten.
- If there is no remote, or a remote that was never pushed to, stop and escalate — that is the highest-severity finding available.
- If the branch is diverged, no push proposal leaves this skill at all; gather what the remote holds and hand the merge decision up.
- If the unpushed range deletes tracked files, treat the push as unsafe until every deleted path is explained.
- If every probe is clean, say so in topology terms and name the condition that would invalidate it.

## Core workflow

1. Run the mechanical pass first — `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py` covers all six probes across every scope at once and is faster than doing it by hand.
2. Run the six probes below by hand for a single repository or to interpret a finding the script raised, classifying each result into one of the two failure classes, or clearing it.
3. For any failure, derive the recovery *sequence* — the ordered commands that fix it — before writing anything down.
4. Record the topology and the sequence in the scope's `MEMORY.md`, never in the instruction file; this is all state.
5. Escalate to the human owner anything whose next command is a push, a merge, a rebase or a restore.
6. Re-run the script after any commit you made, because your own commits changed the answers.

## The six durability probes

Each probe is a command, the question it answers, and what the bad answer looks like.

**1. Lock debris — `ls -la .git/*.lock`, and check for a live git process.** Answers: can this repository be committed to at all? Bad answer: a 0-byte `index.lock`, `HEAD.lock` or `packed-refs.lock` with no git process running. Stale locks block every commit *silently*, so an agent "finishes" a session leaving everything uncommitted and reports success. Delete a 0-byte lock only when no git process is live. **A lock with a live git process is not debris** — another process is mid-write, and removing it corrupts that write. Wait instead.

**2. Working tree — `git status --porcelain`.** Answers: what exists only on this disk? Bad answer: any output at all — uncommitted means undurable. Separate context-file edits (commit them at the owning scope) from in-progress product code belonging to another session (leave it, and record that it is there). Never `reset`, `clean`, `stash` or `restore` to clear this; untracked work has no reflog and is gone permanently.

**3. Upstream topology — `git rev-parse --abbrev-ref --symbolic-full-name @{u}`, then `git rev-list --left-right --count @{u}...HEAD`.** Answers: does a durable copy exist elsewhere, and in what relation? Bad answers, worst first: *no remote* — the work exists on one disk; *remote but no upstream* — configured and never pushed; *ahead and behind, both counts non-zero* — **diverged**.

**4. What the unpushed commits delete — `git diff --name-status @{u}..HEAD`, and count the `D` lines.** Answers: what would a push *remove* from the remote? Bad answer: a catch-all commit ("commit working-tree state", "wip") whose range deletes tracked files nobody decided to delete. **Absence on disk is not evidence of deletion.** On any cloud-synced, network-mounted or virtualized filesystem — sync placeholders, SMB/NFS mounts, sparse checkouts, dev containers — files are routinely evicted or left unmaterialized locally; `git add -A` then records that eviction as a deletion, and the push makes it true on the remote. Read every deleted path before endorsing a push.

**5. Reflog, when a branch looks wrong — `git reflog show <branch>`.** Answers: how did the branch pointer get here? Bad answer: `branch: Reset to <sha>` or `reset: moving to` in recent history — a label was moved instead of merged, which is the usual cause of an unexplained divergence.

**5b. The remote actually exists — `git ls-remote --exit-code <remote> HEAD`.** Answers: is there anywhere to push at all? A configured remote is a local string that nothing validates, so a tracking ref, an ahead-count and a clean `git remote -v` are all consistent with a remote that was deleted, renamed or never created. Bad answer: `repository not found` — the tracking ref is a dead local ref and the work is as undurable as if there were no remote. Classify before concluding: a credential refusal is a fact about *this host*, not about the repository, and an SSH GitHub remote that refuses this machine's key should be probed again over its HTTPS twin before anything is recorded. `context-audit.py` runs this as `GIT-REMOTE-MISSING` / `GIT-REMOTE-UNREACHABLE`, and `--no-remote-probe` skips it when offline.

**6. Remote visibility — `git remote -v`, plus the scope's own record of public or private.** Answers: who can read what a push publishes? Bad answer: a remote whose visibility nobody recorded, or a public remote about to receive work that was meant to stay unreleased.

### Why `--force` is always wrong on a diverged branch

Diverged means the remote holds commits your clone does not. A plain push is rejected, which is the safety mechanism working. `--force` replaces the remote tip with yours and makes the remote-only commits unreachable — for CI, for every clone that already pulled, and for anything installed from that remote, that work is gone. The correct move is always to fetch, establish what the remote already contains, and merge or rebase onto it. The decision belongs to the human owner, and `--force` is never among the options you offer.

### The two failure classes

- **Work that exists on exactly one disk.** No remote, or a remote never pushed to. One disk failure is the whole body of work.
- **A push that destroys more than it saves.** Two shapes seen in practice: a branch label moved rather than merged, leaving local far behind a remote that already carried the work; and a catch-all commit that recorded a sync eviction as a mass deletion and sat unpushed, one command away from stripping the remote.

### Record the sequence, not the warning

In the scope's `MEMORY.md` — never the instruction file — record the branch, its upstream, the topology in words, and for every failing check **the exact ordered commands that fix it** (`git fetch origin` → `git checkout origin/<branch> -- <paths>` → inspect → commit). A warning to be careful is ignored by the next agent under time pressure. A sequence gets run.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| stale lock blocks every commit | confirm no git process is live, delete the 0-byte lock, re-run probe 2 | auditor | if a git process is live, wait — never delete a lock under a running process |
| no remote, or never pushed | record it as the highest-severity finding and write the remote-add and first-push sequence | auditor | human owner; creating or naming a remote is their call |
| diverged branch | fetch, establish what the remote already holds, write the merge sequence | auditor | human owner decides; `--force` is never proposed |
| unpushed range deletes tracked files | read the deleted paths, test whether they are merely evicted locally, record the restore command | auditor | human owner before any push |
| context file asserts an unverified git claim | re-run the command behind it and rewrite the claim as topology | auditor | re-audit the scope if the claim was load-bearing |

## Anti-patterns

- do not run `git push`, `--force`, `reset --hard`, `rebase`, `clean`, `stash` or `restore` — you gather evidence, the human owner decides
- do not restore deleted code blind; identify the restore command, record it, leave the call
- do not repeat a git claim from a context file without re-running the command behind it
- do not record ahead/behind integers as the finding; they move, and the topology does not
- do not hand one push runbook to every repository — a runbook is only as safe as its most divergent repository

## Handoff and downstream impact

- give the human owner the per-repository verdict and the ordered commands, not a caution
- give the scope's memory maintainer the topology sentence to store, and the claim it replaces
- give the audit that spawned you the `GIT-` findings you cleared and the ones you deliberately kept
- give release work an explicit statement that deploy-readiness was checked against the remote, not the disk

## Examples

- **Good fit:** a pass committed context edits across several scopes and someone is about to write "everything is backed up".
- **Good fit:** the audit reports `GIT-DESTRUCTIVE-UNPUSHED` on a repository that other projects install from.
- **Not a fit:** deciding whether a branch's feature work is architecturally correct; that is design review, not durability.

## Completion checklist

- all six probes ran on every repository in scope, this session
- the upstream topology is recorded in words, and any integer quoted carries its measurement date
- every deleted path in an unpushed range is explained before a push is called safe
- each failing check has an ordered recovery sequence in the scope's `MEMORY.md`
- nothing that writes to git history was run, and no push was proposed for a diverged branch

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit finding codes](../../references/context-audit-findings.md)
