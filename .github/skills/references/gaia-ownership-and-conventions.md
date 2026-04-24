# Gaia Ownership and Conventions

Use this reference when deciding which skill should own a change, which repository artifact should be updated, or which naming convention to apply.

## Skill ownership

| Skill | Primary ownership |
|---|---|
| `gaia-process` | Complexity classification, execution path, orchestration, handoffs, and closure expectations |
| `gaia-architecture` | `docs/architecture`, architecture drift resolution, and README sync after architecture changes |
| `gaia-planning` | The repository's `gaia_plan.md`, branching plan structure, dependencies, QA checkpoints, release gates, and proof expectations |
| `gaia-engineering` | Planned implementation work, rapid iteration, implementation stabilization, and engineering standards during delivery |
| `gaia-ui-engineering` | React UI implementation, design-system enforcement, token-only styling, responsive layout discipline, and cleanup of legacy arbitrary values |
| `gaia-testing` | Formal test strategy, test artifacts, regression coverage, and testing evidence |
| `gaia-default-tech-stack` | Default frontend and backend stack selection, design-system standardization, and MCP-ready API baseline when the request or repo does not specify another stack |
| `gaia-skills` | Skill naming, skill structure, `SKILL.md` maintenance, and repository skill conventions |
| `gaia-agents` | Agent naming, agent definitions, and repository custom agent conventions |

## Repository artifacts

- `docs/architecture/**` -> `gaia-architecture`
- `README.md` when architecture messaging changes -> `gaia-architecture`
- `gaia_plan.md` -> `gaia-planning`
- Repository code and implementation changes -> `gaia-engineering`
- React UI implementation and design-system conformance -> `gaia-ui-engineering`
- Formal test files and testing evidence -> `gaia-testing`
- `skills/**`, `.github/skills/**`, or `.claude/skills/**` -> `gaia-skills`
- `agents/**`, `.github/agents/**`, or `.claude/agents/**` -> `gaia-agents`

## Naming conventions

- Skill names: `<lowercase-hyphenated-repo-name>-<lowercase-hyphenated-skill-name>`
- Agent names: `<lowercase-hyphenated-repo-name>-<lowercase-hyphenated-agent-name>`

These are Gaia repository conventions layered on top of any lower-level format constraints required by the underlying skill or agent specification.
