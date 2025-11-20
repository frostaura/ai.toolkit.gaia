<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/7-infrastructure.md -->

[<< Back](./design.md)

# Docker Support & Local Development

Quick-start guide for running the application locally with Docker.

> **📖 For comprehensive infrastructure and deployment architecture**, see [Infrastructure & DevOps Design](./7-infrastructure.md)

## Prerequisites

**Required Software**:
- **Docker Desktop 24+**: [Download here](https://www.docker.com/products/docker-desktop)
- **Docker Compose**: Included with Docker Desktop
- **Git**: For cloning the repository

**System Requirements**:
- **RAM**: 8GB minimum (16GB recommended)
- **Disk Space**: 10GB free space
- **OS**: Linux, macOS, or Windows 10/11 with WSL2

**Windows Users**: After installation, ensure Docker is configured for Linux containers (not Windows containers). Right-click the Docker icon in the system tray and switch if needed.

## Quick Start (3 Commands)

```bash
# 1. Clone repository (if not already done)
git clone <REPOSITORY_URL>
cd <PROJECT_NAME>

# 2. Start all services
docker compose up -d

# 3. Verify services are running
docker compose ps
```

**Access the application**:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger
- **Database**: localhost:5432 (PostgreSQL)

## Docker Compose Services

The `docker-compose.yml` orchestrates the complete application stack:

```yaml
services:
  frontend:
    build: ./src/frontend
    ports:
      - "3000:3000"
    environment:
      - REACT_APP_API_URL=http://localhost:5000
    depends_on:
      - backend

  backend:
    build: ./src/backend
    ports:
      - "5000:5000"
    environment:
      - DATABASE_URL=postgresql://user:password@database:5432/appdb
    depends_on:
      - database

  database:
    image: postgres:15-alpine
    ports:
      - "5432:5432"
    environment:
      - POSTGRES_DB=appdb
      - POSTGRES_USER=user
      - POSTGRES_PASSWORD=password
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## Common Commands

**Start Services**:
```bash
# Start all services in foreground (see logs)
docker compose up

# Start all services in background (detached)
docker compose up -d

# Start specific service only
docker compose up frontend
```

**Stop Services**:
```bash
# Stop all services (keeps containers)
docker compose stop

# Stop and remove containers (data volumes preserved)
docker compose down

# Stop and remove everything including volumes (fresh start)
docker compose down -v
```

**View Logs**:
```bash
# View logs for all services
docker compose logs

# Follow logs in real-time
docker compose logs -f

# View logs for specific service
docker compose logs backend

# View last 100 lines
docker compose logs --tail=100
```

**Rebuild Services**:
```bash
# Rebuild all services (after code changes)
docker compose build

# Rebuild specific service
docker compose build backend

# Rebuild and restart
docker compose up -d --build
```

**Execute Commands Inside Containers**:
```bash
# Open shell in backend container
docker compose exec backend sh

# Run database migrations
docker compose exec backend dotnet ef database update

# Access PostgreSQL CLI
docker compose exec database psql -U user -d appdb
```

## Development Workflow

**Hot Reload Development**:
```yaml
# docker-compose.override.yml (for local development)
services:
  frontend:
    volumes:
      - ./src/frontend:/app
      - /app/node_modules
    environment:
      - FAST_REFRESH=true

  backend:
    volumes:
      - ./src/backend:/app
    command: dotnet watch run
```

**Apply override**:
```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml up
```

## Troubleshooting

**Port Conflicts**:
```bash
# Check what's using a port (macOS/Linux)
lsof -i :3000

# Check what's using a port (Windows)
netstat -ano | findstr :3000

# Modify port in docker-compose.yml
ports:
  - "3001:3000"  # Map host port 3001 to container port 3000
```

**Container Won't Start**:
```bash
# View detailed logs
docker compose logs backend

# Check container status
docker compose ps

# Restart specific service
docker compose restart backend
```

**Database Connection Issues**:
```bash
# Verify database is running
docker compose ps database

# Check database logs
docker compose logs database

# Test connection from backend
docker compose exec backend ping database
```

**Out of Disk Space**:
```bash
# Remove unused images and containers
docker system prune -a

# Remove unused volumes (WARNING: deletes data)
docker volume prune
```

**Rebuild from Scratch**:
```bash
# Nuclear option: remove everything and start fresh
docker compose down -v
docker system prune -a --volumes
docker compose up -d --build
```

## Production Deployment

**Docker Hub** (if configured):
```bash
# Pull pre-built images
docker pull <DOCKER_USERNAME>/<APPLICATION>:latest

# Run with production compose file
docker compose -f docker-compose.prod.yml up -d
```

**Registry URL**: https://hub.docker.com/r/<DOCKER_USERNAME>/<APPLICATION>

**Environment Variables**:
Create `.env` file for production secrets:
```bash
DATABASE_URL=postgresql://prod_user:secure_password@prod-db:5432/prod_db
JWT_SECRET=your-production-secret
API_BASE_URL=https://api.yourdomain.com
```

## Health Checks

**Verify Services**:
```bash
# Check all containers are healthy
docker compose ps

# Test frontend
curl http://localhost:3000

# Test backend API
curl http://localhost:5000/health

# Test database connection
docker compose exec database pg_isready -U user
```

**Automated Health Checks** (in Dockerfile):
```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s \
  CMD curl -f http://localhost:5000/health || exit 1
```

## Performance Optimization

**Multi-Stage Builds** (reduce image size):
```dockerfile
# Build stage
FROM node:18-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build

# Production stage
FROM node:18-alpine
WORKDIR /app
COPY --from=builder /app/dist ./dist
COPY --from=builder /app/node_modules ./node_modules
CMD ["node", "dist/server.js"]
```

**Build Cache Optimization**:
- Copy package files first (cache dependencies)
- Copy source code last (invalidate cache less frequently)
- Use `.dockerignore` to exclude unnecessary files

## Next Steps

- **Architecture Details**: See [Infrastructure Design](./7-infrastructure.md) for complete infrastructure architecture
- **Deployment Strategies**: Learn about blue-green, canary deployments in infrastructure docs
- **Monitoring**: Configure observability in [Observability Design](./9-observability.md)
- **Scaling**: Review [Scalability Design](./10-scalability.md) for production scaling strategies

[<< Back](./design.md)
