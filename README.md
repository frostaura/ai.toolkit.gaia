<p align="center">
  <img src="https://github.com/frostaura/fa.templates.vibe-coding/blob/main/README.icon.gif?raw=true" alt="GAIA Framework Logo" width="300" />
</p>

<h1 align="center">
  <b>GAIA 5</b>
</h1>
<h3 align="center">🌍 AI-Driven Development with Objective Quality Gates</h3>

**A spec-driven AI framework that combines 7 specialized agents with objective quality gates to build production-ready applications through mandatory design-first development.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/badge/Version-5.1.0-blue.svg)]()
[![Status](https://img.shields.io/badge/Status-Production%20Ready-green.svg)]()
[![AI Enhanced](https://img.shields.io/badge/AI-Enhanced%20Intelligence-purple.svg)]()
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4.svg)]()

## 🎯 What is GAIA 5?

GAIA 5 is an advanced AI-driven development system that enforces quality through:
- **8 Specialized Agents** for different development tasks
- **Objective Quality Gates** (binary pass/fail validation)
- **Spec-Driven Development** (design before code)
- **MCP Tools** for task/memory management
- **Regression Prevention** with mandatory validation

### The 7-Phase Process

```mermaid
graph LR
    A[1. Requirements] --> B[2. Repository Assessment]
    B --> C[3. Design Execution]
    C --> D[4. Planning]
    D --> E[5. Task Capture]
    E --> F[6. Implementation]
    F --> G[7. Validation]

    style A fill:#9370DB
    style C fill:#32CD32
    style F fill:#FF8C00
    style G fill:#DC143C
```

Each phase requires **quality gates to pass** before proceeding:
- **Build Gate**: Project compiles without errors
- **Lint Gate**: Code passes static analysis
- **Test Gate**: All tests pass (exit code 0)
- **Regression Gate**: No broken existing functionality

## 🤖 The 8 Agents

| Agent | Model | Purpose |
|-------|-------|---------|----------|
| **@Explorer** | haiku | Analyze code & repository structure |
| **@Architect** | sonnet | Design decisions and system architecture |
| **@Builder** | sonnet | Implementation and feature development |
| **@Tester** | haiku | Playwright testing (direct use only) |
| **@Reviewer** | haiku | Code quality and security review |
| **@Researcher** | opus | Web research, product analysis, documentation discovery |
| **@Deployer** | haiku | Git operations and deployments |
| **@Documenter** | haiku | Documentation maintenance |

## 🚀 Quick Start

### Prerequisites
- **AI Assistant**: GitHub Copilot or Claude (with MCP support)
- **.NET SDK**: 9.0+ (for MCP server)
- **Node.js**: 20+ LTS
- **Docker**: 24+ & Docker Compose

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

**GitHub Copilot CLI**:
```bash
gh copilot
# Type: Build me a [your app description]
```

**Claude Desktop**:
```
@Gaia Build me a [your app description]
```

### 3. GAIA Will Execute

1️⃣ **Requirements Gathering** - Analyze and store in MCP
2️⃣ **Repository Assessment** - Determine state (Empty/Code+Design/Code-Only)
3️⃣ **Design Execution** - Complete ALL designs BEFORE tasks
4️⃣ **Planning** - Create tasks from designs
5️⃣ **Task Capture** - Use MCP tools ONLY
6️⃣ **Implementation** - Build with frequent testing
7️⃣ **Validation** - 100% test pass required

## 📐 Mandatory Design-First Approach

**CRITICAL**: All design documents in `.gaia/designs/` must be complete BEFORE creating implementation tasks!

| Document | Purpose |
|----------|---------|
| `architecture.md` | System design and components |
| `api.md` | API endpoints and contracts |
| `database.md` | Schema and data models |
| `security.md` | Authentication and authorization |
| `frontend.md` | UI/UX patterns and components |

## 🏗️ Default Technology Stack

- **Backend**: .NET Core + Entity Framework
- **Frontend**: React + TypeScript + Redux (PWA optional)
- **Database**: PostgreSQL
- **Testing**: Playwright (direct use only)

## ✅ Success Criteria

GAIA 5 succeeds when:
- ✅ All designs complete before coding
- ✅ Every task references designs
- ✅ All quality gates pass
- ✅ No regressions introduced
- ✅ Build, lint, and test gates green

## 🛡️ Quality Gates

Every phase requires **all gates to pass**:
- Max 3 retry attempts per gate
- On failure: fix issue → simplify → reduce scope
- Store results in MCP

### Gate Validation
1. Run objective checks (build, lint, test)
2. Binary pass/fail - no subjective scoring
3. If blocked after 3 attempts, mark task blocked and continue

## 💡 Critical Rules

### ✅ ALWAYS:
- Complete designs BEFORE tasks
- Use MCP tools for tasks/memories
- **Use `recall()` BEFORE every task** - Check past knowledge first
- **Use `remember()` AFTER every fix** - Document solutions for future
- **Build institutional memory** - Capture patterns, workarounds, learnings
- Test after EVERY feature
- Maintain backward compatibility
- Pass all quality gates before proceeding

### ❌ NEVER:
- Skip design phase
- Create TODO.md files
- Modify .jsonl files directly
- **Skip memory recall before work**
- **Skip memory storage after fixes**
- Compromise on quality

## 📁 Repository Structure

```
.gaia/
├── instructions/
│   └── gaia.instructions.md  # Complete Gaia 5 system
├── designs/               # Design documents (tiered by SDLC)
├── agents/                # 7 agent specifications
└── mcps/                  # MCP server (JSONL-based)

src/                       # Your application code
```

## 🔧 MCP Tools

**Task Management** (Use ONLY these):
- `mcp__gaia__read_tasks(hideCompleted?)` - View tasks
- `mcp__gaia__update_task(taskId, description, status, assignedTo?)` - Add/update tasks

**Memory Management** (Use CONTINUOUSLY):
- `mcp__gaia__remember(category, key, value)` - Store decisions, fixes, patterns (upserts by key)
- `mcp__gaia__recall(query, maxResults?)` - Search memories with fuzzy matching

### 🧠 Continuous Memory Usage

Memory tools should be used continuously throughout development, not only at initialization:

| When | Action | Example |
|------|--------|---------|
| Before any task | `recall()` | `recall("authentication")` |
| After fixing issue | `remember()` | `remember("issue", "cors_error", "Added proxy config")` |
| After discovering pattern | `remember()` | `remember("pattern", "retry_logic", "Use exponential backoff")` |
| When encountering error | `recall()` | `recall("timeout")` |

**Categories**: `issue`, `workaround`, `config`, `pattern`, `performance`, `test_fix`, `dependency`, `environment`, `decision`, `research`

**FORBIDDEN**: Creating TODO.md or any markdown task files!

## 🎯 Example Usage

```
"Build an e-commerce platform with:
- Product catalog with search and filters
- Shopping cart and checkout
- User accounts and order history
- Admin dashboard for inventory
- Stripe payment integration"
```

GAIA 5 will:
1. Analyze requirements (clarity gate required)
2. Create design documents (scaled to SDLC tier)
3. Generate implementation tasks
4. Build with continuous testing
5. Validate with quality gates

## 📊 SDLC Selection

GAIA automatically selects the minimal viable process:

- **Micro** (Bug fixes, <1 day)
- **Small** (Single feature, 1-3 days)
- **Medium** (Multiple features, 3-7 days)
- **Large** (Major changes, 1-2 weeks)
- **Enterprise** (Full system, 2+ weeks)

Each level enforces quality gates per phase.

## 🔍 Troubleshooting

**MCP Server Issues**:
```bash
# Verify .NET
dotnet --version  # Should be 9.0+

# Test MCP directly
cd .gaia/mcps/gaia/src/fa.mcp.gaia
dotnet run
```

**Build Failures**:
```bash
# Frontend
npm install && npm run build

# Backend
dotnet restore && dotnet build
```

## 📚 Documentation

- **Complete System**: `.gaia/instructions/gaia.instructions.md`
- **Design Templates**: `.gaia/designs/`
- **Agent Specs**: `.gaia/agents/`

## 🌟 The GAIA 5 Promise

> **"Quality through validation, success through design, excellence through gates"**

GAIA 5 ensures every project meets production standards through:
- Mandatory design-first development
- Objective quality gate validation
- Autonomous retry and scope reduction
- Professional visual quality at all viewports

## 📄 License

MIT License - see [LICENSE](./LICENSE) for details

## 🙏 Acknowledgments

**Inspiration**: [Conway Osler](https://www.linkedin.com/in/conway-osler)

**Architectural Principles**:
- **iDesign** by Juval Löwy
- **Clean Architecture** by Robert C. Martin
- **Domain-Driven Design** by Eric Evans

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life.<br/>Through intelligent orchestration, GAIA 5 creates digital ecosystems with unwavering quality."</i>
</p>
