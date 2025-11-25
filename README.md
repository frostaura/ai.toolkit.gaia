<p align="center">
  <img src="https://github.com/frostaura/fa.templates.vibe-coding/blob/main/README.icon.gif?raw=true" alt="GAIA Framework Logo" width="300" />
</p>

<h1 align="center">
  <b>GAIA 5</b>
</h1>
<h3 align="center">🌍 AI-Driven Development with Reflection-Based Quality Assurance</h3>

**A spec-driven AI framework that combines 7 specialized agents with reflection metrics to build production-ready applications through mandatory design-first development.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/badge/Version-5.0.0-blue.svg)]()
[![Status](https://img.shields.io/badge/Status-Production%20Ready-green.svg)]()
[![AI Enhanced](https://img.shields.io/badge/AI-Enhanced%20Intelligence-purple.svg)]()
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4.svg)]()

## 🎯 What is GAIA 5?

GAIA 5 is an advanced AI-driven development system that enforces quality through:
- **7 Specialized Agents** for different development tasks
- **Reflection-based Quality Assurance** (100% metrics required)
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

Each phase requires **100% reflection metrics** before proceeding:
- **Clarity, Efficiency, Comprehensiveness**
- **Design Completeness, Template Adherence**
- **Test Coverage, Regression Prevention**
- **Visual Quality, Performance Metrics**

## 🤖 The 7 Agents

| Agent | Model | Purpose |
|-------|-------|---------|
| **@Explorer** | haiku | Analyze code & repository structure |
| **@Architect** | sonnet | Design decisions and system architecture |
| **@Builder** | sonnet | Implementation and feature development |
| **@Tester** | haiku | Playwright testing (direct use only) |
| **@Reviewer** | haiku | Code quality and security review |
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
- **Frontend**: React + TypeScript + Redux (PWA mandatory)
- **Database**: PostgreSQL
- **Testing**: Playwright (direct use only)

## ✅ Success Criteria

GAIA 5 succeeds when:
- ✅ All designs complete before coding
- ✅ Every task references designs
- ✅ All tests pass at 100%
- ✅ No regressions introduced
- ✅ All reflection metrics achieve 100%

## 🛡️ Quality Gates

Every phase requires **100% on ALL metrics**:
- Max 3 improvement attempts
- Store results in MCP
- Flag if can't achieve 100%

### Reflection Process
1. Score each metric (0-100%)
2. WHILE any metric <100%: Improve → Re-score
3. Only proceed at 100%

## 💡 Critical Rules

### ✅ ALWAYS:
- Complete designs BEFORE tasks
- Use MCP tools for tasks/memories
- Test after EVERY feature
- Maintain backward compatibility
- Achieve 100% reflection metrics

### ❌ NEVER:
- Skip design phase
- Create TODO.md files
- Modify .jsonl files directly
- Compromise on quality

## 📁 Repository Structure

```
.gaia/
├── instructions/
│   └── gaia.instructions.md  # Complete Gaia 5 system
├── designs/               # 5 mandatory design documents
├── agents/                # 7 agent specifications
└── mcps/                  # MCP server (JSONL-based)

src/                       # Your application code
```

## 🔧 MCP Tools

**Task Management** (Use ONLY these):
- `mcp__gaia__create_new_plan` - Create project plan
- `mcp__gaia__add_task_to_plan` - Add tasks
- `mcp__gaia__get_tasks_from_plan` - View tasks
- `mcp__gaia__mark_task_as_completed` - Complete tasks

**Memory Management**:
- `mcp__gaia__remember` - Store decisions
- `mcp__gaia__recall` - Search memories

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
1. Analyze requirements (100% clarity required)
2. Create 5 design documents
3. Generate implementation tasks
4. Build with continuous testing
5. Validate with zero regressions

## 📊 SDLC Selection

GAIA automatically selects the minimal viable process:

- **Micro** (Bug fixes, <1 day)
- **Small** (Single feature, 1-3 days)
- **Medium** (Multiple features, 3-7 days)
- **Large** (Major changes, 1-2 weeks)
- **Enterprise** (Full system, 2+ weeks)

Each level enforces 100% reflection metrics per phase.

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

> **"Quality through reflection, success through design, excellence through validation"**

GAIA 5 ensures every project meets production standards through:
- Mandatory design-first development
- Continuous reflection and improvement
- 100% test coverage and regression prevention
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