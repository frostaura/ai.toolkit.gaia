[<< Back](../../README.md)

## Design

System architecture following iDesign architectural principles.

### Architecture Overview

**3-Layer Hierarchical Structure**:
- **Managers** (🟢): Orchestration, use case coordination
- **Engines** (🟠): Business logic, domain algorithms  
- **Data Access** (⚫): I/O operations, persistence
- **Models** (🟣): DTOs, entities, contracts

**Dependency Rules**:
- Managers → Engines, Data Access, Models
- Engines → Data Access, Models (never Managers)
- Data Access → Models only
- Models → Self-contained

### Design Documentation

| Document | Purpose |
| --- | --- |
| [Use Cases](./1-use-cases.md) | System requirements and user goals |
| [Class Diagrams](./2-class.md) | Data models and class structure |
| [Sequence Diagrams](./3-sequence.md) | Use case execution flows |
| [Frontend](./4-frontend.md) | UI/UX specifications |

### Quality Standards

**Testing**: 100% unit test coverage, integration testing, E2E validation
**Performance**: API < 200ms, Frontend LCP < 2.5s, zero build warnings
**Security**: Automated vulnerability scanning, secure coding practices

[<< Back](../../README.md)
