<!-- reference @.docs/design.md -->
<!-- reference @.docs/1-use-cases.md -->
<!-- reference @.docs/2-class.md -->

[<< Back](./../design.md)

# Sequence Diagrams

Below we elaborate on the system's sequence diagrams that show how classes collaborate to fulfill **use cases**.

Ensure we follow our **design principles** and that this document naturally extends the **use cases** and **class diagrams**.

## 📋 Template Guidance

### Purpose & Scope
This document shows **how** the system executes use cases by mapping the **flow of interactions** between classes over time. It bridges static class structure to dynamic behavior.

**🎯 Focus**: Use case execution flows, object collaboration, interaction patterns
**🚫 Avoid**: Implementation details, infrastructure concerns, deployment specifics

### Sequence Diagram Guidelines
- **Use case driven**: Each major diagram should map to a specific use case
- **Appropriate abstraction**: Show key interactions, hide internal details
- **Clear participants**: Use meaningful names from class diagrams
- **Error scenarios**: Include alternative flows for key failure modes

---

## 🎯 Primary Use Case Flows

### UC-001: [Use Case Name from use cases document]
*Map this directly to a use case from your use cases document*

```mermaid
sequenceDiagram
    autonumber
    participant User as User/Client
    participant System as System Interface
    participant Service as [BusinessService]
    participant Repository as [DataRepository]
    participant External as [ExternalSystem]

    User->>System: [Initial Request/Action]
    Note over User,System: [Context of the request]

    System->>System: [Validate Input]
    
    alt Input Invalid
        System-->>User: [Error Response]
    else Input Valid
        System->>Service: [Process Business Logic]
        
        Service->>Repository: [Retrieve Required Data]
        Repository-->>Service: [Return Data]
        
        Service->>Service: [Apply Business Rules]
        
        opt External Integration Required
            Service->>External: [Call External Service]
            External-->>Service: [Return Response]
        end
        
        Service->>Repository: [Save Results]
        Repository-->>Service: [Confirm Save]
        
        Service-->>System: [Return Success]
        System-->>User: [Success Response]
    end
```

### UC-002: [Another Use Case Name]
*Choose your most critical or complex use case*

```mermaid
sequenceDiagram
    autonumber
    participant Actor as [Primary Actor]
    participant Interface as [System Interface]
    participant ServiceA as [Service A]
    participant ServiceB as [Service B]
    participant Storage as [Data Storage]

    Actor->>Interface: [Trigger Action]
    Interface->>ServiceA: [Initial Processing]
    
    ServiceA->>Storage: [Load Context Data]
    Storage-->>ServiceA: [Return Context]
    
    ServiceA->>ServiceB: [Delegate Specialized Logic]
    ServiceB->>ServiceB: [Complex Business Processing]
    ServiceB-->>ServiceA: [Return Processed Result]
    
    ServiceA->>Storage: [Persist Changes]
    Storage-->>ServiceA: [Confirm Persistence]
    
    ServiceA-->>Interface: [Return Final Result]
    Interface-->>Actor: [Present Outcome]
```

## 🔄 Alternative and Error Flows

### Error Handling Pattern
*Standard pattern for handling errors across use cases*

```mermaid
sequenceDiagram
    autonumber
    participant User as User
    participant System as System
    participant Service as Business Service
    participant Data as Data Layer

    User->>System: [Request Action]
    System->>Service: [Process Request]
    
    Service->>Data: [Access Data]
    
    alt Data Access Successful
        Data-->>Service: [Return Data]
        Service->>Service: [Process Successfully]
        Service-->>System: [Return Success]
        System-->>User: [Success Response]
    else Data Access Failed
        Data--xService: [Data Error]
        Service->>Service: [Log Error & Determine Recovery]
        
        alt Recoverable Error
            Service->>Data: [Retry Operation]
            Data-->>Service: [Success on Retry]
            Service-->>System: [Return Success]
            System-->>User: [Success Response]
        else Non-Recoverable Error
            Service-->>System: [Return Business Error]
            System-->>User: [User-Friendly Error Message]
        end
    end
```

### Multi-Actor Collaboration
*When use cases involve multiple actors or systems*

```mermaid
sequenceDiagram
    autonumber
    participant UserA as [Primary User]
    participant UserB as [Secondary User]
    participant System as [System]
    participant NotificationService as [Notification Service]
    participant AuditService as [Audit Service]

    UserA->>System: [Initiate Collaborative Action]
    System->>AuditService: [Log Action Start]
    
    System->>NotificationService: [Notify UserB]
    NotificationService->>UserB: [Send Notification]
    
    UserB->>System: [Respond to Notification]
    System->>System: [Process Combined Input]
    
    System->>NotificationService: [Notify Both Users]
    NotificationService->>UserA: [Send Update]
    NotificationService->>UserB: [Send Update]
    
    System->>AuditService: [Log Completion]
```

## 🔌 Integration Patterns

### External Service Integration
*Pattern for integrating with external systems*

```mermaid
sequenceDiagram
    autonumber
    participant User as User
    participant System as System
    participant IntegrationService as Integration Service
    participant ExternalAPI as External API
    participant FallbackData as Fallback Data

    User->>System: [Request Requiring External Data]
    System->>IntegrationService: [Request External Integration]
    
    IntegrationService->>ExternalAPI: [API Call]
    
    alt External API Available
        ExternalAPI-->>IntegrationService: [Return Data]
        IntegrationService-->>System: [Return Processed Data]
        System-->>User: [Complete Response]
    else External API Unavailable
        ExternalAPI--xIntegrationService: [Service Unavailable]
        IntegrationService->>FallbackData: [Use Cached/Default Data]
        FallbackData-->>IntegrationService: [Return Fallback Data]
        IntegrationService-->>System: [Return Limited Response]
        System-->>User: [Response with Degraded Functionality]
    end
```

## 📊 Complex Business Process Flows

### Multi-Step Business Process
*For use cases that involve multiple sequential steps*

```mermaid
sequenceDiagram
    autonumber
    participant User as User
    participant Orchestrator as Process Orchestrator
    participant StepA as Step A Service
    participant StepB as Step B Service
    participant StepC as Step C Service
    participant Repository as Data Repository

    User->>Orchestrator: [Start Multi-Step Process]
    
    Orchestrator->>Repository: [Create Process Record]
    Repository-->>Orchestrator: [Confirm Creation]
    
    Orchestrator->>StepA: [Execute Step A]
    StepA->>StepA: [Process Step A Logic]
    StepA-->>Orchestrator: [Step A Complete]
    
    Orchestrator->>Repository: [Update Process Status]
    
    Orchestrator->>StepB: [Execute Step B]
    StepB->>StepB: [Process Step B Logic]
    
    alt Step B Fails
        StepB--xOrchestrator: [Step B Failed]
        Orchestrator->>StepA: [Compensate Step A]
        StepA-->>Orchestrator: [Compensation Complete]
        Orchestrator-->>User: [Process Failed - Compensated]
    else Step B Succeeds
        StepB-->>Orchestrator: [Step B Complete]
        
        Orchestrator->>StepC: [Execute Final Step]
        StepC->>StepC: [Process Final Logic]
        StepC-->>Orchestrator: [Final Step Complete]
        
        Orchestrator->>Repository: [Mark Process Complete]
        Orchestrator-->>User: [Process Successfully Completed]
    end
```

## 📋 Use Case to Sequence Mapping

### Mapping Guidelines
**For each major use case, create a sequence diagram that shows:**

1. **Primary Actor Interaction**: How the use case is triggered
2. **System Response Flow**: How the system processes the request
3. **Key Decision Points**: Where business rules are applied
4. **Data Interactions**: When and how data is accessed/modified
5. **Success/Failure Outcomes**: How different scenarios conclude

### Sequence Complexity Guidelines

**Simple Use Cases** (3-5 steps):
- Single sequence diagram showing happy path
- Brief alternative flow for validation errors

**Complex Use Cases** (6+ steps or multiple actors):
- Primary sequence diagram for happy path
- Separate diagrams for key alternative flows
- Error handling pattern diagram

**Integration-Heavy Use Cases**:
- Focus on external system interactions
- Show fallback/retry patterns
- Include timeout and failure scenarios

---

**Template Instructions**:
1. **Replace all placeholders** with actual actors, services, and systems from your class diagrams
2. **Map to specific use cases** - each diagram should fulfill an identified use case
3. **Focus on business value** - show how user goals are achieved through system interactions
4. **Include realistic error scenarios** - show how the system handles common failure modes
5. **Keep appropriate detail level** - enough to understand the flow, not implementation specifics

**Related Documents**:
- Business flows → [1-use-cases.md](./1-use-cases.md)
- Class responsibilities → [2-class.md](./2-class.md)
- Technical architecture → [3-architecture.md](./3-architecture.md)

[<< Back](./../design.md)
