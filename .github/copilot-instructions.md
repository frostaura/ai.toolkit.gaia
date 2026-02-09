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

---

## Memory & Progress Tracking

### Agent Memory & Task Management
- **All agents** must use Gaia memory tools to remember important information across sessions
- **All agents** must use Gaia tasks tools to track progress when taking on workloads
- Each agent may plan and track for itself, creating a **web of plans** across the system
- Memory tools enable agents to:
  - Store key decisions, context, and learnings
  - Maintain continuity across multiple conversations
  - Share information with other agents when needed
- Task tools enable agents to:
  - Break down complex workloads into manageable steps
  - Track progress on multi-step activities
  - Maintain accountability and transparency
  - Coordinate with other agents by exposing their current state
- This decentralized approach allows each agent to maintain its own memory and task tracking while contributing to the overall system intelligence

### When to Use Memory Tools
Agents should query and store memory when they need to:
- **Remember user preferences and context** - User information, working styles, and specific requirements
- **Recall project-specific decisions** - Technology choices, architectural patterns, naming conventions
- **Access best practices** - Lessons learned, proven solutions, and established patterns
- **Get unstuck** - Previous solutions to similar problems, debugging strategies, workarounds
- **Maintain consistency** - Coding standards, project conventions, and recurring patterns
- **Share knowledge** - Store insights that other agents may need to reference later

---

## Self-Improvement & Evolution

### Logging Improvement Requests
- **All agents** must use Gaia's self-improvement tools to log runtime frustrations and improvement opportunities
- When agents encounter difficulties navigating, finding, or solving problems, they should log these experiences
- These tools enable agents to:
  - **Document pain points** - Issues that slow down or block progress
  - **Request new capabilities** - Missing tools or skills that would help future work
  - **Suggest workflow improvements** - Better ways to approach common tasks
  - **Identify knowledge gaps** - Areas where more context or guidance is needed
- Logged improvements will be applied to enhance agent capabilities over time
- Think of this as "wishing improvements into existence" - by documenting what would make your work easier, you help evolve yourself and other agents
- Agents should proactively log frustrations rather than silently struggling, as this accelerates collective improvement
