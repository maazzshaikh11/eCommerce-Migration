# Deployment Model

## Overview

The application is designed for containerized deployment with Docker and can be orchestrated via Kubernetes or Azure Container Apps. During development, the services run locally on separate ports.

## Development Topology

```
Local Machine
├── .NET 10 API          → http://localhost:5001
├── Vue 3 SPA (Vite)     → http://localhost:5002
├── YARP Reverse Proxy   → http://localhost:5003
└── SQL Server LocalDB   → (localdb)\MSSQLLocalDB
```

## Production Topology (Target)

```
┌─────────────────────────────────────────────────┐
│              Load Balancer / Ingress             │
└──────────────────────┬──────────────────────────┘
                       │
┌──────────┬───────────┴───┬──────────────────────┐
│          │               │                      │
▼          ▼               ▼                      ▼
┌────────┐ ┌────────────┐ ┌────────────────┐ ┌─────────┐
│ API    │ │ SPA (nginx) │ │ YARP Proxy     │ │ DB      │
│ .NET10 │ │ Vue 3 dist │ │ .NET 10        │ │ SQL Srv │
└───┬────┘ └──────┬─────┘ └───────┬────────┘ └────┬────┘
    │             │                │               │
    └─────────────┴────────────────┴───────────────┘
                          │
                  ┌───────▼────────┐
                  │  SQL Server    │
                  │  LegacyEcommerceDb │
                  └────────────────┘
```

## Container Definitions

Container definitions are in `docker/`:

| Path | Image | Purpose |
|------|-------|---------|
| `docker/api/Dockerfile` | API | .NET 10 runtime |
| `docker/spa/Dockerfile` | SPA | Vite build + nginx serve |
| `docker/gateway/Dockerfile` | Gateway | YARP proxy |
| `docker/migrations/Dockerfile` | Migrations | DB migration runner |
| `docker/compose/docker-compose.yml` | — | Full stack orchestration |
| `docker/nginx/spa.conf` | — | nginx config for SPA |

## Infrastructure as Code

| Path | Tool | Purpose |
|------|------|---------|
| `infra/terraform/` | Terraform | Azure resources (container apps, database) |
| `infra/bicep/` | Bicep | Azure ARM templates |
| `infra/kubernetes/` | K8s | Deployments, services, ingress |

## Environment Configuration

| Variable | Dev Default | Purpose |
|----------|-------------|---------|
| `JwtSigningKey` | (in appsettings.json) | HS256 signing key (≥32 bytes) |
| `ConnectionStrings:LegacyEcommerceDb` | LocalDB | SQL Server connection string |
| `VITE_API_BASE_URL` | `http://localhost:5001/api` | API base URL for SPA |

> **Security:** Never commit production secrets. Use environment variables or secret managers for production deployments.

## CI/CD

GitHub Actions workflows are in `.github/workflows/`:

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `ci.yml` | Push/PR | Build + test backend and frontend |
| `pr-validation.yml` | PR | Lint, type check, validation |
| `e2e.yml` | PR | Playwright E2E tests |
| `docker-build.yml` | Push to main | Build container images |
| `deploy-staging.yml` | Manual/tag | Deploy to staging |
| `deploy-production.yml` | Manual/tag | Deploy to production |
| `security-scan.yml` | Schedule/PR | Security vulnerability scan |

## Related

- [Target State](target-state.md)
- [Runbooks](../runbooks/local-setup.md)
- [Monorepo Design](monorepo-design.md)