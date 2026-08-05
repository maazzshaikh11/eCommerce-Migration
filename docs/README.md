# Documentation

Complete documentation for the eCommerce migration project — migrating a legacy ASP.NET MVC 5 eCommerce application to a modern Vue 3 + .NET 10 Web API stack using the Strangler Fig pattern.

## Structure

```
docs/
├── api/              ← API endpoint reference and OpenAPI spec
├── architecture/     ← Architecture overview, current/target state, data flow, auth, deployment
├── decisions/        ← Architecture Decision Records (ADRs)
├── migration/        ← Task lists, roadmaps, progress, verification reports
├── reference/        ← Legacy codebase notes, prompt library
└── runbooks/         ← Operational guides (setup, seeding, incidents, rollback)
```

## Quick Links

### Getting Started
- [Local Setup](runbooks/local-setup.md) — Step-by-step environment setup
- [README (root)](../README.md) — Project overview and quick start

### Architecture
- [Architecture Overview](architecture/README.md) — High-level topology, tech stack, request lifecycle
- [Current State (Legacy)](architecture/current-state.md) — ASP.NET MVC 5 monolith analysis
- [Target State](architecture/target-state.md) — Vue 3 + .NET 10 decoupled architecture
- [Data Flow](architecture/data-flow.md) — Request lifecycle diagrams
- [Auth Strategy](architecture/auth-strategy.md) — JWT authentication
- [Deployment Model](architecture/deployment-model.md) — Containers, CI/CD, infrastructure

### Decisions
- [ADR Index](decisions/README.md) — All architecture decision records

### Migration
- [Migration Overview](migration/README.md) — All migration documentation
- [Master Task List](migration/master-task-list.md) — T-001 → T-125 with acceptance criteria
- [Progress](migration/progress.md) — Final status and deviations
- [Feature Parity Report](migration/feature-parity-report.md) — Legacy vs modern audit

### API
- [Endpoint Catalog](api/endpoint-catalog.md) — All REST endpoints
- [OpenAPI Spec](api/openapi.yaml) — Machine-readable spec

### Reference
- [Legacy Codebase Notes](reference/legacy-codebase-notes.md) — Mapping and gotchas

### Runbooks
- [Local Setup](runbooks/local-setup.md)
- [Database Seeding](runbooks/database-seeding.md)
- [Incident Response](runbooks/incident-response.md)
- [Rollback Procedures](runbooks/rollback-procedures.md)