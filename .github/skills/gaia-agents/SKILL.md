---
name: gaia-agents
description: Provides Gaia custom-agent definition guidance with clear role boundaries, strong descriptions, valid tool scopes, delegation rules, and overlap control. Use it by following the agents-specification reference (frontmatter, role contract, tool allow-list, delegation rules) when adding or revising an agent file, and by mirroring changes across both /agents/ and /.github/agents/ trees. Use it when adding, revising, auditing, or rationalizing agent files, role ownership, naming, or contract alignment.
license: MIT
---

# Gaia Agents

## Scope and when to use

Use this skill to keep Gaia's custom agents clear, distinct, and aligned with
the global contract.

Use this skill when:

- adding or revising a custom agent definition
- auditing role overlap or ambiguous tool scopes
- rewriting agent descriptions or instructions for better invocation quality
- aligning agent files to a new shared contract or architecture update

Do not use this skill when:

- the missing step is still an architecture decision
- an existing role can be extended cleanly without creating a new one
- the change belongs in reusable procedural guidance instead of a role definition

## Required inputs

- the current agent roster and `AGENTS.md`
- the purpose, scope, and intended users of the proposed role change
- required tools, MCP servers, and any access constraints
- any overlapping definitions or maintenance pain the change is meant to solve

## Owned outputs

- valid agent definitions with clear names and scopes
- decision-grade descriptions that explain when to invoke or avoid each role
- explicit role boundaries, delegates, and anti-patterns
- overlap decisions that explain why a role was added, merged, or rejected

## Decision tree

- If the gap is global policy, update `AGENTS.md` instead of a role file.
- If an existing role can absorb the behavior cleanly, revise it rather than create a new role.
- If a truly new role is needed, define its mission, adjacent roles, tools, and failure boundaries explicitly.
- If tool access is unclear, make it explicit before considering the definition complete.

## Core workflow

1. Read the agent specification and current roster before editing anything.
2. Identify whether the change is global contract work or local role work.
3. Test the proposal against neighboring roles to avoid overlap.
4. Write or revise the agent with clear mission, use-when rules, delegates, and anti-patterns.
5. Confirm that the role improves routing instead of adding organizational noise.

## New-agent justification checklist

- what gap exists that current agents do not already cover cleanly
- why expanding an existing role would be worse than creating a new one
- what explicit invocation signal should trigger the new role
- what tool scope is truly necessary and how overlap will be prevented

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| overlap with existing role | merge or narrow the proposal | maintainer | reject new role if still redundant |
| unclear tool scope | define least-privilege access | maintainer | block adoption until explicit |
| weak description | rewrite for invocation quality | maintainer | compare against neighboring roles |
| contract mismatch | update local role or contract as appropriate | maintainer | involve architecture if the operating model changed |

## Anti-patterns

- do not create a new agent because one current role feels temporarily overloaded
- do not leave delegation or tool access implied
- do not store global policy inside every role file
- do not let role descriptions stay too short to drive good invocation choices

## Handoff and downstream impact

- tell maintainers whether the change belongs in `AGENTS.md` or only in local role files
- tell engineering when agent maintenance requires direct file edits after design approval
- tell architecture when a role change implies a workflow-model change
- tell skill maintainers when procedural guidance must be updated to match a role rewrite

## Examples

- **Good fit:** rewrite all six Gaia agents to a shared execution template after the contract layer changes.
- **Good fit:** decide whether a proposed analyzer role is actually needed or should be folded into intake and architecture.
- **Not a fit:** define planning procedures or architecture artifacts; those belong to skills and docs.

## Completion checklist

- every changed role has a clear mission and anti-mission
- tool scopes and delegation rules are explicit
- role overlap has been reduced rather than increased
- local role files align with the global contract

## References

- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
- [Agent specification](references/agents-specification.md)
