# Database Design

## Database Selection (Default)
- **Primary DB**: PostgreSQL 15+
- **ORM**: Entity Framework Core
- **Provider**: Npgsql
- **Justification**: ACID compliance, JSONB support, proven scalability

## Schema Design

### Core Tables/Collections

#### Users
```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) UNIQUE NOT NULL,
    username VARCHAR(50) UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) DEFAULT 'user',
    status VARCHAR(20) DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_status ON users(status);
```

#### Sessions
```sql
CREATE TABLE sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id) ON DELETE CASCADE,
    token VARCHAR(255) UNIQUE NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_sessions_token ON sessions(token);
CREATE INDEX idx_sessions_user_id ON sessions(user_id);
```

### Relationships

```
Users ──1:N──> Posts
Users ──1:N──> Comments
Posts ──1:N──> Comments
Users ──N:M──> Roles (via user_roles)
```

## Data Types & Constraints

### Common Fields
- **IDs**: UUID (distributed systems) or BIGSERIAL (single DB)
- **Timestamps**: TIMESTAMP WITH TIME ZONE
- **Status**: ENUM or VARCHAR with CHECK constraint
- **Money**: DECIMAL(19,4) never FLOAT
- **JSON**: JSONB for flexible data

### Constraints
- Foreign keys with appropriate CASCADE rules
- CHECK constraints for business rules
- UNIQUE constraints for natural keys
- NOT NULL for required fields

## Indexes

### Primary Indexes
- Primary keys (automatic)
- Foreign keys (recommended)
- Unique constraints (automatic)

### Performance Indexes
```sql
-- Frequently queried columns
CREATE INDEX idx_table_column ON table(column);

-- Composite indexes for common queries
CREATE INDEX idx_posts_user_status ON posts(user_id, status);

-- Partial indexes for filtered queries
CREATE INDEX idx_users_active ON users(status) WHERE status = 'active';

-- Full-text search
CREATE INDEX idx_posts_search ON posts USING gin(to_tsvector('english', title || ' ' || content));
```

## Migrations

### Naming Convention
```
YYYYMMDDHHMMSS_description.sql
20240101120000_create_users_table.sql
```

### Migration Template
```sql
-- Up Migration
BEGIN;
ALTER TABLE users ADD COLUMN phone VARCHAR(20);
COMMIT;

-- Down Migration
BEGIN;
ALTER TABLE users DROP COLUMN phone;
COMMIT;
```

## Data Access Patterns

### Read Patterns
- Single record by ID
- Lists with pagination
- Filtered searches
- Aggregations and reports

### Write Patterns
- Insert with returning ID
- Bulk inserts
- Update with optimistic locking
- Soft deletes (status/deleted_at)

## Performance Optimization

### Query Optimization
- Use EXPLAIN ANALYZE
- Avoid N+1 queries
- Use JOIN over multiple queries
- Batch operations when possible

### Connection Pooling
```javascript
{
  min: 2,
  max: 10,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 2000
}
```

### Caching Strategy
- Query result caching (Redis)
- Row-level caching
- Invalidation on write

## Backup & Recovery

### Backup Strategy
- Daily full backups
- Hourly incremental backups
- Point-in-time recovery (PITR)
- Geo-redundant storage

### Recovery Procedures
1. Stop application
2. Restore from backup
3. Apply WAL/binlog to recovery point
4. Verify data integrity
5. Resume application

## Monitoring

### Key Metrics
- Query execution time
- Connection pool usage
- Lock contention
- Index usage statistics
- Table/index bloat

### Alerts
- Slow queries > 1s
- Connection pool exhaustion
- Replication lag > 10s
- Disk usage > 80%

## Security

### Access Control
- Principle of least privilege
- Separate read/write users
- No direct DB access from frontend
- SSL/TLS connections required

### Data Protection
- Encryption at rest
- Column-level encryption for PII
- Audit logging for sensitive data
- Regular security updates

## Scaling Strategy

### Vertical Scaling
- Increase CPU/RAM as needed
- Monitor resource usage

### Horizontal Scaling
- Read replicas for read-heavy loads
- Sharding for write-heavy loads
- Consider NoSQL for specific use cases

### Partitioning
```sql
-- Time-based partitioning
CREATE TABLE events_2024_01 PARTITION OF events
FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
```