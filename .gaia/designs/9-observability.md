````markdown
<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/7-infrastructure.md -->

[<< Back](./design.md)

# Observability & Monitoring

Comprehensive monitoring, logging, tracing, and alerting for production systems.

## Template Guidance

**Purpose**: Define monitoring strategy, logging architecture, distributed tracing, and incident response
**Focus**: Observability pillars (logs, metrics, traces), alerting, dashboards, runbooks
**Avoid**: Implementation code, specific tool configurations, deployment details

**Guidelines**: Three pillars of observability, proactive monitoring, actionable alerts, SLO-driven

## Observability Overview

**Observability Pillars**:
- **Logs**: Discrete events with context (what happened)
- **Metrics**: Numerical measurements over time (how much/many)
- **Traces**: Request flows through distributed systems (journey mapping)

**Monitoring Stack**: [Prometheus + Grafana / ELK / Datadog / New Relic / Azure Monitor]

**Observability Principles**:
- **High Cardinality**: Support rich dimensional data
- **Unified Context**: Correlate logs, metrics, and traces
- **Real-Time**: Near-instant visibility into system state
- **Actionable**: Every alert must be actionable with clear remediation
- **Cost-Aware**: Balance observability needs with infrastructure costs

## Logging Architecture

### Structured Logging

**Log Format**: JSON structured logs for machine readability (Winston)

**Standard Log Structure**:
```json
{
  "timestamp": "2025-11-22T19:00:00.000Z",
  "level": "info",
  "message": "Tower placed successfully",
  "service": "tower-defense-backend",
  "environment": "production",
  "context": {
    "userId": "user-123",
    "gameSessionId": "session-456",
    "towerId": "tower-789",
    "towerType": "Arrow",
    "position": {"x": 10, "y": 15},
    "gold": 0,
    "requestId": "req-abc123"
  }
}
```

**Log Levels**:
- **error**: Error events (pathfinding failures, WebSocket disconnections, database errors)
- **warn**: Warning messages (rate limit approaching, slow pathfinding >80ms)
- **info**: General informational messages (tower placed, wave started, player joined) **[Default Production]**
- **debug**: Detailed debugging (enemy positions, state changes) **[Development Only]**

**Game-Specific Log Events**:
- `tower:placed` - Tower placement with position, type, owner
- `pathfinding:recalculated` - Pathfinding duration, affected enemies
- `wave:started` - Wave number, enemy count
- `player:connected` - User ID, session ID, connection timestamp
- `player:disconnected` - User ID, disconnect reason, reconnect timeout
- `auth:login_success` - User ID, IP address (redacted email)
- `auth:login_failed` - Email (redacted), IP address, reason

### Log Categories

**Application Logs**:
- API request/response (method, path, status, duration)
- Business logic events (order created, payment processed)
- Authentication/authorization attempts
- External service calls (success, failure, latency)

**Security Logs**:
- Authentication attempts (success/failure)
- Authorization checks (granted/denied)
- Data access (PII, sensitive data)
- Security events (suspicious activity, rate limit exceeded)

**Audit Logs**:
- Data mutations (create, update, delete)
- Configuration changes
- User actions requiring accountability
- Administrative operations

**Performance Logs**:
- Query execution times
- External API call latencies
- Resource utilization
- Cache hit/miss rates

### Log Aggregation & Storage

**Log Pipeline**:
```mermaid
graph LR
    A[Application] -->|JSON logs| B[Log Shipper]
    B -->|Forward| C[Log Aggregator]
    C -->|Index| D[Log Storage]
    C -->|Stream| E[Real-Time Alerts]
    D -->|Query| F[Log Search UI]
```

**Log Storage Strategy**:
- **Hot Storage**: Last 7 days in Elasticsearch/CloudWatch (fast search)
- **Warm Storage**: 8-30 days in cheaper storage tier
- **Cold Storage**: 31-365 days in S3/Blob Storage (archive)
- **Retention**: Purge logs older than 1 year (or per compliance)

**Log Sampling** (High-Volume Services):
- **INFO logs**: 10% sample rate for high-traffic endpoints
- **WARN/ERROR logs**: 100% retention (never sample errors)
- **Trace logs**: Enabled only for debugging with targeted sampling

### Log Security & Privacy

**PII Redaction**:
```json
{
  "message": "User login successful",
  "userId": "user-123",
  "email": "[REDACTED]",
  "ipAddress": "192.168.x.x"
}
```

**Secret Masking**:
- Never log passwords, tokens, API keys
- Mask credit card numbers (show last 4 digits only)
- Redact sensitive fields automatically

## Metrics & Time-Series Data

### Metric Categories

**RED Metrics** (Request-focused services):
- **Rate**: Requests per second
- **Errors**: Error rate (% of failed requests)
- **Duration**: Response time distribution (p50, p95, p99)

**USE Metrics** (Resource-focused systems):
- **Utilization**: % of resource capacity used (CPU, memory, disk)
- **Saturation**: Work queued but not yet processed
- **Errors**: Error count and rate

**Business Metrics**:
- **Orders per minute**: Business transaction rate
- **Revenue per hour**: Business value generation
- **Active users**: Current system utilization
- **Conversion rate**: Business outcome success

### Key Performance Indicators (KPIs)

**Game Performance Metrics** (CRITICAL):
```
# Pathfinding performance (REQUIREMENT: <100ms)
pathfinding_duration_ms{grid_size="50x50"} histogram
  - P50, P95, P99 latencies
  - Alert if P95 > 100ms

# Game loop FPS (REQUIREMENT: 60fps)
game_loop_fps{session_id} gauge
  - Real-time frame rate per game session
  - Alert if fps < 50 for 1 minute

# WebSocket latency (REQUIREMENT: <100ms)
websocket_latency_ms{event_type="tower:place|enemy:spawn|state:delta"} histogram
  - Round-trip time by event type
  - Alert if P95 > 100ms

# Active game metrics
active_players gauge - Current connected players across all sessions
active_game_sessions gauge - Current running game sessions
```

**Application Metrics**:
```
# HTTP REST API metrics
http_requests_total{method="POST", endpoint="/v1/auth/login", status="200"}
http_request_duration_seconds{method="POST", endpoint="/v1/tribes"}

# Tower placement metrics
tower_placement_count{tower_type="Arrow|Cannon|Magic|Wall"} counter
tower_placement_failed_count{reason="insufficient_gold|tile_occupied"} counter

# Enemy metrics
enemy_spawn_count{enemy_type="Basic|Fast|Heavy"} counter
enemy_killed_count{killed_by="tower|timeout"} counter
enemy_goal_reached_count counter

# Pathfinding metrics
pathfinding_recalculation_count counter
pathfinding_failure_count counter - No valid path (boxed in)
```

**Database Metrics**:
```
db_query_duration_seconds{operation="SELECT|INSERT|UPDATE", table="maps|tribes|users"}
db_connection_pool_size{state="active|idle"}
db_slow_queries_count{threshold=">100ms"} counter
```

**WebSocket Metrics**:
```
websocket_connections_total counter
websocket_active_connections gauge
websocket_messages_sent_total{event_type} counter
websocket_messages_received_total{event_type} counter
websocket_disconnections_total{reason="timeout|error|user_action"} counter
```

**Business Metrics**:
```
# User engagement
user_registrations_total counter
maps_created_total counter
tribes_created_total counter
games_played_total counter
waves_completed_total counter
```

### Metric Collection

**Instrumentation Approach**:
- **Application Instrumentation**: Custom metrics in application code
- **Sidecar Agents**: Collect infrastructure metrics alongside containers
- **Service Mesh**: Automatic metric collection for service-to-service calls
- **Synthetic Monitoring**: Proactive health checks and availability testing

**Collection Frequency**:
- **High-frequency**: 10s intervals for critical metrics
- **Standard**: 60s intervals for most metrics
- **Low-frequency**: 5min intervals for less critical metrics

## Distributed Tracing

### Trace Architecture

**Trace Context Propagation**:
```mermaid
sequenceDiagram
    participant Client
    participant Gateway
    participant API
    participant Database

    Client->>Gateway: Request (TraceID: abc123)
    Gateway->>API: Request (TraceID: abc123, SpanID: span1)
    API->>Database: Query (TraceID: abc123, SpanID: span2)
    Database-->>API: Response (105ms)
    API-->>Gateway: Response (145ms)
    Gateway-->>Client: Response (160ms)
```

**Trace Components**:
- **Trace ID**: Unique identifier for entire request flow
- **Span ID**: Unique identifier for each operation within trace
- **Parent Span ID**: Links spans into hierarchical structure
- **Timing**: Start time and duration of each span
- **Tags**: Key-value metadata (http.method, db.statement)
- **Logs**: Events that occurred within span

### Span Instrumentation

**Auto-Instrumentation**:
- HTTP clients and servers
- Database queries
- Message queue operations
- External service calls

**Manual Instrumentation**:
```javascript
// Example: Custom span for business logic
const span = tracer.startSpan('process_order', {
  childOf: parentSpan,
  tags: {
    'order.id': orderId,
    'order.amount': amount,
    'user.id': userId
  }
});

try {
  const result = await processOrderLogic(orderId);
  span.setTag('order.status', result.status);
  return result;
} catch (error) {
  span.setTag('error', true);
  span.log({ event: 'error', message: error.message });
  throw error;
} finally {
  span.finish();
}
```

**Sampling Strategy**:
- **Always Sample**: Error traces (100%)
- **High Priority**: Authentication, payment, checkout (100%)
- **Standard Traffic**: 10% sample rate
- **Health Checks**: 1% sample rate

## Alerting Strategy

### Alert Design Principles

**Actionable Alerts**:
- Every alert must have clear remediation steps
- Include context (runbook link, recent changes)
- Avoid alert fatigue (tune thresholds carefully)
- Route to appropriate on-call team

**Alert Severity Levels**:
- **Critical (P0)**: Immediate response required, customer impact
- **High (P1)**: Urgent, potential customer impact within 1 hour
- **Medium (P2)**: Important, address within business hours
- **Low (P3)**: Informational, no immediate action required

### Alert Rules

**Availability Alerts**:
```
# Service down
CRITICAL: http_up{job="api-service"} == 0
Duration: 2 minutes
Action: Immediate investigation, page on-call

# High error rate
HIGH: rate(http_requests_total{status=~"5.."}[5m]) > 0.05
Duration: 5 minutes
Action: Investigate error logs, check dependencies
```

**Performance Alerts**:
```
# API latency degradation
HIGH: histogram_quantile(0.95, http_request_duration_seconds) > 1.0
Duration: 10 minutes
Action: Check database performance, external services

# Database slow queries
MEDIUM: db_query_duration_seconds > 5.0
Duration: 15 minutes
Action: Review slow query log, optimize queries
```

**Resource Alerts**:
```
# High CPU usage
HIGH: cpu_usage_percent > 80
Duration: 15 minutes
Action: Scale horizontally or investigate CPU-intensive processes

# High memory usage
HIGH: memory_usage_percent > 90
Duration: 10 minutes
Action: Check for memory leaks, scale resources

# Disk space low
CRITICAL: disk_usage_percent > 90
Duration: 5 minutes
Action: Clean up logs, expand storage, investigate growth
```

**Business Metric Alerts**:
```
# Order processing stopped
CRITICAL: rate(orders_created_total[10m]) == 0
Duration: 10 minutes
Action: Check payment gateway, API availability

# Revenue drop
HIGH: rate(revenue_total_dollars[1h]) < threshold
Duration: 1 hour
Action: Check conversion funnel, investigate user experience issues
```

### Alert Routing

**Notification Channels**:
- **Critical/High**: PagerDuty/Opsgenie (immediate)
- **Medium**: Slack/Teams (business hours)
- **Low**: Email digest (daily summary)

**On-Call Schedule**:
- 24/7 on-call rotation for critical systems
- Escalation policy: Primary → Secondary → Manager
- Alert acknowledgment within 5 minutes (critical)

## Dashboards & Visualization

### Dashboard Categories

**System Health Dashboard**:
- Overall system status (green/yellow/red)
- Service uptime percentages
- Error rate trends
- Request rate and latency

**Application Performance Dashboard**:
- RED metrics for each service
- Dependency health (database, cache, external APIs)
- Top slow endpoints
- Error breakdown by type/endpoint

**Infrastructure Dashboard**:
- CPU, memory, disk, network utilization
- Container/pod counts and health
- Database connection pool metrics
- Load balancer metrics

**Business Metrics Dashboard**:
- Revenue and order trends
- User activity (registrations, logins, active users)
- Conversion funnel visualization
- Feature adoption rates

### Dashboard Best Practices

**Layout Principles**:
- **Top Section**: Critical health indicators (red/yellow/green)
- **Middle Section**: Key metrics and trends
- **Bottom Section**: Detailed breakdowns and drill-downs
- **Time Range**: Last 1 hour (default), with flexible time picker

**Visualization Types**:
- **Gauges**: Current state (CPU usage, error rate)
- **Line Charts**: Trends over time (request rate, latency)
- **Heatmaps**: Distribution (latency percentiles)
- **Tables**: Detailed breakdowns (top errors, slow queries)

## Service Level Objectives (SLOs)

### SLO Definitions

**Availability SLO**:
```
Target: 99.9% uptime (8.76 hours downtime/year)
Measurement: Successful requests / Total requests
Time Window: 30-day rolling window
```

**Latency SLO**:
```
Target: 95% of requests < 200ms
Measurement: p95 response time
Time Window: 7-day rolling window
```

**Error Rate SLO**:
```
Target: < 0.1% error rate
Measurement: Failed requests / Total requests
Time Window: 24-hour rolling window
```

### Error Budget

**Error Budget Calculation**:
```
Allowed downtime per month (99.9% SLO): 43.2 minutes
Error budget remaining: 43.2min - actual_downtime
Error budget consumed: (actual_downtime / 43.2min) * 100%
```

**Error Budget Policy**:
- **Budget > 50%**: Normal feature development velocity
- **Budget 20-50%**: Focus on reliability, defer risky features
- **Budget < 20%**: Feature freeze, all hands on reliability
- **Budget exhausted**: Incident post-mortem, reliability sprint

## Incident Management

### Incident Response Workflow

```mermaid
graph TD
    A[Alert Triggered] --> B[Acknowledge Alert]
    B --> C[Assess Severity]
    C --> D{Critical/High?}
    D -->|Yes| E[Page On-Call]
    D -->|No| F[Create Ticket]
    E --> G[Form Incident Team]
    G --> H[Investigate Root Cause]
    H --> I[Apply Mitigation]
    I --> J{Issue Resolved?}
    J -->|No| H
    J -->|Yes| K[Monitor Recovery]
    K --> L[Post-Incident Review]
    F --> M[Address in Sprint]
```

### Runbooks

**Runbook Structure**:
```markdown
# Runbook: API Service High Latency

## Symptoms
- p95 response time > 1 second
- Users reporting slow page loads
- Alert: "API_HIGH_LATENCY" fired

## Impact
- Degraded user experience
- Potential timeout errors
- Estimated affected users: [XX%]

## Investigation Steps
1. Check database performance dashboard
   - Query: SELECT * FROM slow_queries WHERE duration > 1s
2. Review application logs for errors
   - Grafana: [Link to relevant dashboard]
3. Check external service health
   - Payment gateway status page: [URL]

## Common Causes
- Database slow queries
- External service degradation
- Memory pressure causing GC pauses
- Network latency spikes

## Mitigation Steps
1. Scale application horizontally: `kubectl scale deployment api --replicas=10`
2. Restart stuck pods: `kubectl rollout restart deployment api`
3. Clear cache if stale: `redis-cli FLUSHDB`
4. Increase database connection pool: [Config change link]

## Rollback Procedure
1. Revert to previous version: `kubectl rollout undo deployment api`
2. Verify latency returns to normal
3. Create incident ticket for root cause analysis

## Post-Incident
- Document timeline in incident report
- Schedule blameless post-mortem within 48 hours
- Update runbook with lessons learned
```

### Post-Incident Reviews

**Post-Mortem Template**:
1. **Incident Summary**: What happened, when, impact
2. **Timeline**: Detailed chronology of events
3. **Root Cause**: Technical root cause analysis
4. **Contributing Factors**: What made this possible/worse
5. **Resolution**: How was it resolved
6. **Action Items**: Preventive measures (assigned owners, due dates)
7. **Lessons Learned**: What we learned, what we'll do differently

**Blameless Culture**:
- Focus on systemic improvements, not individual blame
- Psychological safety to discuss failures openly
- Celebrate learning from incidents

## Synthetic Monitoring

### Health Checks

**Endpoint Monitoring**:
```
# Liveness probe (is service running?)
GET /health/live
Expected: 200 OK, response < 1s

# Readiness probe (is service ready for traffic?)
GET /health/ready
Expected: 200 OK, checks database/cache connectivity

# Startup probe (has service completed initialization?)
GET /health/startup
Expected: 200 OK after initialization complete
```

**Multi-Region Monitoring**:
- Health checks from multiple geographic locations
- DNS resolution verification
- SSL certificate expiration monitoring
- Latency testing from user regions

### Synthetic Transactions

**Critical User Flows**:
```
# Order placement flow
1. Login with test user
2. Add product to cart
3. Proceed to checkout
4. Complete payment (test mode)
5. Verify order confirmation
Frequency: Every 5 minutes
Alert: If failure rate > 10% over 15 minutes
```

**API Contract Testing**:
- Validate API responses match schema
- Test authentication flows
- Verify rate limiting behavior
- Check error handling

## Validation Checklist

**Logging** (Winston + JSON):
- [x] Structured JSON logging (Winston library)
- [x] Log levels: error, warn, info (production), debug (development)
- [x] PII redaction (email masking: pla***@example.com)
- [x] Secret masking (never log passwords, JWT tokens)
- [x] Game-specific events: tower:placed, pathfinding:recalculated, player:connected
- [x] Request ID propagation through WebSocket events
- [x] Log rotation (daily, 7-day retention)

**Metrics** (Custom Application Metrics):
- [x] Pathfinding duration histogram (P50, P95, P99) - Alert if P95 > 100ms
- [x] Game loop FPS gauge (target: 60fps) - Alert if <50fps for 1min
- [x] WebSocket latency histogram (by event type) - Alert if P95 > 100ms
- [x] Active players gauge
- [x] Active game sessions gauge
- [x] Tower placement counter (by type)
- [x] Enemy spawn counter (by type)
- [x] Pathfinding recalculation counter

**Tracing** (Request ID Propagation):
- [x] Request ID generated on WebSocket connection
- [x] Propagated through all events (tower:place → pathfinding:recalculate → pathfinding:complete)
- [x] Logged in all structured logs for correlation
- [x] Enables debugging multiplayer issues across backend instances

**Alerting Rules** (Grafana or equivalent):
- [x] **CRITICAL**: Pathfinding duration P95 > 100ms for 5min → Page on-call
- [x] **CRITICAL**: Game loop FPS < 50 for 1min → Page on-call
- [x] **WARNING**: Error rate > 1% for 5min → Slack notification
- [x] **WARNING**: WebSocket disconnection spike (>10% players in 1min) → Slack notification
- [x] **INFO**: Backend instance restart → Slack notification

**Dashboards** (Grafana/CloudWatch):
- [x] **Real-time Performance**: Game loop FPS (line chart), WebSocket latency (histogram), active players (gauge)
- [x] **Pathfinding Performance**: Duration histogram, P95 latency, recalculation rate
- [x] **Multiplayer Health**: Connected players, active sessions, connection stability
- [x] **Error Monitoring**: Error rate, failed pathfinding attempts, WebSocket disconnections
- [x] **System Health**: CPU/memory usage, database connection pool, request rate

**Performance SLOs** (from 1-use-cases.md NFRs):
- [x] Pathfinding SLO: P95 < 100ms (NFR-002)
- [x] Game Loop SLO: 60fps (NFR-001)
- [x] Multiplayer Latency SLO: <100ms round-trip (NFR-003)
- [x] API Response SLO: <200ms P99 for REST endpoints

**Incident Management**:
- [x] Incident classification (P0-Critical, P1-High, P2-Medium, P3-Low)
- [x] Runbooks for common incidents (high pathfinding latency, WebSocket disconnections)
- [x] Post-incident review process (blameless post-mortems)
- [x] Security contact: security@towerdefense.com

**Instructions**: Complete observability design for isometric tower defense game. Winston structured logging with PII redaction, custom metrics (pathfinding duration, game loop FPS, WebSocket latency, active players), request ID tracing, Grafana dashboards, and alerting rules specified. Ready for builder implementation with monitoring integration.

[<< Back](./design.md)

[<< Back](./design.md)

````
