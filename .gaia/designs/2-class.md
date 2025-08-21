<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/1-use-cases.md -->

[<< Back](./design.md)

# Class Diagrams & Data Models

System structural design bridging use cases to implementation.

## Template Guidance

**Purpose**: Define classes, relationships, and data flow
**Focus**: Domain entities, interfaces, data structure
**Avoid**: Implementation details, database optimization

**Guidelines**: Domain-driven design, interface-focused, clear relationships

## System Architecture

```mermaid
classDiagram
    class Managers {
        <<layer>>
        +Orchestration
        +Use Case Coordination
    }
    class Engines {
        <<layer>>
        +Business Logic
        +Domain Rules
    }
    class DataAccess {
        <<layer>>
        +Repositories
        +Persistence
    }
    class Models {
        <<layer>>
        +Entities
        +DTOs
    }
    
    Managers --> Engines: uses
    Managers --> DataAccess: uses
    Engines --> DataAccess: uses
    Engines --> Models: operates on
    DataAccess --> Models: persists
```

## Domain Model

```mermaid
classDiagram
    class EntityA {
        +Id: Guid
        +Name: string
        +Status: EntityStatus
        +CreatedAt: DateTime
        +UpdateStatus(status) void
    }
    
    class EntityB {
        +Id: Guid
        +Title: string
        +OwnerId: Guid
        +AddItem(item) void
    }
    
    class EntityStatus {
        <<enumeration>>
        Draft
        Active
        Inactive
    }
    
    EntityA "1" --> "0..*" EntityB: owns
    EntityA --> EntityStatus: has
```

## Service Layer

```mermaid
classDiagram
    class IApplicationService {
        <<interface>>
        +ExecuteUseCase(request) Task~Result~
    }
    
    class UseCaseService {
        +ExecuteUseCase(request) Task~Result~
        -ValidateRequest(request) bool
        -ProcessBusinessLogic(data) Result
    }
    
    class IRepository {
        <<interface>>
        +GetByIdAsync(id) Task~Entity~
        +SaveAsync(entity) Task~bool~
    }
    
    IApplicationService <|.. UseCaseService: implements
    UseCaseService --> IRepository: uses
```

## Data Layer

```mermaid
classDiagram
    class IEntityRepository {
        <<interface>>
        +GetByIdAsync(id) Task~Entity~
        +SaveAsync(entity) Task~bool~
        +GetByStatusAsync(status) Task~List~Entity~~
    }
    
    class EntityRepository {
        +GetByIdAsync(id) Task~Entity~
        +SaveAsync(entity) Task~bool~
        -MapToEntity(dbModel) Entity
    }
    
    IEntityRepository <|.. EntityRepository: implements
```

## DTOs & Database

```mermaid
classDiagram
    class CreateEntityRequest {
        +Name: string
        +InitialStatus: string
        +Validate() bool
    }
    
    class EntityResponse {
        +Id: Guid
        +Name: string
        +Status: string
        +CreatedAt: DateTime
    }
```

```mermaid
erDiagram
    ENTITY_A ||--o{ ENTITY_B : owns
    ENTITY_A {
        uuid id PK
        varchar name
        varchar status
        timestamp created_at
    }
    ENTITY_B {
        uuid id PK
        varchar title
        uuid owner_id FK
        timestamp created_at
    }
```

## Validation Checklist

**Domain Model**: 
- [ ] Entities map to business concepts from use cases
- [ ] Relationships reflect business rules

**Service Design**: 
- [ ] Services coordinate use cases
- [ ] Interfaces define clear contracts
- [ ] Dependencies flow inward

**Data Design**: 
- [ ] Repositories abstract persistence
- [ ] DTOs provide clean API contracts

**Instructions**: Replace entities with actual business concepts from use cases. Ensure all classes support identified use cases.

[<< Back](./design.md)
