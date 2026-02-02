---
name: default-tech-stack
description: Default technology stack for Gaia projects. ASP.NET Core backend, React/TypeScript frontend, PostgreSQL database. These are defaults that can be overridden by user mandates.
---

# Default Technology Stack

## Backend

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core (.NET 8+) |
| ORM | Entity Framework Core |
| Architecture | Clean Architecture |
| Linting | StyleCop + Roslynator + SonarAnalyzer |

## Frontend

| Component | Technology |
|-----------|------------|
| Framework | React 18+ with TypeScript 5+ |
| State | Redux Toolkit + RTK Query |
| Linting | ESLint (strict) + Prettier |

## Database

| Component | Technology |
|-----------|------------|
| Primary | PostgreSQL 15+ |
| ORM | EF Core with migrations |
| Caching | Redis |

## Security

| Component | Configuration |
|-----------|---------------|
| Auth | JWT (15min access, 7day refresh) |
| Storage | httpOnly cookies preferred |
| Access | Role-based (RBAC) |
| Dev Admin | admin@system.local / Admin123! |

## Testing

| Type | Tool |
|------|------|
| E2E/Visual | Playwright MCP tools |
| Unit | xUnit (.NET), Vitest (React) |
| Coverage | Tiered by complexity |

## Architecture Principles

- **Clean Architecture**: Separation of concerns
- **iDesign**: Service-oriented components
- **API Design**: RESTful conventions
- **Error Handling**: Structured responses
