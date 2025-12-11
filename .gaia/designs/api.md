# LifeOS API Design

## Overview

RESTful API for LifeOS - Personal Life Operating System. Built with ASP.NET Core 8, following Clean Architecture patterns. Single-user mode initially, architected for future multi-user support.

---

## API Standards

### Base URL

| Environment | URL |
|-------------|-----|
| Development | `http://localhost:5000/api` |
| Staging | `https://staging.lifeos.app/api` |
| Production | `https://api.lifeos.app` |

### Versioning

- **URL Path**: `/api/v1/` (current version)
- Future versions: `/api/v2/`, etc.

### Authentication

```http
Authorization: Bearer <jwt_token>
```

- JWT access tokens (15 minute expiry)
- Refresh tokens (7 day expiry) stored as httpOnly cookies
- Single user mode: simplified login flow

### Content Type

```http
Content-Type: application/json
Accept: application/json
```

---

## HTTP Conventions

### Methods

| Method | Usage | Idempotent |
|--------|-------|------------|
| `GET` | Retrieve resources | Yes |
| `POST` | Create resources / Actions | No |
| `PATCH` | Partial update | Yes |
| `DELETE` | Remove resources | Yes |

### Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| `200` | OK | Successful GET/PATCH |
| `201` | Created | Successful POST |
| `204` | No Content | Successful DELETE |
| `400` | Bad Request | Malformed request |
| `401` | Unauthorized | Missing/invalid auth |
| `403` | Forbidden | Insufficient permissions |
| `404` | Not Found | Resource doesn't exist |
| `409` | Conflict | Duplicate/conflict |
| `422` | Unprocessable Entity | Validation failed |
| `429` | Too Many Requests | Rate limit exceeded |
| `500` | Server Error | Unexpected error |

---

## Response Formats

### Success Response (Single Resource)

```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "type": "account",
    "attributes": {
      "name": "Savings Account",
      "currency": "ZAR",
      "currentBalance": 150000.00
    }
  },
  "meta": {
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

### Success Response (Collection)

```json
{
  "data": [
    { "id": "...", "type": "account", "attributes": {...} }
  ],
  "meta": {
    "page": 1,
    "perPage": 20,
    "total": 45,
    "totalPages": 3,
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "links": {
    "first": "/api/v1/accounts?page=1",
    "prev": null,
    "next": "/api/v1/accounts?page=2",
    "last": "/api/v1/accounts?page=3"
  }
}
```

### Error Response

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Request validation failed",
    "details": [
      {
        "field": "amount",
        "code": "POSITIVE_REQUIRED",
        "message": "Amount must be greater than zero"
      },
      {
        "field": "currency",
        "code": "INVALID_FORMAT",
        "message": "Currency must be 3-letter ISO code"
      }
    ],
    "traceId": "0HMR8V5R8S1HP:00000001"
  }
}
```

### Error Codes

| Code | HTTP | Description |
|------|------|-------------|
| `VALIDATION_ERROR` | 422 | Input validation failed |
| `NOT_FOUND` | 404 | Resource not found |
| `UNAUTHORIZED` | 401 | Authentication required |
| `FORBIDDEN` | 403 | Permission denied |
| `CONFLICT` | 409 | Resource conflict |
| `RATE_LIMITED` | 429 | Too many requests |
| `INTERNAL_ERROR` | 500 | Server error |

---

## Query Parameters

### Pagination

```http
GET /api/v1/transactions?page=2&perPage=50
```

- `page`: Page number (default: 1)
- `perPage`: Items per page (default: 20, max: 100)

### Sorting

```http
GET /api/v1/transactions?sort=-transactionDate,amount
```

- Prefix `-` for descending order
- Multiple fields comma-separated

### Filtering

```http
GET /api/v1/transactions?category=expense&from=2024-01-01&to=2024-01-31
```

### Date Ranges

- `from`: Start date (ISO 8601)
- `to`: End date (ISO 8601)

---

## Rate Limiting

### Limits

| Tier | Limit | Window |
|------|-------|--------|
| Standard | 100 requests | per minute |
| Burst | 20 requests | per second |

### Headers

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1705320600
Retry-After: 60
```

---

# API Endpoints

## 1. Authentication

### POST /api/auth/login

Authenticate user and receive JWT tokens.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response (200):**
```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 900,
    "tokenType": "Bearer"
  }
}
```

**Cookies Set:**
```http
Set-Cookie: refreshToken=<token>; HttpOnly; Secure; SameSite=Strict; Path=/api/auth; Max-Age=604800
```

**Errors:**
- `401 UNAUTHORIZED` - Invalid credentials

### POST /api/auth/logout

Invalidate current session and clear refresh token.

**Response (204):** No content

**Cookies Cleared:**
```http
Set-Cookie: refreshToken=; HttpOnly; Secure; SameSite=Strict; Path=/api/auth; Max-Age=0
```

### POST /api/auth/refresh

Refresh access token using refresh token cookie.

**Response (200):**
```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 900,
    "tokenType": "Bearer"
  }
}
```

**Errors:**
- `401 UNAUTHORIZED` - Invalid/expired refresh token

---

## 2. User Profile

### GET /api/profile

Get current user profile.

**Response (200):**
```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "type": "profile",
    "attributes": {
      "email": "user@example.com",
      "username": "dean",
      "homeCurrency": "ZAR",
      "dateOfBirth": "1985-06-15",
      "lifeExpectancyBaseline": 80,
      "defaultAssumptions": {
        "inflationRateAnnual": 0.05,
        "defaultGrowthRate": 0.07,
        "retirementAge": 65
      },
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
  }
}
```

### PATCH /api/profile

Update user profile.

**Request:**
```json
{
  "homeCurrency": "USD",
  "lifeExpectancyBaseline": 85,
  "defaultAssumptions": {
    "inflationRateAnnual": 0.04,
    "retirementAge": 60
  }
}
```

**Response (200):** Updated profile object

**Validation:**
- `homeCurrency`: 3-letter ISO currency code
- `lifeExpectancyBaseline`: Integer 50-120
- `defaultAssumptions.inflationRateAnnual`: Decimal 0-0.5
- `defaultAssumptions.defaultGrowthRate`: Decimal -0.5 to 0.5
- `defaultAssumptions.retirementAge`: Integer 40-100

---

## 3. Dimensions

### GET /api/dimensions

Get all life dimensions with user weights.

**Response (200):**
```json
{
  "data": [
    {
      "id": "d1a2b3c4-...",
      "type": "dimension",
      "attributes": {
        "code": "health",
        "name": "Health & Recovery",
        "description": "Physical health, sleep, exercise, and recovery",
        "icon": "heart",
        "weight": 0.15,
        "defaultWeight": 0.125,
        "sortOrder": 1,
        "isActive": true,
        "currentScore": 72.5
      }
    },
    {
      "id": "d2a2b3c4-...",
      "type": "dimension",
      "attributes": {
        "code": "relationships",
        "name": "Relationships",
        "description": "Family, friends, and meaningful connections",
        "icon": "users",
        "weight": 0.125,
        "defaultWeight": 0.125,
        "sortOrder": 2,
        "isActive": true,
        "currentScore": 85.0
      }
    }
  ],
  "meta": {
    "totalWeight": 1.0
  }
}
```

### GET /api/dimensions/{id}

Get single dimension with related milestones and tasks.

**Response (200):**
```json
{
  "data": {
    "id": "d1a2b3c4-...",
    "type": "dimension",
    "attributes": {
      "code": "health",
      "name": "Health & Recovery",
      "weight": 0.15,
      "currentScore": 72.5
    },
    "relationships": {
      "milestones": [
        { "id": "m1...", "title": "Run a marathon", "status": "active" }
      ],
      "activeTasks": [
        { "id": "t1...", "title": "Morning run", "taskType": "habit" }
      ]
    }
  }
}
```

### PATCH /api/dimensions/{id}

Update dimension weight.

**Request:**
```json
{
  "weight": 0.20
}
```

**Response (200):** Updated dimension

**Validation:**
- `weight`: Decimal 0-1, all dimension weights must sum to 1.0

**Note:** When updating a weight, the API auto-adjusts other dimension weights proportionally unless `autoRebalance: false` is passed.

---

## 4. Milestones

### GET /api/milestones

List milestones with filtering.

**Query Parameters:**
- `dimensionId`: Filter by dimension
- `status`: `active` | `completed` | `abandoned`
- `sort`: `-targetDate`, `title`, etc.

**Response (200):**
```json
{
  "data": [
    {
      "id": "m1a2b3c4-...",
      "type": "milestone",
      "attributes": {
        "title": "Run a marathon",
        "description": "Complete a full marathon under 4 hours",
        "dimensionId": "d1a2b3c4-...",
        "dimensionCode": "health",
        "targetDate": "2024-12-01",
        "targetMetricCode": "running_distance_km",
        "targetMetricValue": 42.195,
        "status": "active",
        "completedAt": null,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    }
  ]
}
```

### GET /api/milestones/{id}

Get milestone with linked tasks.

### POST /api/milestones

Create milestone.

**Request:**
```json
{
  "title": "Run a marathon",
  "description": "Complete a full marathon under 4 hours",
  "dimensionId": "d1a2b3c4-...",
  "targetDate": "2024-12-01",
  "targetMetricCode": "running_distance_km",
  "targetMetricValue": 42.195
}
```

**Validation:**
- `title`: Required, 1-255 characters
- `dimensionId`: Required, valid dimension UUID
- `targetDate`: Optional, ISO date, must be future
- `targetMetricCode`: Optional, must exist in metric_definitions
- `targetMetricValue`: Required if targetMetricCode provided

### PATCH /api/milestones/{id}

Update milestone.

**Request:**
```json
{
  "title": "Run a half marathon first",
  "targetDate": "2024-06-01",
  "status": "active"
}
```

### DELETE /api/milestones/{id}

Delete milestone. Sets linked tasks' `milestone_id` to null.

**Response (204):** No content

---

## 5. Tasks & Habits

### GET /api/tasks

List tasks with filtering.

**Query Parameters:**
- `taskType`: `habit` | `one_off` | `scheduled_event`
- `dimensionId`: Filter by dimension
- `milestoneId`: Filter by milestone
- `isCompleted`: `true` | `false`
- `isActive`: `true` | `false`
- `scheduledFrom`: Date for scheduled tasks
- `scheduledTo`: Date for scheduled tasks
- `tags`: Comma-separated tag list

**Response (200):**
```json
{
  "data": [
    {
      "id": "t1a2b3c4-...",
      "type": "task",
      "attributes": {
        "title": "Morning run",
        "description": "30 minute run before breakfast",
        "taskType": "habit",
        "frequency": "daily",
        "dimensionId": "d1a2b3c4-...",
        "dimensionCode": "health",
        "milestoneId": "m1a2b3c4-...",
        "linkedMetricCode": "running_mins",
        "scheduledDate": null,
        "scheduledTime": null,
        "startDate": "2024-01-01",
        "endDate": null,
        "isCompleted": false,
        "completedAt": null,
        "isActive": true,
        "tags": ["exercise", "morning-routine"],
        "currentStreak": 15,
        "longestStreak": 23,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    }
  ]
}
```

### GET /api/tasks/{id}

Get task with streak info.

### POST /api/tasks

Create task/habit.

**Request:**
```json
{
  "title": "Morning run",
  "description": "30 minute run before breakfast",
  "taskType": "habit",
  "frequency": "daily",
  "dimensionId": "d1a2b3c4-...",
  "milestoneId": "m1a2b3c4-...",
  "linkedMetricCode": "running_mins",
  "startDate": "2024-01-01",
  "tags": ["exercise", "morning-routine"]
}
```

**Validation:**
- `title`: Required, 1-255 characters
- `taskType`: Required, `habit` | `one_off` | `scheduled_event`
- `frequency`: Required for habits, `daily` | `weekly` | `monthly` | `quarterly` | `yearly` | `ad_hoc`
- `scheduledDate`: Required for `scheduled_event`
- `linkedMetricCode`: Optional, must exist in metric_definitions

### PATCH /api/tasks/{id}

Update task.

### DELETE /api/tasks/{id}

Delete task. Also deletes associated streak.

### POST /api/tasks/{id}/complete

Mark task as complete, update streak.

**Request (optional body):**
```json
{
  "completedAt": "2024-01-15T07:30:00Z",
  "metricValue": 35
}
```

- `completedAt`: Defaults to current timestamp
- `metricValue`: If task has `linkedMetricCode`, records this value

**Response (200):**
```json
{
  "data": {
    "id": "t1a2b3c4-...",
    "type": "taskCompletion",
    "attributes": {
      "taskId": "t1a2b3c4-...",
      "completedAt": "2024-01-15T07:30:00Z",
      "currentStreak": 16,
      "longestStreak": 23,
      "metricRecorded": true
    }
  }
}
```

**Side Effects:**
- Updates `streaks` table
- If `linkedMetricCode` provided with `metricValue`, creates metric_record

---

## 6. Metrics

### GET /api/metrics/definitions

List all metric definitions.

**Query Parameters:**
- `dimensionId`: Filter by dimension
- `tags`: Comma-separated tags
- `isActive`: `true` | `false`

**Response (200):**
```json
{
  "data": [
    {
      "id": "md1a2b3c4-...",
      "type": "metricDefinition",
      "attributes": {
        "code": "weight_kg",
        "name": "Body Weight",
        "description": "Body weight in kilograms",
        "dimensionId": "d1a2b3c4-...",
        "dimensionCode": "health",
        "unit": "kg",
        "valueType": "number",
        "aggregationType": "last",
        "minValue": 20,
        "maxValue": 300,
        "targetValue": 80.0,
        "icon": "scale",
        "tags": ["body", "health"],
        "isDerived": false,
        "isActive": true
      }
    }
  ]
}
```

### GET /api/metrics/definitions/{code}

Get single metric definition by code.

### POST /api/metrics/definitions

Create custom metric definition.

**Request:**
```json
{
  "code": "coffee_cups",
  "name": "Coffee Consumption",
  "dimensionId": "d1a2b3c4-...",
  "unit": "cups",
  "valueType": "number",
  "aggregationType": "sum",
  "minValue": 0,
  "maxValue": 20,
  "targetValue": 3,
  "tags": ["nutrition", "health"]
}
```

**Response (201):**
```json
{
  "data": {
    "id": "md1a2b3c4-...",
    "type": "metricDefinition",
    "attributes": {
      "code": "coffee_cups",
      "name": "Coffee Consumption",
      "dimensionId": "d1a2b3c4-...",
      "dimensionCode": "health",
      "unit": "cups",
      "valueType": "number",
      "aggregationType": "sum",
      "minValue": 0,
      "maxValue": 20,
      "targetValue": 3,
      "tags": ["nutrition", "health"],
      "isDerived": false,
      "isActive": true
    }
  }
}
```

**Validation:**
- `code`: Required, unique, lowercase alphanumeric + underscore, 3-50 chars
- `name`: Required, 1-100 characters
- `valueType`: `number` | `boolean` | `enum` | `string`
- `aggregationType`: `sum` | `average` | `last` | `min` | `max` | `count`
- `enumValues`: Required if valueType is `enum`
- `minValue`: Optional, must be less than maxValue if both provided
- `maxValue`: Optional, must be greater than minValue if both provided
- `targetValue`: Optional, goal/target value for the metric (displayed as reference line in charts)

**Errors:**
- `409 CONFLICT` - Metric code already exists

### PATCH /api/metrics/definitions/{code}

Update an existing metric definition.

**Request:**
```json
{
  "name": "Daily Coffee Intake",
  "description": "Number of coffee cups consumed per day",
  "unit": "cups",
  "minValue": 0,
  "maxValue": 15,
  "tags": ["nutrition", "caffeine"],
  "isActive": true
}
```

**Response (200):**
```json
{
  "data": {
    "id": "md1a2b3c4-...",
    "type": "metricDefinition",
    "attributes": {
      "code": "coffee_cups",
      "name": "Daily Coffee Intake",
      "description": "Number of coffee cups consumed per day",
      "dimensionId": "d1a2b3c4-...",
      "dimensionCode": "health",
      "unit": "cups",
      "valueType": "number",
      "aggregationType": "sum",
      "minValue": 0,
      "maxValue": 15,
      "tags": ["nutrition", "caffeine"],
      "isDerived": false,
      "isActive": true
    }
  }
}
```

**Validation:**
- `name`: Optional, 1-100 characters if provided
- `code`: Cannot be changed (immutable)
- `valueType`: Cannot be changed if records exist
- `minValue`/`maxValue`: Updated bounds do not retroactively validate existing records

**Errors:**
- `404 NOT_FOUND` - Metric definition not found
- `422 VALIDATION_ERROR` - Invalid field values

### DELETE /api/metrics/definitions/{code}

Delete a metric definition. Only allowed if no metric records reference this definition.

**Response (204):** No content

**Errors:**
- `404 NOT_FOUND` - Metric definition not found
- `409 CONFLICT` - Cannot delete metric with existing records. Use soft-delete (set `isActive: false`) instead.

**Note:** For metrics with existing records, use PATCH to set `isActive: false` instead of DELETE.

### POST /api/metrics/record

**🔑 CRITICAL: Single ingestion endpoint for all metrics from n8n/automations**

Record one or more metric values in a single request.

**Request:**
```json
{
  "timestamp": "2024-01-15T07:00:00Z",
  "source": "n8n_health_sync",
  "metrics": {
    "weight_kg": 82.5,
    "body_fat_pct": 18.2,
    "steps": 10432,
    "sleep_hours": 7.5,
    "resting_hr": 58
  }
}
```

**Response (201):**
```json
{
  "data": {
    "type": "metricRecordBatch",
    "attributes": {
      "recorded": 5,
      "failed": 0,
      "timestamp": "2024-01-15T07:00:00Z",
      "source": "n8n_health_sync"
    },
    "records": [
      { "code": "weight_kg", "status": "created", "id": "mr1..." },
      { "code": "body_fat_pct", "status": "created", "id": "mr2..." },
      { "code": "steps", "status": "created", "id": "mr3..." },
      { "code": "sleep_hours", "status": "created", "id": "mr4..." },
      { "code": "resting_hr", "status": "created", "id": "mr5..." }
    ]
  }
}
```

**Validation:**
- `timestamp`: Optional, defaults to now, ISO 8601 format
- `source`: Required, identifies data source (e.g., `manual`, `n8n_health_sync`, `apple_health`)
- `metrics`: Required, object with metric codes as keys
- Each metric code must exist in `metric_definitions`
- Values validated against metric definition min/max bounds

**Partial Failure Handling:**
```json
{
  "data": {
    "type": "metricRecordBatch",
    "attributes": {
      "recorded": 4,
      "failed": 1
    },
    "records": [
      { "code": "weight_kg", "status": "created", "id": "mr1..." },
      { "code": "invalid_code", "status": "failed", "error": "Unknown metric code" }
    ]
  }
}
```

### GET /api/metrics/records

List metric records with pagination and filtering.

**Query Parameters:**
- `metricCode`: Required, filter by metric definition code
- `from`: Start date (ISO 8601), defaults to 30 days ago
- `to`: End date (ISO 8601), defaults to now
- `source`: Filter by source (e.g., `manual`, `n8n_health_sync`)
- `page`: Page number (default: 1)
- `perPage`: Items per page (default: 20, max: 100)
- `sort`: `-recordedAt` (default), `recordedAt`

**Response (200):**
```json
{
  "data": [
    {
      "id": "mr1a2b3c4-...",
      "type": "metricRecord",
      "attributes": {
        "metricCode": "weight_kg",
        "metricName": "Body Weight",
        "valueNumber": 82.5,
        "valueBoolean": null,
        "valueString": null,
        "recordedAt": "2024-01-15T07:00:00Z",
        "source": "n8n_health_sync",
        "notes": null
      }
    },
    {
      "id": "mr2a2b3c4-...",
      "type": "metricRecord",
      "attributes": {
        "metricCode": "weight_kg",
        "metricName": "Body Weight",
        "valueNumber": 82.8,
        "valueBoolean": null,
        "valueString": null,
        "recordedAt": "2024-01-14T07:00:00Z",
        "source": "manual",
        "notes": "After workout"
      }
    }
  ],
  "meta": {
    "page": 1,
    "perPage": 20,
    "total": 45,
    "totalPages": 3,
    "metricCode": "weight_kg",
    "from": "2024-01-01T00:00:00Z",
    "to": "2024-01-31T23:59:59Z"
  },
  "links": {
    "first": "/api/v1/metrics/records?metricCode=weight_kg&page=1",
    "prev": null,
    "next": "/api/v1/metrics/records?metricCode=weight_kg&page=2",
    "last": "/api/v1/metrics/records?metricCode=weight_kg&page=3"
  }
}
```

**Errors:**
- `400 BAD_REQUEST` - Missing required metricCode parameter
- `404 NOT_FOUND` - Metric definition not found

### GET /api/metrics/records/{id}

Get a single metric record by ID.

**Response (200):**
```json
{
  "data": {
    "id": "mr1a2b3c4-...",
    "type": "metricRecord",
    "attributes": {
      "metricCode": "weight_kg",
      "metricName": "Body Weight",
      "metricUnit": "kg",
      "valueNumber": 82.5,
      "valueBoolean": null,
      "valueString": null,
      "recordedAt": "2024-01-15T07:00:00Z",
      "source": "n8n_health_sync",
      "notes": null,
      "metadata": {},
      "createdAt": "2024-01-15T07:00:05Z"
    }
  }
}
```

**Errors:**
- `404 NOT_FOUND` - Metric record not found

### PATCH /api/metrics/records/{id}

Update an individual metric record value.

**Request:**
```json
{
  "valueNumber": 82.3,
  "recordedAt": "2024-01-15T07:30:00Z",
  "notes": "Corrected measurement"
}
```

**Response (200):**
```json
{
  "data": {
    "id": "mr1a2b3c4-...",
    "type": "metricRecord",
    "attributes": {
      "metricCode": "weight_kg",
      "metricName": "Body Weight",
      "valueNumber": 82.3,
      "valueBoolean": null,
      "valueString": null,
      "recordedAt": "2024-01-15T07:30:00Z",
      "source": "n8n_health_sync",
      "notes": "Corrected measurement"
    }
  }
}
```

**Validation:**
- `valueNumber`: Must be within min/max bounds of metric definition
- `valueBoolean`: Must be true/false for boolean metrics
- `valueString`: Must match enum values for enum metrics
- `recordedAt`: Optional, ISO 8601 timestamp
- `notes`: Optional, max 2000 characters
- Only one value field can be set based on metric's valueType

**Errors:**
- `404 NOT_FOUND` - Metric record not found
- `422 VALIDATION_ERROR` - Value outside bounds or wrong type

### DELETE /api/metrics/records/{id}

Delete an individual metric record.

**Response (204):** No content

**Errors:**
- `404 NOT_FOUND` - Metric record not found

### GET /api/metrics/history

Get metric history with aggregation. Returns historical data points plus target values for visualization.

**Query Parameters:**
- `codes`: Required, comma-separated metric codes
- `from`: Start date (ISO 8601)
- `to`: End date (ISO 8601)
- `granularity`: `raw` | `hourly` | `daily` | `weekly` | `monthly` (default: `raw`)
- `limit`: Max records per metric (default: 100)

**Example:**
```http
GET /api/metrics/history?codes=weight_kg,steps&from=2024-01-01&to=2024-01-31&granularity=daily
```

**Response (200):**
```json
{
  "data": {
    "weight_kg": {
      "points": [
        { "timestamp": "2024-01-01", "value": 83.2, "source": "manual" },
        { "timestamp": "2024-01-02", "value": 82.9, "source": "n8n_health_sync" },
        { "timestamp": "2024-01-03", "value": 82.5, "source": "n8n_health_sync" }
      ],
      "targetValue": 80.0
    },
    "steps": {
      "points": [
        { "timestamp": "2024-01-01", "value": 8500, "aggregation": "sum" },
        { "timestamp": "2024-01-02", "value": 12340, "aggregation": "sum" },
        { "timestamp": "2024-01-03", "value": 10432, "aggregation": "sum" }
      ],
      "targetValue": 10000
    }
  },
  "meta": {
    "from": "2024-01-01",
    "to": "2024-01-31",
    "granularity": "daily",
    "metricsReturned": ["weight_kg", "steps"]
  }
}
```

**Notes:**
- `targetValue` is included for each metric if defined in `MetricDefinition.TargetValue`
- Historical data is sorted by timestamp descending
- Aggregation is applied based on the metric's `aggregationType`
```

---

## 7. Scores

### GET /api/scores

Get all score definitions with current values.

**Response (200):**
```json
{
  "data": [
    {
      "id": "sd1a2b3c4-...",
      "type": "score",
      "attributes": {
        "code": "life_score",
        "name": "Life Score",
        "description": "Weighted aggregate across all dimensions",
        "dimensionId": null,
        "currentValue": 76.5,
        "previousValue": 74.2,
        "change": 2.3,
        "changePercent": 3.1,
        "periodType": "daily",
        "minScore": 0,
        "maxScore": 100
      }
    },
    {
      "id": "sd2a2b3c4-...",
      "type": "score",
      "attributes": {
        "code": "health_score",
        "name": "Health Score",
        "dimensionId": "d1a2b3c4-...",
        "dimensionCode": "health",
        "currentValue": 72.5,
        "previousValue": 70.0,
        "change": 2.5,
        "periodType": "daily"
      }
    }
  ]
}
```

### GET /api/scores/{code}/history

Get score history.

**Query Parameters:**
- `periodType`: `daily` | `weekly` | `monthly`
- `from`: Start date
- `to`: End date
- `limit`: Max records (default: 30)

**Response (200):**
```json
{
  "data": [
    {
      "periodStart": "2024-01-15",
      "periodEnd": "2024-01-15",
      "value": 76.5,
      "breakdown": {
        "health": { "raw": 72.5, "weighted": 10.88 },
        "relationships": { "raw": 85.0, "weighted": 10.63 },
        "work": { "raw": 68.0, "weighted": 8.50 }
      }
    }
  ],
  "meta": {
    "scoreCode": "life_score",
    "periodType": "daily"
  }
}
```

---

## 8. Streaks

### GET /api/streaks

Get all active streaks.

**Query Parameters:**
- `isActive`: `true` | `false` (default: true)
- `sort`: `-currentStreakLength`, `lastSuccessDate`

**Response (200):**
```json
{
  "data": [
    {
      "id": "st1a2b3c4-...",
      "type": "streak",
      "attributes": {
        "taskId": "t1a2b3c4-...",
        "taskTitle": "Morning run",
        "metricCode": null,
        "currentStreakLength": 15,
        "longestStreakLength": 23,
        "lastSuccessDate": "2024-01-15",
        "streakStartDate": "2024-01-01",
        "missCount": 0,
        "maxAllowedMisses": 1,
        "isActive": true
      }
    }
  ]
}
```

---

## 9. Financial - Accounts

### GET /api/accounts

List accounts.

**Query Parameters:**
- `accountType`: `bank` | `investment` | `loan` | `credit` | `crypto` | `property` | `other`
- `currency`: Filter by currency
- `isActive`: `true` | `false`
- `isLiability`: `true` | `false`

**Response (200):**
```json
{
  "data": [
    {
      "id": "a1a2b3c4-...",
      "type": "account",
      "attributes": {
        "name": "FNB Savings",
        "accountType": "bank",
        "currency": "ZAR",
        "initialBalance": 100000.00,
        "currentBalance": 152340.50,
        "currentBalanceHomeCurrency": 152340.50,
        "balanceUpdatedAt": "2024-01-15T10:00:00Z",
        "isLiability": false,
        "interestRateAnnual": 0.065,
        "interestCompounding": "monthly",
        "metadata": {
          "institution": "First National Bank",
          "accountNumber": "****4521"
        },
        "isActive": true
      }
    }
  ],
  "meta": {
    "totalAssets": 1250000.00,
    "totalLiabilities": 450000.00,
    "netWorth": 800000.00,
    "homeCurrency": "ZAR"
  }
}
```

### GET /api/accounts/{id}

Get single account with recent transactions.

### POST /api/accounts

Create account.

**Request:**
```json
{
  "name": "Bitcoin Holdings",
  "accountType": "crypto",
  "currency": "BTC",
  "initialBalance": 0.5,
  "isLiability": false,
  "interestRateAnnual": null,
  "metadata": {
    "wallet": "cold_storage",
    "address": "bc1q..."
  }
}
```

**Validation:**
- `name`: Required, 1-100 characters
- `accountType`: Required, valid enum
- `currency`: Required, 3-letter code (ZAR, USD, BTC, etc.)
- `initialBalance`: Required, decimal
- `interestRateAnnual`: Optional, decimal 0-1

### PATCH /api/accounts/{id}

Update account.

### DELETE /api/accounts/{id}

Soft-delete account (sets `isActive: false`). Preserves transaction history.

### GET /api/accounts/{id}/balance

Get current balance with FX conversion.

**Query Parameters:**
- `currency`: Target currency (default: user's home currency)

**Response (200):**
```json
{
  "data": {
    "accountId": "a1a2b3c4-...",
    "originalCurrency": "BTC",
    "originalBalance": 0.5,
    "targetCurrency": "ZAR",
    "convertedBalance": 475000.00,
    "fxRate": 950000.00,
    "fxRateTimestamp": "2024-01-15T10:00:00Z",
    "fxSource": "coingecko"
  }
}
```

---

## 10. Financial - Transactions

### GET /api/transactions

List transactions with filtering.

**Query Parameters:**
- `accountId`: Filter by source or target account
- `category`: `income` | `expense` | `transfer` | `interest` | `fee` | `tax` | `investment_contribution` | `loan_payment` | `dividend` | `capital_gain`
- `from`: Start date
- `to`: End date
- `minAmount`: Minimum amount
- `maxAmount`: Maximum amount
- `tags`: Comma-separated tags
- `isReconciled`: `true` | `false`

**Response (200):**
```json
{
  "data": [
    {
      "id": "tx1a2b3c4-...",
      "type": "transaction",
      "attributes": {
        "sourceAccountId": "a1a2b3c4-...",
        "sourceAccountName": "FNB Current",
        "targetAccountId": null,
        "currency": "ZAR",
        "amount": -1500.00,
        "amountHomeCurrency": -1500.00,
        "fxRateUsed": 1.0,
        "category": "expense",
        "subcategory": "groceries",
        "tags": ["food", "weekly-shop"],
        "description": "Woolworths weekly groceries",
        "notes": null,
        "transactionDate": "2024-01-15",
        "recordedAt": "2024-01-15T18:30:00Z",
        "source": "manual",
        "isReconciled": false
      }
    }
  ]
}
```

### POST /api/transactions

Create transaction.

**Request:**
```json
{
  "sourceAccountId": "a1a2b3c4-...",
  "targetAccountId": null,
  "currency": "ZAR",
  "amount": 1500.00,
  "category": "expense",
  "subcategory": "groceries",
  "tags": ["food", "weekly-shop"],
  "description": "Woolworths weekly groceries",
  "transactionDate": "2024-01-15"
}
```

**Validation:**
- At least one of `sourceAccountId` or `targetAccountId` required
- `amount`: Required, positive decimal
- `category`: Required, valid enum
- `transactionDate`: Required, ISO date

**Side Effects:**
- Updates `current_balance` on affected accounts
- Converts to home currency using latest FX rate

### PATCH /api/transactions/{id}

Update transaction. Recalculates account balances.

### DELETE /api/transactions/{id}

Delete transaction. Recalculates account balances.

---

## 11. Financial - FX Rates

### GET /api/fx-rates

Get current FX rates for all tracked currency pairs.

**Response (200):**
```json
{
  "data": [
    {
      "baseCurrency": "USD",
      "quoteCurrency": "ZAR",
      "rate": 18.75,
      "rateTimestamp": "2024-01-15T10:00:00Z",
      "source": "coingecko"
    },
    {
      "baseCurrency": "BTC",
      "quoteCurrency": "ZAR",
      "rate": 950000.00,
      "rateTimestamp": "2024-01-15T10:00:00Z",
      "source": "coingecko"
    }
  ],
  "meta": {
    "homeCurrency": "ZAR",
    "lastRefresh": "2024-01-15T10:00:00Z",
    "nextScheduledRefresh": "2024-01-15T11:00:00Z"
  }
}
```

### POST /api/fx-rates/refresh

Trigger manual FX rate refresh.

**Response (200):**
```json
{
  "data": {
    "refreshed": 3,
    "pairs": ["USD/ZAR", "BTC/ZAR", "BTC/USD"],
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

---

## 12. Financial - Income Sources

### GET /api/income-sources

List income sources.

**Response (200):**
```json
{
  "data": [
    {
      "id": "is1a2b3c4-...",
      "type": "incomeSource",
      "attributes": {
        "name": "Primary Salary",
        "currency": "ZAR",
        "baseAmount": 85000.00,
        "isPreTax": true,
        "taxProfileId": "tp1a2b3c4-...",
        "paymentFrequency": "monthly",
        "nextPaymentDate": "2024-01-25",
        "annualIncreaseRate": 0.05,
        "employerName": "Acme Corp",
        "isActive": true
      }
    }
  ],
  "meta": {
    "totalMonthlyGross": 95000.00,
    "totalMonthlyNet": 68500.00
  }
}
```

### POST /api/income-sources

Create income source.

**Request:**
```json
{
  "name": "Side Consulting",
  "currency": "USD",
  "baseAmount": 2000.00,
  "isPreTax": true,
  "paymentFrequency": "monthly",
  "annualIncreaseRate": 0.03
}
```

### PATCH /api/income-sources/{id}

Update income source.

### DELETE /api/income-sources/{id}

Delete income source.

---

## 13. Financial - Expense Definitions

### GET /api/expense-definitions

List recurring expense definitions.

**Response (200):**
```json
{
  "data": [
    {
      "id": "ed1a2b3c4-...",
      "type": "expenseDefinition",
      "attributes": {
        "name": "Rent",
        "currency": "ZAR",
        "amountType": "fixed",
        "amountValue": 15000.00,
        "amountFormula": null,
        "frequency": "monthly",
        "category": "housing",
        "isTaxDeductible": false,
        "linkedAccountId": "a1a2b3c4-...",
        "inflationAdjusted": true,
        "isActive": true
      }
    }
  ],
  "meta": {
    "totalMonthly": 35000.00,
    "byCategory": {
      "housing": 15000.00,
      "utilities": 3500.00,
      "insurance": 2500.00
    }
  }
}
```

### POST /api/expense-definitions

Create expense definition.

**Request:**
```json
{
  "name": "Medical Aid",
  "currency": "ZAR",
  "amountType": "fixed",
  "amountValue": 4500.00,
  "frequency": "monthly",
  "category": "health",
  "isTaxDeductible": true,
  "inflationAdjusted": true
}
```

### PATCH /api/expense-definitions/{id}

Update expense definition.

### DELETE /api/expense-definitions/{id}

Delete expense definition.

---

## 14. Financial - Tax Profiles

### GET /api/tax-profiles

List tax profiles.

**Response (200):**
```json
{
  "data": [
    {
      "id": "tp1a2b3c4-...",
      "type": "taxProfile",
      "attributes": {
        "name": "SA Tax 2024",
        "taxYear": 2024,
        "countryCode": "ZA",
        "brackets": [
          { "min": 0, "max": 237100, "rate": 0.18, "baseTax": 0 },
          { "min": 237101, "max": 370500, "rate": 0.26, "baseTax": 42678 },
          { "min": 370501, "max": 512800, "rate": 0.31, "baseTax": 77362 },
          { "min": 512801, "max": 673000, "rate": 0.36, "baseTax": 121475 },
          { "min": 673001, "max": 857900, "rate": 0.39, "baseTax": 179147 },
          { "min": 857901, "max": 1817000, "rate": 0.41, "baseTax": 251258 },
          { "min": 1817001, "max": null, "rate": 0.45, "baseTax": 644489 }
        ],
        "uifRate": 0.01,
        "uifCap": 17712.00,
        "vatRate": 0.15,
        "isVatRegistered": false,
        "taxRebates": {
          "primary": 17235,
          "secondary": 9444,
          "tertiary": 3145
        },
        "isActive": true
      }
    }
  ]
}
```

### POST /api/tax-profiles

Create tax profile.

### PATCH /api/tax-profiles/{id}

Update tax profile.

---

## 15. Financial - Simulation

### GET /api/simulations/scenarios

List simulation scenarios.

**Response (200):**
```json
{
  "data": [
    {
      "id": "ss1a2b3c4-...",
      "type": "simulationScenario",
      "attributes": {
        "name": "Baseline Projection",
        "description": "Current trajectory with no major changes",
        "startDate": "2024-01-01",
        "endDate": "2054-01-01",
        "endCondition": null,
        "baseAssumptions": {
          "inflationRate": 0.05,
          "growthRates": {
            "default": 0.07,
            "accounts": {
              "a1...": 0.10,
              "a2...": 0.05
            }
          }
        },
        "isBaseline": true,
        "lastRunAt": "2024-01-15T06:00:00Z"
      }
    }
  ]
}
```

### POST /api/simulations/scenarios

Create simulation scenario.

**Request:**
```json
{
  "name": "Early Retirement",
  "description": "Aggressive savings, retire at 55",
  "startDate": "2024-01-01",
  "endDate": "2050-01-01",
  "baseAssumptions": {
    "inflationRate": 0.05,
    "growthRates": { "default": 0.08 }
  }
}
```

### PATCH /api/simulations/scenarios/{id}

Update scenario.

### DELETE /api/simulations/scenarios/{id}

Delete scenario and all associated projections.

### POST /api/simulations/scenarios/{id}/run

Execute simulation and generate projections.

**Request (optional):**
```json
{
  "recalculateFromStart": true
}
```

**Response (200):**
```json
{
  "data": {
    "scenarioId": "ss1a2b3c4-...",
    "status": "completed",
    "periodsCalculated": 360,
    "executionTimeMs": 1250,
    "startDate": "2024-01-01",
    "endDate": "2054-01-01",
    "keyMilestones": [
      { "description": "Net worth reaches R1M", "date": "2026-08-01" },
      { "description": "Net worth reaches $100K USD", "date": "2028-03-01" },
      { "description": "Retirement age reached", "date": "2045-06-15" }
    ]
  }
}
```

### GET /api/simulations/scenarios/{id}/projections

Get simulation projections.

**Query Parameters:**
- `from`: Start date
- `to`: End date
- `granularity`: `monthly` | `quarterly` | `yearly` (default: monthly)
- `accountId`: Filter by specific account

**Response (200):**
```json
{
  "data": {
    "netWorth": [
      {
        "periodDate": "2024-01-01",
        "totalAssets": 1250000.00,
        "totalLiabilities": 450000.00,
        "netWorth": 800000.00,
        "breakdownByType": {
          "bank": 200000.00,
          "investment": 500000.00,
          "property": 550000.00,
          "loan": -450000.00
        },
        "breakdownByCurrency": {
          "ZAR": 750000.00,
          "USD": 30000.00,
          "BTC": 20000.00
        }
      }
    ],
    "accounts": {
      "a1...": [
        { "periodDate": "2024-01-01", "balance": 152340.50, "balanceHomeCurrency": 152340.50 }
      ]
    }
  }
}
```

### GET /api/simulations/events

List simulation events.

**Query Parameters:**
- `scenarioId`: Filter by scenario

**Response (200):**
```json
{
  "data": [
    {
      "id": "se1a2b3c4-...",
      "type": "simulationEvent",
      "attributes": {
        "scenarioId": "ss1a2b3c4-...",
        "name": "Buy investment property",
        "description": "Purchase second property for rental income",
        "triggerType": "date",
        "triggerDate": "2025-06-01",
        "triggerAge": null,
        "triggerCondition": null,
        "eventType": "purchase",
        "currency": "ZAR",
        "amountType": "fixed",
        "amountValue": 2000000.00,
        "affectedAccountId": "a1...",
        "appliesOnce": true,
        "sortOrder": 1,
        "isActive": true
      }
    }
  ]
}
```

### POST /api/simulations/events

Create simulation event.

**Request:**
```json
{
  "scenarioId": "ss1a2b3c4-...",
  "name": "Salary increase",
  "triggerType": "date",
  "triggerDate": "2024-07-01",
  "eventType": "income_change",
  "amountType": "percentage",
  "amountValue": 0.15,
  "appliesOnce": true
}
```

### PATCH /api/simulations/events/{id}

Update simulation event.

### DELETE /api/simulations/events/{id}

Delete simulation event.

---

## 16. Financial Goals

### GET /api/financial-goals

List financial goals with time-to-acquire projections.

**Query Parameters:**
- `status`: `active` | `completed` | `cancelled` (default: active)
- `accountId`: Filter by linked account
- `sort`: `-targetDate`, `name`, `-projectedAcquireDate`

**Response (200):**
```json
{
  "data": [
    {
      "id": "fg1a2b3c4-...",
      "type": "financialGoal",
      "attributes": {
        "name": "Emergency Fund",
        "description": "6 months of expenses in savings",
        "targetAmount": 150000.00,
        "currentAmount": 75000.00,
        "currency": "ZAR",
        "targetDate": "2024-12-31",
        "projectedAcquireDate": "2024-10-15",
        "linkedAccountId": "a1...",
        "linkedAccountName": "FNB Savings",
        "priority": 1,
        "status": "active",
        "monthlyContribution": 5000.00,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    }
  ],
  "meta": {
    "totalGoals": 5,
    "activeGoals": 3,
    "completedGoals": 2,
    "totalTargetAmount": 500000.00,
    "totalCurrentAmount": 225000.00,
    "overallProgress": 45.0
  }
}
```

### GET /api/financial-goals/{id}

Get financial goal with projection details.

**Response (200):**
```json
{
  "data": {
    "id": "fg1a2b3c4-...",
    "type": "financialGoal",
    "attributes": {
      "name": "Emergency Fund",
      "description": "6 months of expenses in savings",
      "targetAmount": 150000.00,
      "currentAmount": 75000.00,
      "currency": "ZAR",
      "targetDate": "2024-12-31",
      "projectedAcquireDate": "2024-10-15",
      "linkedAccountId": "a1...",
      "linkedAccountName": "FNB Savings",
      "priority": 1,
      "status": "active",
      "monthlyContribution": 5000.00,
      "interestRateApplied": 0.065,
      "projectionDetails": {
        "monthsToGoal": 8,
        "totalContributions": 40000.00,
        "projectedInterest": 2500.00,
        "onTrack": true,
        "daysAheadOfSchedule": 77
      },
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-15T10:00:00Z"
    }
  }
}
```

### POST /api/financial-goals

Create financial goal.

**Request:**
```json
{
  "name": "New Car Down Payment",
  "description": "20% down payment for new vehicle",
  "targetAmount": 80000.00,
  "currency": "ZAR",
  "targetDate": "2025-06-01",
  "linkedAccountId": "a1...",
  "priority": 2,
  "monthlyContribution": 3500.00
}
```

**Response (201):** Created financial goal with projectedAcquireDate calculated

**Validation:**
- `name`: Required, 1-100 characters
- `targetAmount`: Required, positive decimal
- `currency`: 3-letter ISO code
- `targetDate`: Optional, ISO date, must be future
- `linkedAccountId`: Optional, valid account UUID
- `priority`: Integer 1-10
- `monthlyContribution`: Optional, positive decimal

### PATCH /api/financial-goals/{id}

Update financial goal.

**Request:**
```json
{
  "targetAmount": 100000.00,
  "monthlyContribution": 5000.00,
  "status": "active"
}
```

**Response (200):** Updated financial goal with recalculated projectedAcquireDate

### DELETE /api/financial-goals/{id}

Delete financial goal.

**Response (204):** No content

---

## 17. Loan Payoff Projections

### GET /api/accounts/{id}/payoff-projection

Get payoff projection for a liability account.

**Query Parameters:**
- `extraMonthly`: Additional monthly payment (default: 0)
- `lumpSum`: One-time extra payment amount (default: 0)
- `lumpSumDate`: Date for lump sum payment

**Response (200):**
```json
{
  "data": {
    "accountId": "a1...",
    "accountName": "Home Loan",
    "currentBalance": -1500000.00,
    "interestRateAnnual": 0.115,
    "currentMonthlyPayment": 15000.00,
    "payoffProjection": {
      "standard": {
        "payoffDate": "2044-06-01",
        "totalMonths": 240,
        "totalInterest": 2100000.00,
        "totalPayments": 3600000.00
      },
      "withExtraPayments": {
        "payoffDate": "2038-03-01",
        "totalMonths": 165,
        "totalInterest": 1350000.00,
        "totalPayments": 2850000.00,
        "interestSaved": 750000.00,
        "timeSaved": "6 years 3 months"
      }
    },
    "amortizationSchedule": [
      {
        "month": 1,
        "date": "2024-02-01",
        "payment": 15000.00,
        "principal": 700.00,
        "interest": 14375.00,
        "balance": -1499300.00
      }
    ]
  }
}
```

**Notes:**
- Only applicable to liability accounts (`isLiability: true`)
- Returns 400 if account is not a liability
- Amortization schedule limited to first 12 months by default, use `?scheduleMonths=all` for full schedule

---

## 18. Dashboard

### GET /api/dashboard

Aggregated dashboard view.

**Response (200):**
```json
{
  "data": {
    "type": "dashboard",
    "attributes": {
      "netWorth": {
        "current": 800000.00,
        "previousMonth": 785000.00,
        "change": 15000.00,
        "changePercent": 1.91,
        "currency": "ZAR"
      },
      "lifeScore": {
        "current": 76.5,
        "previousWeek": 74.2,
        "change": 2.3,
        "trend": "up"
      },
      "dimensionScores": [
        { "code": "health", "name": "Health", "score": 72.5, "weight": 0.15 },
        { "code": "relationships", "name": "Relationships", "score": 85.0, "weight": 0.125 },
        { "code": "work", "name": "Work", "score": 68.0, "weight": 0.125 }
      ],
      "topStreaks": [
        { "taskTitle": "Morning run", "currentStreak": 15, "longestStreak": 23 },
        { "taskTitle": "Meditation", "currentStreak": 42, "longestStreak": 42 }
      ],
      "recentActivity": [
        { "type": "task_completed", "title": "Morning run", "timestamp": "2024-01-15T07:30:00Z" },
        { "type": "metric_recorded", "metric": "weight_kg", "value": 82.5, "timestamp": "2024-01-15T07:00:00Z" }
      ],
      "upcomingTasks": [
        { "id": "t1...", "title": "Quarterly review", "scheduledDate": "2024-01-20", "taskType": "scheduled_event" }
      ]
    }
  }
}
```

### GET /api/dashboard/net-worth

Get detailed net worth breakdown.

**Query Parameters:**
- `currency`: Target currency (default: user's home currency)

**Response (200):**
```json
{
  "data": {
    "totalAssets": 1250000.00,
    "totalLiabilities": 450000.00,
    "netWorth": 800000.00,
    "currency": "ZAR",
    "byAccountType": {
      "bank": { "count": 2, "total": 200000.00 },
      "investment": { "count": 3, "total": 500000.00 },
      "crypto": { "count": 1, "total": 50000.00 },
      "property": { "count": 1, "total": 500000.00 },
      "loan": { "count": 1, "total": -450000.00 }
    },
    "byCurrency": {
      "ZAR": 750000.00,
      "USD": 30000.00,
      "BTC": 20000.00
    },
    "history": [
      { "date": "2024-01-01", "netWorth": 785000.00 },
      { "date": "2024-01-08", "netWorth": 790000.00 },
      { "date": "2024-01-15", "netWorth": 800000.00 }
    ]
  }
}
```

### GET /api/dashboard/projections

Get key milestone projections.

**Response (200):**
```json
{
  "data": {
    "milestones": [
      {
        "description": "Net worth reaches R1M",
        "projectedDate": "2026-08-01",
        "confidence": "high",
        "scenario": "Baseline Projection"
      },
      {
        "description": "Net worth reaches $100K USD",
        "projectedDate": "2028-03-01",
        "confidence": "medium",
        "scenario": "Baseline Projection"
      },
      {
        "description": "Financially independent (25x expenses)",
        "projectedDate": "2042-01-01",
        "confidence": "low",
        "scenario": "Baseline Projection"
      }
    ],
    "comparisons": [
      {
        "milestone": "Retirement at 55",
        "baselineDate": "2040-06-15",
        "earlyRetirementDate": "2035-06-15",
        "difference": "-5 years"
      }
    ]
  }
}
```

---

## 19. Health & Longevity

### GET /api/longevity

Get current longevity estimate.

**Response (200):**
```json
{
  "data": {
    "type": "longevity",
    "attributes": {
      "baselineLifeExpectancy": 80.0,
      "estimatedYearsAdded": 4.5,
      "adjustedLifeExpectancy": 84.5,
      "estimatedDeathDate": "2069-06-15",
      "confidenceLevel": "moderate",
      "calculatedAt": "2024-01-15T06:00:00Z",
      "breakdown": [
        {
          "modelCode": "exercise_cardio",
          "modelName": "Cardiovascular Exercise",
          "yearsAdded": 2.5,
          "inputValues": { "steps": 10000, "exercise_mins": 45 },
          "notes": "Above average activity level"
        },
        {
          "modelCode": "sleep_quality",
          "modelName": "Sleep Quality",
          "yearsAdded": 0.5,
          "inputValues": { "sleep_hours": 7.2 },
          "notes": "Optimal sleep duration"
        },
        {
          "modelCode": "nutrition",
          "modelName": "Nutrition",
          "yearsAdded": 1.5,
          "inputValues": { "fruit_veg_servings": 6 },
          "notes": "Good nutrition habits"
        }
      ],
      "recommendations": [
        {
          "area": "Exercise",
          "suggestion": "Increase daily steps to 12000 for additional 0.5 years",
          "potentialGain": 0.5
        }
      ]
    }
  }
}
```

### GET /api/longevity/history

Get longevity estimate history.

**Query Parameters:**
- `from`: Start date
- `to`: End date
- `limit`: Max records (default: 30)

**Response (200):**
```json
{
  "data": [
    {
      "calculatedAt": "2024-01-15T06:00:00Z",
      "baselineLifeExpectancy": 80.0,
      "estimatedYearsAdded": 4.5,
      "adjustedLifeExpectancy": 84.5
    },
    {
      "calculatedAt": "2024-01-08T06:00:00Z",
      "baselineLifeExpectancy": 80.0,
      "estimatedYearsAdded": 4.2,
      "adjustedLifeExpectancy": 84.2
    }
  ]
}
```

---

## 20. Webhooks

### POST /api/webhooks/simulation

Trigger simulation run from external automation (n8n).

**Request:**
```json
{
  "scenarioId": "ss1a2b3c4-...",
  "webhookSecret": "wh_secret_xxx"
}
```

**Response (200):**
```json
{
  "data": {
    "status": "queued",
    "jobId": "job_12345",
    "scenarioId": "ss1a2b3c4-...",
    "estimatedCompletionSeconds": 30
  }
}
```

**Authentication:** Webhook secret in request body (alternative to JWT for automation)

---

## Validation Rules

### Common Patterns

| Field Type | Pattern | Example |
|------------|---------|---------|
| UUID | RFC 4122 | `550e8400-e29b-41d4-a716-446655440000` |
| Currency | ISO 4217 (3 chars) | `ZAR`, `USD`, `BTC` |
| Date | ISO 8601 | `2024-01-15` |
| DateTime | ISO 8601 with timezone | `2024-01-15T10:30:00Z` |
| Decimal (money) | Up to 18,4 precision | `1234567890.1234` |
| Percentage | Decimal 0-1 | `0.05` for 5% |

### Field Constraints

| Field | Min | Max | Required |
|-------|-----|-----|----------|
| `title/name` | 1 | 255 | Yes |
| `description` | 0 | 2000 | No |
| `tags` | 0 | 20 items | No |
| `amount` | -999999999999 | 999999999999 | Context |
| `weight` | 0.0 | 1.0 | Yes |
| `score` | 0 | 100 | Auto |

---

## Error Handling

### Validation Error Details

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Request validation failed",
    "details": [
      {
        "field": "amount",
        "code": "RANGE",
        "message": "Amount must be between 0 and 999999999999",
        "params": { "min": 0, "max": 999999999999 }
      }
    ]
  }
}
```

### Business Logic Errors

```json
{
  "error": {
    "code": "BUSINESS_RULE_VIOLATION",
    "message": "Cannot delete account with non-zero balance",
    "details": [
      {
        "rule": "ACCOUNT_BALANCE_ZERO",
        "context": { "currentBalance": 1500.00 }
      }
    ]
  }
}
```

---

## OpenAPI Specification

Full OpenAPI 3.0 specification available at:
- Development: `http://localhost:5000/swagger`
- Production: `https://api.lifeos.app/swagger`

### Swagger UI

Interactive documentation at `/swagger/index.html`

### Export Formats

- OpenAPI JSON: `/swagger/v1/swagger.json`
- OpenAPI YAML: `/swagger/v1/swagger.yaml`

---

## Background Jobs

### Scheduled Tasks (Hangfire)

| Job | Schedule | Description |
|-----|----------|-------------|
| FX Rate Refresh | Hourly | Fetches latest rates from CoinGecko |
| Score Calculation | Daily 2AM | Recalculates all dimension and life scores |
| Streak Update | Daily 12:01AM | Updates streak counters, handles misses |
| Longevity Calc | Weekly Sun 3AM | Recalculates longevity estimates |
| Simulation Run | On-demand | Triggered via API or webhook |

### Job Status API

```http
GET /api/jobs/status/{jobId}
```

**Response (200):**
```json
{
  "data": {
    "jobId": "job_12345",
    "type": "simulation_run",
    "status": "completed",
    "startedAt": "2024-01-15T10:30:00Z",
    "completedAt": "2024-01-15T10:30:45Z",
    "result": { "periodsCalculated": 360 }
  }
}
```