---
name: default-tech-stack
description: Default technology stack for Gaia projects. Use when starting new projects, making tech decisions, or setting up infrastructure. These are defaults and as such, any specific user mandates for any of the below areas, should be honored.
---

# Default Technology Stack

## Backend

| Component    | Technology                                                     |
| ------------ | -------------------------------------------------------------- |
| Framework    | ASP.NET Core (.NET 8+)                                         |
| ORM          | Entity Framework Core                                          |
| Architecture | Clean Architecture                                             |
| Linting      | StyleCop + Roslynator + SonarAnalyzer + .NET Analyzers (STRICT) |

> **STRICT MODE**: `TreatWarningsAsErrors=true`, zero warnings allowed. Build fails on any lint violation.
> See **`.gaia/skills/strict-linting/SKILL.md`** for configuration.

## Frontend

| Component | Technology                        |
| --------- | --------------------------------- |
| Framework | React 18+ with TypeScript 5+      |
| State     | Redux Toolkit + RTK Query         |
| PWA       | Optional (for offline-first)      |
| Linting   | ESLint (strict) + Prettier (STRICT) |

> **STRICT MODE**: `--max-warnings 0`, all warnings are errors. Build fails on any lint violation.
> See **`.gaia/skills/strict-linting/SKILL.md`** for configuration.

## Database

| Component | Technology                            |
| --------- | ------------------------------------- |
| Primary   | PostgreSQL 15+                        |
| ORM       | Entity Framework Core with migrations |
| Caching   | Redis                                 |

## Security

| Component      | Configuration                    |
| -------------- | -------------------------------- |
| Authentication | JWT (15min access, 7day refresh) |
| Token Storage  | httpOnly cookies preferred       |
| Access Control | Role-based (RBAC)                |
| Dev Admin      | admin@system.local / Admin123!   |

## Testing

| Type                  | Tool                           |
| --------------------- | ------------------------------ |
| E2E/Visual            | Playwright MCP tools           |
| Unit Coverage         | 100% (frontend + backend)      |
| Functional Regression | Playwright interactive testing |

## Architecture Principles

- **Clean Architecture**: Separation of concerns, dependency inversion
- **iDesign**: Service-oriented component design
- **API Design**: RESTful with consistent conventions
- **Error Handling**: Structured error responses, proper logging
