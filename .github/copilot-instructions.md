# Shared Development Guidelines

## Overview

These are common instructions that all agents must follow. Agent-specific instructions are defined in individual agent files under `.github/agents/`.

---

## Design Philosophy

### Spec-Driven Design
- All development follows a **strict spec-driven design** approach
- Design specifications in `docs/` are the single source of truth
- All features in design docs must be implemented in the codebase
- All code in the codebase must be reflected in the design docs
- **The architect agent is the gatekeeper** for maintaining spec integrity
- Any changes to specifications, architecture, or design must go through the architect
- This ensures consistency between documentation and implementation at all times

---

## Agent Responsibilities & Permissions

### Documentation Management
- **ONLY the architect agent** is permitted to create, modify, or delete documentation in the `docs/` directory
- No other agent may create or modify documentation files
- All other agents must request documentation changes through the architect
- The architect serves as the **gatekeeper for spec-driven design**, ensuring all documentation accurately reflects the codebase and vice versa
- Documentation must be created on-demand when needed, not as upfront templates

### Technology Stack Decisions
- **Only the architect agent** makes technology stack choices and architectural decisions
- The developer agent must consult the architect before implementing new technologies or patterns
- All architectural changes must go through the architect to maintain spec-driven design integrity
- Default stack decisions are maintained in `skills/default-web-stack/SKILL.md`

### Code Implementation
- **ONLY the developer agent** is permitted to write application code, tests, migrations, and infrastructure configurations
- No other agent may create or modify code files
- All code implementation must be done by the developer agent
- Developers must follow the architecture established by the architect

### Investigation & Analysis
- **The analyst agent** investigates bugs, performance issues, and complex problems
- Other agents should invoke the analyst when stuck or needing deep investigation
- The analyst provides insights but does not implement solutions directly

### Quality Validation
- **The tester agent** performs comprehensive functional and visual validation
- Testing occurs after implementation is complete and quality gates pass
- Testers validate features against use cases defined in design documents

### Workflow Orchestration
- **The workload orchestrator agent** must be invoked for initialization of user requests
- The orchestrator determines which agents to invoke and coordinates the workflow
- The orchestrator ensures proper handoffs between agents and manages dependencies
- All complex or multi-step user requests should be routed through the orchestrator
