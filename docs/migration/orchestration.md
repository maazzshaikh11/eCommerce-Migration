# Migration Orchestration Document

## 1. Project Overview

### 1.1 Scope
This document orchestrates the complete migration of a legacy ASP.NET MVC 5 eCommerce application to a modern decoupled architecture using the **Strangler Fig Pattern**. All implementation occurs in a new monorepo; the legacy repository remains untouched and serves exclusively as reference.

### 1.2 Target Stack
- **Backend:** .NET 10 Web API, stateless, JWT bearer authentication
- **Frontend:** Vue.js 3 (Composition API + `<script setup>`) built with Vite
- **Gateway:** YARP Reverse Proxy
- **Data:** SQL Server (shared with legacy during hybrid phase)
- **Testing:** xUnit + Moq (backend), Vitest + Vue Test Utils (frontend), Playwright (E2E)

### 1.3 Execution Principle
**Sequential execution.** Every task must be completed, verified, and signed off before the next task begins. No parallel workstreams. No code generation in advance. No speculative implementation.

### 1.4 Timeline
10–12 weeks across 10 phases plus cross-cutting concerns.

### 1.5 Stakeholders
- Mentor / Architecture Review Board
- Development Team Lead
- QA / Testing Lead
- DevOps / Infrastructure Lead

---

## 2. Execution Strategy

### 2.1 Sequential Waterfall Within Phases
Each phase is a self-contained unit of work. Tasks inside a phase execute sequentially. A phase gate review is mandatory before entering the next phase.

### 2.2 Strangler Fig Pattern
- The new system grows alongside the legacy system.
- YARP intercepts traffic and routes modernized paths to the new stack.
- Legacy paths remain functional until explicitly migrated.
- No big-bang cutover. Zero-downtime transition.

### 2.3 Test-First Verification
Every task has concrete verification criteria. If a task cannot be verified, it is not complete. Testing is not a phase; it is a gate for every task.

### 2.4 No-Code-in-Advance Policy
Do not scaffold future phases. Do not write code for Task 3.4 while executing Task 1.2. The Implementation Roadmap is the boundary; nothing outside the current task is written.

### 2.5 Rollback at Task Level
If any task fails its verification, execute its rollback procedure before attempting a fix. Do not skip rollback to patch forward.

---

## 3. Sequential Execution Flow

```
Phase 1: Infrastructure & .NET Solution Foundation (T-001 – T-015)
└── T-001  Create modern/ directory
    ↓
└── T-002  Verify .NET 10 SDK installation
    ↓
└── T-003  Create .NET solution file
    ↓
└── T-004  Create .NET 10 Web API project
    ↓
└── T-005  Remove default WeatherForecastController
    ↓
└── T-006  Add YARP NuGet package to API project
    ↓
└── T-007  Add ReverseProxy configuration section to appsettings.json
    ↓
└── T-008  Register YARP services and middleware in Program.cs
    ↓
└── T-009  Verify YARP starts without runtime errors
    ↓
└── T-010  Create shared class library projects
    ↓
└── T-011  Configure Central Package Management (CPM)
    ↓
└── T-012  Create Directory.Build.props
    ↓
└── T-013  Wire inter-project references
    ↓
└── T-014  Port DatabaseHelper from legacy
    ↓
└── T-015  Create temporary health check controller
    ↓
[PHASE 1 GATE REVIEW]
    ↓
Phase 2: JWT Auth Bridge & Vue SPA Shell (T-016 – T-028)
└── T-016  Add JWT NuGet package to API project
    ↓
└── T-017  Implement JWT token service in Core project
    ↓
└── T-018  Configure JWT authentication in Program.cs
    ↓
└── T-019  Add JWT signing key to appsettings.json
    ↓
└── T-020  Add JWT bridge endpoint to legacy MVC
    ↓
└── T-021  Verify Node.js installation for Vue SPA
    ↓
└── T-022  Scaffold Vue SPA with Vite
    ↓
└── T-023  Install Pinia and Vue Router
    ↓
└── T-024  Configure API base URL in environment file
    ↓
└── T-025  Implement fetch wrapper with JWT attachment
    ↓
└── T-026  Implement Pinia auth store
    ↓
└── T-027  Configure Vue Router skeleton
    ↓
└── T-028  Verify full proxy chain end-to-end
    ↓
[PHASE 2 GATE REVIEW]
    ↓
Phase 3: Product Catalog Backend (T-029 – T-045)
└── T-029  Port Product domain model
    ↓
└── T-030  Port Category domain model
    ↓
└── T-031  Port ProductImage and ProductVariant models
    ↓
└── T-032  Create ProductListResponseDto
    ↓
└── T-033  Create ProductDetailDto
    ↓
└── T-034  Create CategoryDto and PagedResult
    ↓
└── T-035  Port IProductRepository interface
    ↓
└── T-036  Port ICategoryRepository interface
    ↓
└── T-037  Port ProductRepository implementation
    ↓
└── T-038  Port CategoryRepository implementation
    ↓
└── T-039  Port IProductService interface
    ↓
└── T-040  Port ICategoryService interface
    ↓
└── T-041  Port ProductService implementation
    ↓
└── T-042  Port CategoryService implementation
    ↓
└── T-043  Register Product and Category services in DI
    ↓
└── T-044  Implement ProductsController with GET endpoints
    ↓
└── T-045  Implement CategoriesController with GET endpoint
    ↓
[PHASE 3 GATE REVIEW]
    ↓
Phase 4: Product Catalog Frontend & Tests (T-046 – T-059)
└── T-046  Add YARP route for product pages
    ↓
└── T-047  Create ProductCard Vue component
    ↓
└── T-048  Create CategoryFilter Vue component
    ↓
└── T-049  Create PaginationControls Vue component
    ↓
└── T-050  Create LoadingSpinner and ErrorAlert components
    ↓
└── T-051  Create Pinia product store
    ↓
└── T-052  Create ProductListView Vue page
    ↓
└── T-053  Create ProductDetailView Vue page
    ↓
└── T-054  Configure Vue Router for product pages
    ↓
└── T-055  Write backend unit tests for ProductService
    ↓
└── T-056  Write backend unit tests for ProductsController
    ↓
└── T-057  Write frontend unit tests for ProductCard component
    ↓
└── T-058  Write frontend unit tests for product store
    ↓
└── T-059  Write E2E test for product catalog flow
    ↓
[PHASE 4 GATE REVIEW]
    ↓
Phase 5: Shopping Cart Backend & Store (T-060 – T-068)
└── T-060  Port CartItem domain model
    ↓
└── T-061  Create cart DTOs
    ↓
└── T-062  Port ICartItemRepository interface
    ↓
└── T-063  Port CartItemRepository implementation
    ↓
└── T-064  Port ICartService interface
    ↓
└── T-065  Port CartService implementation
    ↓
└── T-066  Register cart services in DI
    ↓
└── T-067  Implement CartController with authenticated endpoints
    ↓
└── T-068  Create Pinia cart store
    ↓
[PHASE 5 GATE REVIEW]
    ↓
Phase 6: Shopping Cart Frontend & Tests (T-069 – T-078)
└── T-069  Create AddToCartButton Vue component
    ↓
└── T-070  Create CartView Vue page
    ↓
└── T-071  Create CartSummaryBadge component
    ↓
└── T-072  Add cart route to Vue Router
    ↓
└── T-073  Add YARP route for cart pages
    ↓
└── T-074  Write backend unit tests for CartController
    ↓
└── T-075  Write backend unit tests for CartService
    ↓
└── T-076  Write frontend unit tests for AddToCartButton
    ↓
└── T-077  Write frontend unit tests for CartView
    ↓
└── T-078  Write E2E test for cart flow
    ↓
[PHASE 6 GATE REVIEW]
    ↓
Phase 7: Checkout Backend (T-079 – T-090)
└── T-079  Port Address domain model
    ↓
└── T-080  Port Order and OrderItem domain models
    ↓
└── T-081  Port Customer domain model
    ↓
└── T-082  Create checkout DTOs
    ↓
└── T-083  Port Address repository interface and implementation
    ↓
└── T-084  Port Order repository interface and implementation
    ↓
└── T-085  Port Customer repository interface and implementation
    ↓
└── T-086  Port CheckoutService verbatim
    ↓
└── T-087  Port OrderService
    ↓
└── T-088  Register checkout and order services in DI
    ↓
└── T-089  Implement CheckoutController
    ↓
└── T-090  Implement OrdersController (read-only)
    ↓
[PHASE 7 GATE REVIEW]
    ↓
Phase 8: Checkout Frontend & Tests (T-091 – T-103)
└── T-091  Create Pinia checkout store
    ↓
└── T-092  Create AddressSelector Vue component
    ↓
└── T-093  Create AddressForm Vue component
    ↓
└── T-094  Create AddressStep Vue component
    ↓
└── T-095  Create ReviewStep Vue component
    ↓
└── T-096  Create OrderConfirmation Vue component
    ↓
└── T-097  Create CheckoutWizard Vue page
    ↓
└── T-098  Add checkout and confirmation routes to Vue Router
    ↓
└── T-099  Add YARP route for checkout pages
    ↓
└── T-100  Write backend unit tests for CheckoutService
    ↓
└── T-101  Write backend unit tests for CheckoutController
    ↓
└── T-102  Write frontend unit tests for CheckoutWizard
    ↓
└── T-103  Write E2E test for full checkout flow
    ↓
[PHASE 8 GATE REVIEW]
    ↓
Phase 9: Order History, Home & Search (T-104 – T-116)
└── T-104  Create OrderListView Vue page
    ↓
└── T-105  Create OrderDetailView Vue page
    ↓
└── T-106  Add order routes to Vue Router
    ↓
└── T-107  Add YARP route for order pages
    ↓
└── T-108  Write frontend unit tests for order views
    ↓
└── T-109  Write E2E test for order history flow
    ↓
└── T-110  Integrate legacy account page links in Vue header
    ↓
└── T-111  Create HomeView Vue page
    ↓
└── T-112  Add search input to header
    ↓
└── T-113  Add home route to Vue Router
    ↓
└── T-114  Add YARP routes for homepage and search
    ↓
└── T-115  Write frontend unit tests for HomeView
    ↓
└── T-116  Write E2E test for homepage
    ↓
[PHASE 9 GATE REVIEW]
    ↓
Phase 10: Decommission Legacy (T-117 – T-125)
└── T-117  Analyze YARP traffic logs for legacy usage
    ↓
└── T-118  Create Vue LoginView page
    ↓
└── T-119  Create Vue RegisterView page
    ↓
└── T-120  Add auth endpoints to API
    ↓
└── T-121  Update auth store to use API endpoints
    ↓
└── T-122  Remove legacy fallback from YARP
    ↓
└── T-123  Shut down legacy application
    ↓
└── T-124  (Optional) Consolidate databases
    ↓
└── T-125  Run final full E2E regression suite
    ↓
[PHASE 10 GATE REVIEW — PROJECT COMPLETE]
```

---

## 4. Phase Breakdown

### Phase 1: Infrastructure & .NET Solution Foundation (Week 1, T-001 – T-015)
**Objective:** Establish the monorepo root, .NET 10 solution, YARP reverse proxy, shared class libraries, Central Package Management, inter-project references, ported `DatabaseHelper`, and a temporary health-check endpoint proving SQL Server reachability. No business logic yet.

### Phase 2: JWT Auth Bridge & Vue SPA Shell (Week 1–2, T-016 – T-028)
**Objective:** Implement JWT authentication (token service, API middleware, signing key), add the legacy `IssueToken` bridge endpoint, scaffold the Vue 3 + Vite SPA with Pinia, Vue Router, fetch wrapper, and auth store, and verify the full Legacy → YARP → API/SPA proxy chain end-to-end.

### Phase 3: Product Catalog Backend (Week 2, T-029 – T-045)
**Objective:** Port product, category, product image, and product variant domain models, DTOs, repositories, and services into the new .NET 10 API. Register services in DI. Implement `ProductsController` and `CategoriesController` with GET endpoints. The API serves the full read-only product catalog as JSON.

### Phase 4: Product Catalog Frontend & Tests (Week 3, T-046 – T-059)
**Objective:** Add YARP routes for product pages, build the Vue product UI (reusable components, Pinia product store, list and detail views), wire Vue Router, and complete backend unit tests, frontend unit tests, and an E2E Playwright spec for the product catalog flow. Users browse the catalog entirely through Vue + API.

### Phase 5: Shopping Cart Backend & Store (Week 4, T-060 – T-068)
**Objective:** Port the cart domain model, DTOs, repository and service layers, register cart services in DI, implement the authenticated `CartController`, and create the Pinia cart store. The API manages server-side cart items for an authenticated customer. First slice requiring JWT authentication.

### Phase 6: Shopping Cart Frontend & Tests (Week 5, T-069 – T-078)
**Objective:** Build the cart Vue UI (`AddToCartButton`, `CartView`, `CartSummaryBadge`), add the `/cart` route, route `/cart*` through YARP, and complete backend unit tests, frontend unit tests, and an E2E Playwright spec for the cart flow. The cart is fully functional in Vue + API.

### Phase 7: Checkout Backend (Weeks 6–7, T-079 – T-090)
**Objective:** Port the address, order, order item, and customer domain models, checkout DTOs, repositories, `CheckoutService` (verbatim, preserving transaction and idempotency logic), and `OrderService`. Register services in DI. Implement `CheckoutController` (with `Idempotency-Key` requirement) and read-only `OrdersController`. Highest business risk due to transactions and order placement.

### Phase 8: Checkout Frontend & Tests (Weeks 7–8, T-091 – T-103)
**Objective:** Build the checkout Vue UI (Pinia checkout store with idempotency key, address selector/form, address step, review step, order confirmation, checkout wizard), wire checkout and confirmation routes, route `/checkout*` through YARP, and complete backend unit tests, frontend unit tests, and a full E2E Playwright spec with DB verification. The complete checkout flow works end-to-end.

### Phase 9: Order History, Home & Search (Weeks 9–11, T-104 – T-116)
**Objective:** Build the order history frontend (list, detail, routes, YARP, tests), integrate legacy account page links in the Vue header, then migrate the remaining public read-only pages: `HomeView` with featured products, header search, home route, YARP routes for homepage and search, and corresponding tests. All public read-only pages and order history are served by Vue + API; legacy fallback remains only for account/admin routes.

### Phase 10: Decommission Legacy (Week 12+, T-117 – T-125)
**Objective:** Confirm zero unexpected legacy traffic via YARP log analysis, build Vue login and register pages, add API auth endpoints issuing JWTs directly, switch the auth store from the legacy bridge to the API, remove the YARP legacy fallback, shut down the legacy application, optionally consolidate databases, and run the final full E2E regression suite. The legacy system is decommissioned; all traffic is served by Vue + API.

---

## 5. Task Dependencies

| Task | Depends On |
|------|-----------|
| 0.2 | 0.1 |
| 0.3 | 0.2 |
| 0.4 | 0.3 |
| 0.5 | 0.4 |
| 0.6 | 0.5 |
| 0.7 | 0.2 |
| 0.8 | 0.7 |
| 0.9 | 0.8 |
| 0.10 | 0.9 |
| 0.11 | 0.2 |
| 0.12 | 0.11 |
| 0.13 | 0.12 |
| 0.14 | 0.11 |
| 0.15 | 0.1 |
| 0.16 | 0.15 |
| 0.17 | 0.15 |
| 0.18 | 0.16, 0.13 |
| 0.19 | 0.16, 0.14 |
| 0.20 | 0.6, 0.15, 0.19 |
| 1.1 | 0.8 |
| 1.2 | 1.1 |
| 1.3 | 0.8 |
| 1.4 | 0.9, 1.3 |
| 1.5 | 1.3 |
| 1.6 | 1.4, 1.5 |
| 1.7 | 1.6 |
| 1.8 | 1.6, 1.2 |
| 1.9 | 1.6 |
| 1.10 | 0.5 |
| 1.11 | 0.18, 1.8 |
| 1.12 | 0.18, 1.8 |
| 1.13 | 1.11 |
| 1.14 | 0.16 |
| 1.15 | 0.16 |
| 1.16 | 1.6 |
| 1.17 | 1.8 |
| 1.18 | 1.13 |
| 1.19 | 1.14 |
| 1.20 | 1.11, 1.12 |
| 2.1 | 1.1 |
| 2.2 | 2.1 |
| 2.3 | 1.3, 0.9 |
| 2.4 | 2.3, 1.5 |
| 2.5 | 2.4, 0.13 |
| 2.6 | 0.18 |
| 2.7 | 0.18, 2.5 |
| 2.8 | 2.7 |
| 2.9 | 0.16 |
| 2.10 | 1.15 |
| 2.11 | 1.10 |
| 2.12 | 2.5 |
| 2.13 | 2.4 |
| 2.14 | 2.6 |
| 2.15 | 2.7 |
| 2.16 | 2.7 |
| 3.1 | 2.1 |
| 3.2 | 3.1 |
| 3.3 | 0.9 |
| 3.4 | 3.3, 2.4 |
| 3.5 | 3.3 |
| 3.6 | 3.4, 0.13 |
| 3.7 | 3.5 |
| 3.8 | 0.16 |
| 3.9 | 3.8 |
| 3.10 | 3.8 |
| 3.11 | 3.8 |
| 3.12 | 0.16 |
| 3.13 | 2.10 |
| 3.14 | 2.11 |
| 3.15 | 3.4 |
| 3.16 | 3.6 |
| 3.17 | 3.8 |
| 3.18 | 3.8 |
| 4.1 | 3.7, 0.18 |
| 4.2 | 3.7 |
| 4.3 | 3.13 |
| 4.4 | 3.14 |
| 4.5 | 4.1, 4.2 |
| 4.6 | 4.1 |
| 4.7 | 0.14 |
| 5.1 | 1.11 |
| 5.2 | 1.8 |
| 5.3 | 4.3 |
| 5.4 | 4.4 |
| 5.5 | 5.1 |
| 5.6 | 5.1 |
| 6.1 | 5.4 |
| 6.2 | 0.16 |
| 6.3 | 0.16 |
| 6.4 | 0.13 |
| 6.5 | 6.4 |
| 6.6 | 6.1 |
| 6.7 | 6.6 |
| 6.8 | 6.7 |
| 6.9 | All prior phases |

---

## 6. Entry Criteria, Exit Criteria, Verification Checklist, Deliverables, and Rollback for Every Task

### Phase 1: Infrastructure & .NET Solution Foundation (T-001 – T-015)

#### Task 0.1 — Create `modern/` Directory
- **Entry Criteria:** Monorepo design approved by mentor. Developer has write access to repository.
- **Exit Criteria:** `modern/` folder exists at repository root. Git tracks it (`.gitkeep` or first file committed).
- **Verification Checklist:**
  - [ ] `dir modern/` (Windows) or `ls modern/` (Linux/Mac) shows empty or initialized folder.
  - [ ] Git status shows `modern/` as an untracked or tracked folder.
- **Deliverables:** `modern/` directory in repository.
- **Rollback Strategy:** `git checkout -- modern/` or `rmdir modern /s /q`. No side effects.

#### Task 0.2 — Create .NET 10 API Solution & Project
- **Entry Criteria:** Task 0.1 complete. .NET 10 SDK installed (`dotnet --version` prints 10.x).
- **Exit Criteria:** Solution file and API project file exist. `dotnet build` at solution level succeeds with zero errors.
- **Verification Checklist:**
  - [ ] `modern/Ecommerce.sln` exists.
  - [ ] `modern/Ecommerce.Api/Ecommerce.Api.csproj` exists with `<TargetFramework>net10.0</TargetFramework>`.
  - [ ] `dotnet build` from `modern/` returns `Build succeeded`.
- **Deliverables:** `Ecommerce.sln`, `Ecommerce.Api.csproj`, `Program.cs`, `appsettings.json`.
- **Rollback Strategy:** Delete `modern/Ecommerce.sln` and `modern/Ecommerce.Api/` folders. Re-run Task 0.2.

#### Task 0.3 — Add YARP to API
- **Entry Criteria:** Task 0.2 complete.
- **Exit Criteria:** `Ecommerce.Api.csproj` contains `Yarp.ReverseProxy` package reference. `dotnet restore` succeeds.
- **Verification Checklist:**
  - [ ] Open `Ecommerce.Api.csproj` and confirm `<PackageReference Include="Yarp.ReverseProxy" ... />` exists.
  - [ ] `dotnet restore` completes without 404 package errors.
- **Deliverables:** Modified `Ecommerce.Api.csproj`.
- **Rollback Strategy:** Revert `csproj` file to state from Task 0.2. Remove `obj/` and `bin/` and re-restore.

#### Task 0.4 — Configure YARP Routes in Program.cs
- **Entry Criteria:** Task 0.3 complete.
- **Exit Criteria:** `Program.cs` contains `AddReverseProxy()` and `MapReverseProxy()` calls. Project builds.
- **Verification Checklist:**
  - [ ] `Program.cs` includes `builder.Services.AddReverseProxy()`.
  - [ ] `Program.cs` includes `app.MapReverseProxy()`.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Modified `Program.cs`.
- **Rollback Strategy:** Revert `Program.cs` to Task 0.2 state.

#### Task 0.5 — Add YARP `appsettings.json` Routes
- **Entry Criteria:** Task 0.4 complete.
- **Exit Criteria:** `appsettings.json` contains `ReverseProxy` section with clusters (`apiCluster`, `spaCluster`, `legacyCluster`) and routes (`/api/*`, `/app/*`, `/*`). JSON is syntactically valid.
- **Verification Checklist:**
  - [ ] `appsettings.json` parses without JSON syntax errors (use an online validator or IDE).
  - [ ] `ReverseProxy.Routes` contains at least 3 route definitions.
  - [ ] `ReverseProxy.Clusters` contains `apiCluster`, `spaCluster`, `legacyCluster`.
- **Deliverables:** Modified `appsettings.json`.
- **Rollback Strategy:** Revert `appsettings.json` to Task 0.2 state.

#### Task 0.6 — Verify YARP Builds & Runs
- **Entry Criteria:** Tasks 0.4 and 0.5 complete.
- **Exit Criteria:** API process starts and listens on configured port. A manual HTTP request to `/api/health` (or any path matching the API route) returns a response.
- **Verification Checklist:**
  - [ ] `dotnet run` from `Ecommerce.Api/` starts without unhandled exceptions.
  - [ ] Process listens on expected port (e.g., 5001).
  - [ ] `curl http://localhost:5001/api/health` or browser visit returns HTTP response (200 or 404 — either means routing is active).
- **Deliverables:** Running API instance verified.
- **Rollback Strategy:** Stop process. Revert `Program.cs` and `appsettings.json`. Re-run Task 0.4.

#### Task 0.7 — Add Shared Projects to Solution
- **Entry Criteria:** Task 0.2 complete.
- **Exit Criteria:** Four class library projects exist and are added to solution. Solution builds.
- **Verification Checklist:**
  - [ ] `modern/Ecommerce.Api.Core/`, `Ecommerce.Api.Services/`, `Ecommerce.Api.Data/`, `Ecommerce.Api.Migrations/` exist.
  - [ ] `dotnet sln list` shows all 4 projects plus `Ecommerce.Api`.
  - [ ] `dotnet build` from `modern/` succeeds.
- **Deliverables:** 4 new `.csproj` files, updated `.sln` file.
- **Rollback Strategy:** Remove project folders from disk. Edit `.sln` file to remove references.

#### Task 0.8 — Wire Project References
- **Entry Criteria:** Task 0.7 complete.
- **Exit Criteria:** All inter-project references configured. Build succeeds with zero errors.
- **Verification Checklist:**
  - [ ] `Ecommerce.Api` references `Ecommerce.Api.Services` and `Ecommerce.Api.Core`.
  - [ ] `Ecommerce.Api.Services` references `Ecommerce.Api.Core` and `Ecommerce.Api.Data`.
  - [ ] `Ecommerce.Api.Data` references `Ecommerce.Api.Core`.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Modified `.csproj` files with `<ProjectReference>` nodes.
- **Rollback Strategy:** Remove `<ProjectReference>` nodes and rebuild.

#### Task 0.9 — Port DatabaseHelper
- **Entry Criteria:** Tasks 0.8 and legacy repo read access.
- **Exit Criteria:** `DatabaseHelper.cs` copied to `Ecommerce.Api.Data/`. `System.Data.SqlClient` replaced with `Microsoft.Data.SqlClient`. `Microsoft.Data.SqlClient` NuGet package added. Build succeeds.
- **Verification Checklist:**
  - [ ] File exists at `src/backend/Ecommerce.Api.Data/DatabaseHelper.cs`.
  - [ ] No `using System.Data.SqlClient;` remains in the file.
  - [ ] `Ecommerce.Api.Data.csproj` contains `Microsoft.Data.SqlClient` package.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** `DatabaseHelper.cs`, updated `Ecommerce.Api.Data.csproj`.
- **Rollback Strategy:** Delete ported file. Remove NuGet package reference.

#### Task 0.10 — Test Database Connectivity
- **Entry Criteria:** Task 0.9 complete. SQL Server accessible with `LegacyEcommerceDb` connection string.
- **Exit Criteria:** A temporary health-check endpoint executes `SELECT 1` via `DatabaseHelper` and returns HTTP 200.
- **Verification Checklist:**
  - [ ] Create `HealthController.cs` with `[HttpGet("health")]` action.
  - [ ] Action calls `_db.ExecuteScalar("SELECT 1")`.
  - [ ] `GET /api/health` returns 200.
  - [ ] API logs show successful SQL connection.
- **Deliverables:** `HealthController.cs` (temporary; can be deleted after verification or kept).
- **Rollback Strategy:** Delete `HealthController.cs`. Investigate connection string or SQL Server accessibility.

---

### Phase 2: JWT Auth Bridge & Vue SPA Shell (T-016 – T-028)

#### Task 0.11 — Add JWT NuGet Packages
- **Entry Criteria:** Task 0.2 complete.
- **Exit Criteria:** `Microsoft.AspNetCore.Authentication.JwtBearer` added to API project. Legacy `Ecommerce.Web` has `System.IdentityModel.Tokens.Jwt` installed.
- **Verification Checklist:**
  - [ ] `Ecommerce.Api.csproj` contains JWT Bearer package.
  - [ ] Legacy project references JWT package (check `packages.config` or `packages/` folder).
  - [ ] Both projects restore/build successfully.
- **Deliverables:** Updated `.csproj` / `packages.config` files.
- **Rollback Strategy:** Remove package references and re-restore.

#### Task 0.12 — Implement JWT Token Service (New API)
- **Entry Criteria:** Task 0.11 complete.
- **Exit Criteria:** `JwtTokenService` class exists in `Ecommerce.Api.Core/Security/`. It can generate and validate tokens. Unit test passes.
- **Verification Checklist:**
  - [ ] `JwtTokenService.GenerateToken(userId, customerId, email)` returns non-null string.
  - [ ] A unit test validates the token payload contains expected claims.
  - [ ] `dotnet test` for the service test passes.
- **Deliverables:** `JwtTokenService.cs`, unit test file.
- **Rollback Strategy:** Delete service class and test. Re-implement.

#### Task 0.13 — Configure JWT Auth in API
- **Entry Criteria:** Task 0.12 complete.
- **Exit Criteria:** `Program.cs` configures JWT Bearer authentication. `[Authorize]` on a test endpoint returns 401 without token, 200 with valid token.
- **Verification Checklist:**
  - [ ] `Program.cs` calls `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)`.
  - [ ] `app.UseAuthentication()` and `app.UseAuthorization()` are in correct order.
  - [ ] Create `[Authorize]` test endpoint. Request without token returns 401.
  - [ ] Same request with valid Bearer token returns 200.
- **Deliverables:** Modified `Program.cs`.
- **Rollback Strategy:** Revert `Program.cs` JWT configuration blocks.

#### Task 0.14 — Add JWT Bridge Endpoint to Legacy MVC
- **Entry Criteria:** Task 0.11 complete. Legacy repo is writeable (or a branch exists for bridge work).
- **Exit Criteria:** Legacy `AccountController` has `IssueToken()` action that returns JWT JSON for authenticated users, 302 redirect for anonymous users.
- **Verification Checklist:**
  - [ ] Logged-in user visits `/Account/IssueToken` and receives JSON `{ "token": "..." }`.
  - [ ] Anonymous user is redirected to `/Account/Login`.
  - [ ] Token validates successfully using the same key as Task 0.13.
- **Deliverables:** Modified `AccountController.cs` in legacy repo (or a documented patch).
- **Rollback Strategy:** Revert `AccountController.cs` to pre-migration state.

#### Task 0.15 — Scaffold Vue SPA
- **Entry Criteria:** Task 0.1 complete. Node.js 20+ and npm installed.
- **Exit Criteria:** `modern/Ecommerce.Spa/` exists with standard Vite + Vue 3 + TypeScript scaffold. `npm install` succeeds. `npm run dev` starts dev server.
- **Verification Checklist:**
  - [ ] `package.json` exists with `vue`, `vite`, `typescript` dependencies.
  - [ ] `npm install` completes with zero critical vulnerabilities.
  - [ ] `npm run dev` starts server on expected port (e.g., 5173 or 5002).
  - [ ] Browser displays default Vite + Vue welcome page.
- **Deliverables:** Complete Vite scaffold folder.
- **Rollback Strategy:** Delete `modern/Ecommerce.Spa/` folder. Re-run scaffold command.

#### Task 0.16 — Add Pinia & Vue Router
- **Entry Criteria:** Task 0.15 complete.
- **Exit Criteria:** `pinia` and `vue-router` listed in `package.json`. Dev server still starts cleanly.
- **Verification Checklist:**
  - [ ] `package.json` contains `pinia` and `vue-router`.
  - [ ] `npm install` succeeds.
  - [ ] `npm run dev` starts without errors.
- **Deliverables:** Updated `package.json`, `package-lock.json`.
- **Rollback Strategy:** Revert `package.json` and re-install.

#### Task 0.17 — Configure API Base URL
- **Entry Criteria:** Task 0.15 complete.
- **Exit Criteria:** `.env.development` exists with `VITE_API_BASE_URL=http://localhost:5001/api`.
- **Verification Checklist:**
  - [ ] `.env.development` file exists.
  - [ ] `import.meta.env.VITE_API_BASE_URL` resolves correctly in a test component.
- **Deliverables:** `.env.development` file.
- **Rollback Strategy:** Delete or edit `.env.development`.

#### Task 0.18 — Create `fetch` Wrapper with JWT
- **Entry Criteria:** Tasks 0.16 and 0.13 complete.
- **Exit Criteria:** `client.ts` exists in `src/api/`. Exports `get`, `post`, `put`, `del` functions that attach `Authorization: Bearer <token>` header.
- **Verification Checklist:**
  - [ ] File exists at `src/api/client.ts`.
  - [ ] Functions read token from Pinia `useAuthStore`.
  - [ ] Manual test: call wrapper from browser console → Network tab shows Bearer header.
- **Deliverables:** `client.ts`.
- **Rollback Strategy:** Delete `client.ts`. Re-implement.

#### Task 0.19 — Create Pinia Auth Store
- **Entry Criteria:** Tasks 0.16 and 0.14 complete.
- **Exit Criteria:** `useAuthStore.ts` exists. Can fetch token from `/Account/IssueToken` via proxy and persist to `localStorage`.
- **Verification Checklist:**
  - [ ] Store initializes with `token` ref.
  - [ ] `fetchToken()` calls legacy bridge endpoint successfully.
  - [ ] Token is stored in `localStorage`.
  - [ ] `isAuthenticated` computed property returns true after fetch.
- **Deliverables:** `useAuthStore.ts`.
- **Rollback Strategy:** Delete store file. Re-implement.

#### Task 0.20 — Verify Full Proxy Chain
- **Entry Criteria:** Tasks 0.6, 0.15, and 0.19 complete.
- **Exit Criteria:** All 4 services (Legacy 5000, API 5001, SPA 5002, YARP 5003) running. Browser visit to `http://localhost:5003/app/` loads Vue. Auth store fetches JWT. Test API call with Bearer token succeeds.
- **Verification Checklist:**
  - [ ] Legacy MVC runs on port 5000 and serves existing pages.
  - [ ] API runs on port 5001 and responds to `/api/health`.
  - [ ] SPA dev server runs on port 5002 (or 5173 if using Vite default).
  - [ ] YARP runs on port 5003.
  - [ ] `http://localhost:5003/app/` → Vue app loads.
  - [ ] Vue auth store fetches JWT from `/Account/IssueToken` (via YARP).
  - [ ] `GET /api/health` with Bearer token returns 200.
- **Deliverables:** Verified running system. Screenshot of browser network tab showing successful JWT fetch and API call.
- **Rollback Strategy:** Stop all services. Investigate which link in the chain failed (YARP config, auth setup, CORS, network). Fix and re-verify.

---

### Phase 3: Product Catalog Backend (T-029 – T-045)

#### Task 1.1 — Port Domain Models
- **Entry Criteria:** Task 0.8 complete.
- **Exit Criteria:** `Product.cs`, `Category.cs`, `ProductImage.cs`, `ProductVariant.cs` copied to `Ecommerce.Api.Core/Models/`. No EF references. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist in `src/backend/Ecommerce.Api.Core/Models/`.
  - [ ] No `using System.Data.Entity;` or EF-specific attributes.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** 4 model files.
- **Rollback Strategy:** Delete ported model files.

#### Task 1.2 — Create DTOs
- **Entry Criteria:** Task 1.1 complete.
- **Exit Criteria:** `ProductDto.cs`, `ProductDetailDto.cs`, `CategoryDto.cs`, `PagedResult.cs` exist. No circular references. Build succeeds.
- **Verification Checklist:**
  - [ ] DTO files exist in `src/backend/Ecommerce.Api.Core/DTOs/`.
  - [ ] `ProductDetailDto` does not reference `Product` entity directly.
  - [ ] `PagedResult<T>` is generic and reusable.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** DTO files.
- **Rollback Strategy:** Delete DTO files.

#### Task 1.3 — Port Repository Interfaces
- **Entry Criteria:** Task 0.8 complete.
- **Exit Criteria:** `IProductRepository.cs`, `ICategoryRepository.cs` copied to `Ecommerce.Api.Core/Interfaces/`. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist in `Interfaces/`.
  - [ ] Methods use domain models (not ViewModels).
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Interface files.
- **Rollback Strategy:** Delete interface files.

#### Task 1.4 — Port Repository Implementations
- **Entry Criteria:** Tasks 0.9 and 1.3 complete.
- **Exit Criteria:** `ProductRepository.cs`, `CategoryRepository.cs` copied to `Ecommerce.Api.Data/Repositories/`. `System.Data.SqlClient` replaced. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist in `Repositories/`.
  - [ ] `using Microsoft.Data.SqlClient;` present.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Repository implementation files.
- **Rollback Strategy:** Delete repository files.

#### Task 1.5 — Port Service Interfaces
- **Entry Criteria:** Task 1.3 complete.
- **Exit Criteria:** `IProductService.cs`, `ICategoryService.cs` copied to `Ecommerce.Api.Services/Interfaces/`. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist in `Services/Interfaces/`.
  - [ ] Method signatures match legacy but return domain models/DTOs.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Service interface files.
- **Rollback Strategy:** Delete service interface files.

#### Task 1.6 — Port Service Implementations
- **Entry Criteria:** Tasks 1.4 and 1.5 complete.
- **Exit Criteria:** `ProductService.cs`, `CategoryService.cs` copied to `Ecommerce.Api.Services/Implementations/`. Namespaces updated. Return types updated to DTOs. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist in `Implementations/`.
  - [ ] No Razor/ViewModel references remain.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Service implementation files.
- **Rollback Strategy:** Delete service files.

#### Task 1.7 — Register DI Services
- **Entry Criteria:** Task 1.6 complete.
- **Exit Criteria:** `Program.cs` registers all repositories and services. Build succeeds. DI container resolves `IProductService` without runtime error.
- **Verification Checklist:**
  - [ ] `builder.Services.AddScoped<IProductRepository, ProductRepository>();` present.
  - [ ] `builder.Services.AddScoped<IProductService, ProductService>();` present.
  - [ ] API starts without DI exceptions.
- **Deliverables:** Modified `Program.cs`.
- **Rollback Strategy:** Remove DI registration lines.

#### Task 1.8 — Create ProductsController
- **Entry Criteria:** Tasks 1.6 and 1.2 complete.
- **Exit Criteria:** `ProductsController.cs` exists with `GET /api/products` and `GET /api/products/{id}`. Returns JSON. Swagger/Postman test returns expected DTO shape.
- **Verification Checklist:**
  - [ ] Controller file exists in `Controllers/`.
  - [ ] `GET /api/products` returns `PagedResult<ProductDto>`.
  - [ ] `GET /api/products/{id}` returns `ProductDetailDto`.
  - [ ] Response shape matches DTO design.
- **Deliverables:** `ProductsController.cs`.
- **Rollback Strategy:** Delete controller file.

#### Task 1.9 — Create CategoriesController
- **Entry Criteria:** Task 1.6 complete.
- **Exit Criteria:** `CategoriesController.cs` exists with `GET /api/categories`. Returns `CategoryDto[]`.
- **Verification Checklist:**
  - [ ] Controller file exists.
  - [ ] `GET /api/categories` returns array of categories.
- **Deliverables:** `CategoriesController.cs`.
- **Rollback Strategy:** Delete controller file.

#### Task 1.10 — Add YARP Product Routes
- **Entry Criteria:** Task 0.5 complete.
- **Exit Criteria:** YARP config routes `/products*` and `/product/*` to `spaCluster`. Visiting these paths serves Vue SPA.
- **Verification Checklist:**
  - [ ] `appsettings.json` (or `Program.cs`) contains `/products*` route.
  - [ ] `http://localhost:5003/products` returns Vue SPA (not legacy MVC HTML).
- **Deliverables:** Modified YARP configuration.
- **Rollback Strategy:** Remove `/products*` route rule. Traffic falls back to legacy.

---

### Phase 4: Product Catalog Frontend & Tests (T-046 – T-059)

#### Task 1.11 — Create ProductListView.vue
- **Entry Criteria:** Tasks 0.18 and 1.8 complete.
- **Exit Criteria:** `ProductListView.vue` exists. Fetches products from API. Renders grid. Handles query params.
- **Verification Checklist:**
  - [ ] File exists at `src/views/ProductListView.vue`.
  - [ ] Grid renders with real data from `GET /api/products`.
  - [ ] URL query params update when filters change.
- **Deliverables:** `ProductListView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 1.12 — Create ProductDetailView.vue
- **Entry Criteria:** Tasks 0.18 and 1.8 complete.
- **Exit Criteria:** `ProductDetailView.vue` exists. Fetches product detail. Renders gallery, variants, related products.
- **Verification Checklist:**
  - [ ] File exists at `src/views/ProductDetailView.vue`.
  - [ ] Visiting `/products/123` renders detail for product ID 123.
- **Deliverables:** `ProductDetailView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 1.13 — Create Reusable Components
- **Entry Criteria:** Task 1.11 complete.
- **Exit Criteria:** `ProductCard.vue`, `CategoryFilter.vue`, `PaginationControls.vue`, `LoadingSpinner.vue`, `ErrorAlert.vue` exist and render correctly.
- **Verification Checklist:**
  - [ ] Each component file exists in `src/components/`.
  - [ ] Components render in Storybook or Vitest without errors.
- **Deliverables:** 5 component files.
- **Rollback Strategy:** Delete component files.

#### Task 1.14 — Create Pinia Product Store
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** `useProductStore.ts` exists. Manages filters, pagination, cache. Triggers refetch on filter change.
- **Verification Checklist:**
  - [ ] Store file exists.
  - [ ] Changing `filters.categoryId` triggers new API call.
  - [ ] Pagination updates URL and fetches correct page.
- **Deliverables:** `useProductStore.ts`.
- **Rollback Strategy:** Delete store file.

#### Task 1.15 — Configure Vue Router
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** Vue Router config has `/products` → `ProductListView.vue`, `/products/:id` → `ProductDetailView.vue`. Navigation works. Browser back/forward preserves state.
- **Verification Checklist:**
  - [ ] `router/index.ts` exists with routes.
  - [ ] Clicking product navigates to detail.
  - [ ] Browser back button returns to list with filters intact.
- **Deliverables:** `router/index.ts`.
- **Rollback Strategy:** Revert router config.

#### Task 1.16 — Backend Unit Tests — Services
- **Entry Criteria:** Task 1.6 complete.
- **Exit Criteria:** `ProductServiceTests.cs` exists. Mock `IUnitOfWork`/`IProductRepository`. Verify pagination logic. `dotnet test` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `dotnet test` shows passing tests.
- **Deliverables:** `ProductServiceTests.cs`.
- **Rollback Strategy:** Delete test file. Does not affect production code.

#### Task 1.17 — Backend Unit Tests — Controllers
- **Entry Criteria:** Task 1.8 complete.
- **Exit Criteria:** `ProductsControllerTests.cs` exists. Mock `IProductService`. Verify 200/404 responses. `dotnet test` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `dotnet test` shows passing tests.
- **Deliverables:** `ProductsControllerTests.cs`.
- **Rollback Strategy:** Delete test file.

#### Task 1.18 — Frontend Unit Tests — Components
- **Entry Criteria:** Task 1.13 complete.
- **Exit Criteria:** Vitest tests for `ProductCard.vue` verify props rendering and event emission. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test files exist in `tests/unit/components/`.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Component test files.
- **Rollback Strategy:** Delete test files.

#### Task 1.19 — Frontend Unit Tests — Store
- **Entry Criteria:** Task 1.14 complete.
- **Exit Criteria:** Vitest test for `useProductStore` verifies fetch calls correct URL. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Store test file.
- **Rollback Strategy:** Delete test file.

#### Task 1.20 — E2E Test — Product Catalog
- **Entry Criteria:** Tasks 1.11 and 1.12 complete.
- **Exit Criteria:** Playwright spec visits `/products`, filters by category, clicks product, asserts detail page. `npx playwright test` passes.
- **Verification Checklist:**
  - [ ] Spec file exists at `tests/e2e/specs/product-catalog.spec.ts`.
  - [ ] `npx playwright test` passes.
- **Deliverables:** E2E spec file.
- **Rollback Strategy:** Delete E2E spec. Does not affect production code.

---

### Phase 5: Shopping Cart Backend & Store (T-060 – T-068)

#### Task 2.1 — Port Cart Domain Models
- **Entry Criteria:** Task 1.1 complete.
- **Exit Criteria:** `CartItem.cs`, `ProductVariant.cs` (if not already ported) exist in `Models/`. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Model files.
- **Rollback Strategy:** Delete model files.

#### Task 2.2 — Create Cart DTOs
- **Entry Criteria:** Task 2.1 complete.
- **Exit Criteria:** `CartItemDto.cs`, `CartSummaryDto.cs`, `AddCartItemRequest.cs` exist. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist in `DTOs/`.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** DTO files.
- **Rollback Strategy:** Delete DTO files.

#### Task 2.3 — Port Cart Repository
- **Entry Criteria:** Tasks 1.3 and 0.9 complete.
- **Exit Criteria:** `ICartItemRepository`, `CartItemRepository` ported. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Repository files.
- **Rollback Strategy:** Delete repository files.

#### Task 2.4 — Port CartService
- **Entry Criteria:** Tasks 2.3 and 1.5 complete.
- **Exit Criteria:** `CartService.cs` ported. Return types changed to DTOs. Build succeeds.
- **Verification Checklist:**
  - [ ] File exists in `Implementations/`.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** `CartService.cs`.
- **Rollback Strategy:** Delete service file.

#### Task 2.5 — Create CartController
- **Entry Criteria:** Tasks 2.4 and 0.13 complete.
- **Exit Criteria:** `CartController.cs` exists (all actions `[Authorize]`). Returns correct DTOs. 401 for anonymous, 200 for authenticated.
- **Verification Checklist:**
  - [ ] Controller exists with `GET /api/cart`, `POST /api/cart/items`, `PUT /api/cart/items/{id}`, `DELETE /api/cart/items/{id}`, `GET /api/cart/summary`.
  - [ ] Anonymous requests return 401.
  - [ ] Authenticated requests return 200.
- **Deliverables:** `CartController.cs`.
- **Rollback Strategy:** Delete controller file.

---

### Phase 6: Shopping Cart Frontend & Tests (T-069 – T-078)

#### Task 2.6 — Create AddToCartButton.vue
- **Entry Criteria:** Task 0.18 complete.
- **Exit Criteria:** Component exists. Click calls `cartStore.addItem()`. Includes Bearer token.
- **Verification Checklist:**
  - [ ] Component file exists.
  - [ ] Click triggers API call with `Authorization: Bearer` header.
- **Deliverables:** `AddToCartButton.vue`.
- **Rollback Strategy:** Delete component.

#### Task 2.7 — Create CartView.vue
- **Entry Criteria:** Tasks 0.18 and 2.5 complete.
- **Exit Criteria:** View exists. Fetches cart. Renders table. Quantity update persists. Remove works.
- **Verification Checklist:**
  - [ ] View file exists.
  - [ ] Cart renders with real data.
  - [ ] Quantity change persists after refresh.
- **Deliverables:** `CartView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 2.8 — Create CartSummaryBadge.vue
- **Entry Criteria:** Task 2.7 complete.
- **Exit Criteria:** Badge component exists. Displays `itemCount`. Updates when items added.
- **Verification Checklist:**
  - [ ] Component exists in `TheHeader.vue`.
  - [ ] Badge number updates after adding to cart.
- **Deliverables:** `CartSummaryBadge.vue`.
- **Rollback Strategy:** Delete component.

#### Task 2.9 — Create Pinia Cart Store
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** `useCartStore.ts` exists. Methods call correct API endpoints. No localStorage persistence (server-side cart).
- **Verification Checklist:**
  - [ ] Store file exists.
  - [ ] `addItem` calls `POST /api/cart/items`.
  - [ ] `fetchCart` calls `GET /api/cart`.
- **Deliverables:** `useCartStore.ts`.
- **Rollback Strategy:** Delete store file.

#### Task 2.10 — Update Vue Router
- **Entry Criteria:** Task 1.15 complete.
- **Exit Criteria:** `/cart` route added → `CartView.vue`.
- **Verification Checklist:**
  - [ ] Router config contains `/cart` route.
  - [ ] Navigation to `/cart` works.
- **Deliverables:** Modified `router/index.ts`.
- **Rollback Strategy:** Revert router changes.

#### Task 2.11 — Update YARP Routes
- **Entry Criteria:** Task 1.10 complete.
- **Exit Criteria:** `/cart*` routed to `spaCluster`. Visiting `/cart` serves Vue SPA.
- **Verification Checklist:**
  - [ ] YARP config updated.
  - [ ] `http://localhost:5003/cart` returns Vue SPA.
- **Deliverables:** Modified YARP config.
- **Rollback Strategy:** Remove `/cart*` route.

#### Task 2.12 — Backend Tests — CartController
- **Entry Criteria:** Task 2.5 complete.
- **Exit Criteria:** Tests mock `ClaimsPrincipal` with `customer_id`. Verify 401 without auth, 200 with auth. `dotnet test` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `dotnet test` passes.
- **Deliverables:** `CartControllerTests.cs`.
- **Rollback Strategy:** Delete test file.

#### Task 2.13 — Backend Tests — CartService
- **Entry Criteria:** Task 2.4 complete.
- **Exit Criteria:** Mock `ICartItemRepository`. Verify add, update, remove logic. `dotnet test` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `dotnet test` passes.
- **Deliverables:** `CartServiceTests.cs`.
- **Rollback Strategy:** Delete test file.

#### Task 2.14 — Frontend Tests — AddToCartButton
- **Entry Criteria:** Task 2.6 complete.
- **Exit Criteria:** Vitest: click emits event, calls store method. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Component test file.
- **Rollback Strategy:** Delete test file.

#### Task 2.15 — Frontend Tests — CartView
- **Entry Criteria:** Task 2.7 complete.
- **Exit Criteria:** Vitest: renders items, handles quantity emit. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Component test file.
- **Rollback Strategy:** Delete test file.

#### Task 2.16 — E2E Test — Cart Flow
- **Entry Criteria:** Task 2.7 complete.
- **Exit Criteria:** Playwright: add to cart → visit cart → update qty → remove → verify empty. `npx playwright test` passes.
- **Verification Checklist:**
  - [ ] Spec file exists.
  - [ ] `npx playwright test` passes.
- **Deliverables:** E2E spec file.
- **Rollback Strategy:** Delete E2E spec.

---

### Phase 7: Checkout Backend (T-079 – T-090)

#### Task 3.1 — Port Address/Order Models
- **Entry Criteria:** Task 2.1 complete.
- **Exit Criteria:** `Address.cs`, `Order.cs`, `OrderItem.cs`, `Customer.cs` copied to `Models/`. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Model files.
- **Rollback Strategy:** Delete model files.

#### Task 3.2 — Create Checkout DTOs
- **Entry Criteria:** Task 3.1 complete.
- **Exit Criteria:** `AddressDto.cs`, `OrderDto.cs`, `OrderItemDto.cs`, `PlaceOrderRequest.cs`, `OrderConfirmationDto.cs` exist. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** DTO files.
- **Rollback Strategy:** Delete DTO files.

#### Task 3.3 — Port Address/Order Repositories
- **Entry Criteria:** Task 0.9 complete.
- **Exit Criteria:** `IAddressRepository`, `IOrderRepository`, `ICustomerRepository` + implementations ported. Build succeeds.
- **Verification Checklist:**
  - [ ] Files exist.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** Repository files.
- **Rollback Strategy:** Delete repository files.

#### Task 3.4 — Port CheckoutService Verbatim
- **Entry Criteria:** Tasks 3.3 and 2.4 complete.
- **Exit Criteria:** `CheckoutService.cs` copied exactly. Only namespaces and final return type changed. Transaction logic intact. Idempotency logic intact. Build succeeds.
- **Verification Checklist:**
  - [ ] File exists in `Implementations/`.
  - [ ] Diff against legacy shows only namespace and return type changes.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** `CheckoutService.cs`.
- **Rollback Strategy:** Delete service file. Re-port from legacy.

#### Task 3.5 — Port OrderService
- **Entry Criteria:** Task 3.3 complete.
- **Exit Criteria:** `OrderService.cs` ported. Build succeeds.
- **Verification Checklist:**
  - [ ] File exists.
  - [ ] `dotnet build` succeeds.
- **Deliverables:** `OrderService.cs`.
- **Rollback Strategy:** Delete service file.

#### Task 3.6 — Create CheckoutController
- **Entry Criteria:** Tasks 3.4 and 0.13 complete.
- **Exit Criteria:** Controller exists with `GET /api/checkout/addresses`, `POST /api/checkout/addresses`, `GET /api/checkout/review`, `POST /api/checkout/orders`, `GET /api/checkout/orders/{id}/confirmation`. All `[Authorize]`. `PlaceOrder` requires `Idempotency-Key` header.
- **Verification Checklist:**
  - [ ] Controller exists.
  - [ ] `POST /api/checkout/orders` without `Idempotency-Key` returns 400.
  - [ ] Valid request returns 201.
- **Deliverables:** `CheckoutController.cs`.
- **Rollback Strategy:** Delete controller file.

#### Task 3.7 — Create OrdersController (Read-Only)
- **Entry Criteria:** Task 3.5 complete.
- **Exit Criteria:** `OrdersController.cs` exists with `GET /api/orders` and `GET /api/orders/{id}`. Returns orders for authenticated customer only.
- **Verification Checklist:**
  - [ ] Controller exists.
  - [ ] Returns only orders belonging to authenticated customer's `customer_id`.
- **Deliverables:** `OrdersController.cs`.
- **Rollback Strategy:** Delete controller file.

---

### Phase 8: Checkout Frontend & Tests (T-091 – T-103)

#### Task 3.8 — Create CheckoutWizard.vue
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** Wizard component exists. Step navigation works (Address → Review → Confirmation). State persists across steps.
- **Verification Checklist:**
  - [ ] Component file exists.
  - [ ] Can navigate forward and backward through steps.
  - [ ] State does not reset on step change.
- **Deliverables:** `CheckoutWizard.vue`.
- **Rollback Strategy:** Delete component.

#### Task 3.9 — Create AddressStep.vue
- **Entry Criteria:** Task 3.8 complete.
- **Exit Criteria:** Includes `AddressSelector.vue` (saved) and `AddressForm.vue` (new). Can select or create address.
- **Verification Checklist:**
  - [ ] Components exist.
  - [ ] Selecting address updates checkout store.
  - [ ] Creating new address posts to API.
- **Deliverables:** `AddressStep.vue`, `AddressSelector.vue`, `AddressForm.vue`.
- **Rollback Strategy:** Delete components.

#### Task 3.10 — Create ReviewStep.vue
- **Entry Criteria:** Task 3.8 complete.
- **Exit Criteria:** Displays cart summary + selected address + total. "Place Order" button visible.
- **Verification Checklist:**
  - [ ] Component exists.
  - [ ] Displays accurate order preview.
- **Deliverables:** `ReviewStep.vue`.
- **Rollback Strategy:** Delete component.

#### Task 3.11 — Create OrderConfirmation.vue
- **Entry Criteria:** Task 3.8 complete.
- **Exit Criteria:** Success page shows order number.
- **Verification Checklist:**
  - [ ] Component exists.
  - [ ] Displays order ID from route param.
- **Deliverables:** `OrderConfirmation.vue`.
- **Rollback Strategy:** Delete component.

#### Task 3.12 — Create Pinia Checkout Store
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** `useCheckoutStore.ts` exists. Holds `selectedAddress`, `idempotencyKey` (UUID generated on checkout start).
- **Verification Checklist:**
  - [ ] Store file exists.
  - [ ] `idempotencyKey` is a valid UUID generated on wizard mount.
- **Deliverables:** `useCheckoutStore.ts`.
- **Rollback Strategy:** Delete store file.

#### Task 3.13 — Update Vue Router
- **Entry Criteria:** Task 2.10 complete.
- **Exit Criteria:** `/checkout` → `CheckoutWizard.vue`, `/checkout/confirmation/:orderId` → `OrderConfirmation.vue`. Guards prevent direct access to later steps.
- **Verification Checklist:**
  - [ ] Routes added.
  - [ ] Direct navigation to `/checkout/confirmation/123` without checkout flow is blocked or handled.
- **Deliverables:** Modified `router/index.ts`.
- **Rollback Strategy:** Revert router changes.

#### Task 3.14 — Update YARP Routes
- **Entry Criteria:** Task 2.11 complete.
- **Exit Criteria:** `/checkout*` routed to `spaCluster`.
- **Verification Checklist:**
  - [ ] YARP config updated.
  - [ ] `http://localhost:5003/checkout` serves Vue SPA.
- **Deliverables:** Modified YARP config.
- **Rollback Strategy:** Remove `/checkout*` route.

#### Task 3.15 — Backend Tests — CheckoutService
- **Entry Criteria:** Task 3.4 complete.
- **Exit Criteria:** xUnit tests mock `IUnitOfWork`. Verify: transaction committed once, duplicate idempotency returns existing order, address created when null. `dotnet test` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `dotnet test` passes.
- **Deliverables:** `CheckoutServiceTests.cs`.
- **Rollback Strategy:** Delete test file.

#### Task 3.16 — Backend Tests — CheckoutController
- **Entry Criteria:** Task 3.6 complete.
- **Exit Criteria:** Verify 400 when `Idempotency-Key` missing. Verify 201 on first order, 200 on duplicate. `dotnet test` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `dotnet test` passes.
- **Deliverables:** `CheckoutControllerTests.cs`.
- **Rollback Strategy:** Delete test file.

#### Task 3.17 — Frontend Tests — CheckoutWizard
- **Entry Criteria:** Task 3.8 complete.
- **Exit Criteria:** Vitest: step navigation, form validation, submit sends correct headers. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Component test file.
- **Rollback Strategy:** Delete test file.

#### Task 3.18 — E2E Test — Full Checkout
- **Entry Criteria:** Task 3.8 complete.
- **Exit Criteria:** Playwright: cart → checkout → select/create address → review → place order → confirmation → verify DB has order + items + cart cleared.
- **Verification Checklist:**
  - [ ] Spec file exists.
  - [ ] `npx playwright test` passes.
  - [ ] SQL query confirms order exists in `Orders` table.
  - [ ] SQL query confirms `CartItems` are cleared for customer.
- **Deliverables:** E2E spec file.
- **Rollback Strategy:** Delete E2E spec.

---

### Phase 9: Order History, Home & Search (T-104 – T-116)

#### Task 4.1 — Create OrderListView.vue
- **Entry Criteria:** Tasks 3.7 and 0.18 complete.
- **Exit Criteria:** View exists. Fetches `GET /api/orders`. Renders table with pagination.
- **Verification Checklist:**
  - [ ] View file exists.
  - [ ] Renders real order history for authenticated customer.
- **Deliverables:** `OrderListView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 4.2 — Create OrderDetailView.vue
- **Entry Criteria:** Task 3.7 complete.
- **Exit Criteria:** View exists. Fetches `GET /api/orders/{id}`. Renders items and shipping info.
- **Verification Checklist:**
  - [ ] View file exists.
  - [ ] Displays order details correctly.
- **Deliverables:** `OrderDetailView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 4.3 — Update Vue Router
- **Entry Criteria:** Task 3.13 complete.
- **Exit Criteria:** `/orders` → `OrderListView.vue`, `/orders/:id` → `OrderDetailView.vue`.
- **Verification Checklist:**
  - [ ] Routes added.
  - [ ] Navigation works.
- **Deliverables:** Modified `router/index.ts`.
- **Rollback Strategy:** Revert router changes.

#### Task 4.4 — Update YARP Routes
- **Entry Criteria:** Task 3.14 complete.
- **Exit Criteria:** `/orders*` routed to `spaCluster`.
- **Verification Checklist:**
  - [ ] YARP config updated.
  - [ ] `http://localhost:5003/orders` serves Vue SPA.
- **Deliverables:** Modified YARP config.
- **Rollback Strategy:** Remove `/orders*` route.

#### Task 4.5 — Frontend Tests — Order Components
- **Entry Criteria:** Tasks 4.1 and 4.2 complete.
- **Exit Criteria:** Vitest tests for `OrderListView.vue` and `OrderDetailView.vue`. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test files exist.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Component test files.
- **Rollback Strategy:** Delete test files.

#### Task 4.6 — E2E Test — Order History
- **Entry Criteria:** Task 4.1 complete.
- **Exit Criteria:** Playwright: place order (from Phase 8) → visit `/orders` → click order → verify detail page.
- **Verification Checklist:**
  - [ ] Spec file exists.
  - [ ] `npx playwright test` passes.
- **Deliverables:** E2E spec file.
- **Rollback Strategy:** Delete E2E spec.

#### Task 4.7 — Account Pages — Stay Legacy (Link Integration)
- **Entry Criteria:** Task 0.14 complete.
- **Exit Criteria:** Vue header links to `/Account/Login`, `/Account/Register`, `/Account/Manage`. Links route through YARP fallback to legacy MVC. JWT bridge keeps SPA authenticated after login.
- **Verification Checklist:**
  - [ ] Header contains login/register links.
  - [ ] Clicking login navigates to legacy login page.
  - [ ] After legacy login, redirect back to `/app/` triggers JWT fetch.
  - [ ] SPA shows authenticated state.
- **Deliverables:** Modified `TheHeader.vue`.
- **Rollback Strategy:** Revert header changes.

#### Task 5.1 — Create HomeView.vue
- **Entry Criteria:** Task 1.11 complete.
- **Exit Criteria:** View exists. Renders featured products and hero banner.
- **Verification Checklist:**
  - [ ] View file exists.
  - [ ] Homepage renders with real data.
- **Deliverables:** `HomeView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 5.2 — Create Search Functionality
- **Entry Criteria:** Task 1.8 complete.
- **Exit Criteria:** Search input in `TheHeader.vue`. Calls `GET /api/products?searchQuery=`. Results displayed in `ProductListView.vue`.
- **Verification Checklist:**
  - [ ] Search input exists in header.
  - [ ] Typing and submitting triggers search API call.
  - [ ] Results update product grid.
- **Deliverables:** Modified `TheHeader.vue`, search query param handling in `useProductStore`.
- **Rollback Strategy:** Revert header and store changes.

#### Task 5.3 — Update Vue Router
- **Entry Criteria:** Task 4.3 complete.
- **Exit Criteria:** `/` → `HomeView.vue`.
- **Verification Checklist:**
  - [ ] Route added.
  - [ ] Visiting `/` renders homepage.
- **Deliverables:** Modified `router/index.ts`.
- **Rollback Strategy:** Revert router changes.

#### Task 5.4 — Update YARP Routes
- **Entry Criteria:** Task 4.4 complete.
- **Exit Criteria:** `/` and `/home*` routed to `spaCluster`.
- **Verification Checklist:**
  - [ ] YARP config updated.
  - [ ] `http://localhost:5003/` serves Vue SPA.
- **Deliverables:** Modified YARP config.
- **Rollback Strategy:** Remove `/` route override.

#### Task 5.5 — Frontend Tests — Homepage
- **Entry Criteria:** Task 5.1 complete.
- **Exit Criteria:** Vitest test for `HomeView.vue`. `npm run test:unit` passes.
- **Verification Checklist:**
  - [ ] Test file exists.
  - [ ] `npm run test:unit` passes.
- **Deliverables:** Component test file.
- **Rollback Strategy:** Delete test file.

#### Task 5.6 — E2E Test — Homepage
- **Entry Criteria:** Task 5.1 complete.
- **Exit Criteria:** Playwright: visit `/`, verify featured products render.
- **Verification Checklist:**
  - [ ] Spec file exists.
  - [ ] `npx playwright test` passes.
- **Deliverables:** E2E spec file.
- **Rollback Strategy:** Delete E2E spec.

---

### Phase 10: Decommission Legacy (T-117 – T-125)

#### Task 6.1 — Verify Zero Legacy Traffic
- **Entry Criteria:** Task 5.4 complete.
- **Exit Criteria:** YARP logs/metrics show no requests hitting `legacyCluster` fallback (except `/Account/*`) for 7+ consecutive days.
- **Verification Checklist:**
  - [ ] Access YARP logs or application insights.
  - [ ] Confirm zero unexpected legacy traffic.
  - [ ] Document exceptions (if any) and justify.
- **Deliverables:** Traffic analysis report (markdown file in `docs/migration/`).
- **Rollback Strategy:** If traffic found, investigate source. Do not proceed to Task 6.6 until resolved.

#### Task 6.2 — Build Vue Login Page
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** `LoginView.vue` exists. Form with email/password. Calls API login endpoint (prepared in Task 6.4).
- **Verification Checklist:**
  - [ ] View file exists.
  - [ ] Form renders correctly.
  - [ ] Submit event captured.
- **Deliverables:** `LoginView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 6.3 — Build Vue Register Page
- **Entry Criteria:** Task 0.16 complete.
- **Exit Criteria:** `RegisterView.vue` exists. Form with registration fields. Calls API register endpoint.
- **Verification Checklist:**
  - [ ] View file exists.
  - [ ] Form renders correctly.
- **Deliverables:** `RegisterView.vue`.
- **Rollback Strategy:** Delete view component.

#### Task 6.4 — Add ASP.NET Core Identity to API
- **Entry Criteria:** Task 0.13 complete.
- **Exit Criteria:** `POST /api/auth/login` and `POST /api/auth/register` endpoints exist. Issue JWTs. Password hashing compatible or migration script documented.
- **Verification Checklist:**
  - [ ] Endpoints return valid JWTs.
  - [ ] Token validates with existing JWT middleware.
  - [ ] Existing users from legacy DB can authenticate (if migrating hashes) or registration flow is ready.
- **Deliverables:** `AuthController.cs` (or equivalent), identity configuration in `Program.cs`.
- **Rollback Strategy:** Remove identity endpoints. Keep JWT bridge active.

#### Task 6.5 — Update Auth Store
- **Entry Criteria:** Task 6.4 complete.
- **Exit Criteria:** `useAuthStore.ts` updated to call `POST /api/auth/login` instead of `/Account/IssueToken`.
- **Verification Checklist:**
  - [ ] Store calls new API endpoints.
  - [ ] Login flow works without legacy app.
  - [ ] Token still validates correctly.
- **Deliverables:** Modified `useAuthStore.ts`.
- **Rollback Strategy:** Revert store to Task 0.19 state.

#### Task 6.6 — Remove YARP Legacy Fallback
- **Entry Criteria:** Task 6.1 complete.
- **Exit Criteria:** YARP catch-all route `/*` removed or redirected to `spaCluster`. Vue Router handles 404s.
- **Verification Checklist:**
  - [ ] YARP config no longer has legacy fallback.
  - [ ] `http://localhost:5003/nonexistent` returns Vue SPA (Vue Router shows 404 page).
- **Deliverables:** Modified YARP configuration.
- **Rollback Strategy:** Add legacy fallback route back immediately. Zero-downtime revert.

#### Task 6.7 — Shut Down Legacy MVC
- **Entry Criteria:** Task 6.6 complete.
- **Exit Criteria:** Legacy application stopped in all environments (dev, staging, production). IIS/app pool stopped.
- **Verification Checklist:**
  - [ ] Legacy site returns connection refused or 503.
  - [ ] Monitoring confirms no legacy process running.
- **Deliverables:** Shutdown confirmation log/ticket.
- **Rollback Strategy:** Start legacy app pool. Re-enable YARP fallback.

#### Task 6.8 — (Optional) Merge Databases
- **Entry Criteria:** Task 6.7 complete.
- **Exit Criteria:** `LegacyEcommerceIdentity` tables migrated into `LegacyEcommerceDb` (or connection strings consolidated). API uses single connection string.
- **Verification Checklist:**
  - [ ] All identity tables present in business database.
  - [ ] API connects and authenticates using new single connection string.
- **Deliverables:** Migration script, updated `appsettings.json`.
- **Rollback Strategy:** Restore original two-database configuration from backup.

#### Task 6.9 — Final E2E Regression
- **Entry Criteria:** All prior tasks complete.
- **Exit Criteria:** Full Playwright suite passes: homepage → product → cart → checkout → orders → logout → login.
- **Verification Checklist:**
  - [ ] All E2E specs execute.
  - [ ] `npx playwright test` passes with 100% success rate.
  - [ ] No console errors in browser.
- **Deliverables:** Final E2E execution report.
- **Rollback Strategy:** Not applicable. This is the final task. If failures occur, fix individual tasks and re-run.

---

## 7. Cross-Cutting Orchestration Tasks (Ongoing)

These tasks run continuously and are not part of the sequential flow above. They are prerequisites for maintaining code quality across all phases.

| Task | Frequency | Entry Criteria | Exit Criteria | Verification | Deliverable |
|------|-----------|---------------|---------------|--------------|-------------|
| CC.1 Code Reviews | Every task | PR opened | 1+ approval | Review comments addressed | Approved PR |
| CC.2 Database Backup | Before 3.4, 6.7 | Database accessible | `.bak` file created | Verified restore on test instance | Backup file location documented |
| CC.3 Dependency Updates | Monthly | CI green | `npm audit` / `dotnet list package --vulnerable` clean | Zero high-severity vulnerabilities | Updated package files |
| CC.4 Documentation Updates | Per phase | Phase tasks complete | `docs/` updated with phase notes | Mentor review sign-off | Updated markdown files |
| CC.5 CI Pipeline Setup | Phase 1–4 | GitHub repo created | All workflows execute on push | Build + test badges green | `.github/workflows/*.yml` |

---

## 8. Rollback Strategy Summary

| Scenario | Action |
|----------|--------|
| **Task-level failure** | Delete the file(s) created in that task. Revert modified files to pre-task state using Git. Re-execute the task. |
| **Phase-level failure** | Identify the failing task. Roll back that task. Do not proceed to next task. Fix root cause. Re-verify. |
| **YARP route causes outage** | Remove or comment out the specific route rule in YARP config. Traffic immediately falls back to legacy catch-all. Zero downtime. |
| **Database corruption during migration** | Restore from latest `.bak` backup (Task CC.2). Re-run affected tasks from Phase 1. |
| **Auth bridge stops working** | Revert `AccountController.cs` in legacy repo. Verify legacy cookie auth still functions independently. |
| **Vue SPA breaks in production** | YARP routes can be temporarily reverted to legacy fallback. SPA can be rebuilt and redeployed independently. |
| **Complete project failure** | Legacy system remains untouched and operational. It can continue serving 100% of traffic indefinitely. |

---

## 9. Gate Review Checklist

Before transitioning from any Phase N to Phase N+1, the following must be true:

- [ ] All tasks in Phase N are marked complete.
- [ ] All verification checklists for Phase N tasks are signed off.
- [ ] All deliverables for Phase N are committed to the repository.
- [ ] All unit tests (backend + frontend) for Phase N pass.
- [ ] All E2E tests for Phase N pass.
- [ ] Mentor / Architecture Review Board has approved Phase N completion.
- [ ] Database backup is current (if entering a data-mutating phase).
- [ ] Rollback procedures for Phase N are documented and tested.
- [ ] No critical vulnerabilities in dependencies (`npm audit`, `dotnet list package --vulnerable`).
- [ ] CI pipeline is green.

---

## 10. Sign-Off Log

| Phase | Review Date | Reviewer | Status | Notes |
|-------|-------------|----------|--------|-------|
| Phase 1 | | | Pending | |
| Phase 2 | | | Pending | |
| Phase 3 | | | Pending | |
| Phase 4 | | | Pending | |
| Phase 5 | | | Pending | |
| Phase 6 | | | Pending | |
| Phase 7 | | | Pending | |
| Phase 8 | | | Pending | |
| Phase 9 | | | Pending | |
| Phase 10 | | | Pending | |

---

*This document is the single source of truth for execution order. Do not deviate from sequential task order without written approval from the mentor.*
