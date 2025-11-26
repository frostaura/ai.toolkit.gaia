# TaleWeaver Production Deployment Guide

This guide provides step-by-step instructions for deploying TaleWeaver to a production environment using Docker Compose.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Initial Setup](#initial-setup)
- [First Deployment](#first-deployment)
- [Database Migrations](#database-migrations)
- [AI Model Management](#ai-model-management)
- [Health Checks](#health-checks)
- [SSL/TLS Setup](#ssltls-setup)
- [Backup Procedures](#backup-procedures)
- [Update Procedures](#update-procedures)
- [Monitoring](#monitoring)
- [Troubleshooting](#troubleshooting)
- [Rollback Strategy](#rollback-strategy)

## Prerequisites

### System Requirements
- **OS**: Ubuntu 22.04 LTS or similar Linux distribution
- **CPU**: 8+ cores recommended
- **RAM**: 32GB minimum (64GB recommended for GPU workloads)
- **Storage**: 500GB SSD minimum
- **GPU**: NVIDIA GPU with 16GB+ VRAM (for AI models)

### Software Dependencies
```bash
# Docker (version 24.0+)
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Docker Compose (v2.20+)
sudo apt-get install docker-compose-plugin

# NVIDIA Container Toolkit (for GPU support)
distribution=$(. /etc/os-release;echo $ID$VERSION_ID)
curl -s -L https://nvidia.github.io/nvidia-docker/gpgkey | sudo apt-key add -
curl -s -L https://nvidia.github.io/nvidia-docker/$distribution/nvidia-docker.list | \
  sudo tee /etc/apt/sources.list.d/nvidia-docker.list
sudo apt-get update && sudo apt-get install -y nvidia-container-toolkit
sudo systemctl restart docker
```

### Domain & DNS
- Domain name pointed to server IP
- DNS A records configured:
  - `yourdomain.com` → Server IP
  - `api.yourdomain.com` → Server IP

## Initial Setup

### 1. Clone Repository
```bash
cd /opt
sudo git clone https://github.com/yourusername/taleweaver.git
cd taleweaver
```

### 2. Configure Environment Variables
```bash
# Copy production environment template
cp .env.prod.example .env.prod

# Edit with your production values
nano .env.prod
```

**Critical values to configure:**
- `POSTGRES_PASSWORD`: Strong database password
- `JWT_SECRET`: Generate with `openssl rand -base64 64`
- `STRIPE_SECRET_KEY`: Production Stripe key
- `STRIPE_PUBLISHABLE_KEY`: Production Stripe key
- `STRIPE_WEBHOOK_SECRET`: From Stripe dashboard
- `VAPID_PUBLIC_KEY` & `VAPID_PRIVATE_KEY`: Generate with `npx web-push generate-vapid-keys`
- `DOMAIN`: Your domain name
- `LETSENCRYPT_EMAIL`: Your email for SSL certificates

### 3. Secure Configuration Files
```bash
# Restrict access to environment file
chmod 600 .env.prod

# Ensure only root can read
sudo chown root:root .env.prod
```

## First Deployment

### 1. Pull Latest Images
```bash
# Login to GitHub Container Registry
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Pull images from .env.prod configuration
docker-compose -f docker-compose.prod.yml pull
```

### 2. Start All Services
```bash
# Start in detached mode
docker-compose -f docker-compose.prod.yml up -d

# View logs for all services
docker-compose -f docker-compose.prod.yml logs -f

# View logs for specific service
docker-compose -f docker-compose.prod.yml logs -f backend
```

### 3. Initial Startup Sequence
Services will start in dependency order:
1. **PostgreSQL** (~10 seconds)
2. **Redis** (~5 seconds)
3. **Ollama** (~60 seconds + model download time)
4. **TTS & Image Engines** (~30-120 seconds)
5. **Backend API** (after database ready, ~30 seconds)
6. **Frontend** (after backend ready, ~20 seconds)

**Total startup time**: 5-15 minutes (depending on AI model downloads)

## Database Migrations

### Automatic Migrations
The backend service automatically applies Entity Framework migrations on startup.

### Manual Migration Management
```bash
# View migration status
docker exec taleweaver-backend-prod dotnet ef database update --list

# Apply specific migration
docker exec taleweaver-backend-prod dotnet ef database update MigrationName

# Rollback to previous migration
docker exec taleweaver-backend-prod dotnet ef database update PreviousMigrationName

# Generate new migration (development)
docker exec taleweaver-backend-prod dotnet ef migrations add MigrationName
```

### Database Backup Before Migration
```bash
# Create backup before major migrations
docker exec taleweaver-db-prod pg_dump -U taleweaver_prod taleweaver_production > backup_$(date +%Y%m%d_%H%M%S).sql
```

## AI Model Management

### Ollama Models
Models are automatically downloaded on first startup. To manage models:

```bash
# List installed models
docker exec taleweaver-ollama-prod ollama list

# Pull additional model
docker exec taleweaver-ollama-prod ollama pull llama3:70b

# Remove model
docker exec taleweaver-ollama-prod ollama rm llama3:8b-instruct

# View model info
docker exec taleweaver-ollama-prod ollama show llama3:8b-instruct
```

### TTS Models
Piper TTS downloads models on-demand. Pre-cache popular models:

```bash
# Download US English voice
docker exec taleweaver-tts-prod piper --model en_US-lessac-medium --download-dir /data/models
```

### Image Models
SDXL Turbo downloads automatically. Models persist in the `sd_models` volume.

## Health Checks

### Verify All Services
```bash
# Check service status
docker-compose -f docker-compose.prod.yml ps

# All services should show "healthy" status
```

### Individual Service Health Checks
```bash
# Backend API
curl -f http://localhost:5000/health

# Frontend
curl -f http://localhost:3000/health

# Ollama
curl -f http://localhost:11434/api/tags

# PostgreSQL
docker exec taleweaver-db-prod pg_isready -U taleweaver_prod

# Redis
docker exec taleweaver-redis-prod redis-cli ping
```

### Expected Response Times
- Backend API: < 200ms
- Frontend: < 100ms
- Database queries: < 50ms
- AI generation: 2-10 seconds (model dependent)

## SSL/TLS Setup

### Option 1: Let's Encrypt with Traefik (Recommended)

1. **Install Traefik**:
```bash
# Create traefik configuration
mkdir -p /opt/traefik
nano /opt/traefik/docker-compose.yml
```

2. **Traefik Docker Compose**:
```yaml
services:
  traefik:
    image: traefik:v2.10
    container_name: traefik
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock:ro
      - ./traefik.yml:/traefik.yml:ro
      - ./acme.json:/acme.json
    networks:
      - taleweaver-public
    restart: unless-stopped

networks:
  taleweaver-public:
    external: true
```

3. **Traefik Configuration** (`/opt/traefik/traefik.yml`):
```yaml
entryPoints:
  web:
    address: ":80"
    http:
      redirections:
        entryPoint:
          to: websecure
          scheme: https
  websecure:
    address: ":443"

certificatesResolvers:
  letsencrypt:
    acme:
      email: admin@yourdomain.com
      storage: acme.json
      httpChallenge:
        entryPoint: web

providers:
  docker:
    exposedByDefault: false
```

4. **Update docker-compose.prod.yml** to add Traefik labels:
```yaml
backend:
  labels:
    - "traefik.enable=true"
    - "traefik.http.routers.backend.rule=Host(`api.yourdomain.com`)"
    - "traefik.http.routers.backend.entrypoints=websecure"
    - "traefik.http.routers.backend.tls.certresolver=letsencrypt"

frontend:
  labels:
    - "traefik.enable=true"
    - "traefik.http.routers.frontend.rule=Host(`yourdomain.com`)"
    - "traefik.http.routers.frontend.entrypoints=websecure"
    - "traefik.http.routers.frontend.tls.certresolver=letsencrypt"
```

### Option 2: Manual Nginx + Certbot

```bash
# Install Nginx and Certbot
sudo apt-get install nginx certbot python3-certbot-nginx

# Obtain certificate
sudo certbot --nginx -d yourdomain.com -d api.yourdomain.com

# Nginx will auto-configure SSL
```

## Backup Procedures

### Automated Backups

Create backup script (`/opt/taleweaver/scripts/backup.sh`):
```bash
#!/bin/bash

BACKUP_DIR="/backups/taleweaver"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR

# Database backup
docker exec taleweaver-db-prod pg_dump -U taleweaver_prod taleweaver_production | \
  gzip > $BACKUP_DIR/db_backup_$DATE.sql.gz

# Volumes backup
docker run --rm \
  -v taleweaver_postgres_data:/data \
  -v $BACKUP_DIR:/backup \
  alpine tar czf /backup/volumes_$DATE.tar.gz /data

# Cleanup old backups (keep 14 days)
find $BACKUP_DIR -mtime +14 -delete

echo "Backup completed: $DATE"
```

Set up cron job:
```bash
# Add to crontab
0 2 * * * /opt/taleweaver/scripts/backup.sh >> /var/log/taleweaver-backup.log 2>&1
```

### Manual Backup
```bash
# Database only
docker exec taleweaver-db-prod pg_dump -U taleweaver_prod taleweaver_production > backup.sql

# Full system backup
docker-compose -f docker-compose.prod.yml down
tar czf taleweaver_backup_$(date +%Y%m%d).tar.gz /opt/taleweaver /var/lib/docker/volumes/taleweaver*
docker-compose -f docker-compose.prod.yml up -d
```

### Restore from Backup
```bash
# Stop services
docker-compose -f docker-compose.prod.yml down

# Restore database
cat backup.sql | docker exec -i taleweaver-db-prod psql -U taleweaver_prod taleweaver_production

# Restart services
docker-compose -f docker-compose.prod.yml up -d
```

## Update Procedures

### Rolling Update (Zero Downtime)

1. **Pull Latest Images**:
```bash
docker-compose -f docker-compose.prod.yml pull
```

2. **Update Services One-by-One**:
```bash
# Update backend
docker-compose -f docker-compose.prod.yml up -d --no-deps --build backend

# Wait for health check
sleep 30

# Update frontend
docker-compose -f docker-compose.prod.yml up -d --no-deps --build frontend
```

3. **Verify Health**:
```bash
docker-compose -f docker-compose.prod.yml ps
curl -f http://localhost:5000/health
```

### Full Stack Update
```bash
# Backup first
./scripts/backup.sh

# Pull latest
docker-compose -f docker-compose.prod.yml pull

# Restart all services
docker-compose -f docker-compose.prod.yml up -d

# Monitor logs
docker-compose -f docker-compose.prod.yml logs -f
```

## Monitoring

### View Real-Time Logs
```bash
# All services
docker-compose -f docker-compose.prod.yml logs -f

# Specific service
docker-compose -f docker-compose.prod.yml logs -f backend

# Last 100 lines
docker-compose -f docker-compose.prod.yml logs --tail=100 backend
```

### Resource Usage
```bash
# Container stats
docker stats

# Disk usage
docker system df

# Volume usage
docker volume ls
du -sh /var/lib/docker/volumes/taleweaver*
```

### Health Monitoring Script
Create `/opt/taleweaver/scripts/health-check.sh`:
```bash
#!/bin/bash

services=("backend:5000" "frontend:3000" "ollama:11434")

for service in "${services[@]}"; do
  name="${service%%:*}"
  port="${service##*:}"
  
  if curl -f -s http://localhost:$port/health > /dev/null; then
    echo "✅ $name is healthy"
  else
    echo "❌ $name is down"
    # Send alert (email, Slack, etc.)
  fi
done
```

## Troubleshooting

### Common Issues

**Issue**: Service won't start
```bash
# Check logs
docker-compose -f docker-compose.prod.yml logs service-name

# Check resource usage
docker stats

# Restart service
docker-compose -f docker-compose.prod.yml restart service-name
```

**Issue**: Database connection errors
```bash
# Verify database is ready
docker exec taleweaver-db-prod pg_isready

# Check connection string in .env.prod
docker-compose -f docker-compose.prod.yml exec backend env | grep DATABASE
```

**Issue**: AI models not loading
```bash
# Check Ollama logs
docker-compose -f docker-compose.prod.yml logs ollama

# Manually pull model
docker exec taleweaver-ollama-prod ollama pull llama3:8b-instruct

# Check disk space
df -h
```

**Issue**: Out of memory
```bash
# Check memory usage
docker stats

# Adjust resource limits in docker-compose.prod.yml
# Restart services
docker-compose -f docker-compose.prod.yml restart
```

### Debug Mode
Enable debug logging temporarily:
```bash
# Set LOG_LEVEL=Debug in .env.prod
docker-compose -f docker-compose.prod.yml up -d backend

# View detailed logs
docker-compose -f docker-compose.prod.yml logs -f backend
```

## Rollback Strategy

### Quick Rollback
```bash
# Stop current version
docker-compose -f docker-compose.prod.yml down

# Pull specific version
export BACKEND_IMAGE=ghcr.io/yourusername/taleweaver-backend:abc123
export FRONTEND_IMAGE=ghcr.io/yourusername/taleweaver-frontend:abc123

# Start with previous version
docker-compose -f docker-compose.prod.yml up -d

# Verify health
curl -f http://localhost:5000/health
```

### Database Rollback
```bash
# Restore from backup
cat backup_20240126_020000.sql | docker exec -i taleweaver-db-prod psql -U taleweaver_prod taleweaver_production

# Restart backend
docker-compose -f docker-compose.prod.yml restart backend
```

## Security Checklist

- [ ] `.env.prod` has restrictive permissions (600)
- [ ] Strong database password configured
- [ ] JWT secret is randomly generated (64+ characters)
- [ ] Stripe production keys are configured
- [ ] SSL/TLS certificates are valid
- [ ] Firewall configured (only 80, 443, 22 open)
- [ ] Regular backups enabled
- [ ] Monitoring and alerting configured
- [ ] Docker images are from trusted sources
- [ ] No secrets in environment variables or logs
- [ ] CORS origins properly restricted

## Production Maintenance Schedule

- **Daily**: Health checks, log review
- **Weekly**: Backup verification, disk space check
- **Monthly**: Security updates, dependency updates
- **Quarterly**: Performance review, capacity planning

## Support & Resources

- **Documentation**: [Project README](./README.md)
- **Architecture**: [.gaia/designs/architecture.md](.gaia/designs/architecture.md)
- **API Docs**: [.gaia/designs/api.md](.gaia/designs/api.md)
- **Issues**: GitHub Issues
- **Logs**: `/var/log/taleweaver-*.log`

---

**Last Updated**: 2024-01-26
**Version**: 1.0.0
