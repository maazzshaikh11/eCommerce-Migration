# Monorepo Design Specification

## Overview

A clean, scalable, production-ready monorepo for migrating a legacy ASP.NET MVC 5 eCommerce application to a modern decoupled stack using the **Strangler Fig Pattern**.

The legacy repository remains untouched and serves **only as reference**. All new implementation lives in this monorepo.

---

## 1. Complete Top-Level Folder Structure

```
ecommerce-migration/
├── .github/
├── config/
├── docker/
├── docs/
├── infra/
├── scripts/
├── src/
├── tests/
├── tools/
├── .editorconfig
├── .gitattributes
├── .gitignore
├── CODEOWNERS
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── LICENSE
├── Makefile
├── NUKE.sh
├── NUKE.ps1
├── package.json
└── README.md
```

---

## 2. Purpose of Every Top-Level Folder

| Folder | Purpose | Contents |
|--------|---------|----------|
| `.github/` | GitHub-specific automation | Issue templates, PR templates, CODEOWNERS, Actions workflows, dependabot config |
| `config/` | Shared configuration files | EditorConfig rules, NuGet.config, npmrc, stylecop, lint configs that span multiple projects |
| `docker/` | Container definitions | Dockerfiles, docker-compose files, health check scripts, container entrypoints |
| `docs/` | Project documentation | Architecture Decision Records (ADRs), runbooks, API specs, migration playbooks |
| `infra/` | Infrastructure as Code | Terraform modules, Bicep templates, Kubernetes manifests, Azure ARM templates |
| `scripts/` | Automation scripts | Setup scripts, database migration scripts, seed data scripts, deployment helpers |
| `src/` | **All source code** | Backend, frontend, gateway, shared libraries, migration utilities |
| `tests/` | Cross-cutting tests | End-to-end tests, contract tests, load tests, integration tests that span multiple services |
| `tools/` | Development tooling | Local dev environment configs, linters, code generators, dev certificates |

### Root Files

| File | Purpose |
|------|---------|
| `.editorconfig` | Enforce consistent formatting (C#, JS, TS, JSON, XML, YAML) across the entire repo |
| `.gitattributes` | Normalize line endings (LF for code, CRLF for Windows-specific files) |
| `.gitignore` | Unified ignore rules for .NET, Node.js, Docker, IDEs |
| `CODEOWNERS` | Require specific team members to approve changes to sensitive areas (e.g., `/infra/`, `/src/backend/`) |
| `Directory.Build.props` | Shared MSBuild properties for all .csproj files (language version, nullable reference types, implicit usings, analyzer settings) |
| `Directory.Packages.props` | Central package management (CPM) — all NuGet versions managed in one file |
| `global.json` | Pins the .NET SDK version for reproducible builds |
| `LICENSE` | Project license |
| `Makefile` | Common commands: `make build`, `make test`, `make docker-up`, `make migrate` |
| `NUKE.sh` / `NUKE.ps1` | NUKE build system entry points (cross-platform build orchestration) |
| `package.json` | Root-level npm scripts and shared dev dependencies (prettier, lint-staged, husky) |
| `README.md` | Project overview, quick start, architecture diagram, contribution guidelines |

---

## 3. Folder Structure Inside `/src`

```
src/
├── backend/
│   ├── Ecommerce.Api/
│   ├── Ecommerce.Api.Core/
│   ├── Ecommerce.Api.Services/
│   ├── Ecommerce.Api.Data/
│   └── Ecommerce.Api.Migrations/
├── frontend/
│   └── spa/
│       ├── src/
│       ├── tests/
│       └── public/
├── gateway/
│   └── Ecommerce.Gateway/
├── shared/
│   └── Ecommerce.Contracts/
└── migration/
    └── Ecommerce.Migration.Tool/
```

### 3.1 `/src/backend/` — .NET 10 Web API

| Project | Purpose |
|---------|---------|
| `Ecommerce.Api` | ASP.NET Core 10 Web API project. Entry point. Controllers, middleware, DI configuration, Program.cs |
| `Ecommerce.Api.Core` | Domain models, DTOs, interfaces, exceptions, enums, constants, JWT security helpers, validation rules |
| `Ecommerce.Api.Services` | Business logic. Service implementations and their interfaces. Pure C# — no HTTP, no SQL directly |
| `Ecommerce.Api.Data` | Data access layer. Repositories, UnitOfWork, ADO.NET helpers (`DatabaseHelper`), raw SQL mappings |
| `Ecommerce.Api.Migrations` | Database migration scripts (DbUp or FluentMigrator). Not EF Core Migrations — we use ADO.NET |

**Why this layout:**
- Clean Architecture / Onion Architecture principles: Core has no dependencies. Services depend on Core. Data depends on Core. API depends on all three.
- Controllers are thin — they delegate to Services. Services are testable in isolation.
- `Ecommerce.Api.Migrations` is separate so migration scripts can be reviewed as code and run independently of the API.

### 3.2 `/src/frontend/` — Vue.js 3 SPA

| Folder | Purpose |
|--------|---------|
| `spa/src/api/` | API client functions (`fetch` wrappers), interceptors for JWT, error handling |
| `spa/src/assets/` | Static assets: images, fonts, SCSS variables, global CSS |
| `spa/src/components/` | Reusable UI components (atomic design: atoms, molecules, organisms) |
| `spa/src/composables/` | Vue 3 composition functions (shared reactive logic: `useAuth`, `useCart`, `useProducts`) |
| `spa/src/layouts/` | Page layouts (`DefaultLayout.vue`, `AuthLayout.vue`, `CheckoutLayout.vue`) |
| `spa/src/router/` | Vue Router configuration, route guards, lazy loading |
| `spa/src/stores/` | Pinia stores (`auth.store.ts`, `cart.store.ts`, `product.store.ts`, `checkout.store.ts`) |
| `spa/src/views/` | Page-level view components (one per route) |
| `spa/src/types/` | TypeScript interfaces for API DTOs, domain models, shared types |
| `spa/src/utils/` | Pure utility functions (formatters, validators, guards) |
| `spa/tests/unit/` | Vitest + Vue Test Utils tests mirroring `src/` structure |
| `spa/tests/e2e/` | Playwright end-to-end tests |
| `spa/tests/mocks/` | Mock data, MSW (Mock Service Worker) handlers for offline dev |

### 3.3 `/src/gateway/` — YARP Reverse Proxy

| Project | Purpose |
|---------|---------|
| `Ecommerce.Gateway` | Standalone ASP.NET Core 10 project hosting YARP. Routes traffic to Backend, Frontend, and Legacy during hybrid phase |

**Why separate project:**
- Gateway has a different deployment profile (edge layer, high availability, no business logic).
- Can be scaled independently from the API.
- Configuration (routes, clusters) lives in `appsettings.json` + environment-specific overrides.

### 3.4 `/src/shared/` — Cross-Cutting Contracts

| Project | Purpose |
|---------|---------|
| `Ecommerce.Contracts` | Shared C# contracts (event schemas, API request/response records, gRPC proto files if needed later). Referenced by both Backend and Gateway. Not by Frontend — frontend uses TypeScript types |

**Why separate project:**
- Ensures API contracts are versioned in one place.
- Can be published as a NuGet package for external consumers.
- Prevents accidental coupling between API and Gateway implementations.

### 3.5 `/src/migration/` — Migration Utilities

| Project | Purpose |
|---------|---------|
| `Ecommerce.Migration.Tool` | CLI tool for one-off migration tasks: data reconciliation, legacy schema comparison, JWT bridge testing, seed data generation |

**Why separate project:**
- Keeps migration-specific throwaway code out of production projects.
- Can be run as a console app or containerized job.
- Will be deleted after Phase 6 (decommissioning).

---

## 4. Project Naming Conventions

### .NET Projects

| Pattern | Example | Rule |
|---------|---------|------|
| `{Org}.{Layer}.{Name}` | `Ecommerce.Api.Core` | Organization prefix + layer + name |
| No suffix for entry points | `Ecommerce.Api` | Not `Ecommerce.Api.Web` or `Ecommerce.Api.Host` |
| Test projects | `Ecommerce.Api.Tests` | Same root name + `.Tests` |
| Integration tests | `Ecommerce.Api.IntegrationTests` | Same root name + `.IntegrationTests` |

### Frontend Projects

| Pattern | Example | Rule |
|---------|---------|------|
| kebab-case folder | `src/frontend/spa/` | Lowercase, hyphens for multi-word |
| Component files | `ProductCard.vue` | PascalCase, matches component name |
| Composables | `useAuth.ts` | camelCase, `use` prefix |
| Stores | `auth.store.ts` | kebab-case + `.store.ts` suffix |
| Views | `ProductListView.vue` | PascalCase, ends with `View` |

### General

- **Solution file:** `ecommerce-migration.sln` at repo root (or `/src/` root).
- **Docker images:** `ecommerce/api`, `ecommerce/gateway`, `ecommerce/spa`.
- **Namespaces:** Mirror folder structure. `Ecommerce.Api.Controllers`, `Ecommerce.Api.Services`.

---

## 5. Documentation Structure (`/docs/`)

```
docs/
├── README.md                    # Documentation index and navigation
├── architecture/
│   ├── 01-current-state.md      # Legacy architecture analysis
│   ├── 02-target-state.md       # Target architecture (this doc)
│   ├── 03-data-flow.md          # Request/response flow diagrams
│   ├── 04-auth-strategy.md      # JWT hybrid auth deep dive
│   └── 05-deployment-model.md   # Deployment topology (Docker, K8s, Azure)
├── decisions/
│   ├── 001-monorepo-structure.md
│   ├── 002-why-yarp.md
│   ├── 003-why-ado-net.md
│   ├── 004-jwt-bridge-option-a.md
│   └── 005-vertical-slice-order.md
├── api/
│   ├── openapi.yaml             # Auto-generated or hand-maintained OpenAPI spec
│   └── endpoint-catalog.md      # Human-readable endpoint reference
├── runbooks/
│   ├── local-setup.md
│   ├── database-seeding.md
│   ├── rollback-procedures.md
│   └── incident-response.md
├── migration/
│   ├── phase-0-checklist.md
│   ├── phase-1-checklist.md
│   ├── ...
│   └── phase-6-checklist.md
└── reference/
    └── legacy-codebase-notes.md  # Notes, mappings, and gotchas from legacy repo
```

**ADR Format:** Each file in `decisions/` follows the classic Architecture Decision Record template:
1. Context
2. Decision
3. Consequences
4. Status (proposed / accepted / deprecated / superseded)

---

## 6. Test Project Organization

### Philosophy

Tests live **as close as possible to the code they test**, but **E2E and cross-cutting tests** live at the repo root in `/tests/`.

### Structure

```
tests/
├── e2e/                         # Playwright E2E tests (full browser automation)
│   ├── playwright.config.ts
│   ├── fixtures/
│   ├── specs/
│   │   ├── auth.spec.ts
│   │   ├── product-catalog.spec.ts
│   │   ├── cart.spec.ts
│   │   ├── checkout.spec.ts
│   │   └── order-history.spec.ts
│   └── pages/                   # Page Object Model classes
│       ├── ProductListPage.ts
│       ├── ProductDetailPage.ts
│       ├── CartPage.ts
│       └── CheckoutPage.ts
├── integration/                 # Integration tests (API + real DB in Docker)
│   └── Ecommerce.Api.IntegrationTests/
│       ├── Fixtures/
│       ├── Helpers/
│       └── Scenarios/
├── load/                        # k6 or NBomber load tests
│   └── checkout-load-test.js
└── contracts/                   # Consumer-driven contract tests (Pact)
    └── pact-tests/
```

### Unit Tests (Colocated)

| Source | Test Location |
|--------|---------------|
| `src/backend/Ecommerce.Api/` | `src/backend/Ecommerce.Api.Tests/` (or `tests/unit/backend/`) |
| `src/backend/Ecommerce.Api.Services/` | `src/backend/Ecommerce.Api.Services.Tests/` |
| `src/frontend/spa/src/` | `src/frontend/spa/tests/unit/` |

### Test Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Unit test class | `{ClassName}Tests` | `ProductServiceTests` |
| Test method | `{MethodName}_{Scenario}_{ExpectedResult}` | `GetProducts_WithCategoryFilter_ReturnsFilteredResults` |
| E2E spec | `{feature}.spec.ts` | `checkout.spec.ts` |
| Page Object | `{PageName}Page.ts` | `CheckoutPage.ts` |

---

## 7. CI/CD Organization (`/.github/`)

```
.github/
├── workflows/
│   ├── ci.yml                   # Main CI: build + test on every push/PR
│   ├── pr-validation.yml        # Fast PR checks: lint, format, static analysis
│   ├── e2e.yml                  # Full E2E suite (runs on PR to main, nightly)
│   ├── security-scan.yml        # Trivy, CodeQL, dependency vulnerability scan
│   ├── docker-build.yml         # Build and push container images to registry
│   └── deploy-staging.yml       # Deploy to staging environment
│   └── deploy-production.yml    # Deploy to production (manual trigger or tag push)
├── actions/
│   ├── setup-dotnet/            # Reusable composite action: setup .NET SDK
│   ├── setup-node/              # Reusable composite action: setup Node.js
│   └── docker-login/            # Reusable composite action: authenticate to ACR
├── dependabot.yml               # Automated dependency updates for NuGet and npm
├── CODEOWNERS                   # Require approvals for infra, backend, frontend
└── pull_request_template.md     # PR description template with checklist
```

### CI Pipeline Flow

```
PR Opened
  → PR Validation (fast, < 3 min)
    ├── dotnet format --verify-no-changes
    ├── dotnet build (all projects)
    ├── dotnet test (unit tests only)
    ├── npm run lint
    ├── npm run test:unit
    └── SonarCloud / CodeQL scan
  → Full CI (if PR validation passes)
    ├── Build Docker images
    ├── Run integration tests (with TestContainers / LocalDB)
    ├── Run E2E tests (with Playwright)
    └── Security scan (Trivy on images)
  → Deploy to Staging (on merge to main)
    ├── Push images to ACR
    └── Trigger staging deployment
```

---

## 8. Docker and Deployment Organization (`/docker/`)

```
docker/
├── api/
│   └── Dockerfile               # Multi-stage build: SDK → runtime
├── gateway/
│   └── Dockerfile
├── spa/
│   └── Dockerfile               # Multi-stage: Node build → nginx static serve
├── migrations/
│   └── Dockerfile               # DbUp/FluentMigrator runner image
├── compose/
│   ├── docker-compose.yml       # Full local stack (API + Gateway + SPA + SQL Server)
│   ├── docker-compose.override.yml  # Dev overrides (volume mounts, hot reload)
│   ├── docker-compose.test.yml      # CI test stack (no volume mounts, ephemeral DB)
│   └── docker-compose.infra.yml     # Infrastructure only (SQL Server, Redis if needed)
├── scripts/
│   ├── wait-for-db.sh           # Health check for SQL Server startup
│   ├── entrypoint-api.sh        # API container entrypoint
│   └── entrypoint-migrations.sh # Migration runner entrypoint
└── nginx/
    └── spa.conf                 # Nginx config for serving static SPA files
```

### Local Development (docker-compose)

```bash
# Start everything
make docker-up
# Equivalent to: docker compose -f docker/compose/docker-compose.yml up --build

# Start only infrastructure
make docker-infra
# Equivalent to: docker compose -f docker/compose/docker-compose.infra.yml up
```

### Deployment Targets

| Environment | Trigger | Infrastructure |
|-------------|---------|----------------|
| Local Dev | `make docker-up` | Docker Compose |
| CI / PR | GitHub Actions | ephemeral containers |
| Staging | Merge to `main` | Azure Container Apps / AKS |
| Production | Manual approval or tag push | Azure Container Apps / AKS with blue-green |

---

## 9. Coding Standards

### C# / .NET 10

| Standard | Rule |
|----------|------|
| Language | C# 13 (latest) with `LangVersion` pinned in `Directory.Build.props` |
| Nullable | `Nullable=enable` globally. All reference types are non-nullable by default |
| Implicit Usings | Enabled (`ImplicitUsings=enable`) but reviewed — no magic namespaces |
| File-scoped namespaces | Required for all new files |
| Primary constructors | Preferred for DTOs and simple services |
| Records | Used for DTOs and immutable data: `public record ProductDto(...)` |
| Controllers | Must be thin. No business logic. Max 20 lines per action |
| Async | All I/O-bound methods must be `async`. No `.Result` or `.Wait()` |
| Exception handling | Global middleware catches unhandled exceptions. Controllers do not catch generic exceptions |
| Logging | `ILogger<T>` injected. Structured logging (Serilog) with correlation IDs |
| API responses | Return `ActionResult<T>` or `IActionResult`. Use `ProblemDetails` for errors |

### Vue.js / TypeScript

| Standard | Rule |
|----------|------|
| Syntax | `<script setup lang="ts">` exclusively |
| Components | PascalCase file names matching component name |
| Composables | camelCase with `use` prefix: `useAuth.ts` |
| Stores | One file per store. Suffix `.store.ts` |
| Types | Explicit types for all function parameters and return values. No implicit `any` |
| HTTP | Use native `fetch` via wrapper in `api/client.ts`. No Axios unless required later |
| State | Pinia for global state. `ref`/`computed` for local component state. No `reactive` for objects — use `ref` + shallow copies |
| Styling | SCSS with BEM or scoped CSS. No inline styles |
| Accessibility | All images have `alt`, all buttons have `type`, forms have `label` associations |

### General

| Standard | Rule |
|----------|------|
| Line endings | LF (enforced by `.gitattributes`) |
| Indent | 4 spaces (C#), 2 spaces (JS/TS/YAML/JSON) |
| Max line length | 120 characters |
| Trailing whitespace | Forbidden (enforced by EditorConfig) |
| File encoding | UTF-8 with BOM for C# files (Visual Studio default), UTF-8 without BOM for everything else |
| Git commits | Conventional Commits (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`) |

---

## 10. Branching Strategy

**Strategy:** GitHub Flow (simplified) with `main` as the single source of truth.

```
main  ────────────────────────────────────────────────
       \         \         \         \
feature/\   feature/\   feature/\   feature/\
  api-products  spa-cart   api-auth   docs-adr-001
```

### Branch Types

| Branch | Naming | Lifecycle |
|--------|--------|-----------|
| `main` | `main` | Permanent. Always deployable. Protected. Requires PR + CI pass + code review |
| Feature | `feature/{phase}-{description}` | e.g., `feature/phase0-yarp-gateway`. Short-lived. Deleted after merge |
| Hotfix | `hotfix/{description}` | e.g., `hotfix/jwt-validation-bug`. Branches from `main`, fast-tracked review |
| Release | `release/{version}` | e.g., `release/1.0.0`. Only if we need release stabilization. Optional for this project |
| Docs | `docs/{description}` | Documentation-only changes. No CI build required (but PR validation still runs) |

### Rules

1. **No direct commits to `main`.** Everything goes through a Pull Request.
2. **Squash merge** feature branches into `main` to keep history linear.
3. **Delete feature branches** after merge.
4. **Require CI green** before merge (PR validation workflow).
5. **Require 1 approval** for code changes, 2 approvals for infrastructure (`/infra/`, `/docker/`).
6. **Require signed commits** (optional but recommended for production repos).

---

## 11. Why This Monorepo Structure Is the Best Fit

### 11.1 Atomic Changes Across the Stack

A vertical slice (e.g., Product Catalog) touches the **API controller**, **service**, **repository**, **Vue component**, **Pinia store**, and **YARP route**. In a monorepo, these changes are in a single PR with atomic commits. In a polyrepo, we'd need synchronized PRs across 3+ repositories, version bumps, and deployment orchestration — all for one feature.

### 11.2 Single Source of Truth for Contracts

The `Ecommerce.Contracts` project in `/src/shared/` and the TypeScript interfaces in `/src/frontend/spa/src/types/` are visible to everyone. When an API DTO changes, the frontend type change is in the same commit. No drift. No versioning hell.

### 11.3 Unified Tooling and Standards

- One `.editorconfig`, one `Directory.Build.props`, one `package.json` with shared lint configs.
- `dotnet format` and `prettier` run from root, covering the entire codebase.
- A single GitHub Actions pipeline builds, tests, and deploys everything.

### 11.4 Strangler Fig Pattern Benefits

The monorepo mirrors the **Strangler Fig** philosophy:
- We add new systems (Vue, API, Gateway) without removing old ones.
- The legacy repo is reference-only; we never modify it.
- When Phase 6 arrives (decommissioning), we simply delete `/src/migration/` and archive the old repo. The monorepo itself shrinks organically.

### 11.5 Scalability Without Over-Engineering

This structure supports:
- **Multiple frontend apps** later (e.g., admin panel → `src/frontend/admin/`).
- **Microservices extraction** later (e.g., `src/backend/` could become `src/services/catalog/`, `src/services/orders/`).
- **Multiple gateways** (e.g., mobile BFF → `src/gateway/mobile/`).
- **Shared libraries** that can be published as NuGet/npm packages when needed.

### 11.6 Developer Experience

```bash
# Clone once, everything is there
git clone https://github.com/org/ecommerce-migration.git
cd ecommerce-migration

# One command to start everything
make docker-up

# One command to run all tests
make test

# One command to lint everything
make lint
```

No context switching between repos. No "which version of the API is the frontend expecting?" questions.

---

## 12. ASCII Tree of Complete Repository

```
ecommerce-migration/
├── .github/
│   ├── actions/
│   │   ├── setup-dotnet/
│   │   │   └── action.yml
│   │   ├── setup-node/
│   │   │   └── action.yml
│   │   └── docker-login/
│   │       └── action.yml
│   ├── workflows/
│   │   ├── ci.yml
│   │   ├── pr-validation.yml
│   │   ├── e2e.yml
│   │   ├── security-scan.yml
│   │   ├── docker-build.yml
│   │   ├── deploy-staging.yml
│   │   └── deploy-production.yml
│   ├── dependabot.yml
│   ├── CODEOWNERS
│   └── pull_request_template.md
│
├── config/
│   ├── .editorconfig
│   ├── nuget.config
│   └── stylecop.json
│
├── docker/
│   ├── api/
│   │   └── Dockerfile
│   ├── gateway/
│   │   └── Dockerfile
│   ├── spa/
│   │   └── Dockerfile
│   ├── migrations/
│   │   └── Dockerfile
│   ├── compose/
│   │   ├── docker-compose.yml
│   │   ├── docker-compose.override.yml
│   │   ├── docker-compose.test.yml
│   │   └── docker-compose.infra.yml
│   ├── scripts/
│   │   ├── wait-for-db.sh
│   │   ├── entrypoint-api.sh
│   │   └── entrypoint-migrations.sh
│   └── nginx/
│       └── spa.conf
│
├── docs/
│   ├── README.md
│   ├── architecture/
│   │   ├── 01-current-state.md
│   │   ├── 02-target-state.md
│   │   ├── 03-data-flow.md
│   │   ├── 04-auth-strategy.md
│   │   └── 05-deployment-model.md
│   ├── decisions/
│   │   ├── 001-monorepo-structure.md
│   │   ├── 002-why-yarp.md
│   │   ├── 003-why-ado-net.md
│   │   ├── 004-jwt-bridge-option-a.md
│   │   └── 005-vertical-slice-order.md
│   ├── api/
│   │   ├── openapi.yaml
│   │   └── endpoint-catalog.md
│   ├── runbooks/
│   │   ├── local-setup.md
│   │   ├── database-seeding.md
│   │   ├── rollback-procedures.md
│   │   └── incident-response.md
│   ├── migration/
│   │   ├── phase-0-checklist.md
│   │   ├── phase-1-checklist.md
│   │   ├── phase-2-checklist.md
│   │   ├── phase-3-checklist.md
│   │   ├── phase-4-checklist.md
│   │   ├── phase-5-checklist.md
│   │   └── phase-6-checklist.md
│   └── reference/
│       └── legacy-codebase-notes.md
│
├── infra/
│   ├── terraform/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── outputs.tf
│   │   └── modules/
│   │       ├── database/
│   │       ├── gateway/
│   │       └── container-apps/
│   ├── kubernetes/
│   │   ├── namespace.yml
│   │   ├── api-deployment.yml
│   │   ├── api-service.yml
│   │   ├── gateway-deployment.yml
│   │   ├── gateway-service.yml
│   │   ├── spa-deployment.yml
│   │   ├── spa-service.yml
│   │   └── ingress.yml
│   └── bicep/
│       └── main.bicep
│
├── scripts/
│   ├── setup.ps1
│   ├── setup.sh
│   ├── db-seed.ps1
│   ├── db-seed.sh
│   ├── migrate.ps1
│   ├── migrate.sh
│   └── health-check.ps1
│
├── src/
│   ├── backend/
│   │   ├── Ecommerce.Api/
│   │   │   ├── Controllers/
│   │   │   ├── Middleware/
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── Program.cs
│   │   │   └── Ecommerce.Api.csproj
│   │   ├── Ecommerce.Api.Core/
│   │   │   ├── DTOs/
│   │   │   ├── Models/
│   │   │   ├── Interfaces/
│   │   │   ├── Security/
│   │   │   ├── Exceptions/
│   │   │   └── Ecommerce.Api.Core.csproj
│   │   ├── Ecommerce.Api.Services/
│   │   │   ├── Interfaces/
│   │   │   ├── Implementations/
│   │   │   └── Ecommerce.Api.Services.csproj
│   │   ├── Ecommerce.Api.Data/
│   │   │   ├── Repositories/
│   │   │   ├── DatabaseHelper.cs
│   │   │   ├── UnitOfWork.cs
│   │   │   └── Ecommerce.Api.Data.csproj
│   │   └── Ecommerce.Api.Migrations/
│   │       ├── Scripts/
│   │       ├── MigrationRunner.cs
│   │       └── Ecommerce.Api.Migrations.csproj
│   │
│   ├── frontend/
│   │   └── spa/
│   │       ├── src/
│   │       │   ├── api/
│   │       │   │   └── client.ts
│   │       │   ├── assets/
│   │       │   ├── components/
│   │       │   │   ├── atoms/
│   │       │   │   ├── molecules/
│   │       │   │   └── organisms/
│   │       │   ├── composables/
│   │       │   ├── layouts/
│   │       │   ├── router/
│   │       │   ├── stores/
│   │       │   ├── types/
│   │       │   ├── utils/
│   │       │   ├── views/
│   │       │   ├── App.vue
│   │       │   └── main.ts
│   │       ├── tests/
│   │       │   ├── unit/
│   │       │   │   ├── components/
│   │       │   │   ├── stores/
│   │       │   │   └── composables/
│   │       │   └── e2e/
│   │       │       ├── fixtures/
│   │       │       ├── pages/
│   │       │       └── specs/
│   │       ├── public/
│   │       ├── index.html
│   │       ├── vite.config.ts
│   │       ├── vitest.config.ts
│   │       ├── playwright.config.ts
│   │       ├── tsconfig.json
│   │       ├── package.json
│   │       └── package-lock.json
│   │
│   ├── gateway/
│   │   └── Ecommerce.Gateway/
│   │       ├── appsettings.json
│   │       ├── Program.cs
│   │       └── Ecommerce.Gateway.csproj
│   │
│   ├── shared/
│   │   └── Ecommerce.Contracts/
│   │       ├── Events/
│   │       ├── Requests/
│   │       ├── Responses/
│   │       └── Ecommerce.Contracts.csproj
│   │
│   └── migration/
│       └── Ecommerce.Migration.Tool/
│           ├── Commands/
│           ├── Program.cs
│           └── Ecommerce.Migration.Tool.csproj
│
├── tests/
│   ├── e2e/
│   │   ├── playwright.config.ts
│   │   ├── fixtures/
│   │   ├── pages/
│   │   │   ├── ProductListPage.ts
│   │   │   ├── ProductDetailPage.ts
│   │   │   ├── CartPage.ts
│   │   │   └── CheckoutPage.ts
│   │   └── specs/
│   │       ├── auth.spec.ts
│   │       ├── product-catalog.spec.ts
│   │       ├── cart.spec.ts
│   │       ├── checkout.spec.ts
│   │       └── order-history.spec.ts
│   ├── integration/
│   │   └── Ecommerce.Api.IntegrationTests/
│   │       ├── Fixtures/
│   │       ├── Helpers/
│   │       ├── Scenarios/
│   │       └── Ecommerce.Api.IntegrationTests.csproj
│   ├── load/
│   │   └── checkout-load-test.js
│   └── contracts/
│       └── pact-tests/
│
├── tools/
│   ├── certs/
│   │   └── dev-cert.ps1
│   ├── linters/
│   │   └── .eslintrc.shared.cjs
│   └── generators/
│       └── new-api-controller.ps1
│
├── .editorconfig
├── .gitattributes
├── .gitignore
├── CODEOWNERS
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── LICENSE
├── Makefile
├── NUKE.sh
├── NUKE.ps1
├── package.json
└── README.md
```

---

## Summary Table

| Concern | Decision | Location |
|---------|----------|----------|
| All source code | `/src/` | `src/backend/`, `src/frontend/`, `src/gateway/`, `src/shared/`, `src/migration/` |
| Documentation | `/docs/` | ADRs, architecture, runbooks, API specs, migration checklists |
| E2E tests | `/tests/e2e/` | Playwright specs + Page Object Models |
| Integration tests | `/tests/integration/` | xUnit against real DB in Docker |
| CI/CD | `/.github/workflows/` | GitHub Actions with reusable composite actions |
| Docker | `/docker/` | One Dockerfile per service, compose files for dev/test/infra |
| Infrastructure | `/infra/` | Terraform, Bicep, Kubernetes manifests |
| Automation | `/scripts/` | Setup, seeding, migration runner scripts |
| Dev tooling | `/tools/` | Certificates, shared lint configs, code generators |
| Global config | `/config/` + root files | `.editorconfig`, `Directory.Build.props`, `global.json` |

---

**Status:** Design complete. Awaiting mentor approval before proceeding to repository creation.
