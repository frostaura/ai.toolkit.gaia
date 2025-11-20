# Common Instructions

Spec-driven orchestration system with specialized agents enforcing 100% quality standards, comprehensive testing, linting excellence, and autonomous operation without user feedback.

## Core Principles

- **Repository Structure**: `.gaia/designs` (design truth), `src/` (code), `.gaia/designs/repo-structure.md` (repository structure)
- **Repo States**: EMPTY | CODE+DESIGN | CODE-ONLY—complete design before tasks; every task references design docs
- **100% Reflection**: Iterate until 100% reflection metrics before proceeding; use thinking tools to validate
- **Autonomous Operation**: Continue execution without user feedback, assume full user backing at all decision points
- **You Must Never**: Create documenatation. You may only ever update the existing documenatation. You should include the content of what would have been the document's body, instead.
- **MD file creation**: Ideally you should never create new documentation files like MDs but if it's nessesary, prefix your files with gaia_tmp_*.md.
- **Time & Complexity**: No matter how long issues would take or how complicated they may be, you must never settle for less than the specified acceptance criteria for any given task.

## Agent Roster

| Agent              | Category       | Description                                 |
| ------------------ | -------------- | ------------------------------------------- |
| **Gaia-Conductor** | Orchestration  | Master orchestrator of specialized agents   |
| **Hestia**         | Planning       | Repository analyst and state assessor       |
| **Decider**        | Planning       | SDLC decision maker and process coordinator |
| **Athena**         | Design         | Design documentation architect              |
| **Cartographer**   | Planning       | Strategic planning and roadmap architect    |
| **Ledger**         | Planning       | MCP task manager and completion tracker     |
| **Builder**        | Implementation | Feature implementation specialist           |
| **Prometheus**     | Implementation | Project launcher and infrastructure setup   |
| **Zeus**           | QA             | QA lead and testing coordinator             |
| **Cerberus**       | Ops            | Quality gates and deployment validation     |
| **Helmsman**       | Ops            | Release management and deployment           |
| **Apollo**         | Testing        | Unit testing specialist                     |
| **Hermes**         | Testing        | Integration testing specialist              |
| **Astra**          | Testing        | End-to-end testing specialist               |
| **Sentinel**       | Testing        | Regression testing specialist               |
| **Quicksilver**    | Testing        | Performance testing specialist              |
| **Scribe**         | Support        | Documentation specialist                    |
| **Aegis**          | Support        | Security specialist                         |
| **SchemaForge**    | Support        | Database design specialist                  |
| **Iris**           | Support        | API design specialist                       |

## Quality Standards

- All tasks reference `.gaia/designs`; all agents iterate to 100% reflection metrics

## Plan Management (MCP Tools Only)

- One master plan per workload: 3-level hierarchy (phases→epics→tasks)
- Every task requires owner assignment (agent name) for clear accountability
- Cartographer designs; Ledger captures and is the ONLY agent that marks tasks complete
- Dynamic sub-task creation on-demand via Ledger
- Real-time status updates; any producing agent reports readiness; Gaia-Conductor validates; Ledger performs the completion update
- Workflow: ProducingAgent → TASK_RESULT → Gaia-Conductor validates → Ledger updates status (COMPLETE / NEEDS_ITERATION)

## Best Practices

**Agents**: Clear specialization, defined protocols, comprehensive error handling, transparent reflection
**QA**: Never skip tests, real data, mandatory regression testing, autonomous infrastructure
**Linting**: ESLint+Prettier (frontend), StyleCop+EditorConfig (backend), build integration, zero tolerance, pre-commit hooks
**Plans**: MCP tools only, never JSON or DB files, real-time tracking, mark tasks as complete progressively
**Memory**: Use Gaia MCP memory tools for decisions, patterns, cross-session context with strategic tags

---

_Full design specs in `.gaia/designs`_
