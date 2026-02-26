# SKILL: Gaia Process (Spec-Driven SDLC)

## When to use
Always. This is Gaia’s canonical workflow.

## Inputs
- user request
- current repo state

## Steps
1) **Repo Survey (mandatory)**
   - Delegate to Repo Explorer.
   - If drift/CI failure/skill drift exists: create blocking tasks to fix first.

2) **Determine request class**
   - docs-only / code-only / drift repair / feature
   - decide **use-case change?** (if unsure: yes)

3) **Plan the Task Graph**
   - include docs, foundations, implementation, tests, manual regression, QA review
   - for each task set `required_gates[]` explicitly

4) **Execute via delegation**
   - architect for design + docs
   - developer for implementation
   - tester for tests

5) **Verify + enforce**
   - QA Gatekeeper reviews (veto authority)
   - update skills if conventions changed

6) **Complete**
   - orchestrator writes 1-paragraph completion summary
   - call MCP `mark_done` with proof args

## Outputs
- updated docs and/or code
- updated skills if conventions changed
- passing CI (or blocking tasks)
