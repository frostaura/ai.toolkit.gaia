<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/1-use-cases.md -->
<!-- reference @.gaia/designs/2-class.md -->

[<< Back](./design.md)

# Sequence Diagrams

Use case execution flows showing class collaboration over time.

## Template Guidance

**Purpose**: Show how system executes use cases through object interactions
**Focus**: Use case flows, object collaboration, interaction patterns  
**Avoid**: Implementation details, infrastructure concerns

**Guidelines**: Use case driven, appropriate abstraction, include error scenarios

## Primary Use Case Flows

### UC-001: [Use Case Name]

```mermaid
sequenceDiagram
    autonumber
    participant User
    participant System
    participant Service
    participant Repository

    User->>System: [Initial Request]
    System->>System: Validate Input
    
    alt Input Invalid
        System-->>User: Error Response
    else Input Valid
        System->>Service: Process Request
        Service->>Repository: Retrieve Data
        Repository-->>Service: Return Data
        Service->>Service: Apply Business Rules
        Service-->>System: Return Result
        System-->>User: Success Response
    end
```

### UC-002: [Another Use Case]

```mermaid
sequenceDiagram
    autonumber
    participant Actor
    participant Interface
    participant ServiceA
    participant ServiceB
    participant Storage

    Actor->>Interface: Trigger Action
    Interface->>ServiceA: Process
    ServiceA->>ServiceB: Delegate Logic
    ServiceB-->>ServiceA: Return Result
    ServiceA->>Storage: Persist Changes
    ServiceA-->>Interface: Return Result
    Interface-->>Actor: Present Outcome
```

## Error Handling & Integration Patterns

### Standard Error Pattern
```mermaid
sequenceDiagram
    autonumber
    participant User
    participant System
    participant Service
    participant Data

    User->>System: Request Action
    System->>Service: Process Request
    Service->>Data: Access Data
    
    alt Success
        Data-->>Service: Return Data
        Service-->>System: Success
        System-->>User: Success Response
    else Error
        Data--xService: Data Error
        Service-->>System: Business Error
        System-->>User: User-Friendly Error
    end
```

### External Service Integration
```mermaid
sequenceDiagram
    autonumber
    participant User
    participant System
    participant IntegrationService
    participant ExternalAPI

    User->>System: Request External Data
    System->>IntegrationService: Request Integration
    IntegrationService->>ExternalAPI: API Call
    
    alt API Available
        ExternalAPI-->>IntegrationService: Return Data
        IntegrationService-->>System: Processed Data
        System-->>User: Complete Response
    else API Unavailable
        ExternalAPI--xIntegrationService: Service Unavailable
        IntegrationService-->>System: Fallback Data
        System-->>User: Limited Response
    end
```

## Complex Business Process

### Multi-Step Process Flow
```mermaid
sequenceDiagram
    autonumber
    participant User
    participant Orchestrator
    participant StepA
    participant StepB
    participant Repository

    User->>Orchestrator: Start Process
    Orchestrator->>Repository: Create Process Record
    Orchestrator->>StepA: Execute Step A
    StepA-->>Orchestrator: Step A Complete
    
    Orchestrator->>StepB: Execute Step B
    
    alt Step B Success
        StepB-->>Orchestrator: Step B Complete
        Orchestrator-->>User: Process Complete
    else Step B Fails
        StepB--xOrchestrator: Step B Failed
        Orchestrator->>StepA: Compensate Step A
        Orchestrator-->>User: Process Failed
    end
```

## Mapping Guidelines

**Simple Use Cases**: Single sequence with happy path + validation errors
**Complex Use Cases**: Primary sequence + separate alternative flows
**Integration-Heavy**: Focus on external interactions + fallback patterns

**Instructions**: 
1. Replace placeholders with actual actors/services from class diagrams
2. Map each diagram to specific use cases
3. Show business value achievement through interactions
4. Include realistic error scenarios

[<< Back](./design.md)
