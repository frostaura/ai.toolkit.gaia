# API Design

## API Standards

### Base URL
- Development: `http://localhost:3000/api`
- Staging: `https://staging.example.com/api`
- Production: `https://api.example.com`

### Versioning
- URL versioning: `/api/v1/`
- Header versioning: `Accept: application/vnd.api+json;version=1`

### Authentication
- Bearer token: `Authorization: Bearer <token>`
- API Key: `X-API-Key: <key>`
- Session cookie: `sessionId`

## REST Conventions

### HTTP Methods
- `GET` - Read resources
- `POST` - Create resources
- `PUT` - Full update
- `PATCH` - Partial update
- `DELETE` - Remove resources

### Status Codes
- `200` - Success
- `201` - Created
- `204` - No content
- `400` - Bad request
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not found
- `422` - Validation error
- `429` - Rate limited
- `500` - Server error

### Response Format

#### Success Response
```json
{
  "data": {
    "id": "123",
    "type": "user",
    "attributes": {}
  },
  "meta": {
    "timestamp": "2024-01-01T00:00:00Z"
  }
}
```

#### Error Response
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format"
      }
    ]
  }
}
```

#### Pagination
```json
{
  "data": [],
  "meta": {
    "page": 1,
    "perPage": 20,
    "total": 100,
    "totalPages": 5
  },
  "links": {
    "first": "/api/v1/users?page=1",
    "prev": null,
    "next": "/api/v1/users?page=2",
    "last": "/api/v1/users?page=5"
  }
}
```

## Core Endpoints

### Authentication
- `POST /auth/login` - User login
- `POST /auth/logout` - User logout
- `POST /auth/refresh` - Refresh token
- `POST /auth/register` - User registration
- `POST /auth/forgot-password` - Password reset

### Users
- `GET /users` - List users
- `GET /users/:id` - Get user
- `POST /users` - Create user
- `PATCH /users/:id` - Update user
- `DELETE /users/:id` - Delete user

### Resources (Example)
- `GET /resources` - List with filters
- `GET /resources/:id` - Get single
- `POST /resources` - Create new
- `PATCH /resources/:id` - Update
- `DELETE /resources/:id` - Delete

## Query Parameters

### Filtering
```
GET /api/v1/users?status=active&role=admin
```

### Sorting
```
GET /api/v1/users?sort=-created_at,name
```

### Pagination
```
GET /api/v1/users?page=2&limit=20
```

### Field Selection
```
GET /api/v1/users?fields=id,name,email
```

### Relationships
```
GET /api/v1/users?include=posts,comments
```

## Rate Limiting

### Headers
- `X-RateLimit-Limit`: Request limit
- `X-RateLimit-Remaining`: Remaining requests
- `X-RateLimit-Reset`: Reset timestamp

### Limits
- Anonymous: 100 requests/hour
- Authenticated: 1000 requests/hour
- Premium: 10000 requests/hour

## WebSocket Events

### Connection
```javascript
ws://localhost:3000/socket
```

### Events
- `connection` - Client connected
- `disconnect` - Client disconnected
- `message` - New message
- `notification` - Push notification
- `update` - Real-time update

## API Documentation

### OpenAPI/Swagger
Available at `/api/docs`

### Postman Collection
Export available at `/api/postman`

## Testing

### Example CURL
```bash
curl -X GET https://api.example.com/v1/users \
  -H "Authorization: Bearer token" \
  -H "Accept: application/json"
```

### Example Response
```json
{
  "data": [...],
  "meta": {...}
}
```