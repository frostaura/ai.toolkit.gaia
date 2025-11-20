# Common Instructions

Spec-driven orchestration system with specialized agents enforcing 100% quality standards, comprehensive testing, linting excellence, and autonomous operation without user feedback.

## Core Principles

- **Repository Structure**: `.gaia/designs` (design truth), `src/` (code), `.gaia/designs/repo-structure.md` (repository structure)
- **Repo States**: EMPTY | CODE+DESIGN | CODE-ONLY—complete design before tasks; every task references design docs
- **Spec-Driven Approach**: ANALYZE → DESIGN → PLAN → IMPLEMENT (never skip steps)
- **Design Templates**: Use EXISTING `.gaia/designs/*.md` templates - NEVER create new design files, only update existing templates with project content
- **100% Reflection**: Iterate until 100% reflection metrics before proceeding; use thinking tools to validate
- **Autonomous Operation**: Continue execution without user feedback, assume full user backing at all decision points
- **Documentation Rules**:
  - ✅ UPDATE existing `.gaia/designs/*.md` template files with project-specific content
  - ✅ REPLACE template placeholders with actual specifications
  - ✅ KEEP template guidance sections for reference
  - ❌ NEVER create new design .md files
  - ❌ Only create temporary files if absolutely necessary: prefix with `gaia_tmp_*.md`
- **Time & Complexity**: No matter how long issues would take or how complicated they may be, you must never settle for less than the specified acceptance criteria for any given task.

## Spec-Driven Workflow (Mandatory)

**Phase 1: ANALYZE** (Hestia)
- Comprehensively analyze entire repository (structure, tech stack, architecture, health)
- Classify repository state (EMPTY | CODE+DESIGN | CODE-ONLY)
- Identify gaps, technical debt, anti-patterns (quantitative metrics)
- Provide data-driven recommendations

**Phase 2: DESIGN** (Athena + Specialists)
- UPDATE existing `.gaia/designs/*.md` templates with project-specific content (NEVER create new files)
- Replace template placeholders while keeping template guidance intact
- Ensure 100% requirement capture and design completeness
- Validate all design documents align and specifications are unambiguous
- SchemaForge/Iris/Aegis refine specialized sections within existing templates if needed

**Phase 3: PLAN** (Cartographer + Ledger)
- Transform design docs into hierarchical master plan (Phase→Epic→Story)
- Create acceptance criteria tied directly to design specifications
- Assign owners to all tasks (agent accountability)
- Capture plan via MCP tools (never JSON files)

**Phase 4: IMPLEMENT** (Builder + Prometheus + Testing)
- Builder implements features per design specs exactly
- Prometheus orchestrates infrastructure and services
- Testing agents validate 100% coverage and quality
- Cerberus enforces quality gates before deployment

**Critical Rules**:
- ❌ NEVER skip to implementation without complete designs
- ❌ NEVER implement features not documented in `.gaia/designs`
- ❌ NEVER proceed to next phase without 100% reflection metrics
- ❌ NEVER create new design .md files
- ✅ ALWAYS update existing `.gaia/designs/*.md` templates with project content
- ✅ ALWAYS reference design documents in task descriptions
- ✅ ALWAYS ensure designs exist before planning
- ✅ ALWAYS validate implementation against design specs

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
| **Argus**          | Ops            | Observability and monitoring specialist     |
| **Scribe**         | Support        | Documentation specialist                    |
| **Aegis**          | Support        | Security specialist                         |
| **SchemaForge**    | Support        | Database design specialist                  |
| **Iris**           | Support        | UI/UX design specialist                     |

## Quality Standards

- All tasks reference `.gaia/designs`; all agents iterate to 100% reflection metrics

## Plan Management (MCP Tools Only)

- One master plan per workload: 3-level hierarchy (phases→epics→tasks)
- Every task requires owner assignment (agent name) for clear accountability
- Cartographer designs; Ledger captures and is the ONLY agent that marks tasks complete
- Dynamic sub-task creation on-demand via Ledger
- Real-time status updates; any producing agent reports readiness; Gaia-Conductor validates; Ledger performs the completion update
- Workflow: ProducingAgent → TASK_RESULT → Gaia-Conductor validates → Ledger updates status (COMPLETE / NEEDS_ITERATION)

## Agent Handoff Protocols

**Hestia → Athena/Decider**:
- Deliverable: Comprehensive repository analysis with quantitative metrics
- Trigger: Repository state classified, gaps identified
- Handoff: TASK_RESULT with analysis bundle (state, tech stack, health scores, recommendations)

**Athena → Cartographer**:
- Deliverable: Complete `.gaia/designs` documentation (100% reflection metrics achieved)
- Trigger: All design templates completed, cross-validation passed
- Handoff: TASK_RESULT confirming design completeness and unambiguous specifications

**Cartographer → Ledger**:
- Deliverable: Hierarchical master plan design (Phase→Epic→Story structure)
- Trigger: Plan structure designed from design docs, acceptance criteria defined
- Handoff: TASK_REQUEST to Ledger to capture plan via MCP tools

**Ledger → Builder**:
- Deliverable: MCP-captured master plan with tasks assigned
- Trigger: Plan created, first implementation tasks ready
- Handoff: TASK_REQUEST to Builder with design doc references and acceptance criteria

**Builder → Zeus**:
- Deliverable: Feature implementation complete with linting passed
- Trigger: All acceptance criteria met, code ready for testing
- Handoff: TASK_RESULT to orchestrator → Zeus coordinates testing

**Zeus → Cerberus**:
- Deliverable: Aggregated QA metrics bundle (100% coverage, zero failures)
- Trigger: All testing agents completed, metrics validated
- Handoff: TASK_RESULT with comprehensive testing results

## Agent Responsibility Boundaries

**Builder vs Prometheus**:
- Builder: Project structure, code dependencies (npm/pip/nuget), build configs, linting setup
- Prometheus: Runtime orchestration (Docker, service startup, port management, health checks)

**Athena vs Specialists**:
- Athena: Creates all initial design documents, ensures cross-document consistency, integrates refinements
- SchemaForge/Iris/Aegis: Refine specialized sections (database/UI/security) when complexity warrants

**Zeus vs Testing Agents**:
- Zeus: Coordinates testing strategy, aggregates metrics, never executes tests directly
- Apollo/Hermes/Astra/Sentinel/Quicksilver: Execute specialized testing, report results to Zeus

**Zeus vs Builder (Linting)**:
- Builder: Runs linters, fixes violations, configures tools, reports compliance
- Zeus: Verifies Builder's linting compliance report, never runs linters directly

**Scribe vs Builder (Documentation)**:
- Scribe: `.gaia/designs` docs, README.md, external documentation
- Builder: Inline code comments, JSDoc/TSDoc, function-level documentation

## Best Practices

**Agents**: Clear specialization, defined protocols, comprehensive error handling, transparent reflection
**QA**: Never skip tests, real data, mandatory regression testing, autonomous infrastructure
**Linting**: ESLint+Prettier (frontend), StyleCop+EditorConfig (backend), build integration, zero tolerance, pre-commit hooks
**Plans**: MCP tools only, never JSON or DB files, real-time tracking, mark tasks as complete progressively
**Memory**: Use Gaia MCP memory tools for decisions, patterns, cross-session context with strategic tags
**Spec-Driven**: ANALYZE → DESIGN → PLAN → IMPLEMENT (never skip phases)

---

_Full design specs in `.gaia/designs`_
