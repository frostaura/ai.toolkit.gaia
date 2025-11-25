# System Architecture

## Overview
High-level system design and component interactions.

## Core Components (Default Stack)

### Frontend
- **Technology**: React with TypeScript
- **State Management**: Redux Toolkit
- **PWA**: Offline-first with Service Workers
- **Routing**: React Router v6
- **Styling**: CSS Modules + Tailwind CSS
- **Linting**: ESLint + Prettier

### Backend
- **Framework**: ASP.NET Core (.NET 8+)
- **ORM**: Entity Framework Core
- **Architecture Pattern**: Clean Architecture
- **Authentication**: JWT with refresh tokens
- **API Style**: RESTful with OpenAPI/Swagger
- **Linting**: .NET Analyzers + StyleCop

### Database
- **Primary**: PostgreSQL 15+
- **ORM**: Entity Framework Core with migrations
- **Cache**: Redis for session/cache
- **Connection**: Npgsql provider

### Infrastructure
- **Hosting**: [AWS/GCP/Azure/Vercel/etc.]
- **Container**: [Docker/Kubernetes/etc.]
- **CI/CD**: [GitHub Actions/GitLab CI/etc.]
- **Monitoring**: [DataDog/New Relic/etc.]

## Data Flow

```
User → Frontend → API Gateway → Backend Services → Database
                        ↓
                  Authentication
                        ↓
                  Authorization
```

## Key Patterns

### API Communication
- Request/Response with retry logic
- Error handling and fallbacks
- Rate limiting and throttling

### State Management
- Single source of truth
- Optimistic updates where appropriate
- Cache invalidation strategy

### Security
- Defense in depth
- Principle of least privilege
- Input validation at boundaries

## Scalability Considerations

### Horizontal Scaling
- Stateless services
- Database read replicas
- Load balancing strategy

### Performance
- Lazy loading
- Code splitting
- Database indexing
- CDN for static assets

## Development Workflow

1. Local development with hot reload
2. Feature branches with PR reviews
3. Automated testing (unit, integration, E2E)
4. Staging deployment for QA
5. Production deployment with monitoring

## Technology Decisions

Track key decisions in MCP:
```
mcp__gaia__remember("architecture", "database", "PostgreSQL chosen for ACID compliance")
mcp__gaia__remember("architecture", "frontend", "React with TypeScript for type safety")
```