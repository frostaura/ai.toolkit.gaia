name: gaia-skills
description: Provides Gaia skill-definition maintenance guidance with clear naming, descriptions, ownership boundaries, references, and reuse decisions. Use it by following the skills-specification reference (naming, frontmatter shape, scope sections, references) when adding or revising a SKILL.md, and by mirroring changes across both /skills/ and /.github/skills/ trees. Use it when adding, revising, auditing, or rationalizing skills, improving invocation quality, or consolidating drifting procedural guidance.
license: MIT
---

# Gaia Skills

## Scope and when to use

Use this skill to keep Gaia's procedural layer reusable, concise, and aligned to
the contract and architecture.

Use this skill when:

- adding or revising a skill
- auditing duplicated or drifting skill guidance
- improving skill descriptions, section templates, or references
- deciding whether repeated work deserves a new skill or a stronger existing one

Do not use this skill when:

- the change is really a role-definition issue
- the missing step is still an architecture or contract decision
- the procedural guidance belongs in a reference file rather than `SKILL.md`

## Required inputs

- the current skill roster and shared references
- the procedural gap or maintenance problem being addressed
- naming, metadata, and specification constraints
- any repeated patterns, overlaps, or references worth consolidating

## Owned outputs

- valid skill definitions with strong descriptions and clear triggers
- procedural guidance that tells roles how to do recurring work
- trimmed duplication between shared references and local skills
- documented reasoning for adding, merging, or rejecting a skill

## Decision tree

- If the change is global workflow policy, update `AGENTS.md` or a shared reference.
- If a reference can hold detailed material better than the main skill body, move it out of `SKILL.md`.
- If an existing skill can absorb the new behavior cleanly, revise it instead of adding a new skill.
- If a new skill is justified, define its scope, outputs, and anti-patterns before finalizing the metadata.

## Core workflow

1. Read the skills specification and the current Gaia skill set.
2. Decide whether the change belongs in a skill, a reference, or the contract layer.
3. Write or revise the skill with clear use-when, decision-tree, procedure, and recovery guidance.
4. Keep descriptions specific enough to help discovery while staying within spec limits.
5. Keep references focused and use them to avoid bloating the main skill body.

## New-skill justification checklist

- what repeated pattern or maintenance pain requires a reusable procedure
- why an existing skill cannot absorb the behavior cleanly
- what artifact or outcome the skill should own
- what detail belongs in the main skill body versus a reference file

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| duplicated guidance | centralize it in shared references or the contract | maintainer | re-audit neighboring skills |
| weak description | rewrite it for invocation quality within spec limits | maintainer | compare with neighboring skills |
| procedural gap | add or expand the right skill | maintainer | reject if the pattern is too narrow to reuse |
| skill-contract mismatch | update the skill after the contract is current | maintainer | involve architecture if the workflow changed |

## Anti-patterns

- do not move global workflow rules into every skill body
- do not create a new skill for a one-off edge case
- do not leave references so deep that the main skill becomes unusable
- do not let frontmatter descriptions stay generic or vague

## Handoff and downstream impact

- tell maintainers whether the change belongs in the contract, a skill, or a shared reference
- tell agent maintainers when a role file must change to invoke the revised skill correctly
- tell architecture when skill maintenance implies a workflow-model change
- tell engineering when the skill rewrite needs direct file edits in the repo

## Examples

- **Good fit:** rewrite Gaia's seven skills into a common procedural template after the contract and role layers are clarified.
- **Good fit:** decide whether new line-count and description standards belong in each skill or in a shared rule.
- **Not a fit:** decide who owns a delivery branch; that is role and planning work, not skill maintenance.

## Completion checklist

- the skill has a clear scope, procedure, and recovery path
- descriptions stay within spec and still help discovery
- duplicated guidance has been centralized where appropriate
- references support the skill without making the main file too thin

## References

- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
- [Skills specification](references/skills-specification.md)
