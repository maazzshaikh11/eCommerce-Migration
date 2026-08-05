# Master Task List

## Project: Legacy ASP.NET MVC 5 → Vue.js 3 + .NET 10 Web API Migration

### Execution Rules
- Every task solves exactly one problem.
- Every task has a unique ID (T-001, T-002, ...).
- Every task is independently verifiable.
- No task contains multiple objectives.
- Tasks execute sequentially. Task N depends on Task N-1 by default unless stated otherwise.
- This document is the single source of truth for execution order.

---

## Legend

| Field | Description |
|-------|-------------|
| **Task ID** | Unique sequential identifier |
| **Title** | Short, action-oriented name |
| **Description** | What this task does and why |
| **Objective** | The single concrete outcome |
| **Inputs** | What is required to start this task |
| **Outputs** | What this task produces |
| **Dependencies** | Task IDs that must be complete before this task starts |
| **Files Created** | New files produced by this task |
| **Files Modified** | Existing files changed by this task |
| **Acceptance Criteria** | How to verify this task is complete |
| **Complexity** | Trivial / Low / Medium / High / Critical |
| **Time** | Estimated effort in hours |

---

## Phase 1: Infrastructure & .NET Solution Foundation

**Objective:** Establish the monorepo root, .NET 10 solution, YARP reverse proxy, shared class libraries, database connectivity via ported `DatabaseHelper`, and a temporary health-check endpoint proving SQL Server reachability. No business logic yet.

**Task Range:** T-001 – T-015
**Number of Tasks:** 15

**Entry Criteria:** Approved monorepo design document. Git initialized in parent directory. .NET 10 SDK available on build machine. SQL Server accessible with `LegacyEcommerceDb` connection string.

**Exit Criteria:** `modern/` directory exists and is tracked by Git. `Ecommerce.sln` builds with five projects (API + four class libraries). YARP is registered, configured, and starts without runtime errors. Inter-project references are wired with no circular dependencies. `DatabaseHelper.cs` is ported to `Microsoft.Data.SqlClient` and compiles. `GET /api/health` returns HTTP 200 by executing `SELECT 1`.

**Deliverables:**
- `modern/` repository root with `.gitkeep`
- `Ecommerce.sln` and `Ecommerce.Api` project skeleton
- YARP package, `ReverseProxy` configuration section, and `Program.cs` wiring
- Four shared class libraries (Core, Services, Data, Migrations)
- `Directory.Packages.props` (CPM) and `Directory.Build.props`
- Inter-project `ProjectReference` graph
- Ported `DatabaseHelper.cs` with `Microsoft.Data.SqlClient`
- `HealthController.cs` temporary endpoint

### T-001: Create Repository Root Directory
- **Description:** Initialize the top-level directory for the new monorepo. This is the physical container for all migration work.
- **Objective:** A `modern/` directory exists and is tracked by Git.
- **Inputs:** Approved monorepo design document. Git initialized in parent directory.
- **Outputs:** Empty `modern/` directory with `.gitkeep`.
- **Dependencies:** None
- **Files Created:** `modern/.gitkeep`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `dir modern/` shows directory exists.
  - [ ] `git status` shows `modern/` as an untracked directory.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-002: Verify .NET 10 SDK Installation
- **Description:** Confirm the build environment has .NET 10 SDK installed and accessible from the command line.
- **Objective:** `dotnet --version` returns a 10.x version string.
- **Inputs:** Development machine.
- **Outputs:** Verified SDK version.
- **Dependencies:** T-001
- **Files Created:** None
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `dotnet --version` prints `10.x.x`.
  - [ ] `dotnet --list-sdks` shows .NET 10.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-003: Create .NET Solution File
- **Description:** Generate the solution file that will contain all .NET projects in the monorepo.
- **Objective:** A `.sln` file exists in the `modern/` directory.
- **Inputs:** T-002 verified.
- **Outputs:** `modern/Ecommerce.sln`.
- **Dependencies:** T-002
- **Files Created:** `modern/Ecommerce.sln`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File `modern/Ecommerce.sln` exists.
  - [ ] `dotnet sln list` shows empty solution.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-004: Create .NET 10 Web API Project
- **Description:** Scaffold the entry-point ASP.NET Core 10 Web API project using the `webapi` template.
- **Objective:** A runnable API project skeleton exists with `Program.cs`, `appsettings.json`, and default `WeatherForecastController`.
- **Inputs:** T-003.
- **Outputs:** `modern/Ecommerce.Api/` folder.
- **Dependencies:** T-003
- **Files Created:**
  - `modern/Ecommerce.Api/Ecommerce.Api.csproj`
  - `modern/Ecommerce.Api/Program.cs`
  - `modern/Ecommerce.Api/appsettings.json`
  - `modern/Ecommerce.Api/appsettings.Development.json`
  - `modern/Ecommerce.Api/Controllers/WeatherForecastController.cs`
  - `modern/Ecommerce.Api/Properties/launchSettings.json`
- **Files Modified:** `modern/Ecommerce.sln`
- **Acceptance Criteria:**
  - [ ] `dotnet sln list` shows `Ecommerce.Api`.
  - [ ] `cd modern/Ecommerce.Api && dotnet build` succeeds with zero errors.
- **Complexity:** Trivial
- **Time:** 0.25h

### T-005: Remove Default WeatherForecastController
- **Description:** Delete the scaffolded sample controller to prepare a clean project structure.
- **Objective:** No sample controllers remain in the project.
- **Inputs:** T-004.
- **Outputs:** Clean project without sample code.
- **Dependencies:** T-004
- **Files Created:** None
- **Files Modified:** None (file deleted)
- **Acceptance Criteria:**
  - [ ] `WeatherForecastController.cs` does not exist.
  - [ ] `dotnet build` still succeeds.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-006: Add YARP NuGet Package to API Project
- **Description:** Install the YARP reverse proxy package into the API project.
- **Objective:** YARP package referenced and restorable.
- **Inputs:** T-005.
- **Outputs:** Updated `.csproj` with YARP reference.
- **Dependencies:** T-005
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Ecommerce.Api.csproj`
- **Acceptance Criteria:**
  - [ ] `Ecommerce.Api.csproj` contains `Yarp.ReverseProxy` package reference.
  - [ ] `dotnet restore` completes without package errors.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-007: Add ReverseProxy Configuration Section to appsettings.json
- **Description:** Define the `ReverseProxy` JSON structure with three clusters (api, spa, legacy) and three route rules.
- **Objective:** YARP configuration is present and syntactically valid.
- **Inputs:** T-006.
- **Outputs:** `appsettings.json` with `ReverseProxy` section.
- **Dependencies:** T-006
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] `appsettings.json` parses as valid JSON.
  - [ ] Contains `ReverseProxy` → `Routes` (api, app, legacy).
  - [ ] Contains `ReverseProxy` → `Clusters` (apiCluster, spaCluster, legacyCluster).
  - [ ] `apiCluster` destination is `http://localhost:5001`.
  - [ ] `spaCluster` destination is `http://localhost:5002`.
  - [ ] `legacyCluster` destination is `http://localhost:5000`.
- **Complexity:** Low
- **Time:** 0.5h

### T-008: Register YARP Services and Middleware in Program.cs
- **Description:** Wire YARP into the ASP.NET Core request pipeline by calling `AddReverseProxy` and `MapReverseProxy`.
- **Objective:** YARP is active in the application's service collection and middleware pipeline.
- **Inputs:** T-007.
- **Outputs:** Modified `Program.cs`.
- **Dependencies:** T-007
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Program.cs`
- **Acceptance Criteria:**
  - [ ] `builder.Services.AddReverseProxy().LoadFromConfig(...)` is present.
  - [ ] `app.MapReverseProxy()` is present.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Low
- **Time:** 0.25h

### T-009: Verify YARP Starts Without Runtime Errors
- **Description:** Run the API project and confirm the process starts and YARP loads its configuration without exceptions.
- **Objective:** API process is running with YARP initialized.
- **Inputs:** T-008.
- **Outputs:** Running API instance.
- **Dependencies:** T-008
- **Files Created:** None
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `dotnet run` starts without unhandled exceptions.
  - [ ] Console output shows YARP configuration loaded successfully.
  - [ ] Process listens on expected port.
- **Complexity:** Low
- **Time:** 0.25h

### T-010: Create Shared Class Library Projects
- **Description:** Generate four class library projects (Core, Services, Data, Migrations) and add them to the solution.
- **Objective:** All supporting projects exist and are part of the solution.
- **Inputs:** T-003.
- **Outputs:** Four new `.csproj` files in the solution.
- **Dependencies:** T-009
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Ecommerce.Api.Core.csproj`
  - `modern/Ecommerce.Api.Services/Ecommerce.Api.Services.csproj`
  - `modern/Ecommerce.Api.Data/Ecommerce.Api.Data.csproj`
  - `modern/Ecommerce.Api.Migrations/Ecommerce.Api.Migrations.csproj`
- **Files Modified:** `modern/Ecommerce.sln`
- **Acceptance Criteria:**
  - [ ] `dotnet sln list` shows all four projects.
  - [ ] `dotnet build` from `modern/` succeeds.
- **Complexity:** Trivial
- **Time:** 0.25h

### T-011: Configure Central Package Management (CPM)
- **Description:** Create `Directory.Packages.props` at the solution root to centralize NuGet package versions.
- **Objective:** All package versions managed in one file.
- **Inputs:** T-010.
- **Outputs:** `modern/Directory.Packages.props`.
- **Dependencies:** T-010
- **Files Created:** `modern/Directory.Packages.props`
- **Files Modified:**
  - `modern/Ecommerce.Api/Ecommerce.Api.csproj`
  - `modern/Ecommerce.Api.Core/Ecommerce.Api.Core.csproj`
  - `modern/Ecommerce.Api.Services/Ecommerce.Api.Services.csproj`
  - `modern/Ecommerce.Api.Data/Ecommerce.Api.Data.csproj`
  - `modern/Ecommerce.Api.Migrations/Ecommerce.Api.Migrations.csproj`
- **Acceptance Criteria:**
  - [ ] `Directory.Packages.props` exists.
  - [ ] All `.csproj` files use `Version="$(PackageVersion)"` or CPM-compatible syntax.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Low
- **Time:** 0.5h

### T-012: Create Directory.Build.props
- **Description:** Define shared MSBuild properties (language version, nullable, implicit usings) for all projects.
- **Objective:** Consistent compiler settings across all .NET projects.
- **Inputs:** T-011.
- **Outputs:** `modern/Directory.Build.props`.
- **Dependencies:** T-011
- **Files Created:** `modern/Directory.Build.props`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File exists with `<Nullable>enable</Nullable>`, `<LangVersion>latest</LangVersion>`, `<ImplicitUsings>enable</ImplicitUsings>`.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Trivial
- **Time:** 0.25h

### T-013: Wire Inter-Project References
- **Description:** Configure `ProjectReference` nodes so that API depends on Services and Core; Services depends on Core and Data; Data depends on Core.
- **Objective:** Dependency graph is correctly established and builds.
- **Inputs:** T-012.
- **Outputs:** Updated `.csproj` files with references.
- **Dependencies:** T-012
- **Files Created:** None
- **Files Modified:**
  - `modern/Ecommerce.Api/Ecommerce.Api.csproj`
  - `modern/Ecommerce.Api.Services/Ecommerce.Api.Services.csproj`
  - `modern/Ecommerce.Api.Data/Ecommerce.Api.Data.csproj`
- **Acceptance Criteria:**
  - [ ] `Ecommerce.Api` references `Ecommerce.Api.Services` and `Ecommerce.Api.Core`.
  - [ ] `Ecommerce.Api.Services` references `Ecommerce.Api.Core` and `Ecommerce.Api.Data`.
  - [ ] `Ecommerce.Api.Data` references `Ecommerce.Api.Core`.
  - [ ] `dotnet build` succeeds.
  - [ ] No circular references.
- **Complexity:** Low
- **Time:** 0.25h

### T-014: Port DatabaseHelper from Legacy
- **Description:** Copy the legacy `DatabaseHelper.cs` into `Ecommerce.Api.Data` and replace `System.Data.SqlClient` with `Microsoft.Data.SqlClient`.
- **Objective:** The ADO.NET helper compiles in the new project using the modern SQL client.
- **Inputs:** T-013. Legacy repository read access.
- **Outputs:** Ported `DatabaseHelper.cs`.
- **Dependencies:** T-013
- **Files Created:** `modern/Ecommerce.Api.Data/DatabaseHelper.cs`
- **Files Modified:** `modern/Ecommerce.Api.Data/Ecommerce.Api.Data.csproj` (add `Microsoft.Data.SqlClient` package)
- **Acceptance Criteria:**
  - [ ] File exists in `Ecommerce.Api.Data/`.
  - [ ] No `using System.Data.SqlClient;` remains.
  - [ ] `Microsoft.Data.SqlClient` package referenced.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Low
- **Time:** 0.5h

### T-015: Create Temporary Health Check Controller
- **Description:** Add a minimal API controller that executes `SELECT 1` via `DatabaseHelper` to prove database connectivity.
- **Objective:** Database connectivity verified at runtime.
- **Inputs:** T-014.
- **Outputs:** Running health-check endpoint.
- **Dependencies:** T-014
- **Files Created:** `modern/Ecommerce.Api/Controllers/HealthController.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `GET /api/health` returns HTTP 200.
  - [ ] API logs show successful SQL execution.
- **Complexity:** Low
- **Time:** 0.25h

---

## Phase 2: JWT Auth Bridge & Vue SPA Shell

**Objective:** Implement JWT authentication (token service, API middleware, signing key), add the legacy `IssueToken` bridge endpoint, scaffold the Vue 3 + Vite SPA, install Pinia and Vue Router, build the fetch wrapper and auth store, configure the router skeleton, and verify the full Legacy → YARP → API/SPA proxy chain end-to-end.

**Task Range:** T-016 – T-028
**Number of Tasks:** 13

**Entry Criteria:** Phase 1 complete. `dotnet build` succeeds at solution level. `GET /api/health` returns 200. Node.js 20+ and npm available on the build machine. Legacy repository is read-accessible for the JWT bridge endpoint modification.

**Exit Criteria:** `JwtTokenService` generates and validates HMAC-SHA256 JWTs. API rejects unauthenticated requests to `[Authorize]` endpoints with 401. Legacy `AccountController.IssueToken()` returns a JWT to cookie-authenticated users. Vue SPA scaffolded with Pinia, Vue Router, fetch wrapper attaching `Bearer` tokens, and `useAuthStore`. All four services (Legacy 5000, API 5001, SPA 5002, YARP 5003) run together; authenticated `GET /api/health` succeeds through the proxy.

**Deliverables:**
- `JwtTokenService` + `IJwtTokenService` in Core/Security
- JWT Bearer authentication wired in `Program.cs`
- `JwtSigningKey` in `appsettings.json` / `appsettings.Development.json`
- Legacy `AccountController.IssueToken()` bridge endpoint
- Vue 3 + Vite + TypeScript SPA scaffold (`Ecommerce.Spa`)
- `pinia` and `vue-router` dependencies
- `.env.development` with `VITE_API_BASE_URL`
- `client.ts` fetch wrapper with JWT attachment
- `useAuthStore.ts` with `localStorage` persistence
- `router/index.ts` skeleton
- Verified end-to-end proxy chain

### T-016: Add JWT NuGet Package to API Project
- **Description:** Install `Microsoft.AspNetCore.Authentication.JwtBearer` into the API project.
- **Objective:** JWT authentication package is available.
- **Inputs:** T-015.
- **Outputs:** Updated `.csproj`.
- **Dependencies:** T-015
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Ecommerce.Api.csproj`
- **Acceptance Criteria:**
  - [ ] Package reference present.
  - [ ] `dotnet restore` succeeds.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-017: Implement JWT Token Service in Core Project
- **Description:** Create `JwtTokenService` in `Ecommerce.Api.Core` that generates and validates HMAC-SHA256 JWTs using a symmetric key.
- **Objective:** A reusable service can create JWTs with `sub`, `customer_id`, `email`, and `exp` claims.
- **Inputs:** T-016.
- **Outputs:** `JwtTokenService.cs` and its interface.
- **Dependencies:** T-016
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Security/IJwtTokenService.cs`
  - `modern/Ecommerce.Api.Core/Security/JwtTokenService.cs`
- **Files Modified:** `modern/Ecommerce.Api.Core/Ecommerce.Api.Core.csproj` (add `Microsoft.IdentityModel.Tokens` and `System.IdentityModel.Tokens.Jwt`)
- **Acceptance Criteria:**
  - [ ] `GenerateToken(userId, customerId, email)` returns non-null JWT string.
  - [ ] A unit test validates token payload contains expected claims.
  - [ ] `dotnet test` for the service test passes.
- **Complexity:** Medium
- **Time:** 1.5h

### T-018: Configure JWT Authentication in Program.cs
- **Description:** Register JWT Bearer authentication in the API's service collection and middleware pipeline.
- **Objective:** API validates JWTs on protected endpoints.
- **Inputs:** T-017.
- **Outputs:** Modified `Program.cs` with JWT middleware.
- **Dependencies:** T-017
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Program.cs`
- **Acceptance Criteria:**
  - [ ] `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` present.
  - [ ] `app.UseAuthentication()` and `app.UseAuthorization()` present in correct order.
  - [ ] A test `[Authorize]` endpoint returns 401 without token.
  - [ ] Same endpoint returns 200 with valid token.
- **Complexity:** Medium
- **Time:** 1h

### T-019: Add JWT Signing Key to appsettings.json
- **Description:** Add a 32-byte base64-encoded symmetric signing key to `appsettings.json` and `appsettings.Development.json`.
- **Objective:** JWT service has access to a configured signing key.
- **Inputs:** T-018.
- **Outputs:** Updated configuration files.
- **Dependencies:** T-018
- **Files Created:** None
- **Files Modified:**
  - `modern/Ecommerce.Api/appsettings.json`
  - `modern/Ecommerce.Api/appsettings.Development.json`
- **Acceptance Criteria:**
  - [ ] `JwtSigningKey` property exists in both files.
  - [ ] Key is at least 32 bytes.
  - [ ] API reads key without exception.
- **Complexity:** Low
- **Time:** 0.25h

### T-020: Add JWT Bridge Endpoint to Legacy MVC
- **Description:** Modify legacy `AccountController` to add `IssueToken()` action that reads cookie auth user and returns a JWT JSON response.
- **Objective:** Legacy app can issue JWTs to authenticated users for use by the new API.
- **Inputs:** T-019. Legacy repository write access.
- **Outputs:** Modified `AccountController.cs` in legacy repo.
- **Dependencies:** T-019
- **Files Created:** None
- **Files Modified:** `Ecommerce.Web/Controllers/AccountController.cs` (legacy repo)
- **Acceptance Criteria:**
  - [ ] Logged-in user visiting `/Account/IssueToken` receives JSON `{ "token": "..." }`.
  - [ ] Anonymous user is redirected to `/Account/Login`.
  - [ ] Token validates successfully using the same key as T-019.
- **Complexity:** Medium
- **Time:** 1.5h

### T-021: Verify Node.js Installation for Vue SPA
- **Description:** Confirm Node.js 20+ and npm are installed and accessible.
- **Objective:** Build environment ready for Vite scaffolding.
- **Inputs:** T-001.
- **Outputs:** Verified Node.js version.
- **Dependencies:** T-020
- **Files Created:** None
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `node --version` prints v20.x or higher.
  - [ ] `npm --version` prints 10.x or higher.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-022: Scaffold Vue SPA with Vite
- **Description:** Use `npm create vue@latest` or `npm init vite` to generate a Vue 3 + TypeScript project in `modern/Ecommerce.Spa`.
- **Objective:** A runnable Vue 3 SPA skeleton exists.
- **Inputs:** T-021.
- **Outputs:** Complete Vite scaffold folder.
- **Dependencies:** T-021
- **Files Created:**
  - `modern/Ecommerce.Spa/package.json`
  - `modern/Ecommerce.Spa/vite.config.ts`
  - `modern/Ecommerce.Spa/tsconfig.json`
  - `modern/Ecommerce.Spa/index.html`
  - `modern/Ecommerce.Spa/src/main.ts`
  - `modern/Ecommerce.Spa/src/App.vue`
  - `modern/Ecommerce.Spa/src/style.css`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `package.json` exists with `vue` and `vite` dependencies.
  - [ ] `npm install` completes with zero critical vulnerabilities.
  - [ ] `npm run dev` starts server.
  - [ ] Browser displays default Vite + Vue welcome page.
- **Complexity:** Trivial
- **Time:** 0.5h

### T-023: Install Pinia and Vue Router
- **Description:** Add `pinia` and `vue-router` as dependencies in the SPA project.
- **Objective:** State management and routing libraries are available.
- **Inputs:** T-022.
- **Outputs:** Updated `package.json` and `package-lock.json`.
- **Dependencies:** T-022
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/package.json`
- **Acceptance Criteria:**
  - [ ] `pinia` and `vue-router` listed in `dependencies`.
  - [ ] `npm install` succeeds.
  - [ ] `npm run dev` starts without errors.
- **Complexity:** Trivial
- **Time:** 0.25h

### T-024: Configure API Base URL in Environment File
- **Description:** Create `.env.development` with the base URL for API calls during local development.
- **Objective:** SPA knows where to send API requests.
- **Inputs:** T-023.
- **Outputs:** `.env.development` file.
- **Dependencies:** T-023
- **Files Created:** `modern/Ecommerce.Spa/.env.development`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File contains `VITE_API_BASE_URL=http://localhost:5001/api`.
  - [ ] `import.meta.env.VITE_API_BASE_URL` resolves correctly in a test component.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-025: Implement Fetch Wrapper with JWT Attachment
- **Description:** Create `client.ts` that exports `get`, `post`, `put`, `del` functions using native `fetch` and attaching the Bearer token from an auth store.
- **Objective:** All API calls from the SPA include authentication.
- **Inputs:** T-024.
- **Outputs:** API client module.
- **Dependencies:** T-024
- **Files Created:** `modern/Ecommerce.Spa/src/api/client.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File exists and exports four functions.
  - [ ] Functions accept a token parameter.
  - [ ] Request includes `Authorization: Bearer <token>` header when token provided.
  - [ ] Response body returned as typed generic `T`.
- **Complexity:** Low
- **Time:** 0.5h

### T-026: Implement Pinia Auth Store
- **Description:** Create `useAuthStore` that manages JWT token lifecycle: fetch from legacy bridge, persist to `localStorage`, provide `isAuthenticated` computed property.
- **Objective:** SPA can obtain and manage authentication state.
- **Inputs:** T-025.
- **Outputs:** Auth store module.
- **Dependencies:** T-025
- **Files Created:** `modern/Ecommerce.Spa/src/stores/useAuthStore.ts`
- **Files Modified:** `modern/Ecommerce.Spa/src/main.ts` (register Pinia plugin)
- **Acceptance Criteria:**
  - [ ] Store initializes with token from `localStorage`.
  - [ ] `fetchToken()` calls `/Account/IssueToken` via proxy.
  - [ ] `isAuthenticated` returns true after successful fetch.
  - [ ] `logout()` clears token and `localStorage`.
- **Complexity:** Medium
- **Time:** 1h

### T-027: Configure Vue Router Skeleton
- **Description:** Set up Vue Router with a placeholder route and history mode.
- **Objective:** Client-side routing is active.
- **Inputs:** T-023.
- **Outputs:** Router configuration module.
- **Dependencies:** T-026
- **Files Created:** `modern/Ecommerce.Spa/src/router/index.ts`
- **Files Modified:** `modern/Ecommerce.Spa/src/main.ts` (register router)
- **Acceptance Criteria:**
  - [ ] Router file exists with `createRouter` and `createWebHistory`.
  - [ ] At least one placeholder route defined.
  - [ ] Navigation between routes works in browser.
- **Complexity:** Low
- **Time:** 0.5h

### T-028: Verify Full Proxy Chain End-to-End
- **Description:** Start all four services (Legacy 5000, API 5001, SPA 5002, YARP 5003) and verify traffic flows correctly through the chain.
- **Objective:** The complete infrastructure is operational.
- **Inputs:** T-027.
- **Outputs:** Verified running system.
- **Dependencies:** T-027
- **Files Created:** None
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Legacy MVC serves pages on port 5000.
  - [ ] API responds on port 5001.
  - [ ] SPA dev server runs on port 5002.
  - [ ] YARP runs on port 5003.
  - [ ] `http://localhost:5003/app/` loads Vue app.
  - [ ] Auth store successfully fetches JWT from legacy via YARP.
  - [ ] Authenticated API call (`GET /api/health` with Bearer) returns 200.
- **Complexity:** Medium
- **Time:** 1h

---

## Phase 3: Product Catalog Backend

**Objective:** Port the product, category, product image, and product variant domain models, DTOs, repository interfaces and implementations, service interfaces and implementations into the new .NET 10 API. Register services in DI. Implement `ProductsController` and `CategoriesController` with GET endpoints. The API can serve the full read-only product catalog as JSON.

**Task Range:** T-029 – T-045
**Number of Tasks:** 17

**Entry Criteria:** Phase 2 complete. Full proxy chain verified (T-028). `Ecommerce.Api.Core`, `Ecommerce.Api.Data`, and `Ecommerce.Api.Services` projects exist and build. `DatabaseHelper` is functional. JWT authentication is active on the API.

**Exit Criteria:** `Product`, `Category`, `ProductImage`, `ProductVariant` models compile in Core/Models. DTOs (`ProductListResponseDto`, `ProductDetailDto`, `CategoryDto`, `PagedResult`) exist. Repository interfaces and ADO.NET implementations are ported with `Microsoft.Data.SqlClient`. Service interfaces and implementations are ported with DTO return types. Services and repositories registered in DI. `GET /api/products` returns `PagedResult<ProductListResponseDto>`. `GET /api/products/{id}` returns `ProductDetailDto`. `GET /api/categories` returns `CategoryDto[]`.

**Deliverables:**
- 4 domain model files in `Core/Models`
- 4 DTO files in `Core/DTOs`
- 2 repository interface files in `Core/Interfaces`
- 2 repository implementation files in `Data/Repositories`
- 2 service interface files in `Services/Interfaces`
- 2 service implementation files in `Services/Implementations`
- DI registrations in `Program.cs`
- `ProductsController.cs` with GET endpoints
- `CategoriesController.cs` with GET endpoint

### T-029: Port Product Domain Model
- **Description:** Copy `Product.cs` from legacy `Ecommerce.Core` to new `Ecommerce.Api.Core/Models/`.
- **Objective:** Product entity available in new codebase.
- **Inputs:** T-028. Legacy repository.
- **Outputs:** `Product.cs` in new project.
- **Dependencies:** T-028
- **Files Created:** `modern/Ecommerce.Api.Core/Models/Product.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] No EF-specific attributes or references.
- **Complexity:** Low
- **Time:** 0.25h

### T-030: Port Category Domain Model
- **Description:** Copy `Category.cs` from legacy to new project.
- **Objective:** Category entity available in new codebase.
- **Inputs:** T-029. Legacy repository.
- **Outputs:** `Category.cs` in new project.
- **Dependencies:** T-029
- **Files Created:** `modern/Ecommerce.Api.Core/Models/Category.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] No EF references.
- **Complexity:** Low
- **Time:** 0.25h

### T-031: Port ProductImage and ProductVariant Models
- **Description:** Copy `ProductImage.cs` and `ProductVariant.cs` from legacy to new project.
- **Objective:** Supporting entities available.
- **Inputs:** T-030. Legacy repository.
- **Outputs:** Two model files in new project.
- **Dependencies:** T-030
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Models/ProductImage.cs`
  - `modern/Ecommerce.Api.Core/Models/ProductVariant.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Both files compile.
- **Complexity:** Low
- **Time:** 0.25h

### T-032: Create ProductListResponseDto
- **Description:** Define a DTO record for the product listing endpoint including paged items, categories, and filter state.
- **Objective:** API contract for product listing defined.
- **Inputs:** T-031.
- **Outputs:** DTO file.
- **Dependencies:** T-031
- **Files Created:** `modern/Ecommerce.Api.Core/DTOs/ProductListResponseDto.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Record contains `Products`, `Categories`, `SelectedCategoryId`, `SortOrder`, `SearchQuery`, `PageNumber`, `TotalPages`.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Low
- **Time:** 0.5h

### T-033: Create ProductDetailDto
- **Description:** Define a DTO record for the product detail endpoint including variants, images, and related products.
- **Objective:** API contract for product detail defined.
- **Inputs:** T-032.
- **Outputs:** DTO file.
- **Dependencies:** T-032
- **Files Created:** `modern/Ecommerce.Api.Core/DTOs/ProductDetailDto.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Record contains all product detail fields plus nested collections.
  - [ ] No circular references.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Low
- **Time:** 0.5h

### T-034: Create CategoryDto and PagedResult
- **Description:** Define reusable DTOs for categories and generic pagination.
- **Objective:** Shared DTO structures available.
- **Inputs:** T-033.
- **Outputs:** DTO files.
- **Dependencies:** T-033
- **Files Created:**
  - `modern/Ecommerce.Api.Core/DTOs/CategoryDto.cs`
  - `modern/Ecommerce.Api.Core/DTOs/PagedResult.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `CategoryDto` defined.
  - [ ] `PagedResult<T>` generic defined.
  - [ ] `dotnet build` succeeds.
- **Complexity:** Low
- **Time:** 0.25h

### T-035: Port IProductRepository Interface
- **Description:** Copy `IProductRepository` from legacy `Ecommerce.Core/Interfaces/` to new project.
- **Objective:** Product repository contract available.
- **Inputs:** T-034. Legacy repository.
- **Outputs:** Interface file.
- **Dependencies:** T-034
- **Files Created:** `modern/Ecommerce.Api.Core/Interfaces/IProductRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface compiles.
  - [ ] Methods use domain models (not ViewModels).
- **Complexity:** Low
- **Time:** 0.25h

### T-036: Port ICategoryRepository Interface
- **Description:** Copy `ICategoryRepository` from legacy to new project.
- **Objective:** Category repository contract available.
- **Inputs:** T-035. Legacy repository.
- **Outputs:** Interface file.
- **Dependencies:** T-035
- **Files Created:** `modern/Ecommerce.Api.Core/Interfaces/ICategoryRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-037: Port ProductRepository Implementation
- **Description:** Copy `ProductRepository` from legacy `Ecommerce.Data/Repositories/` to new project, replacing `System.Data.SqlClient` with `Microsoft.Data.SqlClient`.
- **Objective:** Product data access works in new codebase.
- **Inputs:** T-035. Legacy repository.
- **Outputs:** Repository implementation file.
- **Dependencies:** T-036
- **Files Created:** `modern/Ecommerce.Api.Data/Repositories/ProductRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] Uses `Microsoft.Data.SqlClient`.
- **Complexity:** Low
- **Time:** 0.5h

### T-038: Port CategoryRepository Implementation
- **Description:** Copy `CategoryRepository` from legacy to new project with SQL client update.
- **Objective:** Category data access works in new codebase.
- **Inputs:** T-036. Legacy repository.
- **Outputs:** Repository implementation file.
- **Dependencies:** T-037
- **Files Created:** `modern/Ecommerce.Api.Data/Repositories/CategoryRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] Uses `Microsoft.Data.SqlClient`.
- **Complexity:** Low
- **Time:** 0.5h

### T-039: Port IProductService Interface
- **Description:** Copy `IProductService` from legacy `Ecommerce.Core/Interfaces/` to new project.
- **Objective:** Product service contract available.
- **Inputs:** T-038. Legacy repository.
- **Outputs:** Interface file.
- **Dependencies:** T-038
- **Files Created:** `modern/Ecommerce.Api.Services/Interfaces/IProductService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-040: Port ICategoryService Interface
- **Description:** Copy `ICategoryService` from legacy to new project.
- **Objective:** Category service contract available.
- **Inputs:** T-039. Legacy repository.
- **Outputs:** Interface file.
- **Dependencies:** T-039
- **Files Created:** `modern/Ecommerce.Api.Services/Interfaces/ICategoryService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-041: Port ProductService Implementation
- **Description:** Copy `ProductService` from legacy `Ecommerce.Services/` to new project. Change namespace. Update return types to use DTOs.
- **Objective:** Product business logic available in new codebase.
- **Inputs:** T-040. Legacy repository.
- **Outputs:** Service implementation file.
- **Dependencies:** T-040
- **Files Created:** `modern/Ecommerce.Api.Services/Implementations/ProductService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] No Razor/ViewModel references.
  - [ ] Returns domain models/DTOs.
- **Complexity:** Medium
- **Time:** 1h

### T-042: Port CategoryService Implementation
- **Description:** Copy `CategoryService` from legacy to new project with namespace and return type updates.
- **Objective:** Category business logic available.
- **Inputs:** T-041. Legacy repository.
- **Outputs:** Service implementation file.
- **Dependencies:** T-041
- **Files Created:** `modern/Ecommerce.Api.Services/Implementations/CategoryService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] No Razor/ViewModel references.
- **Complexity:** Low
- **Time:** 0.5h

### T-043: Register Product and Category Services in DI
- **Description:** Add scoped DI registrations for repositories and services in `Program.cs`.
- **Objective:** DI container resolves product and category dependencies.
- **Inputs:** T-042.
- **Outputs:** Modified `Program.cs`.
- **Dependencies:** T-042
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Program.cs`
- **Acceptance Criteria:**
  - [ ] `AddScoped<IProductRepository, ProductRepository>()` present.
  - [ ] `AddScoped<IProductService, ProductService>()` present.
  - [ ] `AddScoped<ICategoryRepository, CategoryRepository>()` present.
  - [ ] `AddScoped<ICategoryService, CategoryService>()` present.
  - [ ] API starts without DI exceptions.
- **Complexity:** Low
- **Time:** 0.25h

### T-044: Implement ProductsController with GET Endpoints
- **Description:** Create `ProductsController` with `GET /api/products` (pagination, filter, sort) and `GET /api/products/{id}` (detail).
- **Objective:** Product catalog API endpoints return JSON DTOs.
- **Inputs:** T-043.
- **Outputs:** Controller file.
- **Dependencies:** T-043
- **Files Created:** `modern/Ecommerce.Api/Controllers/ProductsController.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `GET /api/products` returns `PagedResult<ProductDto>`.
  - [ ] `GET /api/products/{id}` returns `ProductDetailDto`.
  - [ ] Response shape matches DTO design.
- **Complexity:** Medium
- **Time:** 1.5h

### T-045: Implement CategoriesController with GET Endpoint
- **Description:** Create `CategoriesController` with `GET /api/categories` returning all categories.
- **Objective:** Category tree API endpoint available.
- **Inputs:** T-044.
- **Outputs:** Controller file.
- **Dependencies:** T-044
- **Files Created:** `modern/Ecommerce.Api/Controllers/CategoriesController.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `GET /api/categories` returns `CategoryDto[]`.
- **Complexity:** Low
- **Time:** 0.5h

---

## Phase 4: Product Catalog Frontend & Tests

**Objective:** Add YARP routes for product pages, build the Vue product UI (reusable components, Pinia product store, list and detail views), wire Vue Router for product routes, and complete backend unit tests, frontend unit tests, and an E2E Playwright spec for the product catalog flow. Users can browse the catalog entirely through Vue + API.

**Task Range:** T-046 – T-059
**Number of Tasks:** 14

**Entry Criteria:** Phase 3 complete. `GET /api/products` and `GET /api/products/{id}` return correct DTOs. `GET /api/categories` returns category array. Vue SPA scaffold, Pinia, Vue Router, fetch wrapper, and auth store are in place from Phase 2.

**Exit Criteria:** YARP routes `/products*` and `/product/*` to `spaCluster`. `ProductCard`, `CategoryFilter`, `PaginationControls`, `LoadingSpinner`, `ErrorAlert` components render. `useProductStore` triggers refetch on filter change. `ProductListView` and `ProductDetailView` render real API data. Vue Router has `/products` and `/products/:id` routes. Backend `ProductService` and `ProductsController` unit tests pass. Frontend `ProductCard` and product store unit tests pass. Playwright product catalog E2E spec passes.

**Deliverables:**
- YARP product route configuration
- 5 reusable Vue components
- `useProductStore.ts`
- `ProductListView.vue` and `ProductDetailView.vue`
- Vue Router product routes
- `ProductServiceTests.cs`, `ProductsControllerTests.cs`
- `ProductCard` and product store Vitest specs
- Product catalog Playwright E2E spec

### T-046: Add YARP Route for Product Pages
- **Description:** Configure YARP to route `/products*` and `/product/*` to the SPA cluster.
- **Objective:** Product browsing traffic intercepted by YARP and sent to Vue SPA.
- **Inputs:** T-045.
- **Outputs:** Updated YARP configuration.
- **Dependencies:** T-045
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json` (or `Program.cs`)
- **Acceptance Criteria:**
  - [ ] `/products*` route present in YARP config.
  - [ ] `http://localhost:5003/products` returns Vue SPA HTML (not legacy MVC).
- **Complexity:** Low
- **Time:** 0.25h

### T-047: Create ProductCard Vue Component
- **Description:** Build a reusable card component that displays product image, name, price, and category.
- **Objective:** Reusable product display component exists.
- **Inputs:** T-046.
- **Outputs:** Vue component file.
- **Dependencies:** T-046
- **Files Created:** `modern/Ecommerce.Spa/src/components/ProductCard.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Component renders correctly with sample data in Vitest.
  - [ ] Emits `add-to-cart` event on button click.
- **Complexity:** Low
- **Time:** 0.5h

### T-048: Create CategoryFilter Vue Component
- **Description:** Build a sidebar component that renders category tree and emits selected category ID.
- **Objective:** Category filtering UI component exists.
- **Inputs:** T-047.
- **Outputs:** Vue component file.
- **Dependencies:** T-047
- **Files Created:** `modern/Ecommerce.Spa/src/components/CategoryFilter.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Component renders category tree.
  - [ ] Clicking category emits `select` event with category ID.
- **Complexity:** Low
- **Time:** 0.5h

### T-049: Create PaginationControls Vue Component
- **Description:** Build a pagination bar component that displays page numbers and emits page change events.
- **Objective:** Pagination UI component exists.
- **Inputs:** T-048.
- **Outputs:** Vue component file.
- **Dependencies:** T-048
- **Files Created:** `modern/Ecommerce.Spa/src/components/PaginationControls.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Component renders correct page numbers for given `page` and `totalPages`.
  - [ ] Clicking "Next" emits `change-page` with incremented page.
- **Complexity:** Low
- **Time:** 0.5h

### T-050: Create LoadingSpinner and ErrorAlert Components
- **Description:** Build two utility components for async state indication and error display.
- **Objective:** Common UI primitives available.
- **Inputs:** T-049.
- **Outputs:** Two Vue component files.
- **Dependencies:** T-049
- **Files Created:**
  - `modern/Ecommerce.Spa/src/components/LoadingSpinner.vue`
  - `modern/Ecommerce.Spa/src/components/ErrorAlert.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Both components render in isolation.
- **Complexity:** Low
- **Time:** 0.5h

### T-051: Create Pinia Product Store
- **Description:** Implement `useProductStore` with state for products, filters, pagination, and actions to fetch from API.
- **Objective:** Product state management in SPA.
- **Inputs:** T-050.
- **Outputs:** Pinia store module.
- **Dependencies:** T-050
- **Files Created:** `modern/Ecommerce.Spa/src/stores/useProductStore.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Store contains `products`, `filters`, `pagination` state.
  - [ ] `fetchProducts()` calls `GET /api/products` with query params.
  - [ ] Changing filters triggers refetch.
- **Complexity:** Medium
- **Time:** 1h

### T-052: Create ProductListView Vue Page
- **Description:** Build the product listing page that composes CategoryFilter, ProductCard grid, and PaginationControls.
- **Objective:** Product browsing page fully functional in Vue.
- **Inputs:** T-051.
- **Outputs:** Vue view component.
- **Dependencies:** T-051
- **Files Created:** `modern/Ecommerce.Spa/src/views/ProductListView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Page fetches and displays real products from API.
  - [ ] URL query params update when filters change.
  - [ ] Pagination works.
- **Complexity:** Medium
- **Time:** 1.5h

### T-053: Create ProductDetailView Vue Page
- **Description:** Build the product detail page that displays gallery, variants, description, and related products.
- **Objective:** Product detail page fully functional in Vue.
- **Inputs:** T-052.
- **Outputs:** Vue view component.
- **Dependencies:** T-052
- **Files Created:** `modern/Ecommerce.Spa/src/views/ProductDetailView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Page fetches product by ID from API.
  - [ ] Displays gallery, variants, description.
  - [ ] Visiting `/products/123` renders product 123.
- **Complexity:** Medium
- **Time:** 1.5h

### T-054: Configure Vue Router for Product Pages
- **Description:** Add routes `/products` → `ProductListView` and `/products/:id` → `ProductDetailView`.
- **Objective:** Client-side routing for product catalog active.
- **Inputs:** T-053.
- **Outputs:** Updated router configuration.
- **Dependencies:** T-053
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/router/index.ts`
- **Acceptance Criteria:**
  - [ ] `/products` renders `ProductListView`.
  - [ ] `/products/:id` renders `ProductDetailView`.
  - [ ] Browser back/forward preserves filter state.
- **Complexity:** Low
- **Time:** 0.5h

### T-055: Write Backend Unit Tests for ProductService
- **Description:** Create xUnit tests for `ProductService` mocking `IUnitOfWork` and `IProductRepository`.
- **Objective:** Product business logic verified.
- **Inputs:** T-043.
- **Outputs:** Test file.
- **Dependencies:** T-054
- **Files Created:** `modern/Ecommerce.Api.Services.Tests/ProductServiceTests.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Tests cover pagination logic.
  - [ ] Tests cover category filtering.
  - [ ] `dotnet test` passes.
- **Complexity:** Medium
- **Time:** 1.5h

### T-056: Write Backend Unit Tests for ProductsController
- **Description:** Create xUnit tests for `ProductsController` mocking `IProductService`.
- **Objective:** Controller behavior verified.
- **Inputs:** T-044.
- **Outputs:** Test file.
- **Dependencies:** T-055
- **Files Created:** `modern/Ecommerce.Api.Tests/Controllers/ProductsControllerTests.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Tests cover 200 response for valid requests.
  - [ ] Tests cover 404 for missing product ID.
  - [ ] `dotnet test` passes.
- **Complexity:** Medium
- **Time:** 1h

### T-057: Write Frontend Unit Tests for ProductCard Component
- **Description:** Create Vitest tests for `ProductCard.vue` verifying props rendering and event emission.
- **Objective:** Component behavior verified.
- **Inputs:** T-047.
- **Outputs:** Test file.
- **Dependencies:** T-056
- **Files Created:** `modern/Ecommerce.Spa/tests/unit/components/ProductCard.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Test renders component with props.
  - [ ] Test verifies `add-to-cart` emit.
  - [ ] `npm run test:unit` passes.
- **Complexity:** Low
- **Time:** 0.5h

### T-058: Write Frontend Unit Tests for Product Store
- **Description:** Create Vitest tests for `useProductStore` verifying fetch calls correct URL with query params.
- **Objective:** Store logic verified.
- **Inputs:** T-051.
- **Outputs:** Test file.
- **Dependencies:** T-057
- **Files Created:** `modern/Ecommerce.Spa/tests/unit/stores/useProductStore.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Test verifies correct API endpoint called.
  - [ ] `npm run test:unit` passes.
- **Complexity:** Low
- **Time:** 0.5h

### T-059: Write E2E Test for Product Catalog Flow
- **Description:** Create Playwright spec that visits `/products`, filters by category, clicks a product, and asserts detail page.
- **Objective:** End-to-end product catalog verified.
- **Inputs:** T-052, T-053.
- **Outputs:** E2E spec file.
- **Dependencies:** T-058
- **Files Created:** `modern/Ecommerce.Spa/tests/e2e/specs/product-catalog.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Spec visits `/products`.
  - [ ] Spec selects category filter.
  - [ ] Spec clicks product and verifies detail page URL and title.
  - [ ] `npx playwright test` passes.
- **Complexity:** Medium
- **Time:** 1h

---

## Phase 5: Shopping Cart Backend & Store

**Objective:** Port the cart domain model, DTOs, repository and service layers, register cart services in DI, implement the authenticated `CartController`, and create the Pinia cart store. The API can manage server-side cart items for an authenticated customer.

**Task Range:** T-060 – T-068
**Number of Tasks:** 9

**Entry Criteria:** Phase 4 complete. Product catalog backend and frontend verified. JWT authentication active. Repository and service patterns established in Phase 3. `DatabaseHelper` available for ADO.NET access.

**Exit Criteria:** `CartItem.cs` compiles in Core/Models. Cart DTOs (`CartItemDto`, `CartSummaryDto`, `AddCartItemRequest`) exist. `ICartItemRepository` and `CartItemRepository` ported. `ICartService` and `CartService` ported with DTO return types. Cart services registered in DI. `CartController` exposes `GET /api/cart`, `POST /api/cart/items`, `PUT /api/cart/items/{id}`, `DELETE /api/cart/items/{id}`, `GET /api/cart/summary`; all actions are `[Authorize]` and return 401 for anonymous, 200 for authenticated. `useCartStore.ts` calls the correct API endpoints with no localStorage persistence.

**Deliverables:**
- `CartItem.cs` model
- 3 cart DTO files
- `ICartItemRepository` + `CartItemRepository`
- `ICartService` + `CartService`
- DI registrations for cart services
- `CartController.cs` (authenticated)
- `useCartStore.ts`

### T-060: Port CartItem Domain Model
- **Description:** Copy `CartItem.cs` from legacy to new project.
- **Objective:** CartItem entity available.
- **Inputs:** T-059. Legacy repository.
- **Outputs:** Model file.
- **Dependencies:** T-059
- **Files Created:** `modern/Ecommerce.Api.Core/Models/CartItem.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-061: Create Cart DTOs
- **Description:** Define `CartItemDto`, `CartSummaryDto`, and `AddCartItemRequest` records.
- **Objective:** Cart API contracts defined.
- **Inputs:** T-060.
- **Outputs:** DTO files.
- **Dependencies:** T-060
- **Files Created:**
  - `modern/Ecommerce.Api.Core/DTOs/CartItemDto.cs`
  - `modern/Ecommerce.Api.Core/DTOs/CartSummaryDto.cs`
  - `modern/Ecommerce.Api.Core/DTOs/AddCartItemRequest.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] All DTOs compile.
- **Complexity:** Low
- **Time:** 0.5h

### T-062: Port ICartItemRepository Interface
- **Description:** Copy `ICartItemRepository` from legacy to new project.
- **Objective:** Cart repository contract available.
- **Inputs:** T-061. Legacy repository.
- **Outputs:** Interface file.
- **Dependencies:** T-061
- **Files Created:** `modern/Ecommerce.Api.Core/Interfaces/ICartItemRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-063: Port CartItemRepository Implementation
- **Description:** Copy `CartItemRepository` from legacy to new project with SQL client update.
- **Objective:** Cart data access works.
- **Inputs:** T-062. Legacy repository.
- **Outputs:** Repository file.
- **Dependencies:** T-062
- **Files Created:** `modern/Ecommerce.Api.Data/Repositories/CartItemRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles with `Microsoft.Data.SqlClient`.
- **Complexity:** Low
- **Time:** 0.5h

### T-064: Port ICartService Interface
- **Description:** Copy `ICartService` from legacy to new project.
- **Objective:** Cart service contract available.
- **Inputs:** T-063. Legacy repository.
- **Outputs:** Interface file.
- **Dependencies:** T-063
- **Files Created:** `modern/Ecommerce.Api.Services/Interfaces/ICartService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-065: Port CartService Implementation
- **Description:** Copy `CartService` from legacy to new project. Update namespace. Change return types to DTOs.
- **Objective:** Cart business logic available.
- **Inputs:** T-064. Legacy repository.
- **Outputs:** Service file.
- **Dependencies:** T-064
- **Files Created:** `modern/Ecommerce.Api.Services/Implementations/CartService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] No Razor/ViewModel references.
  - [ ] Returns DTOs.
- **Complexity:** Medium
- **Time:** 1h

### T-066: Register Cart Services in DI
- **Description:** Add scoped DI registrations for cart repository and service.
- **Objective:** DI container resolves cart dependencies.
- **Inputs:** T-065.
- **Outputs:** Modified `Program.cs`.
- **Dependencies:** T-065
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Program.cs`
- **Acceptance Criteria:**
  - [ ] `AddScoped<ICartItemRepository, CartItemRepository>()` present.
  - [ ] `AddScoped<ICartService, CartService>()` present.
  - [ ] API starts without DI exceptions.
- **Complexity:** Low
- **Time:** 0.25h

### T-067: Implement CartController with Authenticated Endpoints
- **Description:** Create `CartController` with `[Authorize]` on all actions: `GET /api/cart`, `POST /api/cart/items`, `PUT /api/cart/items/{id}`, `DELETE /api/cart/items/{id}`, `GET /api/cart/summary`.
- **Objective:** Cart API endpoints protected and functional.
- **Inputs:** T-066.
- **Outputs:** Controller file.
- **Dependencies:** T-066
- **Files Created:** `modern/Ecommerce.Api/Controllers/CartController.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] All actions have `[Authorize]` attribute.
  - [ ] Anonymous requests return 401.
  - [ ] Authenticated requests return 200 with correct DTOs.
- **Complexity:** Medium
- **Time:** 1.5h

### T-068: Create Pinia Cart Store
- **Description:** Implement `useCartStore` with actions to fetch cart, add item, update quantity, and remove item.
- **Objective:** Cart state management in SPA.
- **Inputs:** T-067.
- **Outputs:** Store module.
- **Dependencies:** T-067
- **Files Created:** `modern/Ecommerce.Spa/src/stores/useCartStore.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Store contains `items`, `itemCount` computed.
  - [ ] `addItem` calls `POST /api/cart/items`.
  - [ ] `fetchCart` calls `GET /api/cart`.
  - [ ] No localStorage persistence (server-side cart).
- **Complexity:** Medium
- **Time:** 1h

---

## Phase 6: Shopping Cart Frontend & Tests

**Objective:** Build the cart Vue UI (`AddToCartButton`, `CartView`, `CartSummaryBadge`), add the `/cart` route to Vue Router, route `/cart*` through YARP to the SPA cluster, and complete backend unit tests, frontend unit tests, and an E2E Playwright spec for the cart flow. The cart is fully functional in Vue + API.

**Task Range:** T-069 – T-078
**Number of Tasks:** 10

**Entry Criteria:** Phase 5 complete. `CartController` returns correct DTOs for authenticated users. `useCartStore` calls the correct API endpoints. Vue SPA, Pinia, Vue Router, fetch wrapper, and auth store are available from Phase 2. Product catalog frontend patterns established in Phase 4.

**Exit Criteria:** `AddToCartButton` calls `cartStore.addItem()` with a Bearer token. `CartView` fetches and renders the cart, with quantity updates and removal persisting. `CartSummaryBadge` displays `itemCount` and updates when items are added. `/cart` route added to Vue Router. `/cart*` routed to `spaCluster` via YARP. `CartController` and `CartService` unit tests pass. `AddToCartButton` and `CartView` Vitest specs pass. Playwright cart flow E2E spec passes.

**Deliverables:**
- `AddToCartButton.vue`, `CartView.vue`, `CartSummaryBadge.vue`
- `/cart` Vue Router entry
- YARP `/cart*` route
- `CartControllerTests.cs`, `CartServiceTests.cs`
- `AddToCartButton` and `CartView` Vitest specs
- Cart flow Playwright E2E spec

### T-069: Create AddToCartButton Vue Component
- **Description:** Build a reusable button component that triggers cart store `addItem` action.
- **Objective:** Add-to-cart interaction available.
- **Inputs:** T-068.
- **Outputs:** Component file.
- **Dependencies:** T-068
- **Files Created:** `modern/Ecommerce.Spa/src/components/AddToCartButton.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Click calls `cartStore.addItem()`.
  - [ ] Includes Bearer token in API call.
- **Complexity:** Low
- **Time:** 0.5h

### T-070: Create CartView Vue Page
- **Description:** Build the cart page that displays items, allows quantity updates, and supports item removal.
- **Objective:** Full cart UI in Vue.
- **Inputs:** T-069.
- **Outputs:** View component.
- **Dependencies:** T-069
- **Files Created:** `modern/Ecommerce.Spa/src/views/CartView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Page fetches and displays cart items.
  - [ ] Quantity change persists.
  - [ ] Remove item updates UI and backend.
- **Complexity:** Medium
- **Time:** 1.5h

### T-071: Create CartSummaryBadge Component
- **Description:** Build a header badge component showing item count from cart store.
- **Objective:** Cart indicator visible site-wide.
- **Inputs:** T-070.
- **Outputs:** Component file.
- **Dependencies:** T-070
- **Files Created:** `modern/Ecommerce.Spa/src/components/CartSummaryBadge.vue`
- **Files Modified:** `modern/Ecommerce.Spa/src/components/TheHeader.vue` (or equivalent)
- **Acceptance Criteria:**
  - [ ] Badge displays correct item count.
  - [ ] Count updates when items added.
- **Complexity:** Low
- **Time:** 0.5h

### T-072: Add Cart Route to Vue Router
- **Description:** Add `/cart` route mapping to `CartView`.
- **Objective:** Cart page accessible via URL.
- **Inputs:** T-071.
- **Outputs:** Updated router config.
- **Dependencies:** T-071
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/router/index.ts`
- **Acceptance Criteria:**
  - [ ] `/cart` renders `CartView`.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-073: Add YARP Route for Cart Pages
- **Description:** Configure YARP to route `/cart*` to SPA cluster.
- **Objective:** Cart page traffic intercepted by YARP.
- **Inputs:** T-072.
- **Outputs:** Updated YARP config.
- **Dependencies:** T-072
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] `/cart*` route present.
  - [ ] `http://localhost:5003/cart` serves Vue SPA.
- **Complexity:** Low
- **Time:** 0.1h

### T-074: Write Backend Unit Tests for CartController
- **Description:** Create xUnit tests for `CartController` mocking `ClaimsPrincipal` with `customer_id`.
- **Objective:** Cart controller behavior verified.
- **Inputs:** T-067.
- **Outputs:** Test file.
- **Dependencies:** T-073
- **Files Created:** `modern/Ecommerce.Api.Tests/Controllers/CartControllerTests.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Tests verify 401 for anonymous requests.
  - [ ] Tests verify 200 for authenticated requests.
  - [ ] `dotnet test` passes.
- **Complexity:** Medium
- **Time:** 1h

### T-075: Write Backend Unit Tests for CartService
- **Description:** Create xUnit tests for `CartService` mocking `ICartItemRepository`.
- **Objective:** Cart business logic verified.
- **Inputs:** T-065.
- **Outputs:** Test file.
- **Dependencies:** T-074
- **Files Created:** `modern/Ecommerce.Api.Services.Tests/CartServiceTests.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Tests cover add, update, remove logic.
  - [ ] `dotnet test` passes.
- **Complexity:** Medium
- **Time:** 1h

### T-076: Write Frontend Unit Tests for AddToCartButton
- **Description:** Create Vitest tests verifying click emits event and calls store method.
- **Objective:** Add-to-cart button behavior verified.
- **Inputs:** T-069.
- **Outputs:** Test file.
- **Dependencies:** T-075
- **Files Created:** `modern/Ecommerce.Spa/tests/unit/components/AddToCartButton.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `npm run test:unit` passes.
- **Complexity:** Low
- **Time:** 0.5h

### T-077: Write Frontend Unit Tests for CartView
- **Description:** Create Vitest tests verifying item rendering and quantity handling.
- **Objective:** Cart view behavior verified.
- **Inputs:** T-070.
- **Outputs:** Test file.
- **Dependencies:** T-076
- **Files Created:** `modern/Ecommerce.Spa/tests/unit/views/CartView.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `npm run test:unit` passes.
- **Complexity:** Low
- **Time:** 0.5h

### T-078: Write E2E Test for Cart Flow
- **Description:** Create Playwright spec for full cart flow: add item, update quantity, remove item, verify empty state.
- **Objective:** End-to-end cart verified.
- **Inputs:** T-070.
- **Outputs:** E2E spec file.
- **Dependencies:** T-077
- **Files Created:** `modern/Ecommerce.Spa/tests/e2e/specs/cart.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Spec adds product to cart from detail page.
  - [ ] Spec verifies cart page shows item.
  - [ ] Spec updates quantity and verifies total.
  - [ ] Spec removes item and verifies empty cart message.
  - [ ] `npx playwright test` passes.
- **Complexity:** Medium
- **Time:** 1.5h

---

## Phase 7: Checkout Backend

**Objective:** Port the address, order, order item, and customer domain models, checkout DTOs, address/order/customer repositories, `CheckoutService` (verbatim, preserving transaction and idempotency logic), and `OrderService`. Register checkout and order services in DI. Implement `CheckoutController` (with `Idempotency-Key` requirement on `PlaceOrder`) and read-only `OrdersController`. The API can place orders and serve order history.

**Task Range:** T-079 – T-090
**Number of Tasks:** 12

**Entry Criteria:** Phase 6 complete. Cart flow verified end-to-end. `DatabaseHelper`, repository, and service patterns established in prior phases. JWT authentication active. Database backup taken before this data-mutating phase (per cross-cutting task CC.2).

**Exit Criteria:** `Address`, `Order`, `OrderItem`, `Customer` models compile. Checkout DTOs (`AddressDto`, `OrderDto`, `OrderItemDto`, `PlaceOrderRequest`, `OrderConfirmationDto`) exist. Address, order, and customer repositories ported. `CheckoutService` ported verbatim with transaction and idempotency logic intact. `OrderService` ported. Services registered in DI. `CheckoutController` exposes address, review, order placement, and confirmation endpoints; `POST /api/checkout/orders` returns 400 without `Idempotency-Key`, 201 on first order, 200 on duplicate. `OrdersController` exposes `GET /api/orders` and `GET /api/orders/{id}` returning only the authenticated customer's orders.

**Deliverables:**
- 4 checkout domain model files
- 5 checkout DTO files
- 3 repository interface + implementation pairs
- `CheckoutService.cs` (verbatim port)
- `OrderService.cs`
- DI registrations for checkout and order services
- `CheckoutController.cs` with idempotency enforcement
- `OrdersController.cs` (read-only)

### T-079: Port Address Domain Model
- **Description:** Copy `Address.cs` from legacy to new project.
- **Objective:** Address entity available.
- **Inputs:** T-078. Legacy repository.
- **Outputs:** Model file.
- **Dependencies:** T-078
- **Files Created:** `modern/Ecommerce.Api.Core/Models/Address.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-080: Port Order and OrderItem Domain Models
- **Description:** Copy `Order.cs` and `OrderItem.cs` from legacy to new project.
- **Objective:** Order entities available.
- **Inputs:** T-079. Legacy repository.
- **Outputs:** Two model files.
- **Dependencies:** T-079
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Models/Order.cs`
  - `modern/Ecommerce.Api.Core/Models/OrderItem.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Both files compile.
- **Complexity:** Low
- **Time:** 0.25h

### T-081: Port Customer Domain Model
- **Description:** Copy `Customer.cs` from legacy to new project.
- **Objective:** Customer entity available.
- **Inputs:** T-080. Legacy repository.
- **Outputs:** Model file.
- **Dependencies:** T-080
- **Files Created:** `modern/Ecommerce.Api.Core/Models/Customer.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
- **Complexity:** Low
- **Time:** 0.25h

### T-082: Create Checkout DTOs
- **Description:** Define `AddressDto`, `OrderDto`, `OrderItemDto`, `PlaceOrderRequest`, `OrderConfirmationDto`.
- **Objective:** Checkout API contracts defined.
- **Inputs:** T-081.
- **Outputs:** DTO files.
- **Dependencies:** T-081
- **Files Created:**
  - `modern/Ecommerce.Api.Core/DTOs/AddressDto.cs`
  - `modern/Ecommerce.Api.Core/DTOs/OrderDto.cs`
  - `modern/Ecommerce.Api.Core/DTOs/OrderItemDto.cs`
  - `modern/Ecommerce.Api.Core/DTOs/PlaceOrderRequest.cs`
  - `modern/Ecommerce.Api.Core/DTOs/OrderConfirmationDto.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] All DTOs compile.
  - [ ] No circular references.
- **Complexity:** Low
- **Time:** 0.5h

### T-083: Port Address Repository Interface and Implementation
- **Description:** Copy `IAddressRepository` and `AddressRepository` from legacy to new project.
- **Objective:** Address data access works.
- **Inputs:** T-082. Legacy repository.
- **Outputs:** Two files.
- **Dependencies:** T-082
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Interfaces/IAddressRepository.cs`
  - `modern/Ecommerce.Api.Data/Repositories/AddressRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface and implementation compile with `Microsoft.Data.SqlClient`.
- **Complexity:** Low
- **Time:** 0.5h

### T-084: Port Order Repository Interface and Implementation
- **Description:** Copy `IOrderRepository` and `OrderRepository` from legacy to new project.
- **Objective:** Order data access works.
- **Inputs:** T-083. Legacy repository.
- **Outputs:** Two files.
- **Dependencies:** T-083
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Interfaces/IOrderRepository.cs`
  - `modern/Ecommerce.Api.Data/Repositories/OrderRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface and implementation compile with `Microsoft.Data.SqlClient`.
- **Complexity:** Low
- **Time:** 0.5h

### T-085: Port Customer Repository Interface and Implementation
- **Description:** Copy `ICustomerRepository` and `CustomerRepository` from legacy to new project.
- **Objective:** Customer data access works.
- **Inputs:** T-084. Legacy repository.
- **Outputs:** Two files.
- **Dependencies:** T-084
- **Files Created:**
  - `modern/Ecommerce.Api.Core/Interfaces/ICustomerRepository.cs`
  - `modern/Ecommerce.Api.Data/Repositories/CustomerRepository.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Interface and implementation compile with `Microsoft.Data.SqlClient`.
- **Complexity:** Low
- **Time:** 0.5h

### T-086: Port CheckoutService Verbatim
- **Description:** Copy `CheckoutService` from legacy to new project. Change only namespaces and final return type. Preserve transaction and idempotency logic exactly.
- **Objective:** Checkout business logic available unchanged.
- **Inputs:** T-085. Legacy repository.
- **Outputs:** Service file.
- **Dependencies:** T-085
- **Files Created:** `modern/Ecommerce.Api.Services/Implementations/CheckoutService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
  - [ ] Diff against legacy shows only namespace and return type changes.
  - [ ] Transaction logic intact.
  - [ ] Idempotency logic intact.
- **Complexity:** Medium
- **Time:** 1.5h

### T-087: Port OrderService
- **Description:** Copy `OrderService` from legacy to new project.
- **Objective:** Order business logic available.
- **Inputs:** T-086. Legacy repository.
- **Outputs:** Service file.
- **Dependencies:** T-086
- **Files Created:** `modern/Ecommerce.Api.Services/Implementations/OrderService.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] File compiles.
- **Complexity:** Low
- **Time:** 0.5h

### T-088: Register Checkout and Order Services in DI
- **Description:** Add scoped DI registrations for address, order, and customer repositories and services.
- **Objective:** DI container resolves checkout and order dependencies.
- **Inputs:** T-087.
- **Outputs:** Modified `Program.cs`.
- **Dependencies:** T-087
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/Program.cs`
- **Acceptance Criteria:**
  - [ ] All new repositories and services registered.
  - [ ] API starts without DI exceptions.
- **Complexity:** Low
- **Time:** 0.25h

### T-089: Implement CheckoutController
- **Description:** Create `CheckoutController` with `[Authorize]` actions: `GET /api/checkout/addresses`, `POST /api/checkout/addresses`, `GET /api/checkout/review`, `POST /api/checkout/orders` (requires `Idempotency-Key` header), `GET /api/checkout/orders/{id}/confirmation`.
- **Objective:** Checkout API endpoints protected and functional.
- **Inputs:** T-088.
- **Outputs:** Controller file.
- **Dependencies:** T-088
- **Files Created:** `modern/Ecommerce.Api/Controllers/CheckoutController.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] All actions `[Authorize]`.
  - [ ] `POST /api/checkout/orders` returns 400 without `Idempotency-Key`.
  - [ ] Valid request returns 201 with `OrderConfirmationDto`.
- **Complexity:** High
- **Time:** 2h

### T-090: Implement OrdersController (Read-Only)
- **Description:** Create `OrdersController` with `[Authorize]` actions: `GET /api/orders`, `GET /api/orders/{id}`. Filter by authenticated customer's `customer_id`.
- **Objective:** Order history API endpoints available.
- **Inputs:** T-089.
- **Outputs:** Controller file.
- **Dependencies:** T-089
- **Files Created:** `modern/Ecommerce.Api/Controllers/OrdersController.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Returns only orders for authenticated customer.
  - [ ] Returns 404 for orders not belonging to customer.
- **Complexity:** Medium
- **Time:** 1h

---

## Phase 8: Checkout Frontend & Tests

**Objective:** Build the checkout Vue UI (Pinia checkout store with idempotency key, address selector/form, address step, review step, order confirmation, checkout wizard), wire checkout and confirmation routes in Vue Router, route `/checkout*` through YARP, and complete backend unit tests, frontend unit tests, and a full E2E Playwright spec covering cart → checkout → place order → confirmation with DB verification. The complete checkout flow works end-to-end.

**Task Range:** T-091 – T-103
**Number of Tasks:** 13

**Entry Criteria:** Phase 7 complete. `CheckoutController` and `OrdersController` return correct DTOs. `Idempotency-Key` enforced on `POST /api/checkout/orders`. Cart frontend functional from Phase 6. Vue SPA, Pinia, Vue Router, fetch wrapper, and auth store available.

**Exit Criteria:** `useCheckoutStore` holds selected address and a UUID idempotency key generated on wizard mount. `AddressSelector`, `AddressForm`, `AddressStep`, `ReviewStep`, `OrderConfirmation`, and `CheckoutWizard` components render and function. `/checkout` and `/checkout/confirmation/:orderId` routes added with guards. `/checkout*` routed to `spaCluster`. `CheckoutService` and `CheckoutController` unit tests pass (transaction committed once, duplicate idempotency returns existing order, missing key returns 400). `CheckoutWizard` Vitest spec passes. Playwright full checkout E2E spec passes and SQL queries confirm order exists and cart items are cleared.

**Deliverables:**
- `useCheckoutStore.ts`
- `AddressSelector.vue`, `AddressForm.vue`, `AddressStep.vue`, `ReviewStep.vue`, `OrderConfirmation.vue`, `CheckoutWizard.vue`
- Checkout and confirmation Vue Router entries
- YARP `/checkout*` route
- `CheckoutServiceTests.cs`, `CheckoutControllerTests.cs`
- `CheckoutWizard` Vitest spec
- Full checkout Playwright E2E spec

### T-091: Create Pinia Checkout Store
- **Description:** Implement `useCheckoutStore` with state for selected address, shipping method, and generated UUID idempotency key.
- **Objective:** Checkout state management in SPA.
- **Inputs:** T-090.
- **Outputs:** Store module.
- **Dependencies:** T-090
- **Files Created:** `modern/Ecommerce.Spa/src/stores/useCheckoutStore.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Store generates UUID on checkout start.
  - [ ] Persists selected address across wizard steps.
  - [ ] Provides `idempotencyKey` for API calls.
- **Complexity:** Medium
- **Time:** 1h

### T-092: Create AddressSelector Vue Component
- **Description:** Build a component that lists customer addresses and allows selection.
- **Objective:** Address selection UI available.
- **Inputs:** T-091.
- **Outputs:** Component file.
- **Dependencies:** T-091
- **Files Created:** `modern/Ecommerce.Spa/src/components/AddressSelector.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Fetches addresses from API.
  - [ ] Selecting address updates checkout store.
- **Complexity:** Low
- **Time:** 0.5h

### T-093: Create AddressForm Vue Component
- **Description:** Build a form component for creating a new address.
- **Objective:** New address creation UI available.
- **Inputs:** T-092.
- **Outputs:** Component file.
- **Dependencies:** T-092
- **Files Created:** `modern/Ecommerce.Spa/src/components/AddressForm.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Form validates required fields.
  - [ ] Submit posts to `POST /api/checkout/addresses`.
  - [ ] Success adds address to selector.
- **Complexity:** Low
- **Time:** 0.5h

### T-094: Create AddressStep Vue Component
- **Description:** Build the first step of checkout wizard combining AddressSelector and AddressForm.
- **Objective:** Address selection step functional.
- **Inputs:** T-093.
- **Outputs:** Component file.
- **Dependencies:** T-093
- **Files Created:** `modern/Ecommerce.Spa/src/components/AddressStep.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Displays saved addresses.
  - [ ] Allows toggling to new address form.
  - [ ] Proceeds to next step only when address selected.
- **Complexity:** Medium
- **Time:** 1h

### T-095: Create ReviewStep Vue Component
- **Description:** Build the order review step displaying cart summary, selected address, and total.
- **Objective:** Order review step functional.
- **Inputs:** T-094.
- **Outputs:** Component file.
- **Dependencies:** T-094
- **Files Created:** `modern/Ecommerce.Spa/src/components/ReviewStep.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Displays accurate order preview.
  - [ ] "Place Order" button visible.
  - [ ] Calls `POST /api/checkout/orders` with `Idempotency-Key` header.
- **Complexity:** Medium
- **Time:** 1h

### T-096: Create OrderConfirmation Vue Component
- **Description:** Build the success page showing order number after placement.
- **Objective:** Confirmation step functional.
- **Inputs:** T-095.
- **Outputs:** Component file.
- **Dependencies:** T-095
- **Files Created:** `modern/Ecommerce.Spa/src/components/OrderConfirmation.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Displays order ID from route parameter.
  - [ ] Shows success message.
- **Complexity:** Low
- **Time:** 0.5h

### T-097: Create CheckoutWizard Vue Page
- **Description:** Build the multi-step checkout wizard page composing AddressStep, ReviewStep, and OrderConfirmation.
- **Objective:** Full checkout flow in Vue.
- **Inputs:** T-096.
- **Outputs:** View component.
- **Dependencies:** T-096
- **Files Created:** `modern/Ecommerce.Spa/src/views/CheckoutWizard.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Step navigation works forward and backward.
  - [ ] State persists across steps.
  - [ ] Cannot access later steps without completing earlier ones.
- **Complexity:** High
- **Time:** 2h

### T-098: Add Checkout and Confirmation Routes to Vue Router
- **Description:** Add `/checkout` → `CheckoutWizard` and `/checkout/confirmation/:orderId` → `OrderConfirmation`.
- **Objective:** Checkout routes accessible.
- **Inputs:** T-097.
- **Outputs:** Updated router config.
- **Dependencies:** T-097
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/router/index.ts`
- **Acceptance Criteria:**
  - [ ] `/checkout` renders wizard.
  - [ ] `/checkout/confirmation/123` renders confirmation.
  - [ ] Guards prevent direct access to confirmation without checkout flow.
- **Complexity:** Low
- **Time:** 0.5h

### T-099: Add YARP Route for Checkout Pages
- **Description:** Configure YARP to route `/checkout*` to SPA cluster.
- **Objective:** Checkout traffic intercepted by YARP.
- **Inputs:** T-098.
- **Outputs:** Updated YARP config.
- **Dependencies:** T-098
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] `/checkout*` route present.
  - [ ] `http://localhost:5003/checkout` serves Vue SPA.
- **Complexity:** Low
- **Time:** 0.1h

### T-100: Write Backend Unit Tests for CheckoutService
- **Description:** Create xUnit tests for `CheckoutService` mocking `IUnitOfWork`. Verify transaction commit, idempotency, and address creation.
- **Objective:** Checkout business logic verified.
- **Inputs:** T-086.
- **Outputs:** Test file.
- **Dependencies:** T-099
- **Files Created:** `modern/Ecommerce.Api.Services.Tests/CheckoutServiceTests.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Test verifies transaction committed exactly once.
  - [ ] Test verifies duplicate idempotency key returns existing order.
  - [ ] Test verifies new address created when `addressId` is null.
  - [ ] `dotnet test` passes.
- **Complexity:** High
- **Time:** 2h

### T-101: Write Backend Unit Tests for CheckoutController
- **Description:** Create xUnit tests for `CheckoutController` verifying `Idempotency-Key` validation and order placement.
- **Objective:** Checkout controller behavior verified.
- **Inputs:** T-089.
- **Outputs:** Test file.
- **Dependencies:** T-100
- **Files Created:** `modern/Ecommerce.Api.Tests/Controllers/CheckoutControllerTests.cs`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Test verifies 400 when `Idempotency-Key` missing.
  - [ ] Test verifies 201 on first order, 200 on duplicate.
  - [ ] `dotnet test` passes.
- **Complexity:** Medium
- **Time:** 1.5h

### T-102: Write Frontend Unit Tests for CheckoutWizard
- **Description:** Create Vitest tests for `CheckoutWizard` verifying step navigation and form submission.
- **Objective:** Checkout wizard behavior verified.
- **Inputs:** T-097.
- **Outputs:** Test file.
- **Dependencies:** T-101
- **Files Created:** `modern/Ecommerce.Spa/tests/unit/views/CheckoutWizard.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Test verifies step navigation.
  - [ ] Test verifies submit sends correct headers.
  - [ ] `npm run test:unit` passes.
- **Complexity:** Medium
- **Time:** 1h

### T-103: Write E2E Test for Full Checkout Flow
- **Description:** Create Playwright spec for complete checkout: cart → checkout → address → review → place order → confirmation → verify DB state.
- **Objective:** End-to-end checkout verified.
- **Inputs:** T-097.
- **Outputs:** E2E spec file.
- **Dependencies:** T-102
- **Files Created:** `modern/Ecommerce.Spa/tests/e2e/specs/checkout.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Spec completes full checkout flow.
  - [ ] SQL query confirms order exists in `Orders` table.
  - [ ] SQL query confirms `CartItems` cleared for customer.
  - [ ] `npx playwright test` passes.
- **Complexity:** Critical
- **Time:** 2h

---

## Phase 9: Order History, Home & Search

**Objective:** Build the order history frontend (list and detail views, routes, YARP routing, tests), integrate legacy account page links in the Vue header, then migrate the remaining public read-only pages: `HomeView` with featured products, header search input, home route, YARP routes for homepage and search, and corresponding frontend unit and E2E tests. All public read-only pages and order history are served by Vue + API; legacy fallback remains only for account/admin routes.

**Task Range:** T-104 – T-116
**Number of Tasks:** 13

**Entry Criteria:** Phase 8 complete. `GET /api/orders` and `GET /api/orders/{id}` return the authenticated customer's orders. Full checkout flow verified. Vue Router and YARP configured for product, cart, and checkout routes. Product list/detail patterns established for reuse on homepage and search.

**Exit Criteria:** `OrderListView` and `OrderDetailView` render real order data. `/orders` and `/orders/:id` routes added; `/orders*` routed to `spaCluster`. Order view Vitest specs and order history Playwright E2E spec pass. Vue header links to legacy `/Account/Login`, `/Account/Register`, `/Account/Manage` via YARP fallback; JWT bridge keeps SPA authenticated after login. `HomeView` renders featured products. Search input in header calls `GET /api/products?searchQuery=` and updates the product grid. `/` route added; `/` and `/home*` routed to `spaCluster`. `HomeView` Vitest spec and homepage Playwright E2E spec pass.

**Deliverables:**
- `OrderListView.vue`, `OrderDetailView.vue`
- `/orders` Vue Router entries and YARP `/orders*` route
- Order view Vitest specs and order history Playwright E2E spec
- Modified `TheHeader.vue` with legacy account links
- `HomeView.vue`
- Header search input and `useProductStore` search query handling
- `/` Vue Router entry and YARP `/` + `/home*` routes
- `HomeView` Vitest spec and homepage Playwright E2E spec

### T-104: Create OrderListView Vue Page
- **Description:** Build the order history page displaying a paginated table of customer orders.
- **Objective:** Order history UI in Vue.
- **Inputs:** T-103.
- **Outputs:** View component.
- **Dependencies:** T-103
- **Files Created:** `modern/Ecommerce.Spa/src/views/OrderListView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Fetches `GET /api/orders`.
  - [ ] Renders table with order number, date, total, status.
  - [ ] Pagination works.
- **Complexity:** Medium
- **Time:** 1h

### T-105: Create OrderDetailView Vue Page
- **Description:** Build the order detail page displaying items, shipping address, and order status.
- **Objective:** Order detail UI in Vue.
- **Inputs:** T-104.
- **Outputs:** View component.
- **Dependencies:** T-104
- **Files Created:** `modern/Ecommerce.Spa/src/views/OrderDetailView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Fetches `GET /api/orders/{id}`.
  - [ ] Displays order items, quantities, prices.
  - [ ] Displays shipping address.
- **Complexity:** Medium
- **Time:** 1h

### T-106: Add Order Routes to Vue Router
- **Description:** Add `/orders` → `OrderListView` and `/orders/:id` → `OrderDetailView`.
- **Objective:** Order history routes accessible.
- **Inputs:** T-105.
- **Outputs:** Updated router config.
- **Dependencies:** T-105
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/router/index.ts`
- **Acceptance Criteria:**
  - [ ] `/orders` renders order list.
  - [ ] `/orders/123` renders order detail.
- **Complexity:** Low
- **Time:** 0.25h

### T-107: Add YARP Route for Order Pages
- **Description:** Configure YARP to route `/orders*` to SPA cluster.
- **Objective:** Order history traffic intercepted by YARP.
- **Inputs:** T-106.
- **Outputs:** Updated YARP config.
- **Dependencies:** T-106
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] `/orders*` route present.
  - [ ] `http://localhost:5003/orders` serves Vue SPA.
- **Complexity:** Low
- **Time:** 0.1h

### T-108: Write Frontend Unit Tests for Order Views
- **Description:** Create Vitest tests for `OrderListView` and `OrderDetailView`.
- **Objective:** Order view behavior verified.
- **Inputs:** T-104, T-105.
- **Outputs:** Test files.
- **Dependencies:** T-107
- **Files Created:**
  - `modern/Ecommerce.Spa/tests/unit/views/OrderListView.spec.ts`
  - `modern/Ecommerce.Spa/tests/unit/views/OrderDetailView.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `npm run test:unit` passes.
- **Complexity:** Low
- **Time:** 1h

### T-109: Write E2E Test for Order History Flow
- **Description:** Create Playwright spec that places an order and verifies it appears in order history.
- **Objective:** End-to-end order history verified.
- **Inputs:** T-104.
- **Outputs:** E2E spec file.
- **Dependencies:** T-108
- **Files Created:** `modern/Ecommerce.Spa/tests/e2e/specs/order-history.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Spec places order (reuses checkout flow).
  - [ ] Spec navigates to `/orders` and verifies order visible.
  - [ ] Spec clicks order and verifies detail page.
  - [ ] `npx playwright test` passes.
- **Complexity:** Medium
- **Time:** 1h

### T-110: Integrate Legacy Account Page Links in Vue Header
- **Description:** Add login/register/manage links in Vue header that route to legacy MVC pages via YARP fallback.
- **Objective:** Account navigation functional during hybrid phase.
- **Inputs:** T-109.
- **Outputs:** Modified header component.
- **Dependencies:** T-109
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/components/TheHeader.vue` (or equivalent)
- **Acceptance Criteria:**
  - [ ] Header contains login/register links.
  - [ ] Links point to `/Account/Login`, `/Account/Register`.
  - [ ] Legacy pages load correctly.
  - [ ] After login, JWT bridge keeps SPA authenticated.
- **Complexity:** Low
- **Time:** 0.5h

### T-111: Create HomeView Vue Page
- **Description:** Build the homepage displaying featured products and hero banner.
- **Objective:** Homepage in Vue.
- **Inputs:** T-110.
- **Outputs:** View component.
- **Dependencies:** T-110
- **Files Created:** `modern/Ecommerce.Spa/src/views/HomeView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Renders featured products (reuses `ProductCard`).
  - [ ] Displays hero banner.
- **Complexity:** Medium
- **Time:** 1h

### T-112: Add Search Input to Header
- **Description:** Add a search input to the Vue header that triggers product search via existing `GET /api/products?searchQuery=`.
- **Objective:** Search functionality in Vue.
- **Inputs:** T-111.
- **Outputs:** Modified header component.
- **Dependencies:** T-111
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/components/TheHeader.vue`
- **Acceptance Criteria:**
  - [ ] Search input present in header.
  - [ ] Submit triggers search API call.
  - [ ] Results update product grid.
- **Complexity:** Low
- **Time:** 0.5h

### T-113: Add Home Route to Vue Router
- **Description:** Add `/` → `HomeView` to router configuration.
- **Objective:** Homepage accessible at root path.
- **Inputs:** T-112.
- **Outputs:** Updated router config.
- **Dependencies:** T-112
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/router/index.ts`
- **Acceptance Criteria:**
  - [ ] `/` renders `HomeView`.
- **Complexity:** Trivial
- **Time:** 0.1h

### T-114: Add YARP Routes for Homepage and Search
- **Description:** Configure YARP to route `/` and `/home*` to SPA cluster.
- **Objective:** Homepage traffic intercepted by YARP.
- **Inputs:** T-113.
- **Outputs:** Updated YARP config.
- **Dependencies:** T-113
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] `/` and `/home*` routes present.
  - [ ] `http://localhost:5003/` serves Vue SPA.
- **Complexity:** Low
- **Time:** 0.1h

### T-115: Write Frontend Unit Tests for HomeView
- **Description:** Create Vitest tests for `HomeView` verifying featured products render.
- **Objective:** Homepage behavior verified.
- **Inputs:** T-111.
- **Outputs:** Test file.
- **Dependencies:** T-114
- **Files Created:** `modern/Ecommerce.Spa/tests/unit/views/HomeView.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] `npm run test:unit` passes.
- **Complexity:** Low
- **Time:** 0.5h

### T-116: Write E2E Test for Homepage
- **Description:** Create Playwright spec verifying homepage loads and featured products display.
- **Objective:** End-to-end homepage verified.
- **Inputs:** T-111.
- **Outputs:** E2E spec file.
- **Dependencies:** T-115
- **Files Created:** `modern/Ecommerce.Spa/tests/e2e/specs/homepage.spec.ts`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Spec visits `/`.
  - [ ] Verifies featured products section visible.
  - [ ] `npx playwright test` passes.
- **Complexity:** Low
- **Time:** 0.5h

---

## Phase 10: Decommission Legacy

**Objective:** Confirm zero unexpected legacy traffic via YARP log analysis, build Vue login and register pages, add API auth endpoints (`POST /api/auth/login`, `POST /api/auth/register`) issuing JWTs directly, switch the auth store from the legacy bridge to the API endpoints, remove the YARP legacy fallback, shut down the legacy application, optionally consolidate databases, and run the final full E2E regression suite. The legacy system is decommissioned; all traffic is served by Vue + API.

**Task Range:** T-117 – T-125
**Number of Tasks:** 9

**Entry Criteria:** Phase 9 complete. All public read-only pages and order history served by Vue + API. Legacy fallback active only for `/Account/*` routes. YARP traffic logs available for 7+ consecutive days. Database backup taken before decommissioning (per cross-cutting task CC.2).

**Exit Criteria:** Traffic analysis report documents zero unexpected legacy traffic. `LoginView` and `RegisterView` Vue pages exist with forms. `POST /api/auth/login` and `POST /api/auth/register` issue valid JWTs that validate with existing middleware. `useAuthStore` calls the API auth endpoints instead of the legacy bridge. YARP legacy catch-all removed; `/*` routes to `spaCluster` and Vue Router handles 404s. Legacy application stopped in all environments (connection refused or 503). (Optional) Identity tables consolidated into the business database with a single API connection string. Final full E2E regression suite passes with 100% success and no console errors.

**Deliverables:**
- `docs/migration/legacy-traffic-analysis.md`
- `LoginView.vue`, `RegisterView.vue`
- `AuthController.cs` with login/register endpoints
- Updated `useAuthStore.ts`
- Updated YARP configuration (legacy fallback removed)
- Legacy shutdown confirmation
- (Optional) `scripts/consolidate-databases.sql` and updated `appsettings.json`
- `docs/migration/final-e2e-report.md`

### T-117: Analyze YARP Traffic Logs for Legacy Usage
- **Description:** Review YARP logs for 7+ consecutive days to confirm no non-account traffic hits legacy fallback.
- **Objective:** Data-driven decision to decommission legacy.
- **Inputs:** T-116.
- **Outputs:** Traffic analysis report.
- **Dependencies:** T-116
- **Files Created:** `docs/migration/legacy-traffic-analysis.md`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Report documents zero unexpected legacy traffic.
  - [ ] Any exceptions documented with justification.
- **Complexity:** Low
- **Time:** 1h

### T-118: Create Vue LoginView Page
- **Description:** Build login page with email/password form.
- **Objective:** Login UI ready for direct API authentication.
- **Inputs:** T-117.
- **Outputs:** View component.
- **Dependencies:** T-117
- **Files Created:** `modern/Ecommerce.Spa/src/views/LoginView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Form with email and password inputs.
  - [ ] Submit triggers login action.
- **Complexity:** Low
- **Time:** 0.5h

### T-119: Create Vue RegisterView Page
- **Description:** Build registration page with required fields.
- **Objective:** Registration UI ready.
- **Inputs:** T-118.
- **Outputs:** View component.
- **Dependencies:** T-118
- **Files Created:** `modern/Ecommerce.Spa/src/views/RegisterView.vue`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Form with registration fields.
  - [ ] Submit triggers registration action.
- **Complexity:** Low
- **Time:** 0.5h

### T-120: Add Auth Endpoints to API
- **Description:** Implement `POST /api/auth/login` and `POST /api/auth/register` that issue JWTs directly using ASP.NET Core Identity or custom logic.
- **Objective:** API becomes authentication authority.
- **Inputs:** T-119.
- **Outputs:** Controller file.
- **Dependencies:** T-119
- **Files Created:** `modern/Ecommerce.Api/Controllers/AuthController.cs`
- **Files Modified:** `modern/Ecommerce.Api/Program.cs` (if Identity registration needed)
- **Acceptance Criteria:**
  - [ ] `POST /api/auth/login` returns valid JWT for correct credentials.
  - [ ] `POST /api/auth/register` creates user and returns JWT.
  - [ ] Token validates with existing JWT middleware.
- **Complexity:** High
- **Time:** 2h

### T-121: Update Auth Store to Use API Endpoints
- **Description:** Modify `useAuthStore` to call new API auth endpoints instead of legacy JWT bridge.
- **Objective:** SPA authenticates directly with API.
- **Inputs:** T-120.
- **Outputs:** Modified store module.
- **Dependencies:** T-120
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Spa/src/stores/useAuthStore.ts`
- **Acceptance Criteria:**
  - [ ] Login calls `POST /api/auth/login`.
  - [ ] Register calls `POST /api/auth/register`.
  - [ ] Token still validates correctly.
- **Complexity:** Medium
- **Time:** 1h

### T-122: Remove Legacy Fallback from YARP
- **Description:** Delete or disable the legacy catch-all route in YARP. Route unmatched paths to SPA cluster (Vue Router handles 404s).
- **Objective:** Legacy no longer receives production traffic.
- **Inputs:** T-121.
- **Outputs:** Updated YARP config.
- **Dependencies:** T-121
- **Files Created:** None
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] Legacy fallback route removed.
  - [ ] `/*` routes to SPA cluster.
  - [ ] `http://localhost:5003/nonexistent` returns Vue SPA (Vue Router 404).
- **Complexity:** Low
- **Time:** 0.25h

### T-123: Shut Down Legacy Application
- **Description:** Stop IIS application pool or process for legacy MVC in all environments.
- **Objective:** Legacy application no longer running.
- **Inputs:** T-122.
- **Outputs:** Shutdown confirmation.
- **Dependencies:** T-122
- **Files Created:** None
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] Legacy site returns connection refused or 503.
  - [ ] Monitoring confirms no legacy process.
- **Complexity:** Low
- **Time:** 0.5h

### T-124: (Optional) Consolidate Databases
- **Description:** If desired, migrate Identity tables into business database and update API connection string.
- **Objective:** Single database for API.
- **Inputs:** T-123.
- **Outputs:** Migration script and updated config.
- **Dependencies:** T-123
- **Files Created:** `scripts/consolidate-databases.sql`
- **Files Modified:** `modern/Ecommerce.Api/appsettings.json`
- **Acceptance Criteria:**
  - [ ] All identity tables present in business DB.
  - [ ] API authenticates using single connection string.
- **Complexity:** Medium
- **Time:** 2h

### T-125: Run Final Full E2E Regression Suite
- **Description:** Execute all Playwright specs in sequence: homepage → product → cart → checkout → orders → logout → login.
- **Objective:** Entire modernized stack verified end-to-end.
- **Inputs:** T-124 (or T-123 if skipping DB consolidation).
- **Outputs:** Final E2E report.
- **Dependencies:** T-124
- **Files Created:** `docs/migration/final-e2e-report.md`
- **Files Modified:** None
- **Acceptance Criteria:**
  - [ ] All E2E specs execute.
  - [ ] 100% pass rate.
  - [ ] No console errors in browser.
- **Complexity:** Medium
- **Time:** 1h

---

## Summary Statistics

| Metric | Value |
|--------|-------|
| Total Tasks | 125 |
| Phase 1 Tasks | 15 (T-001 to T-015) |
| Phase 2 Tasks | 13 (T-016 to T-028) |
| Phase 3 Tasks | 17 (T-029 to T-045) |
| Phase 4 Tasks | 14 (T-046 to T-059) |
| Phase 5 Tasks | 9 (T-060 to T-068) |
| Phase 6 Tasks | 10 (T-069 to T-078) |
| Phase 7 Tasks | 12 (T-079 to T-090) |
| Phase 8 Tasks | 13 (T-091 to T-103) |
| Phase 9 Tasks | 13 (T-104 to T-116) |
| Phase 10 Tasks | 9 (T-117 to T-125) |
| Estimated Total Time | ~95 hours |

### Complexity Distribution

| Complexity | Count |
|------------|-------|
| Trivial | 22 |
| Low | 52 |
| Medium | 41 |
| High | 8 |
| Critical | 2 |

---

## Execution Notes

- **Strict Sequential Order:** Do not skip tasks. Each task's acceptance criteria must be fully met before proceeding.
- **Task Dependencies:** The Dependencies column lists explicit prerequisites. If empty, the task depends only on its immediate predecessor.
- **Rollback:** If any task fails, revert its Files Created and Files Modified to the state before that task. Re-execute the task.
- **Sign-Off:** After each task, the developer initials the Acceptance Criteria checklist. After each phase, the mentor reviews and signs off.
