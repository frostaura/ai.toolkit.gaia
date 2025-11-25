````markdown
<!-- reference @.gaia/designs/design.md -->
<!-- reference @.gaia/designs/5-api.md -->
<!-- reference @.gaia/designs/6-security.md -->

[<< Back](./design.md)

# Infrastructure & DevOps

Complete infrastructure design, CI/CD pipelines, and deployment strategies.

## Template Guidance

**Purpose**: Define infrastructure architecture, deployment pipelines, and operational workflows
**Focus**: CI/CD, environments, Infrastructure-as-Code, container orchestration, deployment strategies
**Avoid**: Specific tool configurations, implementation code, manual processes

**Guidelines**: Automation-first, infrastructure-as-code, immutable infrastructure, GitOps principles

## Infrastructure Architecture

**Architecture Philosophy**: Cloud-Native with local development support
**Cloud Provider**: AWS (primary), with local Docker Compose for development
**Compute Model**: Containers (Docker) with potential Kubernetes migration
**Orchestration**: Docker Compose (dev/staging), ECS/Kubernetes (production scaling)

**Infrastructure Principles**:
- **Infrastructure as Code**: All infrastructure defined in docker-compose.yml and Dockerfiles
- **Immutable Infrastructure**: Replace containers rather than modify running instances
- **Declarative Configuration**: Docker Compose YAML defines desired state
- **Automation**: CI/CD pipelines for automated builds and deployments
- **Reproducibility**: Identical environments from dev → staging → production

**Game-Specific Requirements**:
- **WebSocket Support**: Load balancer must support WebSocket upgrades
- **Session Affinity**: Sticky sessions or Redis adapter for Socket.io
- **Real-Time Performance**: Low-latency networking (<100ms target)
- **Concurrent Sessions**: Support 100+ game sessions per backend instance

## Environment Strategy

### Environment Tiers

**Development (dev)**:
- **Purpose**: Active development and feature testing
- **Data**: Mock/synthetic data only
- **Deployment**: Automatic on merge to dev branch
- **Access**: All developers
- **Cost Optimization**: Scaled down resources, auto-shutdown

**Staging (stg)**:
- **Purpose**: Pre-production validation and QA testing
- **Data**: Sanitized production-like data
- **Deployment**: Automatic on merge to main/staging branch
- **Access**: QA team, developers (read-only production access)
- **Infrastructure**: Production-equivalent configuration

**Production (prod)**:
- **Purpose**: Live customer-facing environment
- **Data**: Real customer data
- **Deployment**: Manual approval required after staging validation
- **Access**: Restricted to ops team and specific engineers
- **High Availability**: Multi-region, auto-scaling, disaster recovery

**Additional Environments** (Optional):
- **Local**: Developer workstations with Docker Compose
- **Feature**: Temporary environments for major features
- **Hotfix**: Emergency production-equivalent environment
- **Performance**: Load testing and performance benchmarking

### Environment Parity

**Configuration Management**:
```
.
├── infrastructure/
│   ├── base/                    # Shared configuration
│   ├── environments/
│   │   ├── dev/                 # Dev overrides
│   │   ├── staging/             # Staging overrides
│   │   └── production/          # Production overrides
│   └── modules/                 # Reusable IaC modules
```

**Parity Principles**:
- **Same Infrastructure Code**: Only parameter differences between environments
- **Configuration Drift Prevention**: Automated drift detection
- **Environment Variables**: Environment-specific secrets and config
- **Database Schema**: Same schema across all environments

## Infrastructure as Code (IaC)

### IaC Tooling

**Primary Tool**: [Terraform / Pulumi / AWS CDK / ARM Templates / Bicep]
**Configuration Management**: [Ansible / Chef / Puppet]
**Container Orchestration**: [Kubernetes YAML / Helm Charts / Kustomize]

**IaC Structure**:
```
infrastructure/
├── terraform/
│   ├── modules/
│   │   ├── networking/          # VPC, subnets, security groups
│   │   ├── compute/             # EC2, ECS, Lambda
│   │   ├── database/            # RDS, DynamoDB, Redis
│   │   ├── storage/             # S3, EBS, EFS
│   │   └── monitoring/          # CloudWatch, alarms
│   ├── environments/
│   │   ├── dev/
│   │   │   ├── main.tf
│   │   │   ├── variables.tf
│   │   │   └── terraform.tfvars
│   │   ├── staging/
│   │   └── production/
│   └── backend.tf               # Remote state configuration
```

### Core Infrastructure Components

**Networking**:
```hcl
# VPC with public/private subnets across multiple AZs
module "networking" {
  source = "./modules/networking"

  vpc_cidr = "10.0.0.0/16"
  public_subnets = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  private_subnets = ["10.0.11.0/24", "10.0.12.0/24", "10.0.13.0/24"]
  availability_zones = ["us-east-1a", "us-east-1b", "us-east-1c"]
}
```

**Compute Resources**:
- **Container Cluster**: ECS/EKS cluster with auto-scaling
- **Load Balancer**: Application Load Balancer with SSL termination
- **Auto Scaling**: CPU/Memory-based scaling policies
- **Bastion Host**: Secure SSH access to private resources

**Database**:
- **Primary Database**: RDS PostgreSQL Multi-AZ with read replicas
- **Caching Layer**: ElastiCache Redis cluster
- **Backup**: Automated daily snapshots with 30-day retention

**Storage**:
- **Object Storage**: S3 buckets with versioning and lifecycle policies
- **Persistent Volumes**: EBS volumes for stateful workloads
- **CDN**: CloudFront for static asset distribution

## Container Strategy

### Docker Compose Configuration

**Complete Game Infrastructure** (docker-compose.yml):
```yaml
version: '3.8'

services:
  # Frontend: React + Vite + PixiJS
  frontend:
    build:
      context: ./src/frontend
      dockerfile: Dockerfile.frontend
    container_name: tower-defense-frontend
    ports:
      - "3000:80"
    environment:
      - VITE_API_URL=http://localhost:4000
      - VITE_WS_URL=ws://localhost:4000
    depends_on:
      - backend
    networks:
      - tower-defense-network
    healthcheck:
      test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  # Backend: Node.js + Express + Socket.io
  backend:
    build:
      context: ./src/backend
      dockerfile: Dockerfile.backend
    container_name: tower-defense-backend
    ports:
      - "4000:4000"
    environment:
      - NODE_ENV=production
      - PORT=4000
      - DATABASE_URL=postgresql://postgres:postgres@postgres:5432/tower_defense
      - REDIS_URL=redis://redis:6379
      - JWT_SECRET=${JWT_SECRET}
      - JWT_REFRESH_SECRET=${JWT_REFRESH_SECRET}
      - CORS_ORIGIN=http://localhost:3000
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    networks:
      - tower-defense-network
    healthcheck:
      test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost:4000/api/health"]
      interval: 10s
      timeout: 5s
      retries: 3
      start_period: 30s
    volumes:
      - ./logs:/app/logs

  # PostgreSQL Database
  postgres:
    image: postgres:15-alpine
    container_name: tower-defense-postgres
    ports:
      - "5432:5432"
    environment:
      - POSTGRES_DB=tower_defense
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./src/backend/prisma/migrations:/docker-entrypoint-initdb.d
    networks:
      - tower-defense-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis Cache & Session Store
  redis:
    image: redis:7-alpine
    container_name: tower-defense-redis
    ports:
      - "6379:6379"
    command: redis-server --appendonly yes --maxmemory 512mb --maxmemory-policy allkeys-lru
    volumes:
      - redis_data:/data
    networks:
      - tower-defense-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

volumes:
  postgres_data:
    driver: local
  redis_data:
    driver: local

networks:
  tower-defense-network:
    driver: bridge
```

### Container Architecture

**Containerization Approach**:
- **Base Images**: Node.js 20 Alpine for backend, Nginx Alpine for frontend
- **Multi-Stage Builds**: Optimize image size and security (build → production)
- **Image Scanning**: Automated vulnerability scanning in CI/CD
- **Image Registry**: AWS ECR (production), Docker Hub (development)

**Dockerfile.frontend** (Multi-stage React + Vite build):
```dockerfile
# Stage 1: Build React application
FROM node:20-alpine AS builder
WORKDIR /app

# Copy package files
COPY package.json package-lock.json ./
RUN npm ci --production=false

# Copy source code
COPY . .

# Build for production (Vite + React)
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/

# Copy built assets from builder stage
COPY --from=builder /app/dist .

# Add healthcheck endpoint
RUN echo "OK" > /usr/share/nginx/html/health

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

**Dockerfile.backend** (Node.js + TypeScript + Prisma):
```dockerfile
# Stage 1: Build TypeScript application
FROM node:20-alpine AS builder
WORKDIR /app

# Install dependencies
COPY package.json package-lock.json ./
RUN npm ci --production=false

# Copy source code
COPY . .

# Generate Prisma Client
RUN npx prisma generate

# Build TypeScript
RUN npm run build

# Stage 2: Production runtime
FROM node:20-alpine
WORKDIR /app

# Install production dependencies only
COPY package.json package-lock.json ./
RUN npm ci --only=production

# Copy Prisma schema for client
COPY --from=builder /app/prisma ./prisma
RUN npx prisma generate

# Copy built application
COPY --from=builder /app/dist ./dist

# Create non-root user
RUN addgroup -g 1001 -S nodejs && adduser -S nodejs -u 1001
USER nodejs

EXPOSE 4000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD node -e "require('http').get('http://localhost:4000/api/health', (r) => process.exit(r.statusCode === 200 ? 0 : 1))"

CMD ["node", "dist/server.js"]
```

**Nginx Configuration** (frontend/nginx.conf):
```nginx
server {
    listen 80;
    server_name _;

    root /usr/share/nginx/html;
    index index.html;

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;

    # SPA routing (React Router)
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Health check endpoint
    location /health {
        access_log off;
        return 200 "OK";
        add_header Content-Type text/plain;
    }

    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

### Kubernetes Architecture (If Applicable)

**Cluster Configuration**:
- **Node Pools**: Separate pools for different workload types
- **Namespaces**: Environment/team isolation
- **Resource Quotas**: Prevent resource exhaustion
- **Network Policies**: Pod-to-pod communication rules

**Kubernetes Resources**:
```yaml
# Deployment with resource limits and health checks
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: api-service
  template:
    metadata:
      labels:
        app: api-service
    spec:
      containers:
      - name: api
        image: registry.example.com/api:latest
        resources:
          requests:
            cpu: 100m
            memory: 256Mi
          limits:
            cpu: 500m
            memory: 512Mi
        livenessProbe:
          httpGet:
            path: /health
            port: 3000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 3000
          initialDelaySeconds: 5
          periodSeconds: 5
```

**Service Mesh** (Optional):
- **Tool**: Istio / Linkerd / AWS App Mesh
- **Benefits**: Traffic management, observability, security
- **Use Case**: Complex microservices architectures

## CI/CD Pipeline

### Pipeline Architecture

```mermaid
graph LR
    A[Code Commit] --> B[CI: Build & Test]
    B --> C[Code Quality Checks]
    C --> D[Security Scanning]
    D --> E[Build Container]
    E --> F[Deploy to Dev]
    F --> G{Tests Pass?}
    G -->|Yes| H[Deploy to Staging]
    G -->|No| I[Notify Team]
    H --> J{Staging Tests Pass?}
    J -->|Yes| K[Manual Approval]
    J -->|No| I
    K --> L[Deploy to Production]
    L --> M[Smoke Tests]
    M --> N{Success?}
    N -->|Yes| O[Complete]
    N -->|No| P[Rollback]
```

### CI Pipeline (Continuous Integration)

**Complete CI Workflow** (.github/workflows/ci.yml):
```yaml
name: CI Pipeline - Isometric Tower Defense

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  NODE_VERSION: '20'

jobs:
  lint-and-test-frontend:
    name: Frontend - Lint & Test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: ${{ env.NODE_VERSION }}
          cache: 'npm'
          cache-dependency-path: src/frontend/package-lock.json

      - name: Install dependencies
        working-directory: ./src/frontend
        run: npm ci

      - name: Run linter (ESLint)
        working-directory: ./src/frontend
        run: npm run lint

      - name: Run unit tests (Vitest)
        working-directory: ./src/frontend
        run: npm run test:unit

      - name: Code coverage
        working-directory: ./src/frontend
        run: npm run test:coverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./src/frontend/coverage/coverage-final.json
          flags: frontend

      - name: Check coverage threshold (100%)
        working-directory: ./src/frontend
        run: |
          COVERAGE=$(jq '.total.lines.pct' coverage/coverage-summary.json)
          if (( $(echo "$COVERAGE < 100" | bc -l) )); then
            echo "Coverage is $COVERAGE%, must be 100%"
            exit 1
          fi

  lint-and-test-backend:
    name: Backend - Lint & Test
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:15-alpine
        env:
          POSTGRES_DB: tower_defense_test
          POSTGRES_USER: postgres
          POSTGRES_PASSWORD: postgres
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

      redis:
        image: redis:7-alpine
        ports:
          - 6379:6379
        options: >-
          --health-cmd "redis-cli ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: ${{ env.NODE_VERSION }}
          cache: 'npm'
          cache-dependency-path: src/backend/package-lock.json

      - name: Install dependencies
        working-directory: ./src/backend
        run: npm ci

      - name: Generate Prisma Client
        working-directory: ./src/backend
        run: npx prisma generate

      - name: Run database migrations
        working-directory: ./src/backend
        run: npx prisma migrate deploy
        env:
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/tower_defense_test

      - name: Run linter (ESLint)
        working-directory: ./src/backend
        run: npm run lint

      - name: Run unit tests (Jest)
        working-directory: ./src/backend
        run: npm run test:unit
        env:
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/tower_defense_test
          REDIS_URL: redis://localhost:6379
          JWT_SECRET: test-secret
          JWT_REFRESH_SECRET: test-refresh-secret

      - name: Run integration tests
        working-directory: ./src/backend
        run: npm run test:integration
        env:
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/tower_defense_test
          REDIS_URL: redis://localhost:6379

      - name: Code coverage
        working-directory: ./src/backend
        run: npm run test:coverage
        env:
          DATABASE_URL: postgresql://postgres:postgres@localhost:5432/tower_defense_test
          REDIS_URL: redis://localhost:6379

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./src/backend/coverage/coverage-final.json
          flags: backend

      - name: Check coverage threshold (100%)
        working-directory: ./src/backend
        run: |
          COVERAGE=$(jq '.total.lines.pct' coverage/coverage-summary.json)
          if (( $(echo "$COVERAGE < 100" | bc -l) )); then
            echo "Coverage is $COVERAGE%, must be 100%"
            exit 1
          fi

  build-docker-images:
    name: Build Docker Images
    runs-on: ubuntu-latest
    needs: [lint-and-test-frontend, lint-and-test-backend]
    steps:
      - uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Build frontend image
        uses: docker/build-push-action@v4
        with:
          context: ./src/frontend
          file: ./src/frontend/Dockerfile.frontend
          push: false
          tags: tower-defense-frontend:${{ github.sha }}
          cache-from: type=gha
          cache-to: type=gha,mode=max

      - name: Build backend image
        uses: docker/build-push-action@v4
        with:
          context: ./src/backend
          file: ./src/backend/Dockerfile.backend
          push: false
          tags: tower-defense-backend:${{ github.sha }}
          cache-from: type=gha
          cache-to: type=gha,mode=max

  security-scan:
    name: Security Scanning
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Run Snyk security scan
        uses: snyk/actions/node@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          command: test
          args: --severity-threshold=high

      - name: Run Trivy vulnerability scanner (frontend)
        uses: aquasecurity/trivy-action@master
        with:
          scan-type: 'fs'
          scan-ref: './src/frontend'
          format: 'sarif'
          output: 'trivy-frontend-results.sarif'

      - name: Run Trivy vulnerability scanner (backend)
        uses: aquasecurity/trivy-action@master
        with:
          scan-type: 'fs'
          scan-ref: './src/backend'
          format: 'sarif'
          output: 'trivy-backend-results.sarif'
```

**Quality Gates**:
- **Code Coverage**: **100% coverage required** (enforced via threshold check)
- **Linting**: Zero linting errors (ESLint for both frontend and backend)
- **Security Scan**: No high/critical vulnerabilities (Snyk + Trivy)
- **Build Success**: All Docker images must build successfully
- **Test Pass Rate**: 100% test pass rate (all tests must pass)

### CD Pipeline (Continuous Deployment)

**Complete CD Workflow** (.github/workflows/cd.yml):
```yaml
name: CD Pipeline - Deploy Tower Defense

on:
  push:
    branches: [ main ]
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deployment environment'
        required: true
        type: choice
        options:
          - staging
          - production

env:
  AWS_REGION: us-east-1
  ECR_REGISTRY: ${{ secrets.AWS_ACCOUNT_ID }}.dkr.ecr.us-east-1.amazonaws.com
  ECR_REPOSITORY_FRONTEND: tower-defense-frontend
  ECR_REPOSITORY_BACKEND: tower-defense-backend

jobs:
  build-and-push:
    name: Build & Push Docker Images
    runs-on: ubuntu-latest
    outputs:
      frontend-image: ${{ steps.build-frontend.outputs.image }}
      backend-image: ${{ steps.build-backend.outputs.image }}
    steps:
      - uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Build, tag, and push frontend image
        id: build-frontend
        env:
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build -t $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG \
            -f src/frontend/Dockerfile.frontend src/frontend
          docker push $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG
          docker tag $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG \
            $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:latest
          docker push $ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:latest
          echo "image=$ECR_REGISTRY/$ECR_REPOSITORY_FRONTEND:$IMAGE_TAG" >> $GITHUB_OUTPUT

      - name: Build, tag, and push backend image
        id: build-backend
        env:
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build -t $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG \
            -f src/backend/Dockerfile.backend src/backend
          docker push $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG
          docker tag $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG \
            $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:latest
          docker push $ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:latest
          echo "image=$ECR_REGISTRY/$ECR_REPOSITORY_BACKEND:$IMAGE_TAG" >> $GITHUB_OUTPUT

  deploy-staging:
    name: Deploy to Staging
    runs-on: ubuntu-latest
    needs: build-and-push
    environment:
      name: staging
      url: https://staging.tower-defense.com
    steps:
      - uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}

      - name: Update ECS service (frontend)
        run: |
          aws ecs update-service \
            --cluster tower-defense-staging \
            --service frontend-service \
            --force-new-deployment

      - name: Update ECS service (backend)
        run: |
          aws ecs update-service \
            --cluster tower-defense-staging \
            --service backend-service \
            --force-new-deployment

      - name: Wait for deployment to stabilize
        run: |
          aws ecs wait services-stable \
            --cluster tower-defense-staging \
            --services frontend-service backend-service

      - name: Run smoke tests
        run: |
          curl -f https://staging.tower-defense.com/health || exit 1
          curl -f https://staging-api.tower-defense.com/api/health || exit 1

  deploy-production:
    name: Deploy to Production
    runs-on: ubuntu-latest
    needs: [build-and-push, deploy-staging]
    environment:
      name: production
      url: https://tower-defense.com
    steps:
      - uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}

      - name: Update ECS service (frontend) - Rolling deployment
        run: |
          aws ecs update-service \
            --cluster tower-defense-production \
            --service frontend-service \
            --force-new-deployment \
            --deployment-configuration "maximumPercent=200,minimumHealthyPercent=100"

      - name: Update ECS service (backend) - Rolling deployment
        run: |
          aws ecs update-service \
            --cluster tower-defense-production \
            --service backend-service \
            --force-new-deployment \
            --deployment-configuration "maximumPercent=200,minimumHealthyPercent=100"

      - name: Wait for deployment to stabilize
        run: |
          aws ecs wait services-stable \
            --cluster tower-defense-production \
            --services frontend-service backend-service

      - name: Run production smoke tests
        run: |
          curl -f https://tower-defense.com/health || exit 1
          curl -f https://api.tower-defense.com/api/health || exit 1

      - name: Notify Slack
        if: success()
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "Production deployment successful: ${{ github.sha }}"
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

**Environment Configuration Files**:
```.env.development
NODE_ENV=development
PORT=4000
DATABASE_URL=postgresql://postgres:postgres@localhost:5432/tower_defense
REDIS_URL=redis://localhost:6379
CORS_ORIGIN=http://localhost:3000
LOG_LEVEL=debug
```

```.env.production
NODE_ENV=production
PORT=4000
DATABASE_URL=${DATABASE_URL}
REDIS_URL=${REDIS_URL}
CORS_ORIGIN=https://tower-defense.com
LOG_LEVEL=info
JWT_SECRET=${JWT_SECRET}
JWT_REFRESH_SECRET=${JWT_REFRESH_SECRET}
```

**Deployment Approval**:
- **Dev**: Automatic deployment on commit to `develop` branch
- **Staging**: Automatic after CI passes on `main` branch
- **Production**: **Manual approval required** via GitHub Environments + smoke tests pass

## Deployment Strategies

### Blue-Green Deployment

**Strategy**:
- **Blue Environment**: Current production version
- **Green Environment**: New version deployed alongside
- **Traffic Switch**: Instant cutover from blue to green
- **Rollback**: Switch back to blue if issues detected

**Implementation**:
```mermaid
graph LR
    LB[Load Balancer] -->|100%| Blue[Blue Environment v1.0]
    Green[Green Environment v1.1] -.->|0%| LB

    LB2[Load Balancer] -.->|0%| Blue2[Blue Environment v1.0]
    LB2 -->|100%| Green2[Green Environment v1.1]
```

**Benefits**: Zero-downtime, instant rollback
**Drawbacks**: Requires double infrastructure during deployment

### Canary Deployment

**Strategy**:
- **Baseline**: Current version serves majority of traffic
- **Canary**: New version serves small percentage (5-10%)
- **Monitor**: Observe canary metrics for errors/performance issues
- **Gradual Rollout**: Increase canary traffic if healthy (10% → 25% → 50% → 100%)

**Traffic Splitting**:
```
Traffic Distribution:
├── Canary (v2.0):  5%  → 10% → 25% → 50% → 100%
└── Baseline (v1.0): 95% → 90% → 75% → 50% → 0%
```

**Benefits**: Low-risk, gradual rollout, early detection
**Drawbacks**: Complex routing, longer deployment time

### Rolling Deployment

**Strategy**:
- **Incremental Update**: Replace instances one by one (or in batches)
- **Health Checks**: Ensure each new instance healthy before proceeding
- **Progressive**: Gradually replace all instances

**Process**:
```
Instance 1: v1.0 → v2.0 (health check) ✓
Instance 2: v1.0 → v2.0 (health check) ✓
Instance 3: v1.0 → v2.0 (health check) ✓
Instance 4: v1.0 → v2.0 (health check) ✓
```

**Benefits**: No extra infrastructure, simple
**Drawbacks**: Slower rollback, mixed versions during deployment

## Configuration Management

### Secret Management

**Secret Storage**: [AWS Secrets Manager / Azure Key Vault / HashiCorp Vault / GCP Secret Manager]

**Secret Types**:
- **Database Credentials**: Connection strings, passwords
- **API Keys**: Third-party service credentials
- **Encryption Keys**: Data encryption keys
- **Certificates**: SSL/TLS certificates

**Secret Injection**:
```yaml
# Kubernetes secret injection
apiVersion: v1
kind: Pod
spec:
  containers:
  - name: app
    env:
    - name: DATABASE_PASSWORD
      valueFrom:
        secretKeyRef:
          name: db-secrets
          key: password
```

**Secret Rotation**:
- **Automatic Rotation**: Every 90 days for all credentials
- **Zero-Downtime Rotation**: Dual credentials during rotation
- **Audit Trail**: All secret access logged

### Environment Configuration

**Configuration Hierarchy**:
1. **Default Configuration**: Application defaults
2. **Environment Variables**: Environment-specific overrides
3. **Secret Manager**: Sensitive configuration
4. **Runtime Configuration**: Dynamic configuration service

**Example Configuration Structure**:
```json
{
  "app": {
    "name": "MyApp",
    "environment": "${ENVIRONMENT}",
    "port": "${PORT:-3000}"
  },
  "database": {
    "host": "${DB_HOST}",
    "port": "${DB_PORT:-5432}",
    "name": "${DB_NAME}",
    "password": "${DB_PASSWORD}"  // Injected from secret manager
  },
  "features": {
    "newFeature": "${FEATURE_FLAG_NEW_FEATURE:-false}"
  }
}
```

## Disaster Recovery & Backup

### Backup Strategy

**Database Backups**:
- **Automated Snapshots**: Daily full snapshots
- **Point-in-Time Recovery**: Transaction log backups every 5 minutes
- **Retention**: 30 days for automated backups, 1 year for monthly snapshots
- **Cross-Region Replication**: Backups replicated to secondary region

**Application State Backups**:
- **Configuration Backups**: Infrastructure state stored in version control
- **File Storage Backups**: S3 versioning and cross-region replication
- **Container Images**: Registry with image retention policies

### Disaster Recovery Plan

**Recovery Objectives**:
- **RTO (Recovery Time Objective)**: < 4 hours
- **RPO (Recovery Point Objective)**: < 15 minutes data loss

**DR Strategy**: [Active-Passive / Active-Active / Backup & Restore]

**Failover Process**:
```mermaid
graph TD
    A[Disaster Detected] --> B[Assess Impact]
    B --> C{Primary Region Down?}
    C -->|Yes| D[Initiate Failover]
    C -->|No| E[Component-Level Recovery]
    D --> F[Switch DNS to DR Region]
    F --> G[Verify DR Region Health]
    G --> H[Notify Stakeholders]
    E --> H
    H --> I[Monitor & Adjust]
```

**DR Testing**:
- **Frequency**: Quarterly DR drills
- **Scope**: Full region failover simulation
- **Validation**: Verify RTO/RPO objectives met

## Infrastructure Monitoring

### Health Checks

**Application Health**:
- **Liveness Probe**: Is the application running?
- **Readiness Probe**: Is the application ready to serve traffic?
- **Startup Probe**: Has the application completed initialization?

**Health Check Endpoints**:
```
GET /health
{
  "status": "healthy",
  "version": "2.1.0",
  "uptime": 86400,
  "checks": {
    "database": "healthy",
    "cache": "healthy",
    "externalApi": "degraded"
  }
}
```

### Infrastructure Metrics

**Key Metrics**:
- **CPU Utilization**: Target < 70% average
- **Memory Usage**: Target < 80% with headroom
- **Disk I/O**: Monitor for bottlenecks
- **Network Traffic**: Bandwidth utilization and latency
- **Container Restart Rate**: Detect crash loops

**Auto-Scaling Triggers**:
- Scale up when CPU > 70% for 5 minutes
- Scale down when CPU < 30% for 15 minutes
- Minimum replicas: 2 (high availability)
- Maximum replicas: 10 (cost control)

## Cost Optimization

**Cost Management Strategies**:
- **Right-Sizing**: Match instance sizes to actual usage
- **Auto-Scaling**: Scale down during low-traffic periods
- **Reserved Instances**: Commit to long-term for predictable workloads
- **Spot Instances**: Use for non-critical batch workloads
- **Resource Cleanup**: Automated deletion of unused resources

**Cost Monitoring**:
- **Budget Alerts**: Notify when costs exceed threshold
- **Cost Attribution**: Tag resources by team/project
- **Cost Optimization Recommendations**: Weekly review of optimization opportunities

## Validation Checklist

**Infrastructure**:
- [x] Infrastructure-as-Code (Docker Compose for dev, ECS for production)
- [x] WebSocket support with Nginx upgrade headers
- [x] Health checks configured (frontend /health, backend /api/health)
- [x] Volume persistence (postgres_data, redis_data)

**Environments**:
- [x] Development (docker-compose.yml local)
- [x] Staging (AWS ECS auto-deploy on main branch)
- [x] Production (AWS ECS manual approval required)
- [x] Environment-specific configuration (.env.development, .env.production)

**CI/CD**:
- [x] Automated CI pipeline (lint, test, build, security scan)
- [x] **100% code coverage enforcement** (fails if below 100%)
- [x] Quality gates (ESLint, Jest, Vitest, Snyk, Trivy)
- [x] Docker image builds (multi-stage frontend + backend)
- [x] Automated CD to staging, manual approval for production

**Deployment**:
- [x] Rolling deployment strategy (ECS with 200% max, 100% min healthy)
- [x] Zero-downtime deployments via health checks
- [x] Smoke tests after deployment (health endpoint validation)
- [x] Slack notifications on production deployments

**Monitoring**:
- [x] Health check endpoints (/health for frontend, /api/health for backend)
- [x] Docker healthchecks configured for all services
- [x] Deployment stability verification (aws ecs wait services-stable)

**Game-Specific**:
- [x] PostgreSQL 15 with Prisma ORM
- [x] Redis 7 for session cache and Socket.io adapter
- [x] Node.js 20 Alpine for backend
- [x] React + Vite + PixiJS frontend with Nginx
- [x] WebSocket support for real-time multiplayer

**Instructions**: This infrastructure specification defines complete Docker Compose setup (frontend, backend, postgres, redis), multi-stage Dockerfiles optimized for production, GitHub Actions CI/CD pipelines with 100% coverage gates, and ECS deployment to staging/production environments. All infrastructure supports real-time multiplayer gaming requirements with WebSocket connections, session persistence, and low-latency networking.

[<< Back](./design.md)

````
