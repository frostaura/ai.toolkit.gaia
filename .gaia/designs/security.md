# LifeOS Security Design

## Overview

LifeOS handles sensitive financial data, health metrics, and life management information. This document defines the security architecture optimized for a single-user system with future multi-user extensibility.

**Security Principles**:
- Single-user focus with multi-user architecture ready
- Defense in depth for financial data protection
- API-first security for automation integrations
- Audit everything, redact sensitives in logs

---

## Authentication

### User Authentication Model

**Current Phase**: Single-user system
**Architecture**: Multi-user ready (user_id foreign keys throughout)

```
┌─────────────────────────────────────────────────────────────┐
│                    Authentication Flow                       │
├─────────────────────────────────────────────────────────────┤
│  Client → POST /api/auth/login → Validate → Issue Tokens    │
│                                                              │
│  Access Token (15min) → Authorization header                 │
│  Refresh Token (7d) → httpOnly cookie                        │
└─────────────────────────────────────────────────────────────┘
```

### Development Account

**Admin Credentials** (Development Only):
- Email: `admin@system.local`
- Password: `Admin123!`
- Note: Seeded on first run in development mode only

### JWT Implementation

**Token Structure**:
```typescript
// Access Token (15 minutes)
interface AccessTokenPayload {
  sub: string;          // User ID (UUID)
  email: string;        // User email
  iat: number;          // Issued at (Unix timestamp)
  exp: number;          // Expires at (15min from iat)
  type: 'access';       // Token type discriminator
}

// Refresh Token (7 days)
interface RefreshTokenPayload {
  sub: string;          // User ID (UUID)
  jti: string;          // Unique token ID (for revocation)
  iat: number;          // Issued at
  exp: number;          // Expires at (7 days from iat)
  type: 'refresh';      // Token type discriminator
}
```

**JWT Configuration**:
```csharp
public class JwtSettings
{
    public string SecretKey { get; set; }      // Min 256-bit key
    public string Issuer { get; set; }         // "lifeos-api"
    public string Audience { get; set; }       // "lifeos-client"
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
    public string Algorithm { get; set; } = "HS256"; // HMAC-SHA256
}
```

**Token Signing**:
- Algorithm: HS256 (HMAC-SHA256)
- Key: 256-bit minimum, stored in environment variable
- Future: Upgrade to RS256 for multi-service architecture

### Token Storage Strategy

**Access Token**:
- Stored in memory (React state/context)
- Never persisted to localStorage/sessionStorage
- Sent via `Authorization: Bearer <token>` header

**Refresh Token**:
- Stored in httpOnly secure cookie
- Cookie name: `lifeos_refresh_token`
- Cookie settings:
  ```typescript
  {
    httpOnly: true,
    secure: true,           // HTTPS only
    sameSite: 'strict',     // CSRF protection
    path: '/api/auth',      // Only sent to auth endpoints
    maxAge: 7 * 24 * 60 * 60 * 1000 // 7 days in ms
  }
  ```

### Token Refresh Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     Token Refresh Flow                       │
├─────────────────────────────────────────────────────────────┤
│  1. Access token expires (or client detects <5min remaining)│
│  2. Client calls POST /api/auth/refresh (cookie auto-sent)  │
│  3. Server validates refresh token:                          │
│     - Check signature and expiry                             │
│     - Verify jti not in blacklist                            │
│     - Load user from database                                │
│  4. Issue new access token (new refresh token on rotation)   │
│  5. Blacklist old refresh token jti                          │
└─────────────────────────────────────────────────────────────┘
```

**Refresh Token Rotation**:
- New refresh token issued every 24 hours
- Old refresh token blacklisted on rotation
- Blacklist stored in database (RefreshTokenBlacklist table)
- Cleanup: Purge blacklist entries older than 7 days

### Logout

```typescript
// POST /api/auth/logout
// 1. Extract refresh token from cookie
// 2. Add jti to blacklist
// 3. Clear refresh token cookie
// 4. Return 204 No Content
```

---

## Authorization

### Single-User Authorization

**Current Model**: All authenticated users have full access
**Middleware Check**: Simply verify valid JWT

```csharp
[Authorize]  // Standard JWT validation only
public class FinancialController : ControllerBase
{
    // All endpoints accessible to authenticated user
}
```

### API Key Authentication (Automation)

**Purpose**: Allow external services (n8n, iOS Shortcuts) to call LifeOS APIs

**API Key Structure**:
```typescript
interface ApiKey {
  id: string;           // UUID
  name: string;         // Descriptive name (e.g., "n8n-metrics")
  key_prefix: string;   // First 8 chars for identification
  key_hash: string;     // SHA-256 hash of full key
  scopes: string[];     // Allowed endpoints/actions
  created_at: Date;
  last_used_at: Date;
  expires_at: Date | null;  // Optional expiry
  is_active: boolean;
}
```

**API Key Format**: `lifeos_[prefix]_[random32chars]`
- Example: `lifeos_nwk8h3j2_a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6`

**API Key Scopes**:
```typescript
const API_KEY_SCOPES = {
  'metrics:write': ['POST /api/metrics/record'],
  'metrics:read': ['GET /api/metrics/*'],
  'financial:read': ['GET /api/financial/*'],
  'webhooks:trigger': ['POST /api/webhooks/*'],
  '*': ['ALL']  // Full access (use sparingly)
};
```

**API Key Authentication Flow**:
```
┌─────────────────────────────────────────────────────────────┐
│  Request: Authorization: ApiKey lifeos_xxx_yyy              │
│                                                              │
│  1. Extract key from header                                  │
│  2. Split prefix: lifeos_xxx                                 │
│  3. Lookup by prefix, get stored hash                        │
│  4. Hash provided key, compare to stored hash                │
│  5. Verify key is active and not expired                     │
│  6. Check scope allows requested endpoint                    │
│  7. Update last_used_at                                      │
│  8. Proceed with request                                     │
└─────────────────────────────────────────────────────────────┘
```

**API Key Management Endpoints**:
- `POST /api/auth/api-keys` - Create new API key (returns full key once)
- `GET /api/auth/api-keys` - List API keys (prefix only, no secrets)
- `DELETE /api/auth/api-keys/{id}` - Revoke API key

---

## Data Protection

### Encryption at Rest

**Sensitive Fields Requiring Encryption**:
```typescript
// Financial data encryption
const ENCRYPTED_FIELDS = {
  FinancialAccount: ['account_number', 'routing_number'],
  FinancialTransaction: [],  // Amount is not encrypted (needed for queries)
  TaxProfile: ['ssn_last_four'],  // Only store last 4
};
```

**Encryption Implementation**:
```csharp
public class EncryptionService
{
    // AES-256-GCM for field-level encryption
    private readonly byte[] _key;  // From environment variable
    
    public string Encrypt(string plaintext)
    {
        // Generate random 12-byte nonce
        // Encrypt with AES-256-GCM
        // Return: base64(nonce + ciphertext + tag)
    }
    
    public string Decrypt(string ciphertext)
    {
        // Decode base64
        // Extract nonce (first 12 bytes)
        // Extract tag (last 16 bytes)
        // Decrypt and verify
    }
}
```

### Encryption in Transit

- **HTTPS Required**: All API endpoints require TLS 1.2+
- **HSTS**: Strict-Transport-Security enforced
- **Certificate**: Let's Encrypt or equivalent

### Password Security

```csharp
public class PasswordService
{
    // Using ASP.NET Core Identity's Argon2id hasher
    public string Hash(string password) => 
        _hasher.HashPassword(null, password);
    
    public bool Verify(string password, string hash) =>
        _hasher.VerifyHashedPassword(null, hash, password) 
            != PasswordVerificationResult.Failed;
}
```

**Password Requirements**:
- Minimum 8 characters
- At least one uppercase, one lowercase, one number
- No common password check (haveibeenpwned API optional)

### Data Redaction in Logs

**Fields to Redact**:
```typescript
const REDACTED_FIELDS = [
  'password',
  'access_token',
  'refresh_token',
  'api_key',
  'account_number',
  'routing_number',
  'ssn',
  'Authorization'  // Header
];
```

**Redaction Pattern**: Replace with `[REDACTED]`

---

## Input Validation

### Validation Framework

**Using FluentValidation for .NET**:

```csharp
public class MetricRecordValidator : AbstractValidator<RecordMetricRequest>
{
    public MetricRecordValidator(IMetricDefinitionRepository repo)
    {
        RuleFor(x => x.MetricId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await repo.ExistsAsync(id))
            .WithMessage("Invalid metric definition");
        
        RuleFor(x => x.Value)
            .NotNull()
            .Must(BeValidForMetricType)
            .WithMessage("Value must match metric data type");
        
        RuleFor(x => x.RecordedAt)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Future timestamps not allowed");
    }
}
```

### Domain-Specific Validation Rules

**Financial Amounts**:
```csharp
public class FinancialAmountValidator : AbstractValidator<decimal>
{
    public FinancialAmountValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0)
            .WithMessage("Amount must be positive")
            .PrecisionScale(18, 2, false)
            .WithMessage("Amount must have max 2 decimal places");
    }
}
```

**Metric Values**:
```csharp
public class MetricValueValidator
{
    public bool Validate(MetricDefinition def, object value)
    {
        return def.DataType switch
        {
            "number" => value is decimal or int or double,
            "integer" => value is int,
            "boolean" => value is bool,
            "duration" => value is string s && TimeSpan.TryParse(s, out _),
            "timestamp" => value is DateTime or string s && DateTime.TryParse(s, out _),
            _ => false
        };
    }
}
```

**Simulation DSL Validation** (Sandboxed):
```csharp
public class SimulationConditionValidator
{
    // Whitelist allowed operations
    private static readonly HashSet<string> AllowedOperators = 
        new() { "==", "!=", ">", "<", ">=", "<=", "&&", "||" };
    
    // Whitelist allowed field references
    private static readonly HashSet<string> AllowedFields =
        new() { "age", "income", "savings", "expenses", "net_worth" };
    
    public ValidationResult Validate(string condition)
    {
        // Parse DSL
        // Verify only allowed operators and fields
        // No function calls, no code execution
        // Return sanitized expression tree
    }
}
```

### Request Size Limits

```csharp
// Program.cs
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;  // 10MB max
});

// Per-endpoint limits
[RequestSizeLimit(1024 * 100)]  // 100KB for JSON payloads
public async Task<IActionResult> RecordMetric([FromBody] RecordMetricRequest request)
```

---

## Security Headers

### Configuration

```csharp
// SecurityHeadersMiddleware.cs
public class SecurityHeadersMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Content Security Policy
        context.Response.Headers["Content-Security-Policy"] = 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +  // React dev
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: blob:; " +
            "font-src 'self'; " +
            "connect-src 'self' https://api.lifeos.local; " +
            "frame-ancestors 'none'; " +
            "form-action 'self';";
        
        // Prevent framing (clickjacking)
        context.Response.Headers["X-Frame-Options"] = "DENY";
        
        // Prevent MIME sniffing
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        
        // XSS Protection (legacy browsers)
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        
        // HTTPS enforcement
        context.Response.Headers["Strict-Transport-Security"] = 
            "max-age=31536000; includeSubDomains; preload";
        
        // Referrer policy
        context.Response.Headers["Referrer-Policy"] = 
            "strict-origin-when-cross-origin";
        
        // Permissions policy
        context.Response.Headers["Permissions-Policy"] = 
            "geolocation=(), microphone=(), camera=()";
        
        await next(context);
    }
}
```

### CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("LifeOSPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://lifeos.local",      // Production
                "http://localhost:3000",      // React dev
                "http://localhost:5173"       // Vite dev
            )
            .AllowCredentials()              // Required for cookies
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .WithHeaders(
                "Content-Type",
                "Authorization",
                "X-Requested-With",
                "X-Request-ID"
            )
            .WithExposedHeaders(
                "X-Request-ID",
                "X-RateLimit-Remaining",
                "X-RateLimit-Reset"
            )
            .SetPreflightMaxAge(TimeSpan.FromHours(1));
    });
});
```

---

## Rate Limiting

### Rate Limit Configuration

```csharp
// RateLimitingConfiguration.cs
public static class RateLimitPolicies
{
    public static void Configure(RateLimiterOptions options)
    {
        // Global default: 100 requests/minute
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
            context => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        
        // Authentication endpoints: Strict limits
        options.AddPolicy("auth", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(15),
                    QueueLimit = 0
                }));
        
        // Metrics ingestion: Higher limits for automation
        options.AddPolicy("metrics", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: GetApiKeyOrIp(context),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1000,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 10
                }));
        
        // Financial operations: Moderate limits
        options.AddPolicy("financial", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User?.Identity?.Name ?? "anonymous",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        
        // Simulation runs: Expensive operation
        options.AddPolicy("simulation", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User?.Identity?.Name ?? "anonymous",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 2
                }));
    }
}
```

### Rate Limit by Endpoint

| Endpoint Category | Limit | Window | Notes |
|-------------------|-------|--------|-------|
| `POST /api/auth/login` | 5 | 15 min | Per IP, prevent brute force |
| `POST /api/auth/refresh` | 10 | 1 min | Per IP |
| `POST /api/metrics/record` | 1000 | 1 min | Per API key, high for n8n |
| `GET /api/metrics/*` | 100 | 1 min | Standard |
| `POST /api/financial/*` | 60 | 1 min | Per user |
| `POST /api/simulation/run` | 10 | 1 min | CPU intensive |
| `GET /api/dashboard/*` | 30 | 1 min | Cached anyway |
| Default | 100 | 1 min | Per IP |

### Rate Limit Headers

```http
HTTP/1.1 200 OK
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1699000000

HTTP/1.1 429 Too Many Requests
Retry-After: 45
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1699000045
```

---

## Audit Logging

### Audit Event Categories

```csharp
public enum AuditEventType
{
    // Authentication events
    LoginAttempt,
    LoginSuccess,
    LoginFailure,
    Logout,
    TokenRefresh,
    ApiKeyCreated,
    ApiKeyRevoked,
    ApiKeyUsed,
    
    // Financial events (ALL logged)
    AccountCreated,
    AccountUpdated,
    AccountDeleted,
    TransactionCreated,
    TransactionUpdated,
    TransactionDeleted,
    TransactionImported,      // Bulk imports
    
    // Simulation events
    SimulationCreated,
    SimulationRun,
    SimulationDeleted,
    
    // Data access events (sensitive)
    FinancialDataExported,
    MetricsDataExported,
    
    // Configuration changes
    ProfileUpdated,
    DimensionWeightChanged,
    TaxProfileUpdated
}
```

### Audit Log Schema

```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public AuditEventType EventType { get; set; }
    public Guid? UserId { get; set; }
    public string? ApiKeyPrefix { get; set; }  // For API key auth
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string RequestPath { get; set; }
    public string RequestMethod { get; set; }
    public int? ResponseStatus { get; set; }
    public string? EntityType { get; set; }    // e.g., "FinancialTransaction"
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }       // JSON, sensitive data redacted
    public TimeSpan Duration { get; set; }
}
```

### Audit Logging Implementation

```csharp
public class AuditLogService : IAuditLogService
{
    public async Task LogAsync(AuditLogEntry entry)
    {
        // Redact sensitive data in details
        var safeDetails = RedactSensitiveData(entry.Details);
        
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            EventType = entry.EventType,
            UserId = entry.UserId,
            ApiKeyPrefix = entry.ApiKeyPrefix,
            IpAddress = entry.IpAddress,
            UserAgent = entry.UserAgent,
            RequestPath = entry.RequestPath,
            RequestMethod = entry.RequestMethod,
            ResponseStatus = entry.ResponseStatus,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            Details = safeDetails,
            Duration = entry.Duration
        };
        
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
```

### Required Audit Logging Points

**Financial Transactions** (ALL operations logged):
```csharp
[HttpPost("transactions")]
public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
{
    var transaction = await _service.CreateAsync(request);
    
    await _auditLog.LogAsync(new AuditLogEntry
    {
        EventType = AuditEventType.TransactionCreated,
        EntityType = "FinancialTransaction",
        EntityId = transaction.Id,
        Details = JsonSerializer.Serialize(new
        {
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,  // Keep amount for audit
            Category = transaction.Category,
            Description = "[REDACTED]"    // Redact description
        })
    });
    
    return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
}
```

**Simulation Runs**:
```csharp
[HttpPost("scenarios/{id}/run")]
public async Task<IActionResult> RunSimulation(Guid id, RunSimulationRequest request)
{
    var result = await _simulationEngine.RunAsync(id, request);
    
    await _auditLog.LogAsync(new AuditLogEntry
    {
        EventType = AuditEventType.SimulationRun,
        EntityType = "SimulationScenario",
        EntityId = id,
        Details = JsonSerializer.Serialize(new
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ProjectionYears = result.Projections.Count
        })
    });
    
    return Ok(result);
}
```

### Audit Log Retention

```csharp
// Cleanup job (runs daily)
public class AuditLogCleanupJob : IHostedService
{
    // Financial audit logs: Keep 7 years (compliance)
    private static readonly TimeSpan FinancialRetention = TimeSpan.FromDays(365 * 7);
    
    // Authentication logs: Keep 90 days
    private static readonly TimeSpan AuthRetention = TimeSpan.FromDays(90);
    
    // General logs: Keep 30 days
    private static readonly TimeSpan GeneralRetention = TimeSpan.FromDays(30);
}
```

---

## API Security Summary

### Authentication Methods by Endpoint Type

| Endpoint Type | Auth Method | Notes |
|---------------|-------------|-------|
| Browser UI | JWT (Bearer token) + httpOnly refresh cookie | Standard user flow |
| n8n Automation | API Key (ApiKey header) | `metrics:write` scope |
| iOS Shortcuts | API Key (ApiKey header) | Scoped to specific actions |
| Webhooks | API Key (ApiKey header) | `webhooks:trigger` scope |
| Health Check | None | `GET /health` public |

### Request/Response Security

**Request Logging** (development/staging):
```csharp
app.Use(async (context, next) =>
{
    var requestId = Guid.NewGuid().ToString("N")[..8];
    context.Response.Headers["X-Request-ID"] = requestId;
    
    _logger.LogInformation(
        "Request {RequestId}: {Method} {Path}",
        requestId,
        context.Request.Method,
        context.Request.Path);
    
    await next();
    
    _logger.LogInformation(
        "Response {RequestId}: {StatusCode} in {Duration}ms",
        requestId,
        context.Response.StatusCode,
        stopwatch.ElapsedMilliseconds);
});
```

**Response Error Handling** (production):
```csharp
// Never expose stack traces or internal errors
app.UseExceptionHandler(app =>
{
    app.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred",
            requestId = context.Response.Headers["X-Request-ID"].ToString()
        });
    });
});
```

---

## Environment Configuration

### Required Environment Variables

```bash
# JWT Configuration
JWT_SECRET_KEY=<min-32-chars-random-string>
JWT_ISSUER=lifeos-api
JWT_AUDIENCE=lifeos-client

# Encryption
ENCRYPTION_KEY=<32-byte-base64-encoded-key>

# Database
DATABASE_CONNECTION_STRING=<connection-string>

# Development only
ASPNETCORE_ENVIRONMENT=Development
SEED_ADMIN_ACCOUNT=true
```

### Production Checklist

- [ ] JWT_SECRET_KEY is cryptographically random (32+ chars)
- [ ] ENCRYPTION_KEY is generated with secure RNG
- [ ] HTTPS enforced (HSTS enabled)
- [ ] SEED_ADMIN_ACCOUNT=false in production
- [ ] Debug/development endpoints disabled
- [ ] Rate limiting enabled on all endpoints
- [ ] Audit logging enabled and monitored
- [ ] Security headers verified (use securityheaders.com)
- [ ] CORS origins restricted to production domain
- [ ] Database connection uses SSL

---

## Future Multi-User Considerations

**Prepared for Later**:
1. User ID foreign keys on all tables ✓
2. JWT payload includes user context ✓
3. API key scoped to user ✓
4. Audit logs include user ID ✓

**To Add for Multi-User**:
1. Role-based access control (RBAC)
2. Tenant isolation (if multi-tenant)
3. User invitation/registration flow
4. Password reset via email
5. MFA (TOTP) support