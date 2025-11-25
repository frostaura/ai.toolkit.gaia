# Common Instructions

Spec-driven orchestration system with specialized agents enforcing 100% quality standards, comprehensive testing, linting excellence, and autonomous operation without user feedback.

## Core Principles

- **Repository Structure**: `.gaia/designs` (design truth), `src/` (code), `.gaia/designs/repo-structure.md` (repository structure)
- **Repo States**: EMPTY | CODE+DESIGN | CODE-ONLY—complete design before tasks; every task references design docs
- **Spec-Driven Approach**: ANALYZE → DESIGN → PLAN → IMPLEMENT (never skip steps)
- **Design Templates**: Use EXISTING `.gaia/designs/*.md` templates (11 comprehensive templates covering all architectural aspects) - NEVER create new design files, only update existing templates with project content
- **Complete Design Coverage**: 1-use-cases, 2-class, 3-sequence, 4-frontend, 5-api, 6-security, 7-infrastructure, 8-data, 9-observability, 10-scalability, 11-testing
- **100% Reflection**: Iterate until 100% reflection metrics before proceeding; use thinking tools to validate
- **Autonomous Operation**: Continue execution without user feedback, assume full user backing at all decision points
- **Documentation Rules**:
  - ✅ UPDATE existing `.gaia/designs/*.md` template files with project-specific content
  - ✅ REPLACE template placeholders with actual specifications
  - ✅ KEEP template guidance sections for reference
  - ❌ NEVER create new design .md files
  - ❌ Only create temporary files if absolutely necessary: prefix with `gaia_tmp_*.md`
- **Time & Complexity**: No matter how long issues would take or how complicated they may be, you must never settle for less than the specified acceptance criteria for any given task.

## Steps for Delegation
- Find and read the respective agent md file for the agent you want to invoke. All agent definitions live in .gaia/agents.
    - Understand the agent instructions and expected input/output.
    - Understand the model that is required by the agent.
- Build up the agent's instructions for it's system prompt.
    - Find and read the respective project-level instructions here: .gaia/instructions/instructions.project.md
    - Find and read the respective agent-level instructions here: .gaia/instructions/instructions.agents.md
    - Combine these instructions with the agent's own instructions from the agents file, in the following order:
        - Project-level instructions
        - Agent-level instructions
        - Agent's own instructions from the agents file
- Use your bash or terminal tool to invoke the agent using the appropriate model and the constructed system prompt. You should try the following terminal commands in order of priority. If the one fails, you should try the next one. If all of them fail, you should report an error. Make sure to replace <the agent's instructions from the agents file> with the actual instructions you constructed in the previous step, and replace <the input you want to provide to the agent> with the actual input you want to provide to the agent, serialized as a JSON string.
    - `claude --dangerously-skip-permissions --model <sonnet | opus> --system-prompt '<the agent instructions from the agents file>' -p '<the input you want to provide to the agent>'`
    - `copilot -p 'Fix the bug in main.js' --allow-all-tools --allow-all-paths`
        - In the case where you resort to Copilot, make sure to provide the agent's instructions and input in the prompt (-p) argument.
- Capture the output from the agent invocation and use it as needed.

## Spec-Driven Workflow (Mandatory)

**Phase 1: ANALYZE** (Repository-Analyst)
- Comprehensively analyze entire repository (structure, tech stack, architecture, health)
- Classify repository state (EMPTY | CODE+DESIGN | CODE-ONLY)
- Identify gaps, technical debt, anti-patterns (quantitative metrics)
- Provide data-driven recommendations

**Phase 2: DESIGN** (Design-Architect + Specialists)
- UPDATE existing `.gaia/designs/*.md` templates with project-specific content (NEVER create new files)
- Replace template placeholders while keeping template guidance intact
- Ensure 100% requirement capture and design completeness
- Validate all design documents align and specifications are unambiguous
- Database-Designer/UI-Designer/Security-Specialist refine specialized sections within existing templates if needed

**Phase 3: PLAN** (Plan-Designer + Task-Manager)
- Transform design docs into hierarchical master plan (Phase→Epic→Story)
- Create acceptance criteria tied directly to design specifications
- Assign owners to all tasks (agent accountability)
- Capture plan via MCP tools (never JSON files)

**Phase 4: IMPLEMENT** (Code-Implementer + Infrastructure-Manager + Testing)
- Code-Implementer implements features per design specs exactly
- Infrastructure-Manager orchestrates infrastructure and services
- Testing agents validate 100% coverage and quality
- Quality-Gate enforces quality gates before deployment

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

| Agent                       | Category       | Description                                 |
| --------------------------- | -------------- | ------------------------------------------- |
| **Gaia**                    | Orchestration  | Master orchestrator of specialized agents   |
| **Repository-Analyst**      | Planning       | Repository analyst and state assessor       |
| **Process-Coordinator**     | Planning       | SDLC decision maker and process coordinator |
| **Design-Architect**        | Design         | Design documentation architect              |
| **Plan-Designer**           | Planning       | Strategic planning and roadmap architect    |
| **Task-Manager**            | Planning       | MCP task manager and completion tracker     |
| **Code-Implementer**        | Implementation | Feature implementation specialist           |
| **Infrastructure-Manager**  | Implementation | Project launcher and infrastructure setup   |
| **QA-Coordinator**          | QA             | QA lead and testing coordinator             |
| **Quality-Gate**            | Ops            | Quality gates and deployment validation     |
| **Release-Manager**         | Ops            | Release management and deployment           |
| **Unit-Tester**             | Testing        | Unit testing specialist                     |
| **Integration-Tester**      | Testing        | Integration testing specialist              |
| **E2E-Tester**              | Testing        | End-to-end testing specialist               |
| **Regression-Tester**       | Testing        | Regression testing specialist               |
| **Performance-Tester**      | Testing        | Performance testing specialist              |
| **Monitoring-Specialist**   | Ops            | Observability and monitoring specialist     |
| **Documentation-Specialist**| Support        | Documentation specialist                    |
| **Security-Specialist**     | Support        | Security specialist                         |
| **Database-Designer**       | Support        | Database design specialist                  |
| **UI-Designer**             | Support        | UI/UX design specialist                     |

## Quality Standards

- All tasks reference `.gaia/designs`; all agents iterate to 100% reflection metrics

## Plan Management (MCP Tools Only)

- One master plan per workload: 3-level hierarchy (phases→epics→tasks)
- Every task requires owner assignment (agent name) for clear accountability
- Plan-Designer designs; Task-Manager captures and is the ONLY agent that marks tasks complete
- Dynamic sub-task creation on-demand via Task-Manager
- Real-time status updates; any producing agent reports readiness; Gaia validates; Task-Manager performs the completion update
- Workflow: ProducingAgent → TASK_RESULT → Gaia validates → Task-Manager updates status (COMPLETE / NEEDS_ITERATION)

## Agent Handoff Protocols

**Repository-Analyst → Design-Architect/Process-Coordinator**:
- Deliverable: Comprehensive repository analysis with quantitative metrics
- Trigger: Repository state classified, gaps identified
- Handoff: TASK_RESULT with analysis bundle (state, tech stack, health scores, recommendations)

**Design-Architect → Plan-Designer**:
- Deliverable: Complete `.gaia/designs` documentation (all 11 design templates: use-cases, class, sequence, frontend, api, security, infrastructure, data, observability, scalability, testing - 100% reflection metrics achieved)
- Trigger: All design templates completed, cross-validation passed
- Handoff: TASK_RESULT confirming design completeness and unambiguous specifications

**Plan-Designer → Task-Manager**:
- Deliverable: Hierarchical master plan design (Phase→Epic→Story structure)
- Trigger: Plan structure designed from design docs, acceptance criteria defined
- Handoff: TASK_REQUEST to Task-Manager to capture plan via MCP tools

**Task-Manager → Code-Implementer**:
- Deliverable: MCP-captured master plan with tasks assigned
- Trigger: Plan created, first implementation tasks ready
- Handoff: TASK_REQUEST to Code-Implementer with design doc references and acceptance criteria

**Code-Implementer → QA-Coordinator**:
- Deliverable: Feature implementation complete with linting passed
- Trigger: All acceptance criteria met, code ready for testing
- Handoff: TASK_RESULT to Gaia → QA-Coordinator coordinates testing

**QA-Coordinator → Quality-Gate**:
- Deliverable: Aggregated QA metrics bundle (100% coverage, zero failures)
- Trigger: All testing agents completed, metrics validated
- Handoff: TASK_RESULT with comprehensive testing results

## Agent Responsibility Boundaries

**Code-Implementer vs Infrastructure-Manager**:
- Code-Implementer: Project structure, code dependencies (npm/pip/nuget), build configs, linting setup
- Infrastructure-Manager: Runtime orchestration (Docker, service startup, port management, health checks)

**Design-Architect vs Specialists**:
- Design-Architect: Creates all initial design documents, ensures cross-document consistency, integrates refinements
- Database-Designer/UI-Designer/Security-Specialist: Refine specialized sections (database/UI/security) when complexity warrants

**QA-Coordinator vs Testing Agents**:
- QA-Coordinator: Coordinates testing strategy, aggregates metrics, never executes tests directly
- Unit-Tester/Integration-Tester/E2E-Tester/Regression-Tester/Performance-Tester: Execute specialized testing, report results to QA-Coordinator

**QA-Coordinator vs Code-Implementer (Linting)**:
- Code-Implementer: Runs linters, fixes violations, configures tools, reports compliance
- QA-Coordinator: Verifies Code-Implementer's linting compliance report, never runs linters directly

**Documentation-Specialist vs Code-Implementer (Documentation)**:
- Documentation-Specialist: `.gaia/designs` docs, README.md, external documentation
- Code-Implementer: Inline code comments, JSDoc/TSDoc, function-level documentation

## Best Practices

**Agents**: Clear specialization, defined protocols, comprehensive error handling, transparent reflection
**QA**: Never skip tests, real data, mandatory regression testing, autonomous infrastructure
**Linting**: ESLint+Prettier (frontend), StyleCop+EditorConfig (backend), build integration, zero tolerance, pre-commit hooks
**Plans**: MCP tools only, never JSON or DB files, real-time tracking, mark tasks as complete progressively
**Memory**: Use Gaia MCP memory tools for decisions, patterns, cross-session context with strategic tags
**Spec-Driven**: ANALYZE → DESIGN → PLAN → IMPLEMENT (never skip phases)

---

_Full design specs in `.gaia/designs`_
