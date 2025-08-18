<!-- reference @.docs/design.md -->
<!-- reference @.docs/1-use-cases.md -->

[<< Back](./../design.md)

# Class Diagrams & Data Models
Below we elaborate on the system's class diagrams and data relationships. This document translates use cases into concrete class structures and data models.

Ensure we follow our **design principles** and that this document naturally extends the **use cases**.

## 📋 Template Guidance

### Purpose & Scope
This document defines the **structural design** of the system - what classes exist, how they relate, and what data they manage. It bridges **use cases** to technical implementation.

**🎯 Focus**: Domain entities, relationships, interfaces, and data flow
**🚫 Avoid**: Implementation details, database optimization, deployment specifics

### Class Diagram Guidelines
- **Domain-driven**: Start with business entities from **use cases**
- **Interface-focused**: Define contracts before implementations  
- **Relationship clarity**: Show how classes collaborate to fulfill **use cases**
- **Appropriate abstraction**: Not too detailed, not too high-level

## 🏗️ System Architecture Overview

### High-Level Class Structure
```mermaid
classDiagram
    %% Core application layers and their relationships
    class Presentation {
        <<layer>>
        +Controllers
        +ViewModels
        +DTOs
    }
    
    class Business {
        <<layer>>
        +Services
        +Managers
        +Domain Logic
    }
    
    class Data {
        <<layer>>
        +Repositories
        +Data Access
        +Persistence
    }
    
    class Domain {
        <<layer>>
        +Entities
        +Value Objects
        +Domain Services
    }
    
    Presentation --> Business: uses
    Business --> Domain: operates on
    Business --> Data: retrieves/stores
    Data --> Domain: persists
```

## 📊 Domain Model

### Core Business Entities
*Map directly from **use cases** and business requirements*

```mermaid
classDiagram
    %% Primary domain entities derived from **use cases**
    class EntityA {
        +Id: Guid
        +Name: string
        +Status: EntityStatus
        +CreatedAt: DateTime
        +GetDetails() EntityDetails
        +UpdateStatus(status EntityStatus) void
    }
    
    class EntityB {
        +Id: Guid
        +Title: string
        +OwnerId: Guid
        +Items: List~EntityC~
        +AddItem(item EntityC) void
        +RemoveItem(itemId Guid) bool
    }
    
    class EntityC {
        +Id: Guid
        +Value: decimal
        +Type: ItemType
        +ParentId: Guid
        +Calculate() decimal
    }
    
    class EntityStatus {
        <<enumeration>>
        Draft
        Active
        Inactive
        Archived
    }
    
    class ItemType {
        <<enumeration>>
        TypeA
        TypeB
        TypeC
    }
    
    %% Relationships based on business rules
    EntityA "1" --> "0..*" EntityB: owns
    EntityB "1" --> "0..*" EntityC: contains
    EntityA --> EntityStatus: has
    EntityC --> ItemType: has
```

### Domain Services
*Services that implement business logic spanning multiple entities*

```mermaid
classDiagram
    class IDomainService {
        <<interface>>
        +ExecuteBusinessRule(entities List~Entity~) BusinessResult
        +ValidateBusinessConstraints(entity Entity) ValidationResult
    }
    
    class BusinessRuleEngine {
        +ExecuteBusinessRule(entities List~Entity~) BusinessResult
        +ValidateBusinessConstraints(entity Entity) ValidationResult
        -ApplyRule(rule BusinessRule, entity Entity) RuleResult
    }
    
    class INotificationService {
        <<interface>>
        +SendNotification(message Notification) Task
        +BroadcastUpdate(update SystemUpdate) Task
    }
    
    IDomainService <|.. BusinessRuleEngine: implements
```

## 🔌 Service Layer

### Application Services
*Orchestrate **use cases** and coordinate between layers*

```mermaid
classDiagram
    class IApplicationService {
        <<interface>>
        +ExecuteUseCase(request UseCaseRequest) Task~UseCaseResult~
    }
    
    class UseCaseService {
        -IRepository _repository
        -IDomainService _domainService
        -INotificationService _notificationService
        +UseCaseService(deps...)
        +ExecuteUseCase(request UseCaseRequest) Task~UseCaseResult~
        -ValidateRequest(request UseCaseRequest) ValidationResult
        -ProcessBusinessLogic(data BusinessData) ProcessResult
    }
    
    class IRepository {
        <<interface>>
        +GetByIdAsync(id Guid) Task~Entity~
        +SaveAsync(entity Entity) Task~bool~
        +DeleteAsync(id Guid) Task~bool~
    }
    
    IApplicationService <|.. UseCaseService: implements
    UseCaseService --> IRepository: uses
    UseCaseService --> IDomainService: uses
    UseCaseService --> INotificationService: uses
```

## 💾 Data Layer

### Repository Pattern
*Abstract data access based on domain needs*

```mermaid
classDiagram
    class IEntityRepository {
        <<interface>>
        +GetByIdAsync(id Guid) Task~Entity~
        +GetByStatusAsync(status EntityStatus) Task~List~Entity~~
        +SaveAsync(entity Entity) Task~bool~
        +DeleteAsync(id Guid) Task~bool~
    }
    
    class EntityRepository {
        -DbContext _context
        +EntityRepository(DbContext context)
        +GetByIdAsync(id Guid) Task~Entity~
        +GetByStatusAsync(status EntityStatus) Task~List~Entity~~
        +SaveAsync(entity Entity) Task~bool~
        +DeleteAsync(id Guid) Task~bool~
        -MapToEntity(dbModel DbModel) Entity
        -MapToDbModel(entity Entity) DbModel
    }
    
    class DbContext {
        +DbSet~EntityDbModel~ Entities
        +SaveChangesAsync() Task~int~
        #OnModelCreating(ModelBuilder builder) void
    }
    
    IEntityRepository <|.. EntityRepository: implements
    EntityRepository --> DbContext: uses
```

## 🔄 Data Transfer Objects

### API Request/Response Models
*Define data contracts for external communication*

```mermaid
classDiagram
    class CreateEntityRequest {
        +Name: string
        +InitialStatus: string
        +Validate() ValidationResult
    }
    
    class EntityResponse {
        +Id: Guid
        +Name: string
        +Status: string
        +CreatedAt: DateTime
        +ItemCount: int
    }
    
    class UpdateEntityRequest {
        +Id: Guid
        +Name: string
        +Status: string
        +Validate() ValidationResult
    }
    
    class PagedResponse~T~ {
        +Items: List~T~
        +TotalCount: int
        +PageSize: int
        +CurrentPage: int
        +HasNextPage: bool
    }
    
    PagedResponse --> EntityResponse: contains
```

## 🗄️ Database Schema (ERD)

### Entity Relationships
*Database representation of domain model*

```mermaid
erDiagram
    ENTITY_A ||--o{ ENTITY_B : owns
    ENTITY_A {
        uuid id PK
        varchar name
        varchar status
        timestamp created_at
        timestamp updated_at
    }
    
    ENTITY_B ||--o{ ENTITY_C : contains
    ENTITY_B {
        uuid id PK
        varchar title
        uuid owner_id FK
        timestamp created_at
        timestamp updated_at
    }
    
    ENTITY_C {
        uuid id PK
        decimal value
        varchar type
        uuid parent_id FK
        timestamp created_at
        timestamp updated_at
    }
```

## 🔧 Integration Points

### External Service Interfaces
*Define contracts for external system integration*

```mermaid
classDiagram
    class IExternalService {
        <<interface>>
        +CallExternalAPIAsync(request ApiRequest) Task~ApiResponse~
        +HandleWebhookAsync(payload WebhookPayload) Task
    }
    
    class IEventPublisher {
        <<interface>>
        +PublishEventAsync(event DomainEvent) Task
        +SubscribeToEvents(handler EventHandler) void
    }
    
    class DomainEvent {
        +Id: Guid
        +EventType: string
        +Payload: object
        +Timestamp: DateTime
        +SourceId: Guid
    }
```

## 📋 Class Design Checklist

### Domain Model Validation
- [ ] All entities map to business concepts from **use cases**
- [ ] Relationships reflect real business rules
- [ ] Entities encapsulate behavior, not just data
- [ ] Value objects are used for complex business concepts

### Service Design Validation  
- [ ] Services coordinate **use cases** without business logic
- [ ] Interfaces define clear contracts
- [ ] Dependencies flow inward (toward domain)
- [ ] Services are stateless and focused

### Data Design Validation
- [ ] Repositories abstract persistence concerns
- [ ] DTOs provide clean API contracts
- [ ] Database design supports query patterns
- [ ] Foreign keys enforce referential integrity

**Template Instructions**:
1. Replace placeholder entities with actual business concepts from **use cases**
2. Ensure all classes support the identified **use cases**
3. Keep class responsibilities focused and clear
4. Define interfaces before implementations
5. Show relationships that matter for understanding system behavior

[<< Back](./../design.md)
