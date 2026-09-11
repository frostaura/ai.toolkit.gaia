---
name: fa-engineering-deploy-chain
description: Provides deploy-chain activation guidance for turning a repository whose CI workflow already carries build-and-deploy jobs into one that actually publishes and redeploys. Use it by reading the secret names out of the workflow's own env block, creating the deployment target before setting its identifier, matching the image namespace the target pulls against the one the pipeline pushes, and proving activation on a real push to the gating branch. Use it when a build job fails on registry credentials, when a deploy job fails on a missing or empty secret, when a new repository has inherited a sibling's workflow, when every job is green yet nothing redeploys, or before calling any repository deploy-ready. It activates a pipeline that already exists and never authors the compose stack or the language baseline underneath it — those are `fa-engineering-containerization` and the matching `fa-engineering-stack-*` skill.
license: MIT
---

# Gaia Deploy Chain

## Scope and when to use

Use this skill to activate a deploy chain that is already authored but not yet
live: the workflow exists, the jobs run, and nothing reaches the running
environment.

Use this skill when:

- a build or push job fails on registry credentials
- a deploy job fails because a required secret is missing or empty
- a new repository was just created and its CI workflow was copied from a sibling
- every job reports success yet the running environment never changes
- you are about to write the words "deploy-ready" about a repository

Do not use this skill when:

- the workflow does not yet contain build or deploy jobs — that is authoring work
- the open question is whether this chain is the right one; see `fa-engineering-default-tech-stack`
- the repository is not yet safe to push at all; run `fa-foundation-repo-durability` first

## Required inputs

- the repository's own CI workflow file, read directly — the deploy step's `env:` block and the build steps' `tags:` lines
- the exact secret names that workflow references, transcribed rather than assumed
- access to the container registry and the deployment orchestrator, with authority to issue fresh credentials
- the repository's memory store, whose deployment topic records which secrets are set and whether the deployment target exists
- the branch the deploy job is gated on

## Owned outputs

- one complete set of repository secrets, set on this repository
- a deployment target that exists, with an identifier that was read rather than guessed
- an image namespace that matches on both sides of the chain
- one observed end-to-end run on the gating branch, with the redeploy step — not the build step — as the evidence
- a memory topic recording activation state, never secret values, with the index regenerated from it

## Decision tree

- If the workflow's secret names are unread, stop and read them; assumed names produce silent misconfiguration.
- If the deployment target does not exist, create it before setting any identifier secret.
- If an identifier has no real value yet, leave its secret unset rather than placeholder — unset fails loudly, placeholder fails quietly.
- If every job is green and nothing redeployed, suspect the namespace before the credentials.
- If the deploy job is gated on the default branch, a pull-request or feature-branch re-run proves nothing; arrange a real push.
- If the repository is dirty, diverged, or missing an upstream, activation waits.

## Core workflow

1. Run `fa-foundation-repo-durability` first — activation ends in a push, and a repository that is not safe to push is not ready to activate.
2. Read the workflow end to end and transcribe the exact secret names from the deploy step's `env:` block and the exact image references from the build steps' `tags:` lines.
3. Create the deployment target in the orchestrator, pointing it at the exact image names the build steps will push, then read back its endpoint and stack identifiers.
4. Set every secret the workflow names, in one pass, from freshly issued values — a full set on this repository, whatever a sibling already has.
5. Push to the branch the deploy job is gated on and watch the run through to the deploy step.
6. Confirm the redeploy happened by inspecting the running environment: the served image digest or build timestamp must be newer than the commit you just pushed.
7. Record activation state in the repository's memory store — which secrets are set, whether the target exists, and the date of the observed redeploy. **Write it into that scope's `memory/` topic, then regenerate the index**; `MEMORY.md` is derived from the topic files and a hand-added row is discarded by the next regeneration.

## The traps, in the order they bite

**1. Secrets do not inherit; every repository carries its own full set.** There is no ambient account- or organisation-level secret store unless someone deliberately created one and scoped it to this repository. A sibling that "deployed fine yesterday" is evidence about that sibling and nothing else. Treat every new repository as starting from zero.

**2. Secrets are write-only, so budget for re-issuing rather than copying.** You cannot read a value back off a sibling repository — the platform shows you the name and never the value. Each value must come from wherever it was originally issued: the registry for the push token, the orchestrator for its API token and identifiers. Price that re-issuing into the work before you start, not once you are half-configured.

**3. The deployment target must exist before its identifier has a value.** This is the ordering that catches people. Most secrets get set, the last one needs a stack identifier nobody has created yet, and it is left blank or invented — and the repository now *looks* configured while the deploy job fails. Create the target, read its real identifier, then set the secret.

**4. Namespace mismatch is the worst trap, because it is silent.** The build steps tag the image under one namespace; the deployment target pulls whatever its own definition names. If those differ by a single character, every job in the pipeline reports success and the running environment never changes. CI cannot detect this — CI's responsibility ends at the push. Compare the two strings by eye, deliberately, once.

**5. Presence of a deploy job is not the ability to run it.** The job is usually gated on a push to the default branch. Re-running the workflow from a pull request or a feature branch never exercises it, so a repository can carry a deploy job that has never once executed.

**Worked example — the shape of a typical chain.** A container-registry-to-stack-orchestrator chain needs six values: registry username and registry access token for build and push; orchestrator URL, orchestrator API token, orchestrator endpoint identifier and orchestrator stack identifier for deploy. Six is this example's number, not a law, and the names differ per repository and per tool. **The binding rule: read the names out of the workflow's own `env:` block, never assume them, and never carry a name across from another repository.**

## Failure recovery

| Failure mode                     | Recovery                                                           | Owner    | Escalation                                    |
| -------------------------------- | ------------------------------------------------------------------ | -------- | --------------------------------------------- |
| build fails on registry credentials | issue a fresh registry token, set both registry secrets here        | engineer | involve the registry account owner            |
| deploy fails on an empty identifier | create the target, read its identifier, then set the secret         | engineer | involve the orchestrator owner                |
| all jobs green, nothing redeployed | compare pushed namespace against the target's pull namespace        | engineer | re-author the target definition               |
| deploy job never runs            | check the branch gate and push to the gating branch                 | engineer | involve planning if the branch model blocks it |
| a secret value is unavailable    | re-issue at the source; never copy from a sibling                   | engineer | escalate to the human owner holding the account |

## Anti-patterns

- do not call a repository deploy-ready because its workflow contains a deploy job
- do not set an identifier secret to a guessed or placeholder value to unblock a pipeline
- do not paste a secret value into a file, a commit, a log, an issue, or an agent transcript
- do not hand-write a row into `MEMORY.md`; edit the `memory/` topic and regenerate the index, or the record vanishes on the next run
- do not treat green build jobs as evidence that anything deployed
- do not deploy an image built before the last code fix; check the image is newer than `HEAD`
- do not record secret values in the memory store — record only which names are set

## Handoff and downstream impact

- tell testing which environment is now live and which commit it is serving
- tell release that deploy-readiness is an observed fact with a date, not an inference from the workflow file
- tell the next agent, through the memory topic, which secrets are set and whether the target exists
- tell architecture when activation exposed a chain the repository's documented deploy story does not match

## Examples

- **Good fit:** a newly created repository whose workflow was copied from a sibling and whose first push fails on registry credentials.
- **Good fit:** a pipeline green for a week while the running environment still serves last month's build.
- **Not a fit:** deciding whether this repository should deploy through a stack orchestrator at all; that is a stack decision.

## Completion checklist

- every secret the workflow names is set on this repository, from freshly issued values
- the deployment target exists and its identifier was read, not guessed
- the pushed image namespace and the pulled image namespace are the same string
- one real push to the gating branch was watched through the deploy step
- the running environment serves an image newer than the commit that triggered it
- a `memory/` topic records activation state and the index was regenerated from it, and no secret value appears anywhere in the repository

## References

- [Gaia delivery policy](../../references/delivery-policy.md)
- [Gaia ownership and conventions](../../references/ownership-and-conventions.md)
