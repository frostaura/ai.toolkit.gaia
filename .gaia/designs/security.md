# Security Design

## Authentication

### Methods
- **Primary**: JWT with refresh tokens
- **Alternative**: Session-based with secure cookies
- **MFA**: TOTP (Time-based One-Time Password)

### JWT Implementation
```javascript
{
  header: {
    alg: "RS256",
    typ: "JWT"
  },
  payload: {
    sub: "user_id",
    iat: 1234567890,
    exp: 1234567890,
    scope: ["read", "write"]
  }
}
```

### Token Management
- Access token: 15 minutes
- Refresh token: 7 days
- Refresh rotation on use
- Blacklist on logout

## Authorization

### Access Control Models
- **RBAC** (Role-Based Access Control)
- **ABAC** (Attribute-Based Access Control)
- **ACL** (Access Control Lists)

### Permission Structure
```javascript
{
  resource: "posts",
  action: "write",
  conditions: {
    owner: true,
    status: "draft"
  }
}
```

### Middleware Example
```javascript
requirePermission('posts.write')
requireRole(['admin', 'editor'])
requireOwnership('post')
```

## Data Protection

### Encryption
- **At Rest**: AES-256-GCM
- **In Transit**: TLS 1.3+
- **Key Management**: AWS KMS/HashiCorp Vault

### Sensitive Data Handling
```javascript
// PII fields to encrypt
const encryptedFields = [
  'ssn',
  'creditCard',
  'bankAccount',
  'medicalRecords'
];

// Fields to redact in logs
const redactedFields = [
  'password',
  'token',
  'apiKey',
  'secret'
];
```

### Password Security
- **Hashing**: Argon2id or bcrypt (cost 12+)
- **Requirements**: Min 8 chars, complexity rules
- **Reset**: Time-limited tokens
- **History**: Prevent reuse of last 5

## Input Validation

### Validation Rules
```javascript
// Example schema
const userSchema = {
  email: {
    type: 'email',
    required: true,
    maxLength: 255
  },
  password: {
    type: 'string',
    required: true,
    minLength: 8,
    pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/
  },
  age: {
    type: 'number',
    min: 13,
    max: 120
  }
};
```

### Sanitization
- HTML encoding for display
- SQL parameterization
- NoSQL injection prevention
- Command injection prevention

## Security Headers

### HTTP Security Headers
```javascript
{
  'Content-Security-Policy': "default-src 'self'",
  'X-Frame-Options': 'DENY',
  'X-Content-Type-Options': 'nosniff',
  'X-XSS-Protection': '1; mode=block',
  'Strict-Transport-Security': 'max-age=31536000; includeSubDomains',
  'Referrer-Policy': 'strict-origin-when-cross-origin'
}
```

### CORS Configuration
```javascript
{
  origin: ['https://trusted-domain.com'],
  credentials: true,
  methods: ['GET', 'POST', 'PUT', 'DELETE'],
  allowedHeaders: ['Content-Type', 'Authorization']
}
```

## Rate Limiting

### Strategies
```javascript
{
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: 100, // requests per window
  message: 'Too many requests',
  standardHeaders: true,
  legacyHeaders: false,
}
```

### Endpoints
- Login: 5 attempts per 15 minutes
- API: 100 requests per 15 minutes
- Password reset: 3 per hour
- Registration: 10 per hour per IP

## Session Management

### Session Security
- Secure flag (HTTPS only)
- HttpOnly flag (no JS access)
- SameSite=Strict
- Session rotation on privilege escalation
- Absolute and idle timeouts

### Session Storage
```javascript
{
  secret: process.env.SESSION_SECRET,
  resave: false,
  saveUninitialized: false,
  cookie: {
    secure: true,
    httpOnly: true,
    maxAge: 1000 * 60 * 60 * 2, // 2 hours
    sameSite: 'strict'
  }
}
```

## Vulnerability Prevention

### OWASP Top 10
1. **Injection**: Parameterized queries, input validation
2. **Broken Auth**: MFA, secure session management
3. **Sensitive Data**: Encryption, minimal exposure
4. **XXE**: Disable XML external entities
5. **Broken Access**: Proper authorization checks
6. **Security Misconfig**: Secure defaults, hardening
7. **XSS**: Output encoding, CSP headers
8. **Insecure Deserial**: JSON only, schema validation
9. **Vulnerable Components**: Regular updates, scanning
10. **Insufficient Logging**: Comprehensive audit logs

## Audit & Logging

### Security Events to Log
```javascript
const securityEvents = [
  'login_attempt',
  'login_success',
  'login_failure',
  'logout',
  'password_change',
  'permission_denied',
  'data_access',
  'configuration_change'
];
```

### Log Format
```json
{
  "timestamp": "2024-01-01T00:00:00Z",
  "event": "login_attempt",
  "user_id": "123",
  "ip": "192.168.1.1",
  "user_agent": "Mozilla/5.0...",
  "result": "success",
  "metadata": {}
}
```

## Incident Response

### Response Plan
1. **Detect**: Monitoring and alerts
2. **Contain**: Isolate affected systems
3. **Investigate**: Root cause analysis
4. **Eradicate**: Remove threat
5. **Recover**: Restore services
6. **Learn**: Post-mortem and improvements

### Contact Chain
```yaml
severity_1:
  - Security Team Lead
  - CTO
  - CEO
severity_2:
  - Security Team
  - Engineering Lead
severity_3:
  - On-call Engineer
```

## Compliance

### Standards
- **GDPR**: Privacy by design, data portability
- **CCPA**: User data rights, opt-out
- **PCI DSS**: Payment card security
- **HIPAA**: Healthcare data protection
- **SOC 2**: Security controls audit

### Data Retention
```javascript
{
  user_data: '3 years after account closure',
  logs: '90 days',
  audit_logs: '7 years',
  payment_records: '7 years',
  temp_data: '24 hours'
}
```

## Security Testing

### Testing Types
- **SAST**: Static code analysis
- **DAST**: Dynamic application testing
- **Dependency Scanning**: Vulnerable packages
- **Penetration Testing**: Annual third-party
- **Security Reviews**: Code review checklist

### CI/CD Integration
```yaml
security_checks:
  - dependency_scan
  - sast_scan
  - secret_detection
  - container_scan
  - license_check
```