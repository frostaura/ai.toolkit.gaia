<p align="center">
  <img src="https://github.com/frostaura/fa.templates.vibe-coding/blob/main/README.icon.gif?raw=true" alt="GAIA Framework Logo" width="300" />
</p>

<h1 align="center">
  <b>GAIA 6</b>
</h1>
<h3 align="center">🌍 Adaptive AI-Driven Development</h3>

**A streamlined AI framework with 5 consolidated agents, adaptive process selection, and tiered quality gates for efficient production-ready development.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/badge/Version-6.0.0-blue.svg)]()
[![Status](https://img.shields.io/badge/Status-Production%20Ready-green.svg)]()
[![AI Enhanced](https://img.shields.io/badge/AI-Enhanced%20Intelligence-purple.svg)]()

## 🎯 What is GAIA 6?

GAIA 6 is an optimized AI-driven development system featuring:

- **5 Consolidated Agents** (down from 9) with mesh communication
- **Adaptive Process** - Right-size workflow to task complexity
- **Tiered Quality Gates** - Coverage requirements scale with complexity
- **On-Demand Design Docs** - Create only what's needed
- **MCP Memory** - Persistent institutional knowledge

### Adaptive Process Selection

| Complexity | Indicators | Phases |
|------------|------------|--------|
| **Trivial** | Typo fix, 1-line change | Fix → Verify |
| **Simple** | Bug fix, small tweak | Analyze → Fix → Test |
| **Standard** | Single feature | Plan → Implement → Test → Deploy |
| **Complex** | Multiple features | Design → Plan → Implement → Validate → Deploy |
| **Enterprise** | Full system | Full phased development |

## 🤖 The 5 Agents

| Agent | Purpose | Consolidates |
|-------|---------|--------------|
| **@Planner** | Strategy, design, research, orchestration | Gaia + Architect + Researcher |
| **@Developer** | All code, tests, infrastructure | Builder |
| **@Quality** | Testing, review, security validation | Tester + Reviewer |
| **@Operator** | Git, deployments, documentation | Deployer + Documenter |
| **@Analyst** | Fast codebase analysis, investigation | Explorer |

### Mesh Communication

Agents can invoke each other directly—no orchestrator bottleneck:
- `@Developer → @Quality`: "Validate before I continue"
- `@Quality → @Developer`: "Fix issue at line 45"
- `@Planner → @Analyst`: "What framework does this use?"

## 🚀 Quick Start

### Prerequisites

- **AI Assistant**: GitHub Copilot or Claude (with MCP support)
- **.NET SDK**: 9.0+ (for MCP server)
- **Node.js**: 20+ LTS

### 1. Setup MCP Server

Create `~/.config/copilot/mcp-config.json`:

```json
{
  "mcpServers": {
    "Gaia": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/PATH/TO/ai.toolkit.gaia/.gaia/mcps/gaia/src/fa.mcp.gaia/fa.mcp.gaia.csproj"
      ],
      "transport": "stdio"
    }
  }
}
```

### 2. Launch GAIA

```bash
# GitHub Copilot CLI
gh copilot
# Type: Build me a [your app description]

# Claude Desktop
@Planner Build me a [your app description]
```

## 📐 On-Demand Design Documents

Created only when needed, not upfront templates:

| Document | Purpose | Required From |
|----------|---------|---------------|
| `design.md` | Use cases + architecture | Standard |
| `api.md` | API contracts | When API changes |
| `data.md` | Database schema | When DB changes |
| `security.md` | Auth + access control | When security changes |

## 🛡️ Tiered Quality Gates

Coverage scales with complexity:

| Tier | Coverage | Gates |
|------|----------|-------|
| Trivial | None | Manual verification |
| Simple | 50% touched code | Build + Lint |
| Standard | 70% touched code | Build + Lint + Test |
| Complex | 80% all code | All + E2E |
| Enterprise | 90%+ all code | All + Security + Performance |

## 🧠 Memory System

```javascript
// Before work - check past knowledge
recall("authentication")
recall("cors error")

// After fixes - store solutions
remember("fix", "cors_error", "Added proxy config", "ProjectWide")
remember("pattern", "retry", "Use exponential backoff", "ProjectWide")
```

**Categories**: `fix`, `pattern`, `config`, `decision`, `warning`, `context`

## 🔧 MCP Tools

**Task Management**:
- `read_tasks(hideCompleted?)` - View tasks
- `update_task(taskId, description, status, assignedTo?)` - Manage tasks

**Memory**:
- `remember(category, key, value, duration)` - Store knowledge
- `recall(query, maxResults?)` - Search memories

## 📁 Repository Structure

```
.gaia/
├── agents/                # 5 consolidated agent definitions
│   ├── planner.md
│   ├── developer.md
│   ├── quality.md
│   ├── operator.md
│   └── analyst.md
├── skills/                # Shared capabilities (11 skills)
├── designs/               # On-demand design documents
├── instructions/          # Core system instructions
└── mcps/                  # MCP server
```

## 💡 Critical Rules

### ✅ ALWAYS:
- Use `recall()` before starting work
- Use `remember()` after fixes and discoveries
- Match process depth to task complexity
- Maintain backward compatibility

### ❌ NEVER:
- Create TODO.md or task files (use MCP)
- Skip design for Complex+ tasks
- Over-engineer simple tasks
- Ignore quality gates

## 📊 Improvements from GAIA 5

| Aspect | GAIA 5 | GAIA 6 |
|--------|--------|--------|
| Agents | 9 (orchestrator bottleneck) | 5 (mesh communication) |
| Process | 7 fixed phases | Adaptive (2-6 phases) |
| Design Docs | 6 required for large | 4 on-demand |
| Coverage | 100% always | Tiered (50-90%+) |
| WBS | Always 4 levels | Adaptive depth |

## 🔍 Skills Reference

| Skill | Purpose |
|-------|---------|
| mcp-memory-management | Memory patterns |
| design-document-management | Doc requirements |
| work-breakdown-structure | Adaptive WBS |
| quality-gate-validation | Gate execution |
| sdlc-tier-selection | Complexity tiers |
| playwright-testing | E2E testing |
| visual-excellence | UI standards |
| web-research | Research patterns |
| default-tech-stack | Tech defaults |
| strict-linting | Lint config |
| reflection | Post-task learning |

## 📄 License

MIT License - see [LICENSE](./LICENSE) for details

## 🙏 Acknowledgments

**Inspiration**: [Conway Osler](https://www.linkedin.com/in/conway-osler)

**Principles**: iDesign (Juval Löwy), Clean Architecture (Robert C. Martin), Domain-Driven Design (Eric Evans)

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life.<br/>Through adaptive orchestration, GAIA 6 creates digital ecosystems with right-sized quality."</i>
</p>
