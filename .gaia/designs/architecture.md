# LifeOS System Architecture

## Overview

LifeOS is a Personal Life Operating System that tracks 8 life dimensions, simulates financial/behavioral futures, and presents everything in a cohesive dashboard. The system is designed around simulation-first principles, scientific feedback loops, and multi-currency financial modeling.

### Guiding Principles

1. **Single Brain, Many Dimensions** - 8 core dimensions: Health, Relationships, Work, Play, Asset Care, Create, Growth, Community
2. **Scientific Feedback** - Every metric grounded in rational models (risk reduction, longevity, behavioral science)
3. **Simulation-first Design** - Data powers projections ("What if I buy X?", "When will I hit $1M?")
4. **Manual/API-driven Data Ingestion** - Single metrics endpoint for automations (Shortcuts, n8n)
5. **Multi-currency & Multi-Account** - ZAR, USD, BTC, arbitrary currencies with FX rates
6. **Deterministic Projections** - User-specified growth/interest rates
7. **On-Demand, Scheduled, Webhook-triggered Simulations**
8. **GameFi & Streaks** - Financial behaviors have streaks and health scores

---

## Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION                              │
│  React + TypeScript + Redux Toolkit + TailwindCSS               │
│  (Dark, glassy, gradient UI - Firstbase One inspired)           │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                         API LAYER                                │
│  ASP.NET Core 8+ Web API                                        │
│  Controllers → MediatR Commands/Queries → Application Layer     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                            │
│  LifeOS.Application                                              │
│  ├── Commands (CQRS Write Operations)                           │
│  ├── Queries (CQRS Read Operations)                             │
│  ├── Services (Simulation Engine, Score Calculator)             │
│  ├── DTOs & Mappings                                            │
│  └── Interfaces (Ports for Infrastructure)                      │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                       DOMAIN LAYER                               │
│  LifeOS.Domain                                                   │
│  ├── Entities (User, Dimension, Account, Transaction, etc.)    │
│  ├── Value Objects (Money, Currency, DateRange, etc.)           │
│  ├── Domain Events                                               │
│  ├── Aggregates (FinancialPortfolio, HealthProfile)             │
│  └── Domain Services (LongevityEstimator, StreakCalculator)     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                           │
│  LifeOS.Infrastructure                                           │
│  ├── Persistence (EF Core + PostgreSQL)                         │
│  ├── External Services (FX Rates API - CoinGecko)               │
│  ├── Background Jobs (Hangfire)                                  │
│  ├── Caching (Redis)                                             │
│  └── Messaging (Domain Event Dispatcher)                        │
└─────────────────────────────────────────────────────────────────┘
```

---

## Component Architecture

### Backend Components

```
LifeOS.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── DimensionsController.cs
│   ├── MetricsController.cs          # Definitions + Records CRUD
│   ├── MetricRecordsController.cs    # Individual record operations
│   ├── AccountsController.cs
│   ├── TransactionsController.cs
│   ├── SimulationsController.cs
│   ├── HealthController.cs
│   └── DashboardController.cs
├── Middleware/
│   ├── ExceptionHandling.cs
│   ├── RequestLogging.cs
│   └── RateLimiting.cs
└── Filters/
    └── ValidationFilter.cs

LifeOS.Application/
├── Commands/
│   ├── Metrics/
│   │   ├── RecordMetricCommand.cs
│   │   ├── CreateMetricDefinitionCommand.cs
│   │   ├── UpdateMetricDefinitionCommand.cs
│   │   ├── DeleteMetricDefinitionCommand.cs
│   │   ├── UpdateMetricRecordCommand.cs
│   │   └── DeleteMetricRecordCommand.cs
│   ├── Transactions/CreateTransactionCommand.cs
│   ├── Simulations/RunSimulationCommand.cs
│   └── Accounts/CreateAccountCommand.cs
├── Queries/
│   ├── Dashboard/GetDashboardQuery.cs
│   ├── Metrics/
│   │   ├── GetMetricDefinitionsQuery.cs
│   │   ├── GetMetricDefinitionByCodeQuery.cs
│   │   ├── GetMetricRecordsQuery.cs
│   │   └── GetMetricRecordByIdQuery.cs
│   ├── Simulations/GetProjectionQuery.cs
│   └── Dimensions/GetDimensionScoresQuery.cs
├── Services/
│   ├── ISimulationEngine.cs
│   ├── IScoreCalculator.cs
│   ├── ILongevityEstimator.cs
│   ├── IStreakService.cs
│   └── IMetricDefinitionService.cs
└── Interfaces/
    ├── IFxRateProvider.cs
    ├── IMetricsRepository.cs
    ├── IMetricDefinitionsRepository.cs
    └── ISimulationRepository.cs

LifeOS.Domain/
├── Entities/
│   ├── User.cs
│   ├── Dimension.cs
│   ├── Milestone.cs
│   ├── Task.cs
│   ├── Habit.cs
│   ├── Metric.cs
│   ├── Score.cs
│   ├── Streak.cs
│   ├── Account.cs
│   ├── Transaction.cs
│   ├── FxRate.cs
│   ├── IncomeSource.cs
│   ├── ExpenseCategory.cs
│   ├── TaxProfile.cs
│   ├── FutureEvent.cs
│   ├── SimulationScenario.cs
│   └── HealthMetric.cs
├── ValueObjects/
│   ├── Money.cs
│   ├── Currency.cs
│   ├── DateRange.cs
│   ├── Percentage.cs
│   └── MetricValue.cs
├── Aggregates/
│   ├── FinancialPortfolio.cs
│   └── HealthProfile.cs
├── Events/
│   ├── TransactionRecorded.cs
│   ├── MetricUpdated.cs
│   ├── StreakBroken.cs
│   └── MilestoneReached.cs
└── Services/
    ├── LongevityEstimator.cs
    └── StreakCalculator.cs

LifeOS.Infrastructure/
├── Persistence/
│   ├── LifeOSDbContext.cs
│   ├── Configurations/             # EF Core entity configs
│   ├── Repositories/
│   └── Migrations/
├── ExternalServices/
│   ├── CoinGeckoFxRateProvider.cs
│   └── HttpClientFactory configs
├── BackgroundJobs/
│   ├── FxRateRefreshJob.cs
│   ├── ScheduledSimulationJob.cs
│   ├── ScoreRecomputationJob.cs
│   └── StreakEvaluationJob.cs
└── Caching/
    └── RedisCacheService.cs
```

### Frontend Components

```
frontend/src/
├── app/
│   ├── store.ts                    # Redux store configuration
│   └── hooks.ts                    # Typed Redux hooks
├── features/
│   ├── auth/
│   ├── dashboard/
│   │   ├── DashboardPage.tsx
│   │   ├── DimensionCard.tsx
│   │   ├── ScoreRadar.tsx
│   │   └── dashboardSlice.ts
│   ├── dimensions/
│   │   ├── DimensionList.tsx
│   │   └── DimensionDetail.tsx
│   ├── finance/
│   │   ├── AccountsPage.tsx
│   │   ├── TransactionsPage.tsx
│   │   ├── SimulationPage.tsx
│   │   └── ProjectionChart.tsx
│   ├── health/
│   │   ├── HealthMetrics.tsx
│   │   └── LongevityEstimate.tsx
│   ├── streaks/
│   │   └── StreakDisplay.tsx
│   └── metrics/
│       └── MetricInput.tsx
├── components/
│   ├── ui/                         # Glassy, gradient components
│   ├── charts/
│   └── layout/
├── services/
│   └── api.ts                      # RTK Query API definitions
└── styles/
    └── tailwind.config.js
```

---

## Data Flow Patterns

### Metrics Ingestion Flow

```
External Source (Shortcuts/n8n/Manual)
          │
          ▼
    POST /api/metrics
    {
      "dimension": "health",
      "type": "weight",
      "value": 75.5,
      "unit": "kg",
      "timestamp": "2024-01-15T08:00:00Z"
    }
          │
          ▼
┌─────────────────────────────────────┐
│     MetricsController               │
│     → RecordMetricCommand           │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│     Application Layer               │
│     • Validate metric               │
│     • Store in database             │
│     • Publish MetricUpdated event   │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│     Domain Event Handlers           │
│     • Update dimension score        │
│     • Evaluate streak continuity    │
│     • Trigger longevity recalc      │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│     WebSocket/SignalR               │
│     Push update to dashboard        │
└─────────────────────────────────────┘
```

### Financial Simulation Flow

```
User Request (On-demand/Scheduled/Webhook)
          │
          ▼
    POST /api/simulations/run
    {
      "scenarioId": "uuid",
      "parameters": {
        "projectionMonths": 60,
        "growthRates": {...},
        "futureEvents": [...]
      }
    }
          │
          ▼
┌─────────────────────────────────────┐
│     SimulationEngine                │
│     1. Load current portfolio       │
│     2. Apply FX rates               │
│     3. Project month-by-month       │
│     4. Apply future events          │
│     5. Calculate milestones         │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│     Simulation Result               │
│     • Monthly projections           │
│     • Milestone dates               │
│     • Net worth trajectory          │
│     • Currency breakdown            │
└─────────────────────────────────────┘
```

### Score Calculation Flow

```
Metric Update / Scheduled Job
          │
          ▼
┌─────────────────────────────────────┐
│     ScoreCalculator                 │
│     For each dimension:             │
│     1. Gather relevant metrics      │
│     2. Apply dimension weights      │
│     3. Calculate scientific score   │
│     4. Factor in streak bonuses     │
└─────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────┐
│     Aggregate Life Score            │
│     • Weighted average of dims      │
│     • Historical trend              │
│     • Improvement velocity          │
└─────────────────────────────────────┘
```

---

## Background Job Architecture

### Hangfire Job Configuration

```csharp
// Job Schedule Configuration
services.AddHangfire(config => 
    config.UsePostgreSqlStorage(connectionString));

// Scheduled Jobs
RecurringJob.AddOrUpdate<FxRateRefreshJob>(
    "fx-rate-refresh",
    job => job.Execute(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<ScoreRecomputationJob>(
    "score-recomputation", 
    job => job.Execute(),
    Cron.Daily(hour: 3));  // 3 AM

RecurringJob.AddOrUpdate<StreakEvaluationJob>(
    "streak-evaluation",
    job => job.Execute(),
    Cron.Daily(hour: 0));  // Midnight

RecurringJob.AddOrUpdate<ScheduledSimulationJob>(
    "scheduled-simulations",
    job => job.Execute(),
    Cron.Daily(hour: 4));  // 4 AM
```

### Job Responsibilities

| Job | Trigger | Purpose |
|-----|---------|---------|
| FxRateRefreshJob | Hourly | Fetch latest rates from CoinGecko for all tracked currencies |
| ScoreRecomputationJob | Daily 3AM | Recalculate all dimension scores with latest metrics |
| StreakEvaluationJob | Daily Midnight | Check streak continuity, mark broken streaks |
| ScheduledSimulationJob | Daily 4AM | Run any user-scheduled projection simulations |
| WebhookSimulationJob | On-demand | Triggered by external webhooks for immediate simulation |

---

## Simulation Engine Design

### Core Simulation Components

```csharp
public interface ISimulationEngine
{
    Task<SimulationResult> RunProjection(SimulationRequest request);
    Task<SimulationResult> RunWhatIf(WhatIfScenario scenario);
    Task<MilestoneProjection> ProjectMilestone(MilestoneTarget target);
}

public class SimulationEngine : ISimulationEngine
{
    private readonly IAccountRepository _accounts;
    private readonly IFxRateProvider _fxRates;
    private readonly IFutureEventRepository _events;
    
    public async Task<SimulationResult> RunProjection(SimulationRequest request)
    {
        // 1. Snapshot current state
        var portfolio = await _accounts.GetPortfolioSnapshot(request.UserId);
        
        // 2. Convert all to base currency
        var fxRates = await _fxRates.GetCurrentRates(request.BaseCurrency);
        portfolio.NormalizeToBase(fxRates);
        
        // 3. Project forward month-by-month
        var projections = new List<MonthlyProjection>();
        var currentState = portfolio.Clone();
        
        for (int month = 1; month <= request.ProjectionMonths; month++)
        {
            // Apply growth rates
            currentState.ApplyGrowthRates(request.GrowthRates);
            
            // Apply recurring income/expenses
            currentState.ApplyRecurring(month);
            
            // Apply future events (one-time purchases, windfalls)
            var events = request.FutureEvents.Where(e => e.Month == month);
            currentState.ApplyEvents(events);
            
            projections.Add(new MonthlyProjection(month, currentState.Clone()));
        }
        
        // 4. Calculate milestone dates
        var milestones = CalculateMilestones(projections, request.Milestones);
        
        return new SimulationResult(projections, milestones);
    }
}
```

### "What If" Scenario Types

```csharp
public enum WhatIfType
{
    Purchase,           // "What if I buy a car for $30K?"
    SalaryChange,       // "What if I get a 20% raise?"
    InvestmentReturn,   // "What if markets return 12% instead of 7%?"
    MajorExpense,       // "What if I have a medical emergency?"
    IncomeStop,         // "What if I lose my job for 6 months?"
    CurrencyShift       // "What if ZAR devalues 30%?"
}
```

### Deterministic Projection Parameters

```csharp
public class GrowthRates
{
    public decimal DefaultAnnualReturn { get; set; }     // e.g., 7%
    public Dictionary<string, decimal> AccountRates { get; set; }  // Per-account overrides
    public Dictionary<Currency, decimal> CurrencyRates { get; set; }  // Per-currency
    public decimal InflationRate { get; set; }           // For real returns
}
```

---

## Technology Stack Decisions

### Backend Stack

| Component | Technology | Rationale |
|-----------|------------|-----------|
| Framework | .NET 8+ | Long-term support, performance, Clean Architecture support |
| Web API | ASP.NET Core | Mature, well-documented, excellent tooling |
| ORM | Entity Framework Core | Code-first migrations, LINQ support, PostgreSQL provider |
| Database | PostgreSQL 15+ | ACID compliance, JSON support, time-series extensions |
| Caching | Redis | Fast session/cache layer, pub/sub for real-time |
| Background Jobs | Hangfire | Persistent jobs, dashboard, PostgreSQL storage |
| CQRS | MediatR | Clean separation of commands/queries |
| Mapping | AutoMapper | DTO ↔ Entity mapping |
| Validation | FluentValidation | Expressive validation rules |
| API Docs | Swagger/OpenAPI | Auto-generated API documentation |

### Frontend Stack

| Component | Technology | Rationale |
|-----------|------------|-----------|
| Build Tool | Vite | Fast HMR, modern bundling |
| Framework | React 18+ | Component-based, large ecosystem |
| Language | TypeScript 5+ | Type safety, better DX |
| State | Redux Toolkit + RTK Query | Predictable state, built-in caching/fetching |
| Styling | TailwindCSS | Utility-first, custom theming for glassy UI |
| Charts | Recharts or Victory | Financial projections, radar charts |
| Forms | React Hook Form | Performant form handling |
| Routing | React Router v6 | Standard routing solution |

### External Services

| Service | Provider | Purpose |
|---------|----------|---------|
| FX Rates | CoinGecko API | Fiat and crypto exchange rates |
| Auth | JWT (self-hosted) | Token-based authentication |

---

## Key Architectural Decisions

### Decision 1: Simulation-First Architecture

**Choice**: All financial data supports forward projections as primary use case
**Rationale**: Core differentiator is "what if" scenarios; optimized for deterministic simulations
**Trade-offs**: More complex data model, but enables unique product value

### Decision 2: Single Metrics Endpoint

**Choice**: Universal `/api/metrics` endpoint for all data ingestion
**Rationale**: Simplifies automation integrations (Shortcuts, n8n, Zapier)
**Trade-offs**: Generic endpoint requires rich type discrimination

### Decision 3: Deterministic over Probabilistic Projections

**Choice**: User-specified growth rates, not Monte Carlo simulations
**Rationale**: Transparency, reproducibility, simpler mental model
**Trade-offs**: Less "realistic" but more predictable and understandable

### Decision 4: Multi-Currency First-Class Support

**Choice**: Every monetary value is Money(amount, currency) pair
**Rationale**: Users manage ZAR, USD, BTC, EUR simultaneously
**Trade-offs**: FX conversion complexity in all calculations

### Decision 5: Background Job-Driven Score Updates

**Choice**: Scores recomputed via scheduled jobs, not real-time
**Rationale**: Reduce computational load, batch processing efficiency
**Trade-offs**: Small delay in score updates (acceptable for daily metrics)

---

## Scalability Considerations

### Horizontal Scaling Strategy

```
                    ┌─────────────┐
                    │ Load        │
                    │ Balancer    │
                    └──────┬──────┘
                           │
         ┌─────────────────┼─────────────────┐
         ▼                 ▼                 ▼
┌─────────────┐   ┌─────────────┐   ┌─────────────┐
│  API Pod 1  │   │  API Pod 2  │   │  API Pod 3  │
└─────────────┘   └─────────────┘   └─────────────┘
         │                 │                 │
         └─────────────────┼─────────────────┘
                           ▼
                  ┌─────────────┐
                  │ PostgreSQL  │
                  │ (Primary)   │
                  └─────────────┘
                           │
                  ┌────────┴────────┐
                  ▼                 ▼
          ┌─────────────┐   ┌─────────────┐
          │ Read        │   │ Redis       │
          │ Replica     │   │ Cache       │
          └─────────────┘   └─────────────┘
```

### Performance Optimizations

- **Stateless API**: JWT tokens, no server session state
- **Database Indexing**: On user_id, dimension, timestamp columns
- **Query Optimization**: EF Core compiled queries for hot paths
- **Caching Strategy**: Redis for FX rates, user preferences
- **Lazy Loading**: Load dimension details on demand
- **Code Splitting**: Frontend route-based lazy loading

---

## Development Workflow

1. **Local Development**: Docker Compose for PostgreSQL, Redis
2. **Feature Branches**: PR-based reviews
3. **Testing Pipeline**: Unit → Integration → E2E (Playwright)
4. **Staging Deployment**: Automated on merge to develop
5. **Production Deployment**: Manual promotion with quality gates

---

## Security Architecture

See `security.md` for detailed security design including:
- JWT authentication with refresh tokens
- Role-based access control
- API rate limiting
- Data encryption at rest and in transit
- CORS configuration

---

## API Architecture

See `api.md` for detailed API design including:
- RESTful endpoint specifications
- Request/response schemas
- Error handling conventions
- Webhook specifications

---

## Frontend Architecture

See `frontend.md` for detailed frontend design including:
- Component architecture (atoms, molecules, organisms)
- State management patterns (Redux Toolkit, RTK Query)
- Visual design guidelines (dark glass theme)
- Chart components (MetricSparkline, TimeScaleSlider)
- Responsive breakpoints and accessibility

---

## Database Architecture

See `database.md` for detailed data model including:
- Entity relationship diagrams
- Table schemas
- Index strategies
- Migration approach