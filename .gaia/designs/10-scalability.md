````markdown
<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/7-infrastructure.md -->
<!-- reference @.gaia/designs/8-data.md -->

[<< Back](./design.md)

# Scalability & Performance

Complete scalability architecture and performance optimization strategies.

## Template Guidance

**Purpose**: Define scalability strategies, performance optimization, and capacity planning
**Focus**: Horizontal/vertical scaling, load balancing, caching, async processing, performance benchmarks
**Avoid**: Implementation code, specific tool configurations, premature optimization

**Guidelines**: Measure first, scale horizontally, design for distributed systems, optimize bottlenecks

## Scalability Overview

**Scalability Philosophy**: Horizontal-First (stateless backend instances)
**Target Scale**: 10,000 concurrent users, 100+ game sessions/instance, 1000 req/sec REST API
**Growth Rate**: Expected 3x growth in first 6 months post-launch

**Scalability Principles**:
- **Horizontal Scaling**: Multiple backend instances (3+ in production) behind load balancer
- **Stateless Services**: All state in PostgreSQL or Redis (no local state in backend instances)
- **Socket.io Redis Adapter**: Sticky sessions for WebSocket connections (route same session to same instance)
- **Auto-Scaling**: Horizontal pod autoscaling based on CPU >70% or active_players >500/instance
- **Caching**: Redis for game session state, tribe configurations, map metadata

## Horizontal Scaling

### Stateless Application Design

**Requirements for Horizontal Scaling**:
- **No Local State**: Session data in distributed cache (Redis)
- **Shared Storage**: Files in object storage (S3), not local disk
- **Configuration Externalization**: Environment variables, config service
- **Load Balancer Awareness**: Health checks, graceful shutdown

**Session Management**:
```
User Request → Load Balancer → App Instance (any) → Redis (shared session)
```

**Benefits**:
- Add/remove instances dynamically
- Zero-downtime deployments
- Cost-effective (horizontal scales cheaper than vertical)
- Fault tolerance (instance failure doesn't lose all state)

### Auto-Scaling Strategy

**Scaling Metrics**:
- **CPU Utilization**: Scale when avg CPU > 70% for 5 minutes
- **Memory Utilization**: Scale when avg memory > 80% for 5 minutes
- **Request Rate**: Scale when requests/sec > threshold
- **Response Time**: Scale when p95 latency > target SLA

**Auto-Scaling Configuration**:
```yaml
# Kubernetes HPA example
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-service
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: api-service
  minReplicas: 3
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
  behavior:
    scaleUp:
      stabilizationWindowSeconds: 60
      policies:
      - type: Percent
        value: 50
        periodSeconds: 60
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
      - type: Percent
        value: 25
        periodSeconds: 60
```

**Scaling Policies**:
- **Scale Up**: Aggressive (50% increase, 1-minute stabilization)
- **Scale Down**: Conservative (25% decrease, 5-minute stabilization)
- **Minimum Replicas**: 3 (high availability across AZs)
- **Maximum Replicas**: 20 (cost control, prevent runaway scaling)

## Load Balancing

### Load Balancer Architecture

```mermaid
graph TD
    Users[Users] --> DNS[DNS]
    DNS --> CDN[CDN / Edge]
    CDN --> GLB[Global Load Balancer]
    GLB --> RLB1[Regional LB - US-East]
    GLB --> RLB2[Regional LB - EU-West]
    RLB1 --> App1[App Instance 1]
    RLB1 --> App2[App Instance 2]
    RLB1 --> App3[App Instance 3]
    RLB2 --> App4[App Instance 4]
    RLB2 --> App5[App Instance 5]
```

### Load Balancing Algorithms

**Round Robin**:
- Simple, even distribution
- Best for: Homogeneous instances, similar request complexity

**Least Connections**:
- Route to instance with fewest active connections
- Best for: Long-lived connections, varied request complexity

**Weighted Round Robin**:
- Distribute traffic based on instance capacity weights
- Best for: Mixed instance sizes, gradual rollouts (canary)

**IP Hash**:
- Route same client IP to same instance (sticky sessions)
- Best for: Stateful applications (avoid if possible)

### Health Checks

**Load Balancer Health Checks**:
```
Endpoint: GET /health/ready
Interval: 10 seconds
Timeout: 5 seconds
Healthy Threshold: 2 consecutive successes
Unhealthy Threshold: 3 consecutive failures
```

**Health Check Response**:
```json
{
  "status": "healthy",
  "checks": {
    "database": "ok",
    "cache": "ok",
    "externalApi": "ok"
  }
}
```

**Graceful Shutdown**:
1. Instance receives termination signal
2. Remove from load balancer (fail health checks)
3. Drain existing connections (30-60 second grace period)
4. Complete in-flight requests
5. Shutdown application

## Caching Architecture

### Multi-Layer Caching

```mermaid
graph TD
    Client[Client] --> CDN[CDN Cache]
    CDN -->|Miss| API[API Gateway]
    API --> AppCache[Application Cache]
    AppCache -->|Miss| Redis[Redis Cache]
    Redis -->|Miss| DB[Database]
    DB --> Redis
    Redis --> AppCache
    AppCache --> API
```

**Cache Layers**:
1. **CDN Cache**: Static assets, public API responses (edge caching)
2. **API Gateway Cache**: Cacheable API responses
3. **Application Cache**: In-memory cache (process-local, minimal)
4. **Distributed Cache**: Redis for session data, frequently accessed data
5. **Database Cache**: Query result cache, materialized views

### Cache Strategies (Redis)

**Game Session State Cache** (Write-Through):
```typescript
async function updateGameState(sessionId: string, updates: Partial<GameState>) {
  // Update PostgreSQL (authoritative source)
  await prisma.gameSession.update({
    where: { id: sessionId },
    data: { gameState: updates },
  });

  // Update Redis cache immediately
  await redis.setex(
    `session:${sessionId}`,
    1800, // 30 minutes TTL (average session duration)
    JSON.stringify(updates)
  );
}
```

**Tribe Configuration Cache** (Cache-Aside + TTL):
```typescript
async function getTribeConfig(tribeId: string): Promise<Tribe> {
  // Check cache first
  const cached = await redis.get(`tribe:${tribeId}`);
  if (cached) {
    return JSON.parse(cached); // Cache hit
  }

  // Cache miss - query database
  const tribe = await prisma.tribe.findUnique({ where: { id: tribeId } });

  // Store in cache with 1-hour TTL (tribes rarely change)
  await redis.setex(`tribe:${tribeId}`, 3600, JSON.stringify(tribe));

  return tribe;
}
```

**Map Metadata Cache** (Cache-Aside + Invalidation):
```typescript
async function getMapMetadata(mapId: string): Promise<Map> {
  const cached = await redis.get(`map:${mapId}`);
  if (cached) return JSON.parse(cached);

  const map = await prisma.map.findUnique({
    where: { id: mapId },
    include: { user: { select: { username: true } } },
  });

  // Cache for 1 hour (maps change infrequently)
  await redis.setex(`map:${mapId}`, 3600, JSON.stringify(map));

  return map;
}

// Invalidate cache on map update
async function updateMap(mapId: string, updates: Partial<Map>) {
  await prisma.map.update({ where: { id: mapId }, data: updates });
  await redis.del(`map:${mapId}`); // Invalidate cache
}
```

### Cache Invalidation

**TTL-Based Expiration**:
- **Short TTL**: 30s-5min for frequently changing data
- **Medium TTL**: 15min-1hr for semi-static data
- **Long TTL**: 1hr-24hr for rarely changing data

**Event-Based Invalidation**:
```python
# When user profile updated
def update_user_profile(user_id, data):
    database.update(...)

    # Invalidate related cache keys
    redis.delete(f"user:{user_id}:profile")
    redis.delete(f"user:{user_id}:orders")
    redis.delete(f"user:{user_id}:preferences")
```

**Cache Stampede Prevention**:
```python
import threading

lock = threading.Lock()

def get_with_lock(key):
    value = redis.get(key)
    if value:
        return value

    # Only one thread fetches from DB
    with lock:
        # Double-check cache (another thread may have populated)
        value = redis.get(key)
        if value:
            return value

        # Fetch from database
        value = expensive_database_query()
        redis.setex(key, ttl, value)
        return value
```

## Asynchronous Processing

### Queue-Based Architecture

```mermaid
graph LR
    API[API Service] -->|Publish| Queue[Message Queue]
    Queue -->|Subscribe| Worker1[Worker 1]
    Queue -->|Subscribe| Worker2[Worker 2]
    Queue -->|Subscribe| Worker3[Worker 3]
    Worker1 --> DB[Database]
    Worker2 --> DB
    Worker3 --> DB
```

**Use Cases for Async Processing**:
- **Email Sending**: Decouple from API request
- **Image Processing**: Resize, optimize images
- **Report Generation**: Long-running computations
- **Data Export**: Large file generation
- **Webhook Delivery**: External HTTP calls
- **Batch Operations**: Bulk updates, imports

### Message Queue Patterns

**Work Queue** (Single Queue, Multiple Workers):
```
Producer → Queue → Worker 1
                 → Worker 2
                 → Worker 3
```
**Use Case**: Distribute tasks evenly across workers

**Pub/Sub** (One Message, Multiple Subscribers):
```
Publisher → Topic → Subscriber A
                  → Subscriber B
                  → Subscriber C
```
**Use Case**: Event broadcasting (user registered → welcome email + analytics + CRM)

**Priority Queue**:
```
High Priority Queue → Dedicated Workers (fast)
Normal Priority Queue → Standard Workers
Low Priority Queue → Background Workers
```
**Use Case**: Critical tasks processed first (payment > email)

### Task Processing

**Job Structure**:
```json
{
  "jobId": "uuid",
  "type": "send_email",
  "priority": "high",
  "data": {
    "to": "user@example.com",
    "template": "welcome_email",
    "params": {...}
  },
  "retries": 0,
  "maxRetries": 3,
  "createdAt": "2025-11-20T10:30:00Z"
}
```

**Retry Strategy**:
- **Exponential Backoff**: 1s, 2s, 4s, 8s, 16s
- **Max Retries**: 3 attempts
- **Dead Letter Queue**: Failed jobs after max retries
- **Idempotency**: Safe to retry (no duplicate side effects)

**Worker Scaling**:
- Monitor queue depth (messages waiting)
- Scale workers when queue > threshold
- Auto-scale based on message rate

## Database Scalability

### Read Scaling

**Read Replicas**:
```mermaid
graph TD
    App[Application] -->|Write| Primary[Primary DB]
    Primary -->|Replicate| Replica1[Read Replica 1]
    Primary -->|Replicate| Replica2[Read Replica 2]
    App -->|Read| Replica1
    App -->|Read| Replica2
```

**Read/Write Splitting**:
```python
# Route writes to primary
def create_user(data):
    primary_db.execute("INSERT INTO users ...")

# Route reads to replicas (load balanced)
def get_user(user_id):
    replica_db.query("SELECT * FROM users WHERE id = ?", user_id)
```

**Replication Lag**:
- Typically < 1 second
- Use primary for read-after-write consistency
- Use replicas for analytics, reporting (eventual consistency OK)

### Write Scaling

**Database Sharding**:
```
Shard 1: Users A-M
Shard 2: Users N-Z

Route user "Alice" → Shard 1
Route user "Zoe" → Shard 2
```

**Sharding Strategies**:
- **Range-Based**: Shard by ID range (1-1M, 1M-2M)
- **Hash-Based**: Hash user ID, modulo shard count
- **Geographic**: Shard by region (US, EU, APAC)

**Sharding Challenges**:
- **Cross-Shard Queries**: Avoid or use distributed query engine
- **Shard Rebalancing**: Complex when adding new shards
- **Distributed Transactions**: Use saga pattern or avoid

### Connection Pooling

**Connection Pool Configuration**:
```python
pool = ConnectionPool(
    host="db.example.com",
    port=5432,
    min_size=5,      # Minimum connections
    max_size=20,     # Maximum connections
    max_overflow=10, # Burst capacity
    timeout=30,      # Connection timeout
    recycle=3600     # Recycle connections hourly
)
```

**Pool Sizing**:
```
Optimal Pool Size = (Core Count * 2) + Effective Spindle Count
Example: 4 cores, SSD → (4 * 2) + 1 = 9 connections per instance
```

## Performance Optimization

### API Performance Targets

**Latency Targets**:
- **p50 (median)**: < 100ms
- **p95**: < 200ms
- **p99**: < 500ms
- **p99.9**: < 1000ms

**Throughput Targets**:
- **Peak Load**: 10,000 requests/second
- **Sustained Load**: 5,000 requests/second
- **Database Queries**: < 50ms (p95)
- **External API Calls**: < 500ms (p95)

### Performance Optimization Techniques

**N+1 Query Problem**:
```sql
-- BAD: N+1 queries (1 + N)
SELECT * FROM users;  -- 1 query
-- Then for each user:
SELECT * FROM orders WHERE user_id = ?;  -- N queries

-- GOOD: Single query with JOIN
SELECT u.*, o.*
FROM users u
LEFT JOIN orders o ON u.id = o.user_id;
```

**Pagination**:
```sql
-- Limit results to prevent memory exhaustion
SELECT * FROM products
ORDER BY created_at DESC
LIMIT 20 OFFSET 0;  -- First page
```

**Selective Field Projection**:
```sql
-- Only query needed fields
SELECT id, name, price FROM products;  -- Not SELECT *
```

**Index Optimization**:
```sql
-- Ensure queries use indexes (check with EXPLAIN)
EXPLAIN ANALYZE SELECT * FROM orders WHERE user_id = '123';

-- Create covering index for query
CREATE INDEX idx_orders_user_created ON orders(user_id, created_at);
```

**Batch Operations**:
```python
# BAD: N database calls
for user_id in user_ids:
    database.update("UPDATE users SET active = true WHERE id = ?", user_id)

# GOOD: Single batch update
database.execute(
    "UPDATE users SET active = true WHERE id IN (?)",
    user_ids
)
```

### Frontend Performance

**Asset Optimization**:
- **Code Splitting**: Load only necessary JavaScript
- **Lazy Loading**: Load images/components on demand
- **Minification**: Reduce file sizes (uglify, terser)
- **Compression**: Gzip/Brotli compression

**Core Web Vitals Targets**:
- **LCP (Largest Contentful Paint)**: < 2.5s
- **FID (First Input Delay)**: < 100ms
- **CLS (Cumulative Layout Shift)**: < 0.1

**Caching Headers**:
```
# Static assets (long cache)
Cache-Control: public, max-age=31536000, immutable

# API responses (short cache)
Cache-Control: public, max-age=300

# No cache (sensitive data)
Cache-Control: no-store, no-cache, must-revalidate
```

## Capacity Planning

### Resource Estimation

**CPU Requirements**:
```
Expected Requests/Second: 5,000
CPU per Request: 10ms
Total CPU: 5,000 * 0.01s = 50 CPU-seconds/second = 50 cores

With 70% target utilization: 50 / 0.7 = 72 cores
With HA (3 AZ): 72 * 1.5 = 108 cores total
```

**Memory Requirements**:
```
Average Request Memory: 10 MB
Concurrent Requests per Instance: 100
Memory per Instance: 100 * 10 MB = 1 GB

Plus base overhead (OS, runtime): +2 GB = 3 GB per instance
```

**Storage Growth**:
```
Current Data: 500 GB
Monthly Growth: 50 GB
Projected 12-month: 500 + (50 * 12) = 1,100 GB

With backups (30-day retention): 1,100 * 2 = 2,200 GB
With replication (3x): 2,200 * 3 = 6,600 GB total
```

### Growth Planning

**Traffic Projections**:
- **Current**: 1,000 req/sec peak
- **6-month**: 2,500 req/sec (2.5x growth)
- **12-month**: 5,000 req/sec (5x growth)
- **18-month**: 10,000 req/sec (10x growth)

**Infrastructure Scaling Plan**:
```
Month 0-6:  Current infrastructure sufficient
Month 6-12: Add read replicas, increase cache capacity
Month 12-18: Implement database sharding, scale compute 2x
Month 18+:  Multi-region deployment, CDN expansion
```

## Performance Benchmarking

### Load Testing

**Load Test Scenarios**:
1. **Baseline**: Normal traffic (1,000 req/sec)
2. **Peak Load**: Expected peak (5,000 req/sec)
3. **Stress Test**: 2x peak load (10,000 req/sec)
4. **Spike Test**: Sudden traffic burst (0 → 5,000 req/sec in 10s)
5. **Endurance Test**: Sustained peak load for 2+ hours

**Load Testing Tools**: [JMeter / Gatling / k6 / Locust]

**Load Test Metrics**:
- Response time (p50, p95, p99)
- Throughput (requests/second)
- Error rate (% failed requests)
- Resource utilization (CPU, memory, disk, network)

### Performance Profiling

**Profiling Tools**:
- **Application Profiling**: py-spy (Python), Node Clinic (Node.js)
- **Database Profiling**: pg_stat_statements (PostgreSQL), slow query log (MySQL)
- **APM Tools**: New Relic, Datadog APM, Dynatrace

**Profiling Focus**:
- Identify slow endpoints (> 200ms)
- Find CPU-intensive operations
- Detect memory leaks
- Optimize database queries

## Validation Checklist

**Horizontal Scaling** (Stateless Backend):
- [x] Stateless application design (all state in PostgreSQL or Redis, no local state)
- [x] Auto-scaling configured (Kubernetes HPA or equivalent): CPU >70% or active_players >500/instance
- [x] Minimum 3 replicas for high availability (across 3 availability zones)
- [x] Graceful shutdown: 30s drain period, complete in-flight requests before shutdown

**Load Balancing** (Nginx/ALB):
- [x] Nginx or AWS ALB with WebSocket upgrade header proxying
- [x] Sticky sessions: IP hash for routing players in same game session to same backend instance
- [x] Health checks: GET /health endpoint every 10s, remove unhealthy instances from pool
- [x] Socket.io Redis adapter for multi-instance WebSocket synchronization

**Caching** (Redis):
- [x] Game session state cache (TTL: 30min, write-through)
- [x] Tribe configuration cache (TTL: 1 hour, cache-aside)
- [x] Map metadata cache (TTL: 1 hour, cache-aside with invalidation on update)
- [x] Cache warm-up on backend startup (load common tribes/maps)
- [x] Cache invalidation: Delete on resource update (maps, tribes)

**Database Scalability** (PostgreSQL):
- [x] Connection pooling: Prisma with max 10 connections per instance (3 instances = 30 total)
- [x] Read replicas: Use read replica for map queries (GET /maps), write to primary for game state
- [x] Indexes: username, creatorId, gameSessionId (from 8-data.md)
- [x] Query optimization: Avoid N+1 queries, use Prisma's include for eager loading

**Performance Benchmarks** (from 1-use-cases.md NFRs):
- [x] Game Loop: 60fps at 1920x1080 with 100 enemies + 50 towers (NFR-001)
- [x] Pathfinding: <100ms for A* on 50x50 grid with 500 obstacles (NFR-002)
- [x] Multiplayer Latency: <100ms round-trip for WebSocket events with 4 players (NFR-003)
- [x] Concurrent Players: 8+ players per game session without performance degradation
- [x] Concurrent Sessions: 100+ game sessions per backend instance (target: 300 at 70% CPU)
- [x] API Response Time: <200ms for all REST endpoints (99th percentile)

**Capacity Planning**:
- [x] Baseline: 1 backend instance supports ~100 game sessions (~400 concurrent players)
- [x] Growth: Add 1 instance per 100 sessions, auto-scale at 70% CPU threshold
- [x] Database: PostgreSQL can handle 10k game sessions with current schema
- [x] Redis: 2GB Redis instance supports ~5k cached game sessions
- [x] Traffic Projections: Current 1k req/sec peak → 6-month 2.5k req/sec → 12-month 5k req/sec

**Network Optimization**:
- [x] WebSocket compression enabled (permessage-deflate)
- [x] Delta state updates (only send changed entities, not full state every frame)
- [x] Batch updates: Send state:delta every 100ms (not every 16ms frame)
- [x] CDN for static assets (React build, PixiJS sprites, audio files)

**Load Testing**:
- [x] Baseline: Normal traffic (1,000 req/sec)
- [x] Peak Load: Expected peak (5,000 req/sec)
- [x] Stress Test: 2x peak load (10,000 req/sec)
- [x] Spike Test: Sudden traffic burst (0 → 5,000 req/sec in 10s)
- [x] Endurance Test: Sustained peak load for 2+ hours

**Instructions**: Complete scalability design for isometric tower defense game. Horizontal scaling (3+ stateless backend instances with Socket.io Redis adapter), load balancing (Nginx with sticky sessions), caching (Redis for game state/tribes/maps), database scaling (Prisma connection pooling + read replicas), and performance benchmarks (60fps, <100ms pathfinding, <100ms multiplayer latency) specified. Capacity planning for 3x growth in 6 months documented. Ready for implementation.

[<< Back](./design.md)

[<< Back](./design.md)

````
