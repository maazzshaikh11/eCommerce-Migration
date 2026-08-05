# Prompt Library

## Migration Project: Legacy ASP.NET MVC 5 → Vue.js 3 + .NET 10 Web API

### Usage
- Execute prompts in strict sequential order (T-001 → T-125).
- One prompt per task. No prompt continues to the next task automatically.
- After completing a prompt, STOP and wait for Orchestrator approval.
- The Orchestrator (MiniMax) verifies acceptance criteria before triggering the next prompt.

### Global Constraints (Apply to Every Prompt)
- Monorepo root: `C:\Users\Administrator\Desktop\eCommerce\`
- New code lives in `modern/` subdirectory; legacy code in `Ecommerce.*` folders is READ-ONLY reference.
- .NET 10 SDK required. Vue 3 + Vite + TypeScript for frontend.
- No code outside the scope of the current task.
- No skipping ahead. No speculative implementation.
- File-scoped namespaces, nullable enabled, async for all I/O in C#.
- `<script setup lang="ts">` for all Vue components.
- Native `fetch` for HTTP (no Axios).

---

## Prompt T-001: Create Repository Root Directory

**Context:** This is the first task of a migration project that moves a legacy ASP.NET MVC 5 eCommerce app to a modern Vue.js 3 + .NET 10 Web API stack using the Strangler Fig Pattern. All new code will live in a `modern/` directory within the existing repository at `C:\Users\Administrator\Desktop\eCommerce\`.

**Objective:** Create a `modern/` directory at the repository root to contain all new migration code.

**Current Project State:** The repository exists with legacy code in `Ecommerce.Web/`, `Ecommerce.Services/`, `Ecommerce.Data/`, `Ecommerce.Core/`. No `modern/` directory exists yet.

**Inputs:** Approved monorepo design document. Git repository initialized.

**Constraints:** Do not modify any existing files. Only create the `modern/` directory.

**Expected Output:** An empty `modern/` directory tracked by Git via a `.gitkeep` file.

**Acceptance Criteria:**
- [ ] `modern/` directory exists at `C:\Users\Administrator\Desktop\eCommerce\modern\`
- [ ] `modern/.gitkeep` file exists
- [ ] `git status` shows `modern/` as a new tracked path

**Stopping Condition:** After creating the directory and `.gitkeep`, STOP. Do not scaffold any projects. Wait for Orchestrator approval.

---

## Prompt T-002: Verify .NET 10 SDK Installation

**Context:** Before scaffolding .NET projects, the build environment must have .NET 10 SDK installed and accessible from the command line.

**Objective:** Confirm .NET 10 SDK is installed and the CLI is available.

**Current Project State:** `modern/` directory exists (T-001 complete).

**Inputs:** Development machine with PowerShell access.

**Constraints:** Do not install SDK if missing — report the gap and stop. Do not create any files.

**Expected Output:** Verified SDK version string from `dotnet --version`.

**Acceptance Criteria:**
- [ ] `dotnet --version` prints a version starting with `10.`
- [ ] `dotnet --list-sdks` shows at least one .NET 10 SDK entry

**Stopping Condition:** After running both commands and confirming output, STOP. Do not create any projects. Wait for Orchestrator approval.

---

## Prompt T-003: Create .NET Solution File

**Context:** A solution file is needed to organize all .NET projects in the monorepo.

**Objective:** Generate `Ecommerce.sln` in the `modern/` directory.

**Current Project State:** `modern/` directory exists. .NET 10 SDK verified (T-002 complete).

**Inputs:** `modern/` directory, .NET 10 CLI.

**Constraints:** Solution file must be at `modern/Ecommerce.sln`. Do not add any projects yet.

**Expected Output:** `modern/Ecommerce.sln` file.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.sln` exists
- [ ] `dotnet sln modern/Ecommerce.sln list` runs without error (empty solution)

**Stopping Condition:** After creating the solution file, STOP. Do not create projects. Wait for Orchestrator approval.

---

## Prompt T-004: Create .NET 10 Web API Project

**Context:** The API project is the entry point for the .NET 10 backend. It will host controllers, middleware, and YARP configuration.

**Objective:** Scaffold an ASP.NET Core 10 Web API project and add it to the solution.

**Current Project State:** `modern/Ecommerce.sln` exists (T-003 complete).

**Inputs:** Solution file, .NET 10 CLI.

**Constraints:** Use `dotnet new webapi` template. Target `net10.0`. Place in `modern/Ecommerce.Api/`.

**Expected Output:** A complete API project skeleton added to the solution.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Api/Ecommerce.Api.csproj` exists with `<TargetFramework>net10.0</TargetFramework>`
- [ ] `modern/Ecommerce.Api/Program.cs` exists
- [ ] `modern/Ecommerce.Api/appsettings.json` exists
- [ ] `dotnet sln modern/Ecommerce.sln list` shows `Ecommerce.Api`
- [ ] `dotnet build` from `modern/Ecommerce.Api/` succeeds with zero errors

**Stopping Condition:** After scaffolding and verifying build, STOP. Do not remove default controllers or add packages. Wait for Orchestrator approval.

---

## Prompt T-005: Remove Default WeatherForecastController

**Context:** The scaffolded API project includes a sample `WeatherForecastController` that must be removed to prepare a clean project structure.

**Objective:** Delete the default sample controller and its model class.

**Current Project State:** `Ecommerce.Api` project scaffolded with default sample code (T-004 complete).

**Inputs:** `modern/Ecommerce.Api/Controllers/WeatherForecastController.cs` (and `WeatherForecast.cs` if present).

**Constraints:** Do not modify `Program.cs` beyond removing controller-specific lines if necessary. Do not add new code.

**Expected Output:** Clean API project without sample controllers.

**Acceptance Criteria:**
- [ ] No `WeatherForecastController.cs` exists in the project
- [ ] No `WeatherForecast.cs` model exists
- [ ] `dotnet build` still succeeds

**Stopping Condition:** After deleting sample files and verifying build, STOP. Wait for Orchestrator approval.

---

## Prompt T-006: Add YARP NuGet Package to API Project

**Context:** YARP (Yet Another Reverse Proxy) will handle traffic routing between legacy MVC, the new API, and the Vue SPA.

**Objective:** Add the `Yarp.ReverseProxy` NuGet package to the API project.

**Current Project State:** Clean API project without sample code (T-005 complete).

**Inputs:** `modern/Ecommerce.Api/Ecommerce.Api.csproj`.

**Constraints:** Use the latest stable version of `Yarp.ReverseProxy`. Do not configure YARP yet — only add the package.

**Expected Output:** Updated `.csproj` with YARP package reference.

**Acceptance Criteria:**
- [ ] `Ecommerce.Api.csproj` contains `<PackageReference Include="Yarp.ReverseProxy" ... />`
- [ ] `dotnet restore` completes without package errors
- [ ] `dotnet build` succeeds

**Stopping Condition:** After adding package and verifying restore/build, STOP. Do not configure routes. Wait for Orchestrator approval.

---

## Prompt T-007: Add ReverseProxy Configuration Section to appsettings.json

**Context:** YARP requires a `ReverseProxy` configuration section defining clusters (destinations) and routes (path matching rules). This is an architectural routing decision.

**Objective:** Define three clusters (`apiCluster` → localhost:5001, `spaCluster` → localhost:5002, `legacyCluster` → localhost:5000) and three routes (`/api/{**catch-all}`, `/app/{**catch-all}`, `{**catch-all}` fallback).

**Current Project State:** YARP package added to API project (T-006 complete).

**Inputs:** `modern/Ecommerce.Api/appsettings.json`.

**Constraints:** JSON must be syntactically valid. Cluster destinations must use the exact ports specified. The fallback route must have lowest priority. Do not modify `Program.cs` in this task.

**Expected Output:** `appsettings.json` with a complete `ReverseProxy` section.

**Acceptance Criteria:**
- [ ] `appsettings.json` parses as valid JSON
- [ ] `ReverseProxy:Routes` contains three entries: `api`, `app`, `legacy`
- [ ] `ReverseProxy:Clusters` contains `apiCluster`, `spaCluster`, `legacyCluster`
- [ ] `apiCluster` destination address is `http://localhost:5001`
- [ ] `spaCluster` destination address is `http://localhost:5002`
- [ ] `legacyCluster` destination address is `http://localhost:5000`
- [ ] `api` route matches `/api/{**catch-all}`
- [ ] `app` route matches `/app/{**catch-all}`
- [ ] `legacy` route matches `{**catch-all}`

**Stopping Condition:** After writing the config and validating JSON, STOP. Do not wire YARP in Program.cs. Wait for Orchestrator approval.

---

## Prompt T-008: Register YARP Services and Middleware in Program.cs

**Context:** YARP must be registered in the ASP.NET Core service collection and mapped in the request pipeline.

**Objective:** Wire YARP into `Program.cs` by calling `AddReverseProxy().LoadFromConfig(...)` and `MapReverseProxy()`.

**Current Project State:** `appsettings.json` has YARP configuration (T-007 complete).

**Inputs:** `modern/Ecommerce.Api/Program.cs`, `modern/Ecommerce.Api/appsettings.json`.

**Constraints:** `MapReverseProxy()` must be placed before fallback routing. Do not add authentication or other middleware yet.

**Expected Output:** Modified `Program.cs` with YARP services and middleware registered.

**Acceptance Criteria:**
- [ ] `builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))` is present
- [ ] `app.MapReverseProxy()` is present
- [ ] `dotnet build` succeeds

**Stopping Condition:** After wiring YARP and verifying build, STOP. Do not run the project. Wait for Orchestrator approval.

---

## Prompt T-009: Verify YARP Starts Without Runtime Errors

**Context:** Before adding more projects, confirm the API starts and YARP loads its configuration without throwing exceptions.

**Objective:** Run the API project and verify YARP initializes correctly.

**Current Project State:** YARP wired in `Program.cs` (T-008 complete).

**Inputs:** `modern/Ecommerce.Api/` project.

**Constraints:** Stop the process after verification. Do not leave it running.

**Expected Output:** Console output showing successful YARP startup.

**Acceptance Criteria:**
- [ ] `dotnet run` starts without unhandled exceptions
- [ ] Console/log output shows YARP routes loaded
- [ ] Process listens on an HTTP port (check launchSettings.json for the port)

**Stopping Condition:** After verifying startup and stopping the process, STOP. Wait for Orchestrator approval.

---

## Prompt T-010: Create Shared Class Library Projects

**Context:** The API needs supporting class libraries for Core (models, DTOs, interfaces), Services (business logic), Data (repositories, DatabaseHelper), and Migrations (DB scripts).

**Objective:** Create four class library projects and add them to the solution.

**Current Project State:** API project running with YARP (T-009 complete).

**Inputs:** `modern/Ecommerce.sln`, .NET 10 CLI.

**Constraints:** Target `net10.0` for all projects. Do not wire project references yet (that's the next task). Place each in `modern/` root.

**Expected Output:** Four new `.csproj` files in the solution.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Api.Core/Ecommerce.Api.Core.csproj` exists
- [ ] `modern/Ecommerce.Api.Services/Ecommerce.Api.Services.csproj` exists
- [ ] `modern/Ecommerce.Api.Data/Ecommerce.Api.Data.csproj` exists
- [ ] `modern/Ecommerce.Api.Migrations/Ecommerce.Api.Migrations.csproj` exists
- [ ] `dotnet sln modern/Ecommerce.sln list` shows all five projects
- [ ] `dotnet build` from `modern/` succeeds

**Stopping Condition:** After creating all four projects and verifying build, STOP. Do not wire references. Wait for Orchestrator approval.

---

## Prompt T-011: Configure Central Package Management (CPM)

**Context:** Central Package Management allows all NuGet package versions to be controlled from a single file, ensuring consistency across projects.

**Objective:** Create `Directory.Packages.props` and enable CPM across the solution.

**Current Project State:** Five projects in solution (T-010 complete).

**Inputs:** All `.csproj` files in `modern/`.

**Constraints:** Move any existing package version to `Directory.Packages.props`. All `.csproj` files must use CPM format (no `Version` attribute on `PackageReference`; instead use `Version="$(YarpReverseProxyVersion)"` or enable CPM which auto-resolves).

**Expected Output:** `modern/Directory.Packages.props` file with centralized versions.

**Acceptance Criteria:**
- [ ] `modern/Directory.Packages.props` exists
- [ ] File contains `<Project>` with `<PropertyGroup>` for versions
- [ ] YARP package version is centralized
- [ ] `dotnet build` succeeds
- [ ] `dotnet list package` does not show per-project version overrides

**Stopping Condition:** After configuring CPM and verifying build, STOP. Wait for Orchestrator approval.

---

## Prompt T-012: Create Directory.Build.props

**Context:** Shared MSBuild properties ensure all .NET projects use the same compiler settings (nullable, language version, implicit usings).

**Objective:** Create `Directory.Build.props` with shared build settings.

**Current Project State:** CPM configured (T-011 complete).

**Inputs:** `modern/` directory.

**Constraints:** Do not override any project-specific settings. Include: `Nullable=enable`, `LangVersion=latest`, `ImplicitUsings=enable`.

**Expected Output:** `modern/Directory.Build.props` file.

**Acceptance Criteria:**
- [ ] File exists at `modern/Directory.Build.props`
- [ ] Contains `<Nullable>enable</Nullable>`
- [ ] Contains `<LangVersion>latest</LangVersion>`
- [ ] Contains `<ImplicitUsings>enable</ImplicitUsings>`
- [ ] `dotnet build` from `modern/` succeeds

**Stopping Condition:** After creating the file and verifying build, STOP. Wait for Orchestrator approval.

---

## Prompt T-013: Wire Inter-Project References

**Context:** Projects must reference each other following Clean Architecture: API → Services + Core; Services → Core + Data; Data → Core.

**Objective:** Add `ProjectReference` nodes to configure the dependency graph.

**Current Project State:** `Directory.Build.props` created (T-012 complete).

**Inputs:** All `.csproj` files.

**Constraints:** No circular references. API references Services and Core only. Services references Core and Data. Data references Core.

**Expected Output:** Updated `.csproj` files with correct references.

**Acceptance Criteria:**
- [ ] `Ecommerce.Api.csproj` references `Ecommerce.Api.Services` and `Ecommerce.Api.Core`
- [ ] `Ecommerce.Api.Services.csproj` references `Ecommerce.Api.Core` and `Ecommerce.Api.Data`
- [ ] `Ecommerce.Api.Data.csproj` references `Ecommerce.Api.Core`
- [ ] `dotnet build` from `modern/` succeeds
- [ ] No circular reference warnings or errors

**Stopping Condition:** After wiring references and verifying build, STOP. Wait for Orchestrator approval.

---

## Prompt T-014: Port DatabaseHelper from Legacy

**Context:** The legacy `DatabaseHelper.cs` wraps ADO.NET operations with `SqlConnection`, `SqlCommand`, and parameterized queries. It must be ported to use `Microsoft.Data.SqlClient` (the modern SQL client).

**Objective:** Copy `DatabaseHelper.cs` from `Ecommerce.Data/DatabaseHelper.cs` to `modern/Ecommerce.Api.Data/DatabaseHelper.cs`, replacing `System.Data.SqlClient` with `Microsoft.Data.SqlClient`.

**Current Project State:** Project references wired (T-013 complete).

**Inputs:** Legacy file at `Ecommerce.Data/DatabaseHelper.cs` (read-only reference).

**Constraints:** Preserve all method signatures and logic exactly. Only change the `using` directive and namespace. Add `Microsoft.Data.SqlClient` NuGet package to `Ecommerce.Api.Data.csproj`.

**Expected Output:** Ported `DatabaseHelper.cs` in new project.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Api.Data/DatabaseHelper.cs` exists
- [ ] No `using System.Data.SqlClient;` remains in the file
- [ ] `using Microsoft.Data.SqlClient;` is present
- [ ] `Ecommerce.Api.Data.csproj` references `Microsoft.Data.SqlClient` package
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting and verifying build, STOP. Do not test connectivity yet. Wait for Orchestrator approval.

---

## Prompt T-015: Create Temporary Health Check Controller

**Context:** Before proceeding, database connectivity must be verified at runtime using the ported `DatabaseHelper`.

**Objective:** Create a minimal `HealthController` with a `GET /api/health` endpoint that executes `SELECT 1` via `DatabaseHelper`.

**Current Project State:** `DatabaseHelper` ported (T-014 complete).

**Inputs:** `Ecommerce.Api.Data/DatabaseHelper.cs`, API project.

**Constraints:** Register `DatabaseHelper` in DI as scoped or singleton (match legacy pattern). The health endpoint should return 200 on success, 500 on failure. This controller is temporary but can remain.

**Expected Output:** `HealthController.cs` and updated `Program.cs` with DI registration.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Api/Controllers/HealthController.cs` exists
- [ ] `GET /api/health` returns HTTP 200 when database is accessible
- [ ] Endpoint executes `SELECT 1` via `DatabaseHelper`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating the controller and verifying the endpoint returns 200, STOP. This completes Phase 1. Wait for Orchestrator approval before Phase 2.

---

## Prompt T-016: Add JWT NuGet Package to API Project

**Context:** Phase 2 begins. JWT Bearer authentication is required for the API to validate tokens issued by the legacy MVC bridge.

**Objective:** Add `Microsoft.AspNetCore.Authentication.JwtBearer` to the API project.

**Current Project State:** Health controller verified (T-015 complete).

**Inputs:** `Ecommerce.Api.csproj`.

**Constraints:** Add only the package. Do not configure auth in `Program.cs` yet. Use CPM versioning.

**Expected Output:** Updated `.csproj` and `Directory.Packages.props`.

**Acceptance Criteria:**
- [ ] `Ecommerce.Api.csproj` contains `Microsoft.AspNetCore.Authentication.JwtBearer` reference
- [ ] `dotnet restore` succeeds
- [ ] `dotnet build` succeeds

**Stopping Condition:** After adding package and verifying build, STOP. Wait for Orchestrator approval.

---

## Prompt T-017: Implement JWT Token Service in Core Project

**Context:** A reusable service is needed to generate and validate JWTs with `sub`, `customer_id`, `email`, and `exp` claims using HMAC-SHA256.

**Objective:** Create `JwtTokenService` in `Ecommerce.Api.Core/Security/` that generates and validates JWTs.

**Current Project State:** JWT package added to API (T-016 complete).

**Inputs:** Signing key from configuration (32+ bytes). `Ecommerce.Api.Core` project.

**Constraints:** Add `System.IdentityModel.Tokens.Jwt` and `Microsoft.IdentityModel.Tokens` packages to `Ecommerce.Api.Core.csproj`. Use symmetric key. Token expiry: 1 hour. Include a unit test that validates the token.

**Expected Output:** `IJwtTokenService.cs`, `JwtTokenService.cs`, and a unit test file.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Api.Core/Security/IJwtTokenService.cs` exists
- [ ] `modern/Ecommerce.Api.Core/Security/JwtTokenService.cs` exists
- [ ] `GenerateToken(userId, customerId, email)` returns non-null JWT string
- [ ] Token contains `sub`, `customer_id`, `email`, `exp` claims
- [ ] Unit test validates token payload
- [ ] `dotnet test` passes

**Stopping Condition:** After implementing service and passing unit test, STOP. Do not configure auth middleware. Wait for Orchestrator approval.

---

## Prompt T-018: Configure JWT Authentication in Program.cs

**Context:** The API must validate JWTs on protected endpoints using the `JwtBearer` authentication scheme.

**Objective:** Register JWT Bearer authentication in the service collection and add authentication/authorization middleware to the pipeline.

**Current Project State:** `JwtTokenService` implemented (T-017 complete).

**Inputs:** `Program.cs`, signing key from `appsettings.json`.

**Constraints:** `TokenValidationParameters` must validate issuer signing key and lifetime. Set `ClockSkew` to `TimeSpan.Zero`. `UseAuthentication()` must come before `UseAuthorization()`. Add a test `[Authorize]` endpoint to verify.

**Expected Output:** Modified `Program.cs` with JWT middleware.

**Acceptance Criteria:**
- [ ] `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` is present
- [ ] `app.UseAuthentication()` is present
- [ ] `app.UseAuthorization()` is present after `UseAuthentication()`
- [ ] A test `[Authorize]` endpoint returns 401 without token
- [ ] Same endpoint returns 200 with valid Bearer token
- [ ] `dotnet build` succeeds

**Stopping Condition:** After configuring and verifying with test endpoint, STOP. Wait for Orchestrator approval.

---

## Prompt T-019: Add JWT Signing Key to appsettings.json

**Context:** The JWT signing key must be stored in configuration files for both development and production environments.

**Objective:** Add a 32-byte base64-encoded symmetric signing key to `appsettings.json` and `appsettings.Development.json`.

**Current Project State:** JWT auth configured in `Program.cs` (T-018 complete).

**Inputs:** `appsettings.json`, `appsettings.Development.json`.

**Constraints:** Key must be at least 32 bytes (256 bits). Use a placeholder key for development. Do not commit production keys.

**Expected Output:** Updated config files with `JwtSigningKey` property.

**Acceptance Criteria:**
- [ ] `appsettings.json` contains `"JwtSigningKey": "..."` (at least 44 chars base64)
- [ ] `appsettings.Development.json` contains same or development-specific key
- [ ] API reads key without exception on startup
- [ ] `dotnet build` succeeds

**Stopping Condition:** After adding key and verifying startup, STOP. Wait for Orchestrator approval.

---

## Prompt T-020: Add JWT Bridge Endpoint to Legacy MVC

**Context:** During the hybrid phase, the legacy MVC app (with cookie auth) must issue JWTs that the new API can validate. This endpoint bridges cookie auth to JWT auth.

**Objective:** Add an `IssueToken()` action to legacy `AccountController` that reads the cookie-authenticated user, resolves `CustomerId`, generates a JWT, and returns it as JSON.

**Current Project State:** JWT signing key configured (T-019 complete). Legacy repo at `C:\Users\Administrator\Desktop\eCommerce\Ecommerce.Web\`.

**Inputs:** Legacy `AccountController.cs`, `ApplicationUser` with `CustomerId` claim, same signing key as T-019.

**Constraints:** Add `System.IdentityModel.Tokens.Jwt` NuGet package to legacy project. Action must be `[Authorize]` and `[HttpGet]`. Return `Json(new { token }, JsonRequestBehavior.AllowGet)`. Use the same signing key as the new API. Anonymous users must get 302 redirect to login.

**Expected Output:** Modified `AccountController.cs` in legacy project.

**Acceptance Criteria:**
- [ ] `AccountController.IssueToken()` action exists with `[Authorize]` and `[HttpGet]`
- [ ] Logged-in user visiting `/Account/IssueToken` receives JSON `{ "token": "..." }`
- [ ] Anonymous user gets 302 redirect to `/Account/Login`
- [ ] Token validates successfully using the same key configured in T-019
- [ ] Legacy project builds successfully

**Stopping Condition:** After modifying legacy controller and verifying token issuance, STOP. Wait for Orchestrator approval.

---

## Prompt T-021: Verify Node.js Installation for Vue SPA

**Context:** The Vue SPA requires Node.js 20+ and npm 10+ for Vite scaffolding and development.

**Objective:** Confirm Node.js and npm are installed and meet version requirements.

**Current Project State:** Legacy JWT bridge complete (T-020 complete).

**Inputs:** Development machine.

**Constraints:** Do not install Node.js if missing — report the gap and stop.

**Expected Output:** Verified version strings.

**Acceptance Criteria:**
- [ ] `node --version` prints v20.x or higher
- [ ] `npm --version` prints 10.x or higher

**Stopping Condition:** After running version checks, STOP. Do not scaffold the SPA. Wait for Orchestrator approval.

---

## Prompt T-022: Scaffold Vue SPA with Vite

**Context:** The frontend is a Vue 3 SPA built with Vite, using TypeScript, Pinia, and Vue Router.

**Objective:** Scaffold a Vite + Vue 3 + TypeScript project in `modern/Ecommerce.Spa/`.

**Current Project State:** Node.js verified (T-021 complete).

**Inputs:** Node.js 20+, npm 10+.

**Constraints:** Use `npm create vue@latest` or `npm init vite@latest` with `vue-ts` template. Place in `modern/Ecommerce.Spa/`. Do not install Pinia or Vue Router yet.

**Expected Output:** Complete Vite scaffold folder.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Spa/package.json` exists with `vue`, `vite`, `typescript` dependencies
- [ ] `modern/Ecommerce.Spa/vite.config.ts` exists
- [ ] `modern/Ecommerce.Spa/tsconfig.json` exists
- [ ] `npm install` completes with zero critical vulnerabilities
- [ ] `npm run dev` starts dev server
- [ ] Browser displays default Vite + Vue welcome page

**Stopping Condition:** After scaffolding and verifying dev server, STOP. Do not add dependencies. Wait for Orchestrator approval.

---

## Prompt T-023: Install Pinia and Vue Router

**Context:** State management (Pinia) and client-side routing (Vue Router) are required for the SPA.

**Objective:** Add `pinia` and `vue-router` as dependencies.

**Current Project State:** Vite scaffold complete (T-022 complete).

**Inputs:** `modern/Ecommerce.Spa/package.json`.

**Constraints:** Install latest stable versions. Do not configure stores or routes yet.

**Expected Output:** Updated `package.json` and `package-lock.json`.

**Acceptance Criteria:**
- [ ] `pinia` listed in `dependencies` in `package.json`
- [ ] `vue-router` listed in `dependencies` in `package.json`
- [ ] `npm install` succeeds
- [ ] `npm run dev` starts without errors

**Stopping Condition:** After installing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-024: Configure API Base URL in Environment File

**Context:** The SPA needs to know the API base URL for local development.

**Objective:** Create `.env.development` with `VITE_API_BASE_URL`.

**Current Project State:** Pinia and Vue Router installed (T-023 complete).

**Inputs:** API runs on `http://localhost:5001`.

**Constraints:** Use Vite's environment variable convention (`VITE_` prefix). Do not create `.env.production` yet.

**Expected Output:** `modern/Ecommerce.Spa/.env.development` file.

**Acceptance Criteria:**
- [ ] `.env.development` file exists
- [ ] Contains `VITE_API_BASE_URL=http://localhost:5001/api`
- [ ] `import.meta.env.VITE_API_BASE_URL` resolves correctly in a test component

**Stopping Condition:** After creating the file and verifying resolution, STOP. Wait for Orchestrator approval.

---

## Prompt T-025: Implement Fetch Wrapper with JWT Attachment

**Context:** All API calls from the SPA must include the JWT Bearer token. A central `fetch` wrapper ensures consistent header attachment and error handling.

**Objective:** Create `client.ts` that exports `get`, `post`, `put`, `del` functions using native `fetch`.

**Current Project State:** API base URL configured (T-024 complete).

**Inputs:** `VITE_API_BASE_URL` from environment.

**Constraints:** Use native `fetch` (no Axios). Functions must accept a token parameter for the Bearer header. Return typed generics. Throw on non-OK responses.

**Expected Output:** `modern/Ecommerce.Spa/src/api/client.ts`.

**Acceptance Criteria:**
- [ ] File exists at `src/api/client.ts`
- [ ] Exports `get<T>`, `post<T>`, `put<T>`, `del` functions
- [ ] Functions accept `(path: string, token?: string, body?: unknown)`
- [ ] Request includes `Authorization: Bearer <token>` when token provided
- [ ] Response body returned as typed `T`
- [ ] Non-OK responses throw an error

**Stopping Condition:** After creating the wrapper, STOP. Do not create stores. Wait for Orchestrator approval.

---

## Prompt T-026: Create Pinia Auth Store

**Context:** The SPA needs to manage JWT lifecycle: fetch from legacy bridge, persist to localStorage, provide `isAuthenticated` computed.

**Objective:** Implement `useAuthStore` in `src/stores/useAuthStore.ts`.

**Current Project State:** Fetch wrapper created (T-025 complete). Legacy bridge endpoint at `/Account/IssueToken` (T-020 complete).

**Inputs:** Legacy bridge endpoint URL, `localStorage`.

**Constraints:** Use Composition API style (`defineStore` with setup function). Token persisted to `localStorage` under key `jwt`. `fetchToken()` calls `/Account/IssueToken` via the proxy (no direct legacy URL). `logout()` clears token and localStorage. Register Pinia in `main.ts`.

**Expected Output:** `useAuthStore.ts` and modified `main.ts`.

**Acceptance Criteria:**
- [ ] `src/stores/useAuthStore.ts` exists
- [ ] Store initializes with token from `localStorage`
- [ ] `fetchToken()` calls `/Account/IssueToken` and stores result
- [ ] `isAuthenticated` computed returns `true` when token exists
- [ ] `logout()` clears token and `localStorage`
- [ ] `main.ts` registers Pinia with `app.use(createPinia())`

**Stopping Condition:** After creating store and registering Pinia, STOP. Wait for Orchestrator approval.

---

## Prompt T-027: Configure Vue Router Skeleton

**Context:** Client-side routing must be set up with history mode before adding page routes.

**Objective:** Create `src/router/index.ts` with `createRouter` and `createWebHistory`.

**Current Project State:** Pinia auth store created (T-026 complete).

**Inputs:** Vue Router installed (T-023).

**Constraints:** Use `createWebHistory()`. Add one placeholder route (`/` → a temporary `HomeView` or `App` component). Register router in `main.ts`.

**Expected Output:** `src/router/index.ts` and modified `main.ts`.

**Acceptance Criteria:**
- [ ] `src/router/index.ts` exists
- [ ] Uses `createRouter` with `createWebHistory()`
- [ ] At least one route defined
- [ ] `main.ts` registers router with `app.use(router)`
- [ ] Navigation between routes works in browser

**Stopping Condition:** After configuring router and verifying navigation, STOP. Wait for Orchestrator approval.

---

## Prompt T-028: Verify Full Proxy Chain End-to-End

**Context:** All four services must run simultaneously and traffic must flow correctly through YARP to verify the complete infrastructure.

**Objective:** Start Legacy MVC (5000), API (5001), SPA (5002), YARP (5003) and verify the full chain.

**Current Project State:** Vue router configured (T-027 complete). All infrastructure code written.

**Inputs:** All running services.

**Constraints:** Verify each link in the chain independently before testing the full chain. Stop all services after verification.

**Expected Output:** Verified running system with documented evidence.

**Acceptance Criteria:**
- [ ] Legacy MVC serves pages on port 5000
- [ ] API responds on port 5001 (`GET /api/health` returns 200)
- [ ] SPA dev server runs on port 5002
- [ ] YARP runs on port 5003
- [ ] `http://localhost:5003/app/` loads Vue app
- [ ] Auth store fetches JWT from `/Account/IssueToken` via YARP
- [ ] Authenticated API call (`GET /api/health` with Bearer token) returns 200
- [ ] All services stopped after verification

**Stopping Condition:** After verifying all criteria and stopping services, STOP. This completes Phase 2. Wait for Orchestrator approval before Phase 3.

---

## Prompt T-029: Port Product Domain Model

**Context:** Phase 3 begins. The Product entity is the core domain model for the product catalog vertical slice.

**Objective:** Copy `Product.cs` from legacy `Ecommerce.Core/Models/` to `modern/Ecommerce.Api.Core/Models/Product.cs`.

**Current Project State:** Phase 2 complete (T-028 passed). All infrastructure running.

**Inputs:** Legacy file at `Ecommerce.Core/Models/Product.cs` (read-only reference).

**Constraints:** Preserve all properties. Remove any EF-specific attributes (`[Key]`, `[Table]`, etc.). Change namespace to `Ecommerce.Api.Core.Models`. No dependencies on other projects.

**Expected Output:** `Product.cs` in new project.

**Acceptance Criteria:**
- [ ] `modern/Ecommerce.Api.Core/Models/Product.cs` exists
- [ ] Namespace is `Ecommerce.Api.Core.Models`
- [ ] No EF attributes or `using System.ComponentModel.DataAnnotations` / `System.ComponentModel.DataAnnotations.Schema`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting and verifying build, STOP. Wait for Orchestrator approval.

---

## Prompt T-030: Port Category Domain Model

**Context:** The Category entity is needed for product filtering and navigation.

**Objective:** Copy `Category.cs` from legacy to `modern/Ecommerce.Api.Core/Models/Category.cs`.

**Current Project State:** Product model ported (T-029 complete).

**Inputs:** Legacy `Ecommerce.Core/Models/Category.cs`.

**Constraints:** Remove EF attributes. Change namespace. No circular reference to `Product` (break the cycle — Category should not have a `Products` navigation collection if it creates circular DTO issues).

**Expected Output:** `Category.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists with namespace `Ecommerce.Api.Core.Models`
- [ ] No EF attributes
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-031: Port ProductImage and ProductVariant Models

**Context:** Product detail page requires images and variant data.

**Objective:** Copy `ProductImage.cs` and `ProductVariant.cs` from legacy to new project.

**Current Project State:** Category model ported (T-030 complete).

**Inputs:** Legacy `Ecommerce.Core/Models/ProductImage.cs` and `ProductVariant.cs`.

**Constraints:** Remove EF attributes. Change namespace.

**Expected Output:** Two model files in new project.

**Acceptance Criteria:**
- [ ] Both files exist in `Ecommerce.Api.Core/Models/`
- [ ] Namespaces updated
- [ ] No EF attributes
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting both files, STOP. Wait for Orchestrator approval.

---

## Prompt T-032: Create ProductListResponseDto

**Context:** The product listing endpoint needs a DTO that includes paged items, categories, and filter state.

**Objective:** Define `ProductListResponseDto` record in `Ecommerce.Api.Core/DTOs/`.

**Current Project State:** All product-related models ported (T-031 complete).

**Inputs:** Domain models: `Product`, `Category`.

**Constraints:** Use C# `record` type. Include: `Products` (IEnumerable of ProductDto), `Categories` (IEnumerable of CategoryDto), `SelectedCategoryId` (int?), `SortOrder` (string), `SearchQuery` (string), `PageNumber` (int), `TotalPages` (int). Also create `ProductDto` record (Id, Name, Price, ImageUrl, CategoryName).

**Expected Output:** `ProductListResponseDto.cs` and `ProductDto.cs`.

**Acceptance Criteria:**
- [ ] `ProductListResponseDto` record exists with all specified fields
- [ ] `ProductDto` record exists with Id, Name, Price, ImageUrl, CategoryName
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating DTOs, STOP. Wait for Orchestrator approval.

---

## Prompt T-033: Create ProductDetailDto

**Context:** The product detail endpoint needs a DTO with nested collections for variants, images, and related products.

**Objective:** Define `ProductDetailDto` record.

**Current Project State:** `ProductListResponseDto` and `ProductDto` created (T-032 complete).

**Inputs:** Domain models: `Product`, `ProductImage`, `ProductVariant`.

**Constraints:** Use record type. Include: Id, Name, Description, Price, Images (IEnumerable of ImageDto), Variants (IEnumerable of VariantDto), RelatedProducts (IEnumerable of ProductDto). No circular references. Create `ImageDto` and `VariantDto` as well.

**Expected Output:** `ProductDetailDto.cs`, `ImageDto.cs`, `VariantDto.cs`.

**Acceptance Criteria:**
- [ ] `ProductDetailDto` record exists with all fields
- [ ] `ImageDto` and `VariantDto` records exist
- [ ] No circular references
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating DTOs, STOP. Wait for Orchestrator approval.

---

## Prompt T-034: Create CategoryDto and PagedResult

**Context:** Reusable DTOs for categories and generic pagination are needed across multiple endpoints.

**Objective:** Define `CategoryDto` and generic `PagedResult<T>`.

**Current Project State:** Product DTOs created (T-033 complete).

**Inputs:** `Category` domain model.

**Constraints:** `CategoryDto`: Id, Name, Description, ProductCount. `PagedResult<T>`: Items (IEnumerable<T>), TotalCount (int), Page (int), PageSize (int).

**Expected Output:** `CategoryDto.cs` and `PagedResult.cs`.

**Acceptance Criteria:**
- [ ] `CategoryDto` record exists
- [ ] `PagedResult<T>` generic record exists
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating DTOs, STOP. Wait for Orchestrator approval.

---

## Prompt T-035: Port IProductRepository Interface

**Context:** The product repository interface defines the data access contract.

**Objective:** Copy `IProductRepository` from legacy `Ecommerce.Core/Interfaces/` to new project.

**Current Project State:** All DTOs created (T-034 complete).

**Inputs:** Legacy `Ecommerce.Core/Interfaces/IProductRepository.cs`.

**Constraints:** Change namespace to `Ecommerce.Api.Core.Interfaces`. Methods must use domain models (not ViewModels). If legacy returns ViewModels, change return types to domain models or PagedResult.

**Expected Output:** `IProductRepository.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Core/Interfaces/`
- [ ] Namespace is `Ecommerce.Api.Core.Interfaces`
- [ ] Methods use domain models
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-036: Port ICategoryRepository Interface

**Context:** Category repository interface needed for category data access.

**Objective:** Copy `ICategoryRepository` from legacy to new project.

**Current Project State:** `IProductRepository` ported (T-035 complete).

**Inputs:** Legacy `Ecommerce.Core/Interfaces/ICategoryRepository.cs`.

**Constraints:** Same as T-035.

**Expected Output:** `ICategoryRepository.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Core/Interfaces/`
- [ ] Namespace updated
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-037: Port ProductRepository Implementation

**Context:** The repository implementation contains ADO.NET data access logic.

**Objective:** Copy `ProductRepository` from legacy `Ecommerce.Data/Repositories/` to `modern/Ecommerce.Api.Data/Repositories/`, replacing SQL client.

**Current Project State:** `IProductRepository` ported (T-036 complete).

**Inputs:** Legacy `Ecommerce.Data/Repositories/ProductRepository.cs`.

**Constraints:** Replace `System.Data.SqlClient` with `Microsoft.Data.SqlClient`. Change namespace to `Ecommerce.Api.Data.Repositories`. Preserve all SQL queries and logic.

**Expected Output:** `ProductRepository.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Data/Repositories/`
- [ ] Uses `Microsoft.Data.SqlClient`
- [ ] Namespace is `Ecommerce.Api.Data.Repositories`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-038: Port CategoryRepository Implementation

**Context:** Category repository implementation needed for category data.

**Objective:** Copy `CategoryRepository` from legacy to new project with SQL client swap.

**Current Project State:** `ProductRepository` ported (T-037 complete).

**Inputs:** Legacy `Ecommerce.Data/Repositories/CategoryRepository.cs`.

**Constraints:** Same as T-037.

**Expected Output:** `CategoryRepository.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Data/Repositories/`
- [ ] Uses `Microsoft.Data.SqlClient`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-039: Port IProductService Interface

**Context:** The product service interface defines business logic contract.

**Objective:** Copy `IProductService` from legacy `Ecommerce.Core/Interfaces/` to new project.

**Current Project State:** Repositories ported (T-038 complete).

**Inputs:** Legacy `Ecommerce.Core/Interfaces/IProductService.cs`.

**Constraints:** Change namespace to `Ecommerce.Api.Services.Interfaces`. Methods should return domain models or DTOs, not ViewModels.

**Expected Output:** `IProductService.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Interfaces/`
- [ ] Namespace updated
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-040: Port ICategoryService Interface

**Context:** Category service interface needed.

**Objective:** Copy `ICategoryService` from legacy to new project.

**Current Project State:** `IProductService` ported (T-039 complete).

**Inputs:** Legacy `Ecommerce.Core/Interfaces/ICategoryService.cs`.

**Constraints:** Same as T-039.

**Expected Output:** `ICategoryService.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Interfaces/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-041: Port ProductService Implementation

**Context:** The ProductService contains business logic for product listing and filtering.

**Objective:** Copy `ProductService` from legacy `Ecommerce.Services/` to new project. Change namespace and update return types to DTOs.

**Current Project State:** Service interfaces ported (T-040 complete).

**Inputs:** Legacy `Ecommerce.Services/Services/ProductService.cs`.

**Constraints:** Change namespace to `Ecommerce.Api.Services.Implementations`. Remove ViewModel references. Map domain models to DTOs at return points. Preserve all business logic (filtering, sorting, pagination calculations).

**Expected Output:** `ProductService.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Implementations/`
- [ ] No `using` references to Razor or ViewModels
- [ ] Returns DTOs or domain models
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-042: Port CategoryService Implementation

**Context:** CategoryService contains business logic for category retrieval.

**Objective:** Copy `CategoryService` from legacy to new project.

**Current Project State:** `ProductService` ported (T-041 complete).

**Inputs:** Legacy `Ecommerce.Services/Services/CategoryService.cs`.

**Constraints:** Same as T-041.

**Expected Output:** `CategoryService.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Implementations/`
- [ ] No ViewModel references
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-043: Register Product and Category Services in DI

**Context:** Services and repositories must be registered in the DI container.

**Objective:** Add scoped DI registrations for all ported services and repositories in `Program.cs`.

**Current Project State:** All services ported (T-042 complete).

**Inputs:** `Program.cs`, ported service and repository classes.

**Constraints:** Use `AddScoped` for all. Register `DatabaseHelper` as well (singleton or scoped, matching legacy pattern).

**Expected Output:** Modified `Program.cs`.

**Acceptance Criteria:**
- [ ] `AddScoped<IProductRepository, ProductRepository>()` present
- [ ] `AddScoped<IProductService, ProductService>()` present
- [ ] `AddScoped<ICategoryRepository, CategoryRepository>()` present
- [ ] `AddScoped<ICategoryService, CategoryService>()` present
- [ ] `DatabaseHelper` registered in DI
- [ ] API starts without DI exceptions

**Stopping Condition:** After registering and verifying startup, STOP. Wait for Orchestrator approval.

---

## Prompt T-044: Implement ProductsController with GET Endpoints

**Context:** The ProductsController exposes product listing and detail endpoints.

**Objective:** Create `ProductsController` with `GET /api/products` and `GET /api/products/{id}`.

**Current Project State:** DI configured (T-043 complete).

**Inputs:** `IProductService`, `ICategoryService`, DTOs.

**Constraints:** Use `[ApiController]` and `[Route("api/products")]`. `GET /api/products` accepts query params: `categoryId`, `sortOrder`, `searchQuery`, `page`. Returns `ProductListResponseDto`. `GET /api/products/{id}` returns `ProductDetailDto` or 404. Controller must be thin — delegate to service.

**Expected Output:** `ProductsController.cs`.

**Acceptance Criteria:**
- [ ] `GET /api/products` returns `ProductListResponseDto` with correct shape
- [ ] `GET /api/products/{id}` returns `ProductDetailDto` for valid ID
- [ ] `GET /api/products/{id}` returns 404 for invalid ID
- [ ] Query params (categoryId, sortOrder, searchQuery, page) are accepted and passed to service
- [ ] `dotnet build` succeeds

**Stopping Condition:** After implementing and verifying with manual API call, STOP. Wait for Orchestrator approval.

---

## Prompt T-045: Implement CategoriesController with GET Endpoint

**Context:** Categories endpoint for category tree navigation.

**Objective:** Create `CategoriesController` with `GET /api/categories`.

**Current Project State:** `ProductsController` implemented (T-044 complete).

**Inputs:** `ICategoryService`.

**Constraints:** Use `[ApiController]` and `[Route("api/categories")]`. Returns `CategoryDto[]`.

**Expected Output:** `CategoriesController.cs`.

**Acceptance Criteria:**
- [ ] `GET /api/categories` returns `CategoryDto[]`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After implementing and verifying, STOP. This completes Phase 3. Wait for Orchestrator approval before Phase 4.

---

## Prompt T-046: Add YARP Route for Product Pages

**Context:** Phase 4 begins. YARP must intercept product page traffic and route it to the Vue SPA.

**Objective:** Add `/products*` and `/product/*` routes to YARP config pointing to `spaCluster`.

**Current Project State:** Category controller implemented (T-045 complete).

**Inputs:** `appsettings.json` YARP config.

**Constraints:** Do not remove existing routes. Add new routes with higher priority than the legacy fallback.

**Expected Output:** Updated `appsettings.json`.

**Acceptance Criteria:**
- [ ] `/products*` route added pointing to `spaCluster`
- [ ] `/product/*` route added pointing to `spaCluster`
- [ ] `http://localhost:5003/products` serves Vue SPA (not legacy MVC)
- [ ] Existing routes still work

**Stopping Condition:** After adding routes and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-047: Create ProductCard Vue Component

**Context:** A reusable card component for displaying products in grids.

**Objective:** Build `ProductCard.vue` component that displays product image, name, price, and category.

**Current Project State:** YARP product routes configured (T-046 complete).

**Inputs:** `ProductDto` TypeScript type.

**Constraints:** Use `<script setup lang="ts">`. Accept `product: ProductDto` prop. Emit `add-to-cart` event with product ID on button click. Emit `click` event on card click.

**Expected Output:** `ProductCard.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/components/ProductCard.vue`
- [ ] Uses `<script setup lang="ts">`
- [ ] Accepts `product` prop
- [ ] Renders product name, price, image
- [ ] Emits `add-to-cart` on button click
- [ ] Emits `click` on card click

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-048: Create CategoryFilter Vue Component

**Context:** Sidebar component for category filtering in product listing.

**Objective:** Build `CategoryFilter.vue` that renders category list and emits selected category ID.

**Current Project State:** `ProductCard` created (T-047 complete).

**Inputs:** `CategoryDto` TypeScript type.

**Constraints:** Use `<script setup lang="ts">`. Accept `categories: CategoryDto[]` prop. Emit `select` event with category ID (or null for "all"). Highlight selected category.

**Expected Output:** `CategoryFilter.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/components/CategoryFilter.vue`
- [ ] Renders category list
- [ ] Emits `select` with category ID on click
- [ ] Highlights selected category

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-049: Create PaginationControls Vue Component

**Context:** Reusable pagination bar for product listing and order history.

**Objective:** Build `PaginationControls.vue`.

**Current Project State:** `CategoryFilter` created (T-048 complete).

**Inputs:** None.

**Constraints:** Use `<script setup lang="ts">`. Props: `currentPage: number`, `totalPages: number`. Emit `change-page` with new page number. Disable Previous/Next at boundaries.

**Expected Output:** `PaginationControls.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/components/PaginationControls.vue`
- [ ] Renders page numbers for given `currentPage` and `totalPages`
- [ ] Emits `change-page` with correct page number
- [ ] Previous disabled on page 1; Next disabled on last page

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-050: Create LoadingSpinner and ErrorAlert Components

**Context:** Utility components for async state and error display.

**Objective:** Build `LoadingSpinner.vue` and `ErrorAlert.vue`.

**Current Project State:** `PaginationControls` created (T-049 complete).

**Inputs:** None.

**Constraints:** `LoadingSpinner`: simple spinner, accepts optional `message` prop. `ErrorAlert`: displays error message, accepts `message: string` prop, optional `dismissable` boolean.

**Expected Output:** Two component files.

**Acceptance Criteria:**
- [ ] `LoadingSpinner.vue` exists and renders spinner
- [ ] `ErrorAlert.vue` exists and renders error message
- [ ] Both use `<script setup lang="ts">`

**Stopping Condition:** After creating both components, STOP. Wait for Orchestrator approval.

---

## Prompt T-051: Create Pinia Product Store

**Context:** Product state management for the SPA.

**Objective:** Implement `useProductStore` in `src/stores/useProductStore.ts`.

**Current Project State:** Utility components created (T-050 complete).

**Inputs:** `client.ts` API wrapper, `ProductDto` and `CategoryDto` types.

**Constraints:** Use Composition API store. State: `products`, `categories`, `filters` (categoryId, sortOrder, searchQuery), `pagination` (page, pageSize, totalCount). Actions: `fetchProducts()` calls `GET /api/products` with query params. `fetchCategories()` calls `GET /api/categories`. Changing filters triggers refetch.

**Expected Output:** `useProductStore.ts`.

**Acceptance Criteria:**
- [ ] Store exists at `src/stores/useProductStore.ts`
- [ ] Contains `products`, `categories`, `filters`, `pagination` state
- [ ] `fetchProducts()` calls API with correct query params
- [ ] `fetchCategories()` calls API
- [ ] Store is reactive (changing filters triggers refetch in component)

**Stopping Condition:** After creating store, STOP. Wait for Orchestrator approval.

---

## Prompt T-052: Create ProductListView Vue Page

**Context:** The main product listing page composing filter, grid, and pagination.

**Objective:** Build `ProductListView.vue` that fetches and displays products.

**Current Project State:** Product store created (T-051 complete).

**Inputs:** `useProductStore`, `ProductCard`, `CategoryFilter`, `PaginationControls`, `LoadingSpinner`, `ErrorAlert`.

**Constraints:** Use `<script setup lang="ts">`. Fetch products on mount. Update URL query params when filters change. Read URL query params on mount to initialize filters. Show loading spinner during fetch. Show error alert on failure.

**Expected Output:** `ProductListView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/ProductListView.vue`
- [ ] Renders product grid using `ProductCard`
- [ ] Category filter works and updates URL
- [ ] Pagination works and updates URL
- [ ] Loading spinner shows during fetch
- [ ] Error alert shows on API failure
- [ ] Real data from API renders correctly

**Stopping Condition:** After creating and verifying with real data, STOP. Wait for Orchestrator approval.

---

## Prompt T-053: Create ProductDetailView Vue Page

**Context:** Product detail page with gallery, variants, and related products.

**Objective:** Build `ProductDetailView.vue` that fetches a product by ID.

**Current Project State:** `ProductListView` created (T-052 complete).

**Inputs:** `client.ts`, `ProductDetailDto` type, `ProductCard` (for related products).

**Constraints:** Use `<script setup lang="ts">`. Fetch product by route param `id`. Display gallery, variants, description, price. Show related products using `ProductCard`. Show loading and error states.

**Expected Output:** `ProductDetailView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/ProductDetailView.vue`
- [ ] Fetches product by `id` from route param
- [ ] Displays gallery, variants, description, price
- [ ] Displays related products
- [ ] Loading and error states handled
- [ ] Visiting `/products/123` renders product 123

**Stopping Condition:** After creating and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-054: Configure Vue Router for Product Pages

**Context:** Routes must be configured for product listing and detail pages.

**Objective:** Add `/products` and `/products/:id` routes to Vue Router.

**Current Project State:** `ProductDetailView` created (T-053 complete).

**Inputs:** `router/index.ts`, `ProductListView`, `ProductDetailView`.

**Constraints:** Use lazy loading (`() => import(...)`). `/products` → `ProductListView`. `/products/:id` → `ProductDetailView`.

**Expected Output:** Updated `router/index.ts`.

**Acceptance Criteria:**
- [ ] `/products` route maps to `ProductListView`
- [ ] `/products/:id` route maps to `ProductDetailView`
- [ ] Lazy loading used
- [ ] Browser back/forward preserves filter state
- [ ] Navigation works in browser

**Stopping Condition:** After configuring and verifying navigation, STOP. Wait for Orchestrator approval.

---

## Prompt T-055: Write Backend Unit Tests for ProductService

**Context:** Unit tests verify product business logic in isolation.

**Objective:** Create xUnit tests for `ProductService` mocking `IUnitOfWork` and `IProductRepository`.

**Current Project State:** Product routes configured (T-054 complete).

**Inputs:** `ProductService`, `IProductRepository`, Moq.

**Constraints:** Create test project `Ecommerce.Api.Services.Tests` if it doesn't exist. Use xUnit + Moq. Test: pagination logic, category filtering, search query, sorting.

**Expected Output:** `ProductServiceTests.cs`.

**Acceptance Criteria:**
- [ ] Test file exists in test project
- [ ] Tests cover pagination calculation
- [ ] Tests cover category filtering
- [ ] Tests cover search query
- [ ] `dotnet test` passes with all tests green

**Stopping Condition:** After writing tests and verifying pass, STOP. Wait for Orchestrator approval.

---

## Prompt T-056: Write Backend Unit Tests for ProductsController

**Context:** Controller tests verify HTTP response behavior.

**Objective:** Create xUnit tests for `ProductsController` mocking `IProductService`.

**Current Project State:** Service tests pass (T-055 complete).

**Inputs:** `ProductsController`, `IProductService`, Moq.

**Constraints:** Create test project `Ecommerce.Api.Tests` if it doesn't exist. Use xUnit + Moq. Test: 200 for valid request, 404 for invalid product ID, correct DTO shape returned.

**Expected Output:** `ProductsControllerTests.cs`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Tests verify 200 response for valid product list request
- [ ] Tests verify 404 for missing product ID
- [ ] Tests verify correct DTO shape
- [ ] `dotnet test` passes

**Stopping Condition:** After writing tests and verifying pass, STOP. Wait for Orchestrator approval.

---

## Prompt T-057: Write Frontend Unit Tests for ProductCard Component

**Context:** Component tests verify rendering and event emission.

**Objective:** Create Vitest tests for `ProductCard.vue`.

**Current Project State:** Backend controller tests pass (T-056 complete).

**Inputs:** `ProductCard.vue`, `@vue/test-utils`.

**Constraints:** Use Vitest + Vue Test Utils. Test: renders product name/price/image from props. Emits `add-to-cart` with product ID on button click. Emits `click` on card click.

**Expected Output:** `ProductCard.spec.ts`.

**Acceptance Criteria:**
- [ ] Test file exists at `tests/unit/components/ProductCard.spec.ts`
- [ ] Test verifies props rendering (name, price, image)
- [ ] Test verifies `add-to-cart` event emission
- [ ] Test verifies `click` event emission
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing tests and verifying pass, STOP. Wait for Orchestrator approval.

---

## Prompt T-058: Write Frontend Unit Tests for Product Store

**Context:** Store tests verify API call behavior.

**Objective:** Create Vitest tests for `useProductStore`.

**Current Project State:** Component tests pass (T-057 complete).

**Inputs:** `useProductStore`, `vi` mock.

**Constraints:** Mock `fetch` or API client. Test: `fetchProducts` calls correct URL with query params. `fetchCategories` calls correct URL.

**Expected Output:** `useProductStore.spec.ts`.

**Acceptance Criteria:**
- [ ] Test file exists at `tests/unit/stores/useProductStore.spec.ts`
- [ ] Test verifies correct API endpoint called
- [ ] Test verifies query params passed correctly
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing tests and verifying pass, STOP. Wait for Orchestrator approval.

---

## Prompt T-059: Write E2E Test for Product Catalog Flow

**Context:** E2E tests verify the full flow from browser through YARP to API.

**Objective:** Create Playwright spec for product catalog: visit → filter → click product → verify detail.

**Current Project State:** Store tests pass (T-058 complete).

**Inputs:** Playwright config, running services.

**Constraints:** Create `tests/e2e/specs/product-catalog.spec.ts`. Visit `/products`. Select a category filter. Verify URL updates. Verify grid updates. Click a product. Verify detail page renders with product name.

**Expected Output:** `product-catalog.spec.ts`.

**Acceptance Criteria:**
- [ ] Spec file exists
- [ ] Spec visits `/products`
- [ ] Spec selects category and verifies URL update
- [ ] Spec clicks product and verifies detail page
- [ ] `npx playwright test` passes

**Stopping Condition:** After writing E2E test and verifying pass, STOP. This completes Phase 4. Wait for Orchestrator approval before Phase 5.

---

## Prompt T-060: Port CartItem Domain Model

**Context:** Phase 5 begins. CartItem entity is needed for cart operations.

**Objective:** Copy `CartItem.cs` from legacy to new project.

**Current Project State:** Phase 4 complete (T-059 passed).

**Inputs:** Legacy `Ecommerce.Core/Models/CartItem.cs`.

**Constraints:** Remove EF attributes. Change namespace.

**Expected Output:** `CartItem.cs` in new project.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Core/Models/`
- [ ] Namespace updated
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-061: Create Cart DTOs

**Context:** Cart API endpoints need DTOs for request/response contracts.

**Objective:** Define `CartItemDto`, `CartSummaryDto`, `AddCartItemRequest`.

**Current Project State:** `CartItem` model ported (T-060 complete).

**Inputs:** `CartItem` domain model.

**Constraints:** `CartItemDto`: Id, ProductId, ProductName, Quantity, UnitPrice, VariantId, ImageUrl. `CartSummaryDto`: ItemCount, TotalAmount. `AddCartItemRequest`: ProductId, Quantity, VariantId (optional).

**Expected Output:** Three DTO files.

**Acceptance Criteria:**
- [ ] All three DTO files exist in `Ecommerce.Api.Core/DTOs/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating DTOs, STOP. Wait for Orchestrator approval.

---

## Prompt T-062: Port ICartItemRepository Interface

**Context:** Cart repository contract needed.

**Objective:** Copy `ICartItemRepository` from legacy to new project.

**Current Project State:** Cart DTOs created (T-061 complete).

**Inputs:** Legacy `Ecommerce.Core/Interfaces/ICartItemRepository.cs`.

**Constraints:** Change namespace. Methods must use domain models.

**Expected Output:** `ICartItemRepository.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Core/Interfaces/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-063: Port CartItemRepository Implementation

**Context:** Cart data access implementation.

**Objective:** Copy `CartItemRepository` from legacy to new project with SQL client swap.

**Current Project State:** `ICartItemRepository` ported (T-062 complete).

**Inputs:** Legacy `Ecommerce.Data/Repositories/CartItemRepository.cs`.

**Constraints:** Replace `System.Data.SqlClient` with `Microsoft.Data.SqlClient`. Change namespace.

**Expected Output:** `CartItemRepository.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Data/Repositories/`
- [ ] Uses `Microsoft.Data.SqlClient`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-064: Port ICartService Interface

**Context:** Cart service contract needed.

**Objective:** Copy `ICartService` from legacy to new project.

**Current Project State:** `CartItemRepository` ported (T-063 complete).

**Inputs:** Legacy `Ecommerce.Core/Interfaces/ICartService.cs`.

**Constraints:** Change namespace. Return DTOs or domain models.

**Expected Output:** `ICartService.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Interfaces/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-065: Port CartService Implementation

**Context:** Cart business logic for add, update, remove operations.

**Objective:** Copy `CartService` from legacy to new project. Update return types to DTOs.

**Current Project State:** `ICartService` ported (T-064 complete).

**Inputs:** Legacy `Ecommerce.Services/Services/CartService.cs`.

**Constraints:** Change namespace. Remove ViewModel references. Map to DTOs at return points. Preserve all business logic.

**Expected Output:** `CartService.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Implementations/`
- [ ] No ViewModel references
- [ ] Returns DTOs
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-066: Register Cart Services in DI

**Context:** Cart services must be registered in DI container.

**Objective:** Add scoped registrations for cart repository and service in `Program.cs`.

**Current Project State:** `CartService` ported (T-065 complete).

**Inputs:** `Program.cs`.

**Constraints:** Use `AddScoped`.

**Expected Output:** Modified `Program.cs`.

**Acceptance Criteria:**
- [ ] `AddScoped<ICartItemRepository, CartItemRepository>()` present
- [ ] `AddScoped<ICartService, CartService>()` present
- [ ] API starts without DI exceptions

**Stopping Condition:** After registering and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-067: Implement CartController with Authenticated Endpoints

**Context:** Cart endpoints require authentication. The customer ID is extracted from JWT claims.

**Objective:** Create `CartController` with `[Authorize]` on all actions.

**Current Project State:** Cart DI configured (T-066 complete).

**Inputs:** `ICartService`, JWT claims (`customer_id`).

**Constraints:** Use `[ApiController]`, `[Route("api/cart")]`, `[Authorize]`. Endpoints: `GET /api/cart`, `POST /api/cart/items`, `PUT /api/cart/items/{id}`, `DELETE /api/cart/items/{id}`, `GET /api/cart/summary`. Extract `customer_id` from `User.FindFirst("customer_id")`. Return 401 for anonymous, 200 for authenticated.

**Expected Output:** `CartController.cs`.

**Acceptance Criteria:**
- [ ] All actions have `[Authorize]`
- [ ] Anonymous requests return 401
- [ ] Authenticated requests return 200 with correct DTOs
- [ ] `customer_id` extracted from JWT claims correctly
- [ ] `dotnet build` succeeds

**Stopping Condition:** After implementing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-068: Create Pinia Cart Store

**Context:** Cart state management in SPA.

**Objective:** Implement `useCartStore` with actions for cart operations.

**Current Project State:** `CartController` implemented (T-067 complete).

**Inputs:** `client.ts`, `CartItemDto`, `useAuthStore` (for token).

**Constraints:** State: `items` (ref). Computed: `itemCount`, `totalAmount`. Actions: `fetchCart()`, `addItem(productId, quantity, variantId?)`, `updateItem(id, quantity)`, `removeItem(id)`. No localStorage persistence (server-side cart). Use auth store token for API calls.

**Expected Output:** `useCartStore.ts`.

**Acceptance Criteria:**
- [ ] Store exists at `src/stores/useCartStore.ts`
- [ ] `fetchCart` calls `GET /api/cart`
- [ ] `addItem` calls `POST /api/cart/items`
- [ ] `updateItem` calls `PUT /api/cart/items/{id}`
- [ ] `removeItem` calls `DELETE /api/cart/items/{id}`
- [ ] `itemCount` computed works
- [ ] No localStorage persistence

**Stopping Condition:** After creating store, STOP. This completes Phase 5. Wait for Orchestrator approval before Phase 6.

---

## Prompt T-069: Create AddToCartButton Vue Component

**Context:** Phase 6 begins. Reusable button for adding products to cart from product card or detail page.

**Objective:** Build `AddToCartButton.vue` that calls cart store `addItem`.

**Current Project State:** Cart store created (T-068 complete).

**Inputs:** `useCartStore`, `useAuthStore`.

**Constraints:** Use `<script setup lang="ts">`. Props: `productId`, `variantId?`, `quantity?` (default 1). On click, call `cartStore.addItem`. Show loading state. Show success feedback. If not authenticated, redirect to legacy login.

**Expected Output:** `AddToCartButton.vue`.

**Acceptance Criteria:**
- [ ] Component exists at `src/components/AddToCartButton.vue`
- [ ] Click calls `cartStore.addItem()` with correct params
- [ ] Shows loading state during API call
- [ ] Shows success feedback after add
- [ ] Redirects to login if not authenticated

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-070: Create CartView Vue Page

**Context:** Full cart page with item list, quantity controls, and removal.

**Objective:** Build `CartView.vue`.

**Current Project State:** `AddToCartButton` created (T-069 complete).

**Inputs:** `useCartStore`, `CartItemDto`.

**Constraints:** Fetch cart on mount. Display items in table: image, name, price, quantity input, line total, remove button. Quantity change calls `updateItem`. Remove calls `removeItem`. Show cart total. Show empty state message. Show loading and error states.

**Expected Output:** `CartView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/CartView.vue`
- [ ] Fetches and displays cart items
- [ ] Quantity change persists to backend
- [ ] Remove item works and updates UI
- [ ] Cart total displays correctly
- [ ] Empty state message shows when no items
- [ ] Loading and error states handled

**Stopping Condition:** After creating and verifying with real data, STOP. Wait for Orchestrator approval.

---

## Prompt T-071: Create CartSummaryBadge Component

**Context:** Header badge showing cart item count.

**Objective:** Build `CartSummaryBadge.vue` and integrate into header.

**Current Project State:** `CartView` created (T-070 complete).

**Inputs:** `useCartStore`.

**Constraints:** Display `cartStore.itemCount`. Update reactively when items added. Link to `/cart`.

**Expected Output:** `CartSummaryBadge.vue` and modified header component.

**Acceptance Criteria:**
- [ ] `CartSummaryBadge.vue` exists
- [ ] Displays correct item count from store
- [ ] Count updates when items added
- [ ] Links to `/cart`
- [ ] Integrated into header component

**Stopping Condition:** After creating and integrating, STOP. Wait for Orchestrator approval.

---

## Prompt T-072: Add Cart Route to Vue Router

**Context:** Cart page must be accessible via URL.

**Objective:** Add `/cart` route to Vue Router.

**Current Project State:** `CartSummaryBadge` created (T-071 complete).

**Inputs:** `router/index.ts`, `CartView`.

**Constraints:** Use lazy loading. `/cart` → `CartView`.

**Expected Output:** Updated `router/index.ts`.

**Acceptance Criteria:**
- [ ] `/cart` route added with lazy import
- [ ] Navigation to `/cart` renders `CartView`
- [ ] Browser back/forward works

**Stopping Condition:** After adding route and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-073: Add YARP Route for Cart Pages

**Context:** Cart traffic must route to SPA.

**Objective:** Add `/cart*` route to YARP config.

**Current Project State:** Cart route in Vue Router (T-072 complete).

**Inputs:** `appsettings.json` YARP config.

**Constraints:** Do not remove existing routes.

**Expected Output:** Updated `appsettings.json`.

**Acceptance Criteria:**
- [ ] `/cart*` route present pointing to `spaCluster`
- [ ] `http://localhost:5003/cart` serves Vue SPA
- [ ] Existing routes still work

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-074: Write Backend Unit Tests for CartController

**Context:** Controller tests for cart with authentication context.

**Objective:** Create xUnit tests for `CartController` mocking `ICartService` with mock `ClaimsPrincipal`.

**Current Project State:** YARP cart routes configured (T-073 complete).

**Inputs:** `CartController`, `ICartService`, Moq.

**Constraints:** Mock `ClaimsPrincipal` with `customer_id` claim. Test: 401 for anonymous, 200 for authenticated, correct customer ID passed to service.

**Expected Output:** `CartControllerTests.cs`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Tests verify 401 for anonymous requests
- [ ] Tests verify 200 for authenticated requests
- [ ] Tests verify correct `customer_id` passed to service
- [ ] `dotnet test` passes

**Stopping Condition:** After writing and verifying tests, STOP. Wait for Orchestrator approval.

---

## Prompt T-075: Write Backend Unit Tests for CartService

**Context:** Service tests for cart business logic.

**Objective:** Create xUnit tests for `CartService` mocking `ICartItemRepository`.

**Current Project State:** Controller tests pass (T-074 complete).

**Inputs:** `CartService`, `ICartItemRepository`, Moq.

**Constraints:** Test: add item, update quantity, remove item, get cart summary.

**Expected Output:** `CartServiceTests.cs`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Tests cover add, update, remove, summary logic
- [ ] `dotnet test` passes

**Stopping Condition:** After writing and verifying tests, STOP. Wait for Orchestrator approval.

---

## Prompt T-076: Write Frontend Unit Tests for AddToCartButton

**Context:** Component test for add-to-cart button.

**Objective:** Create Vitest tests for `AddToCartButton.vue`.

**Current Project State:** Service tests pass (T-075 complete).

**Inputs:** `AddToCartButton.vue`, `@vue/test-utils`.

**Constraints:** Test: click emits event, calls store method, shows loading state.

**Expected Output:** `AddToCartButton.spec.ts`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Test verifies click calls store method
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-077: Write Frontend Unit Tests for CartView

**Context:** Component test for cart view.

**Objective:** Create Vitest tests for `CartView.vue`.

**Current Project State:** Button tests pass (T-076 complete).

**Inputs:** `CartView.vue`, `@vue/test-utils`.

**Constraints:** Test: renders items, handles quantity change emit, handles remove emit.

**Expected Output:** `CartView.spec.ts`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Test verifies item rendering
- [ ] Test verifies quantity change handling
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-078: Write E2E Test for Cart Flow

**Context:** E2E test for complete cart flow.

**Objective:** Create Playwright spec for cart: add → update qty → remove → verify empty.

**Current Project State:** Cart view tests pass (T-077 complete).

**Inputs:** Running services, Playwright.

**Constraints:** Spec: visit product detail, click add to cart, navigate to cart, verify item, change quantity, verify total, remove item, verify empty.

**Expected Output:** `cart.spec.ts`.

**Acceptance Criteria:**
- [ ] Spec file exists
- [ ] Spec adds product to cart
- [ ] Spec verifies cart page shows item
- [ ] Spec updates quantity and verifies total
- [ ] Spec removes item and verifies empty cart
- [ ] `npx playwright test` passes

**Stopping Condition:** After writing and verifying, STOP. This completes Phase 6. Wait for Orchestrator approval before Phase 7.

---

## Prompt T-079: Port Address Domain Model

**Context:** Phase 7 begins. Address entity needed for checkout.

**Objective:** Copy `Address.cs` from legacy to new project.

**Current Project State:** Phase 6 complete (T-078 passed).

**Inputs:** Legacy `Ecommerce.Core/Models/Address.cs`.

**Constraints:** Remove EF attributes. Change namespace.

**Expected Output:** `Address.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Core/Models/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-080: Port Order and OrderItem Domain Models

**Context:** Order entities needed for checkout and order history.

**Objective:** Copy `Order.cs` and `OrderItem.cs` from legacy to new project.

**Current Project State:** `Address` ported (T-079 complete).

**Inputs:** Legacy model files.

**Constraints:** Remove EF attributes. Change namespace.

**Expected Output:** Two model files.

**Acceptance Criteria:**
- [ ] Both files exist in `Ecommerce.Api.Core/Models/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-081: Port Customer Domain Model

**Context:** Customer entity needed for checkout.

**Objective:** Copy `Customer.cs` from legacy to new project.

**Current Project State:** Order models ported (T-080 complete).

**Inputs:** Legacy `Ecommerce.Core/Models/Customer.cs`.

**Constraints:** Remove EF attributes. Change namespace.

**Expected Output:** `Customer.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Core/Models/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-082: Create Checkout DTOs

**Context:** Checkout endpoints need DTOs for addresses, orders, and order placement.

**Objective:** Define `AddressDto`, `OrderDto`, `OrderItemDto`, `PlaceOrderRequest`, `OrderConfirmationDto`.

**Current Project State:** Customer model ported (T-081 complete).

**Inputs:** Domain models.

**Constraints:** `PlaceOrderRequest`: AddressId (int?), NewAddress (AddressDto?), IdempotencyKey (string). `OrderConfirmationDto`: OrderId, OrderNumber, TotalAmount, OrderDate, Items (IEnumerable<OrderItemDto>).

**Expected Output:** Five DTO files.

**Acceptance Criteria:**
- [ ] All five DTO files exist
- [ ] No circular references
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating DTOs, STOP. Wait for Orchestrator approval.

---

## Prompt T-083: Port Address Repository Interface and Implementation

**Context:** Address data access needed for checkout.

**Objective:** Copy `IAddressRepository` and `AddressRepository` from legacy to new project.

**Current Project State:** Checkout DTOs created (T-082 complete).

**Inputs:** Legacy files.

**Constraints:** Replace SQL client. Change namespace.

**Expected Output:** Two files.

**Acceptance Criteria:**
- [ ] Both files exist
- [ ] Uses `Microsoft.Data.SqlClient`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-084: Port Order Repository Interface and Implementation

**Context:** Order data access needed for checkout and order history.

**Objective:** Copy `IOrderRepository` and `OrderRepository` from legacy to new project.

**Current Project State:** Address repository ported (T-083 complete).

**Inputs:** Legacy files.

**Constraints:** Same as T-083.

**Expected Output:** Two files.

**Acceptance Criteria:**
- [ ] Both files exist
- [ ] Uses `Microsoft.Data.SqlClient`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-085: Port Customer Repository Interface and Implementation

**Context:** Customer data access needed for checkout.

**Objective:** Copy `ICustomerRepository` and `CustomerRepository` from legacy to new project.

**Current Project State:** Order repository ported (T-084 complete).

**Inputs:** Legacy files.

**Constraints:** Same as T-083.

**Expected Output:** Two files.

**Acceptance Criteria:**
- [ ] Both files exist
- [ ] Uses `Microsoft.Data.SqlClient`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-086: Port CheckoutService Verbatim

**Context:** CheckoutService is the most complex service with transactions and idempotency. It must be ported exactly as-is.

**Objective:** Copy `CheckoutService` from legacy to new project. Change only namespaces and return type.

**Current Project State:** All checkout repositories ported (T-085 complete).

**Inputs:** Legacy `Ecommerce.Services/Services/CheckoutService.cs`.

**Constraints:** CRITICAL: Do not refactor logic. Preserve transaction boundaries. Preserve idempotency check. Preserve order of operations (check idempotency → begin transaction → create address → create order → create items → clear cart → commit). Change namespace to `Ecommerce.Api.Services.Implementations`. Change return type from ViewModel to `OrderConfirmationDto`. Port `IUnitOfWork` and `UnitOfWork` as well if not already ported.

**Expected Output:** `CheckoutService.cs` (and `IUnitOfWork`/`UnitOfWork` if needed).

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Implementations/`
- [ ] `dotnet build` succeeds
- [ ] Diff against legacy shows ONLY namespace and return type changes
- [ ] Transaction logic intact (BeginTransaction, Commit, Rollback)
- [ ] Idempotency check intact (GetByIdempotencyKey)

**Stopping Condition:** After porting and verifying diff, STOP. Wait for Orchestrator approval.

---

## Prompt T-087: Port OrderService

**Context:** OrderService needed for order history queries.

**Objective:** Copy `OrderService` from legacy to new project.

**Current Project State:** `CheckoutService` ported (T-086 complete).

**Inputs:** Legacy `Ecommerce.Services/Services/OrderService.cs`.

**Constraints:** Change namespace. Update return types to DTOs.

**Expected Output:** `OrderService.cs`.

**Acceptance Criteria:**
- [ ] File exists in `Ecommerce.Api.Services/Implementations/`
- [ ] `dotnet build` succeeds

**Stopping Condition:** After porting, STOP. Wait for Orchestrator approval.

---

## Prompt T-088: Register Checkout and Order Services in DI

**Context:** All checkout-related services must be registered.

**Objective:** Add scoped registrations for address, order, customer repositories and services in `Program.cs`.

**Current Project State:** `OrderService` ported (T-087 complete).

**Inputs:** `Program.cs`.

**Constraints:** Use `AddScoped`. Register `IUnitOfWork`/`UnitOfWork` if not already registered.

**Expected Output:** Modified `Program.cs`.

**Acceptance Criteria:**
- [ ] All new repositories registered (`IAddressRepository`, `IOrderRepository`, `ICustomerRepository`)
- [ ] All new services registered (`ICheckoutService`, `IOrderService`)
- [ ] `IUnitOfWork` registered
- [ ] API starts without DI exceptions

**Stopping Condition:** After registering and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-089: Implement CheckoutController

**Context:** Checkout endpoints with idempotency key header requirement.

**Objective:** Create `CheckoutController` with `[Authorize]` actions for addresses, review, and order placement.

**Current Project State:** DI configured (T-088 complete).

**Inputs:** `ICheckoutService`, JWT claims.

**Constraints:** `[ApiController]`, `[Route("api/checkout")]`, `[Authorize]`. Endpoints: `GET /api/checkout/addresses`, `POST /api/checkout/addresses`, `GET /api/checkout/review`, `POST /api/checkout/orders` (requires `Idempotency-Key` header → 400 if missing), `GET /api/checkout/orders/{id}/confirmation`. Extract `customer_id` from JWT.

**Expected Output:** `CheckoutController.cs`.

**Acceptance Criteria:**
- [ ] All actions have `[Authorize]`
- [ ] `POST /api/checkout/orders` returns 400 when `Idempotency-Key` header missing
- [ ] Valid request returns 201 with `OrderConfirmationDto`
- [ ] Duplicate idempotency key returns 200 with existing order
- [ ] `dotnet build` succeeds

**Stopping Condition:** After implementing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-090: Implement OrdersController (Read-Only)

**Context:** Order history endpoints for authenticated customers.

**Objective:** Create `OrdersController` with `GET /api/orders` and `GET /api/orders/{id}`.

**Current Project State:** `CheckoutController` implemented (T-089 complete).

**Inputs:** `IOrderService`, JWT claims.

**Constraints:** `[ApiController]`, `[Route("api/orders")]`, `[Authorize]`. `GET /api/orders` returns orders for authenticated customer only. `GET /api/orders/{id}` returns 404 if order doesn't belong to customer.

**Expected Output:** `OrdersController.cs`.

**Acceptance Criteria:**
- [ ] Both endpoints have `[Authorize]`
- [ ] Returns only orders for authenticated customer's `customer_id`
- [ ] Returns 404 for orders not belonging to customer
- [ ] `dotnet build` succeeds

**Stopping Condition:** After implementing and verifying, STOP. This completes Phase 7. Wait for Orchestrator approval before Phase 8.

---

## Prompt T-091: Create Pinia Checkout Store

**Context:** Phase 8 begins. Checkout state management with idempotency key.

**Objective:** Implement `useCheckoutStore` with state for selected address, shipping, and UUID idempotency key.

**Current Project State:** `OrdersController` implemented (T-090 complete).

**Inputs:** `client.ts`, `useAuthStore`, `crypto.randomUUID()`.

**Constraints:** State: `selectedAddressId` (ref<number|null>), `idempotencyKey` (ref<string>, generated on checkout start), `addresses` (ref). Actions: `fetchAddresses()`, `createAddress(data)`, `placeOrder()`. `placeOrder` sends `Idempotency-Key` header.

**Expected Output:** `useCheckoutStore.ts`.

**Acceptance Criteria:**
- [ ] Store exists at `src/stores/useCheckoutStore.ts`
- [ ] `idempotencyKey` is a valid UUID generated on store init
- [ ] `fetchAddresses` calls `GET /api/checkout/addresses`
- [ ] `createAddress` calls `POST /api/checkout/addresses`
- [ ] `placeOrder` calls `POST /api/checkout/orders` with `Idempotency-Key` header

**Stopping Condition:** After creating store, STOP. Wait for Orchestrator approval.

---

## Prompt T-092: Create AddressSelector Vue Component

**Context:** Component for selecting saved addresses during checkout.

**Objective:** Build `AddressSelector.vue`.

**Current Project State:** Checkout store created (T-091 complete).

**Inputs:** `useCheckoutStore`.

**Constraints:** Fetch addresses on mount. Display as radio buttons or cards. Emit `select` with address ID. Highlight selected.

**Expected Output:** `AddressSelector.vue`.

**Acceptance Criteria:**
- [ ] Component exists
- [ ] Fetches addresses from API
- [ ] Displays addresses as selectable list
- [ ] Updates checkout store on selection
- [ ] Highlights selected address

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-093: Create AddressForm Vue Component

**Context:** Form for creating a new address during checkout.

**Objective:** Build `AddressForm.vue` with validation.

**Current Project State:** `AddressSelector` created (T-092 complete).

**Inputs:** `useCheckoutStore`.

**Constraints:** Fields: street, city, state, zip, country. Validate required fields. On submit, call `checkoutStore.createAddress`. Show success and add to selector list.

**Expected Output:** `AddressForm.vue`.

**Acceptance Criteria:**
- [ ] Component exists
- [ ] Form validates required fields
- [ ] Submit calls `POST /api/checkout/addresses`
- [ ] Success adds address to store
- [ ] Validation errors displayed

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-094: Create AddressStep Vue Component

**Context:** First step of checkout wizard combining selector and form.

**Objective:** Build `AddressStep.vue` that toggles between saved addresses and new address form.

**Current Project State:** `AddressForm` created (T-093 complete).

**Inputs:** `AddressSelector`, `AddressForm`, `useCheckoutStore`.

**Constraints:** Default to showing saved addresses. Toggle to form on "Add New Address" click. Proceed to next step only when address selected. Emit `next` event.

**Expected Output:** `AddressStep.vue`.

**Acceptance Criteria:**
- [ ] Component exists
- [ ] Displays saved addresses by default
- [ ] Toggles to new address form on button click
- [ ] Proceeds only when address selected
- [ ] Emits `next` event

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-095: Create ReviewStep Vue Component

**Context:** Order review step before placing order.

**Objective:** Build `ReviewStep.vue` displaying cart summary, selected address, and total.

**Current Project State:** `AddressStep` created (T-094 complete).

**Inputs:** `useCartStore`, `useCheckoutStore`.

**Constraints:** Display cart items, quantities, line totals. Display selected address. Display grand total. "Place Order" button calls `checkoutStore.placeOrder()`. On success, emit `placed` with order ID. Show loading during placement. Show error on failure.

**Expected Output:** `ReviewStep.vue`.

**Acceptance Criteria:**
- [ ] Component exists
- [ ] Displays accurate order preview
- [ ] "Place Order" button calls API with `Idempotency-Key` header
- [ ] Emits `placed` event with order ID on success
- [ ] Shows loading during API call
- [ ] Shows error on failure

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-096: Create OrderConfirmation Vue Component

**Context:** Success page after order placement.

**Objective:** Build `OrderConfirmation.vue` showing order number.

**Current Project State:** `ReviewStep` created (T-095 complete).

**Inputs:** Route param `orderId`.

**Constraints:** Display order ID from route param. Show success message. Link to continue shopping and view orders.

**Expected Output:** `OrderConfirmation.vue`.

**Acceptance Criteria:**
- [ ] Component exists
- [ ] Displays order ID from route parameter
- [ ] Shows success message
- [ ] Links to `/products` and `/orders`

**Stopping Condition:** After creating component, STOP. Wait for Orchestrator approval.

---

## Prompt T-097: Create CheckoutWizard Vue Page

**Context:** Multi-step checkout wizard composing address, review, and confirmation steps.

**Objective:** Build `CheckoutWizard.vue` with step navigation and state persistence.

**Current Project State:** All checkout step components created (T-096 complete).

**Inputs:** `AddressStep`, `ReviewStep`, `OrderConfirmation`, `useCheckoutStore`.

**Constraints:** Steps: Address → Review → Confirmation. Forward navigation only when current step valid. Backward navigation allowed. State persists across steps. On order placed, navigate to confirmation. Prevent direct access to later steps via URL.

**Expected Output:** `CheckoutWizard.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/CheckoutWizard.vue`
- [ ] Step navigation works forward and backward
- [ ] State persists across steps
- [ ] Cannot skip to later steps without completing earlier ones
- [ ] On order placed, shows confirmation

**Stopping Condition:** After creating and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-098: Add Checkout and Confirmation Routes to Vue Router

**Context:** Checkout routes must be accessible and guarded.

**Objective:** Add `/checkout` and `/checkout/confirmation/:orderId` routes.

**Current Project State:** `CheckoutWizard` created (T-097 complete).

**Inputs:** `router/index.ts`.

**Constraints:** `/checkout` → `CheckoutWizard`. `/checkout/confirmation/:orderId` → `OrderConfirmation`. Add route guard preventing direct access to confirmation without checkout flow.

**Expected Output:** Updated `router/index.ts`.

**Acceptance Criteria:**
- [ ] `/checkout` route added
- [ ] `/checkout/confirmation/:orderId` route added
- [ ] Route guard prevents direct confirmation access
- [ ] Navigation works

**Stopping Condition:** After adding routes and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-099: Add YARP Route for Checkout Pages

**Context:** Checkout traffic must route to SPA.

**Objective:** Add `/checkout*` route to YARP config.

**Current Project State:** Checkout routes in Vue Router (T-098 complete).

**Inputs:** `appsettings.json`.

**Constraints:** Do not remove existing routes.

**Expected Output:** Updated `appsettings.json`.

**Acceptance Criteria:**
- [ ] `/checkout*` route present pointing to `spaCluster`
- [ ] `http://localhost:5003/checkout` serves Vue SPA
- [ ] Existing routes still work

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-100: Write Backend Unit Tests for CheckoutService

**Context:** Critical tests for transaction, idempotency, and address creation.

**Objective:** Create xUnit tests for `CheckoutService` mocking `IUnitOfWork`.

**Current Project State:** YARP checkout routes configured (T-099 complete).

**Inputs:** `CheckoutService`, `IUnitOfWork`, Moq.

**Constraints:** Test: transaction committed exactly once on success. Transaction rolled back on failure. Duplicate idempotency key returns existing order without creating new one. New address created when `addressId` is null. Cart cleared after order placement.

**Expected Output:** `CheckoutServiceTests.cs`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Test verifies transaction committed once
- [ ] Test verifies transaction rolled back on error
- [ ] Test verifies idempotency returns existing order
- [ ] Test verifies address creation when null
- [ ] Test verifies cart cleared
- [ ] `dotnet test` passes

**Stopping Condition:** After writing and verifying tests, STOP. Wait for Orchestrator approval.

---

## Prompt T-101: Write Backend Unit Tests for CheckoutController

**Context:** Controller tests for idempotency header validation.

**Objective:** Create xUnit tests for `CheckoutController`.

**Current Project State:** Service tests pass (T-100 complete).

**Inputs:** `CheckoutController`, `ICheckoutService`, Moq.

**Constraints:** Test: 400 when `Idempotency-Key` missing. 201 on first order. 200 on duplicate idempotency key. 401 for anonymous.

**Expected Output:** `CheckoutControllerTests.cs`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Tests verify 400 without `Idempotency-Key`
- [ ] Tests verify 201 on first order
- [ ] Tests verify 200 on duplicate
- [ ] Tests verify 401 for anonymous
- [ ] `dotnet test` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-102: Write Frontend Unit Tests for CheckoutWizard

**Context:** Component tests for wizard step navigation.

**Objective:** Create Vitest tests for `CheckoutWizard.vue`.

**Current Project State:** Controller tests pass (T-101 complete).

**Inputs:** `CheckoutWizard.vue`, `@vue/test-utils`.

**Constraints:** Test: step navigation, form validation triggers, submit sends correct headers.

**Expected Output:** `CheckoutWizard.spec.ts`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Tests verify step navigation
- [ ] Tests verify submit sends `Idempotency-Key` header
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-103: Write E2E Test for Full Checkout Flow

**Context:** Critical E2E test for the highest-risk slice.

**Objective:** Create Playwright spec for complete checkout: cart → checkout → address → review → place order → confirmation → verify DB.

**Current Project State:** Wizard tests pass (T-102 complete).

**Inputs:** Running services, SQL Server access.

**Constraints:** Spec: add item to cart, navigate to checkout, select/create address, proceed to review, place order, verify confirmation page, verify order in database, verify cart cleared in database.

**Expected Output:** `checkout.spec.ts`.

**Acceptance Criteria:**
- [ ] Spec file exists
- [ ] Spec completes full checkout flow
- [ ] SQL query confirms order exists in `Orders` table
- [ ] SQL query confirms `CartItems` cleared for customer
- [ ] `npx playwright test` passes

**Stopping Condition:** After writing and verifying, STOP. This completes Phase 8. Wait for Orchestrator approval before Phase 9.

---

## Prompt T-104: Create OrderListView Vue Page

**Context:** Phase 9 begins. Order history page for authenticated customers.

**Objective:** Build `OrderListView.vue` displaying paginated order history.

**Current Project State:** Phase 8 complete (T-103 passed).

**Inputs:** `client.ts`, `OrderDto` type, `PaginationControls`.

**Constraints:** Fetch `GET /api/orders` on mount. Display table: order number, date, total, status. Paginate. Show loading and error states. Show empty state.

**Expected Output:** `OrderListView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/OrderListView.vue`
- [ ] Fetches orders from API
- [ ] Renders table with order number, date, total, status
- [ ] Pagination works
- [ ] Loading and error states handled
- [ ] Empty state shows when no orders

**Stopping Condition:** After creating and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-105: Create OrderDetailView Vue Page

**Context:** Order detail page with items and shipping info.

**Objective:** Build `OrderDetailView.vue`.

**Current Project State:** `OrderListView` created (T-104 complete).

**Inputs:** `client.ts`, `OrderDto` type.

**Constraints:** Fetch `GET /api/orders/{id}`. Display order items, quantities, prices, shipping address, order status, total. Show loading and error states.

**Expected Output:** `OrderDetailView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/OrderDetailView.vue`
- [ ] Fetches order by ID from route param
- [ ] Displays order items, quantities, prices
- [ ] Displays shipping address
- [ ] Displays order status and total
- [ ] Loading and error states handled

**Stopping Condition:** After creating and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-106: Add Order Routes to Vue Router

**Context:** Order history routes must be configured.

**Objective:** Add `/orders` and `/orders/:id` routes.

**Current Project State:** `OrderDetailView` created (T-105 complete).

**Inputs:** `router/index.ts`.

**Constraints:** Lazy load. `/orders` → `OrderListView`. `/orders/:id` → `OrderDetailView`.

**Expected Output:** Updated `router/index.ts`.

**Acceptance Criteria:**
- [ ] `/orders` route added
- [ ] `/orders/:id` route added
- [ ] Navigation works

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-107: Add YARP Route for Order Pages

**Context:** Order history traffic must route to SPA.

**Objective:** Add `/orders*` route to YARP config.

**Current Project State:** Order routes in Vue Router (T-106 complete).

**Inputs:** `appsettings.json`.

**Constraints:** Do not remove existing routes.

**Expected Output:** Updated `appsettings.json`.

**Acceptance Criteria:**
- [ ] `/orders*` route present pointing to `spaCluster`
- [ ] `http://localhost:5003/orders` serves Vue SPA
- [ ] Existing routes still work

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-108: Write Frontend Unit Tests for Order Views

**Context:** Unit tests for order list and detail views.

**Objective:** Create Vitest tests for `OrderListView` and `OrderDetailView`.

**Current Project State:** YARP order routes configured (T-107 complete).

**Inputs:** `OrderListView.vue`, `OrderDetailView.vue`.

**Constraints:** Test rendering and data display.

**Expected Output:** Two test files.

**Acceptance Criteria:**
- [ ] Both test files exist
- [ ] Tests verify rendering of order data
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-109: Write E2E Test for Order History Flow

**Context:** E2E test for order history after checkout.

**Objective:** Create Playwright spec: place order → visit orders → verify visible → click → verify detail.

**Current Project State:** Order view tests pass (T-108 complete).

**Inputs:** Running services, checkout E2E flow.

**Constraints:** Reuse checkout flow to place order. Navigate to `/orders`. Verify order visible. Click and verify detail page.

**Expected Output:** `order-history.spec.ts`.

**Acceptance Criteria:**
- [ ] Spec file exists
- [ ] Spec places order (or reuses existing order)
- [ ] Spec navigates to `/orders` and verifies order visible
- [ ] Spec clicks order and verifies detail page
- [ ] `npx playwright test` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-110: Integrate Legacy Account Page Links in Vue Header

**Context:** During hybrid phase, account pages (login/register/manage) stay in legacy MVC. Vue header must link to them.

**Objective:** Add login/register/manage links to Vue header that route to legacy pages via YARP fallback.

**Current Project State:** Order history E2E pass (T-109 complete).

**Inputs:** `TheHeader.vue` (or equivalent header component).

**Constraints:** Links point to `/Account/Login`, `/Account/Register`, `/Account/Manage`. These routes are NOT in Vue Router — they fall through YARP to legacy. After legacy login, JWT bridge keeps SPA authenticated. Show different links based on auth state (Login vs. Logout/Manage).

**Expected Output:** Modified header component.

**Acceptance Criteria:**
- [ ] Header contains login link when not authenticated
- [ ] Header contains logout/manage links when authenticated
- [ ] Links navigate to legacy pages correctly
- [ ] After legacy login, SPA obtains JWT via bridge
- [ ] SPA shows authenticated state after redirect back

**Stopping Condition:** After integrating and verifying, STOP. Wait for Orchestrator approval before continuing to T-111 within Phase 9.

---

## Prompt T-111: Create HomeView Vue Page

**Context:** Continuing Phase 9. Homepage with featured products and hero banner.

**Objective:** Build `HomeView.vue`.

**Current Project State:** T-110 passed. Order history and account link integration complete.

**Inputs:** `ProductCard`, `client.ts`.

**Constraints:** Fetch featured products (e.g., `GET /api/products?pageSize=8` or a dedicated featured endpoint). Display hero banner. Reuse `ProductCard` for featured grid. Show loading state.

**Expected Output:** `HomeView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/HomeView.vue`
- [ ] Displays featured products using `ProductCard`
- [ ] Displays hero banner
- [ ] Loading state handled
- [ ] Real data from API renders

**Stopping Condition:** After creating and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-112: Add Search Input to Header

**Context:** Search functionality for product discovery.

**Objective:** Add search input to header that triggers product search.

**Current Project State:** `HomeView` created (T-111 complete).

**Inputs:** `TheHeader.vue`, `useProductStore`.

**Constraints:** Search input in header. On submit, navigate to `/products?searchQuery=<value>`. Use existing `GET /api/products?searchQuery=` endpoint.

**Expected Output:** Modified `TheHeader.vue`.

**Acceptance Criteria:**
- [ ] Search input exists in header
- [ ] Submit navigates to `/products` with `searchQuery` query param
- [ ] Product list filters by search query
- [ ] Search works from any page

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-113: Add Home Route to Vue Router

**Context:** Homepage must be accessible at root path.

**Objective:** Add `/` route mapping to `HomeView`.

**Current Project State:** Search added to header (T-112 complete).

**Inputs:** `router/index.ts`.

**Constraints:** `/` → `HomeView`. Lazy load.

**Expected Output:** Updated `router/index.ts`.

**Acceptance Criteria:**
- [ ] `/` route added
- [ ] Visiting `/` renders `HomeView`
- [ ] Navigation works

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-114: Add YARP Routes for Homepage and Search

**Context:** Homepage traffic must route to SPA.

**Objective:** Add `/` and `/home*` routes to YARP config.

**Current Project State:** Home route in Vue Router (T-113 complete).

**Inputs:** `appsettings.json`.

**Constraints:** Route `/` and `/home*` to `spaCluster`. This makes Vue SPA the default. Keep legacy fallback for `/Account/*` routes only.

**Expected Output:** Updated `appsettings.json`.

**Acceptance Criteria:**
- [ ] `/` route points to `spaCluster`
- [ ] `/home*` route points to `spaCluster`
- [ ] `http://localhost:5003/` serves Vue SPA
- [ ] Legacy `/Account/*` routes still work (fallback)

**Stopping Condition:** After adding and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-115: Write Frontend Unit Tests for HomeView

**Context:** Unit tests for homepage.

**Objective:** Create Vitest tests for `HomeView.vue`.

**Current Project State:** YARP homepage routes configured (T-114 complete).

**Inputs:** `HomeView.vue`.

**Constraints:** Test: renders featured products section, hero banner.

**Expected Output:** `HomeView.spec.ts`.

**Acceptance Criteria:**
- [ ] Test file exists
- [ ] Test verifies featured products render
- [ ] `npm run test:unit` passes

**Stopping Condition:** After writing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-116: Write E2E Test for Homepage

**Context:** E2E test for homepage.

**Objective:** Create Playwright spec for homepage.

**Current Project State:** HomeView tests pass (T-115 complete).

**Inputs:** Running services.

**Constraints:** Visit `/`, verify featured products visible, verify hero banner visible.

**Expected Output:** `homepage.spec.ts`.

**Acceptance Criteria:**
- [ ] Spec file exists
- [ ] Spec visits `/`
- [ ] Verifies featured products section visible
- [ ] Verifies hero banner visible
- [ ] `npx playwright test` passes

**Stopping Condition:** After writing and verifying, STOP. This completes Phase 9. Wait for Orchestrator approval before Phase 10.

---

## Prompt T-117: Analyze YARP Traffic Logs for Legacy Usage

**Context:** Phase 10 begins. Before decommissioning legacy, verify no unexpected traffic hits legacy fallback.

**Objective:** Review YARP logs for 7+ consecutive days and document findings.

**Current Project State:** Phase 9 complete (T-116 passed).

**Inputs:** YARP access logs or application insights.

**Constraints:** Document zero unexpected legacy traffic (except `/Account/*`). Any exceptions must be documented with justification. This is an analysis task, not a code task.

**Expected Output:** Traffic analysis report in `docs/migration/legacy-traffic-analysis.md`.

**Acceptance Criteria:**
- [ ] Report file exists
- [ ] Report covers 7+ consecutive days
- [ ] Report documents zero unexpected legacy traffic OR justified exceptions
- [ ] Report includes recommendation to proceed or not

**Stopping Condition:** After writing report, STOP. Wait for Orchestrator approval.

---

## Prompt T-118: Create Vue LoginView Page

**Context:** Login page for direct API authentication (post-legacy).

**Objective:** Build `LoginView.vue` with email/password form.

**Current Project State:** Traffic analysis complete (T-117 complete).

**Inputs:** `useAuthStore`.

**Constraints:** Use `<script setup lang="ts">`. Form with email and password inputs. Submit calls `authStore.login()`. Show validation errors. Show loading state. Redirect to `/` on success.

**Expected Output:** `LoginView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/LoginView.vue`
- [ ] Form with email and password inputs
- [ ] Submit triggers login action
- [ ] Validation errors displayed
- [ ] Loading state shown
- [ ] Redirects to `/` on success

**Stopping Condition:** After creating, STOP. Wait for Orchestrator approval.

---

## Prompt T-119: Create Vue RegisterView Page

**Context:** Registration page for direct API authentication.

**Objective:** Build `RegisterView.vue` with registration form.

**Current Project State:** `LoginView` created (T-118 complete).

**Inputs:** `useAuthStore`.

**Constraints:** Form with email, password, confirm password, name. Submit calls `authStore.register()`. Show validation errors. Redirect to `/` on success.

**Expected Output:** `RegisterView.vue`.

**Acceptance Criteria:**
- [ ] File exists at `src/views/RegisterView.vue`
- [ ] Form with required fields
- [ ] Submit triggers registration action
- [ ] Validation errors displayed
- [ ] Redirects to `/` on success

**Stopping Condition:** After creating, STOP. Wait for Orchestrator approval.

---

## Prompt T-120: Add Auth Endpoints to API

**Context:** API must become the authentication authority, issuing JWTs directly for login and registration.

**Objective:** Implement `POST /api/auth/login` and `POST /api/auth/register` in `AuthController`.

**Current Project State:** `RegisterView` created (T-119 complete).

**Inputs:** `JwtTokenService`, user store/database.

**Constraints:** `POST /api/auth/login`: validate credentials, generate JWT, return token. `POST /api/auth/register`: create user, generate JWT, return token. Use same signing key. Password hashing must be compatible with legacy or migration script documented.

**Expected Output:** `AuthController.cs` and modified `Program.cs` (if Identity registration needed).

**Acceptance Criteria:**
- [ ] `POST /api/auth/login` returns valid JWT for correct credentials
- [ ] `POST /api/auth/login` returns 401 for incorrect credentials
- [ ] `POST /api/auth/register` creates user and returns JWT
- [ ] Token validates with existing JWT middleware
- [ ] `dotnet build` succeeds

**Stopping Condition:** After implementing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-121: Update Auth Store to Use API Endpoints

**Context:** Auth store must switch from legacy JWT bridge to direct API authentication.

**Objective:** Modify `useAuthStore` to call new API auth endpoints instead of `/Account/IssueToken`.

**Current Project State:** Auth endpoints implemented (T-120 complete).

**Inputs:** `useAuthStore.ts`, new API endpoints.

**Constraints:** `login(email, password)` calls `POST /api/auth/login`. `register(data)` calls `POST /api/auth/register`. Remove `fetchToken` method that called legacy bridge. Keep `logout` and `isAuthenticated`. Token still persisted to localStorage.

**Expected Output:** Modified `useAuthStore.ts`.

**Acceptance Criteria:**
- [ ] `login` calls `POST /api/auth/login`
- [ ] `register` calls `POST /api/auth/register`
- [ ] Legacy `fetchToken` method removed
- [ ] Token still validates correctly with API
- [ ] `logout` still clears token

**Stopping Condition:** After modifying and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-122: Remove Legacy Fallback from YARP

**Context:** Legacy app no longer needed for production traffic. Remove YARP fallback route.

**Objective:** Remove the `legacyCluster` catch-all route. Route `/*` to `spaCluster` (Vue Router handles 404s).

**Current Project State:** Auth store updated (T-121 complete).

**Inputs:** `appsettings.json` YARP config.

**Constraints:** Remove `legacyCluster` and `legacy` route. Update `/*` catch-all to point to `spaCluster`. Keep `/api/*` route to `apiCluster`. Keep `/Account/*` route to legacy temporarily (or remove if auth is fully on API).

**Expected Output:** Updated `appsettings.json`.

**Acceptance Criteria:**
- [ ] Legacy fallback route removed
- [ ] `/*` routes to `spaCluster`
- [ ] `/api/*` still routes to `apiCluster`
- [ ] `http://localhost:5003/nonexistent` returns Vue SPA (Vue Router 404)
- [ ] Legacy app no longer receives production traffic

**Stopping Condition:** After removing and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-123: Shut Down Legacy Application

**Context:** Legacy MVC app is no longer needed. Shut it down in all environments.

**Objective:** Stop IIS application pool or process for legacy MVC.

**Current Project State:** Legacy fallback removed (T-122 complete).

**Inputs:** IIS / app pool configuration.

**Constraints:** Document the shutdown. Confirm no process running. This is an infrastructure operation.

**Expected Output:** Shutdown confirmation.

**Acceptance Criteria:**
- [ ] Legacy site returns 503 or connection refused
- [ ] Monitoring confirms no legacy process running
- [ ] Shutdown documented

**Stopping Condition:** After shutting down and confirming, STOP. Wait for Orchestrator approval.

---

## Prompt T-124: Consolidate Databases (Optional)

**Context:** Optionally merge `LegacyEcommerceIdentity` tables into `LegacyEcommerceDb` for single connection string.

**Objective:** Create migration script to move identity tables and update API connection string.

**Current Project State:** Legacy app shut down (T-123 complete).

**Inputs:** Both databases, `appsettings.json`.

**Constraints:** Create SQL script to copy identity tables. Test restore on separate instance. Update `appsettings.json` with single connection string. This task is optional — can be skipped if keeping two databases.

**Expected Output:** Migration script and updated config.

**Acceptance Criteria:**
- [ ] SQL migration script exists
- [ ] Script tested on separate instance
- [ ] All identity tables present in business DB
- [ ] API authenticates using single connection string
- [ ] `dotnet build` succeeds

**Stopping Condition:** After creating and verifying, STOP. Wait for Orchestrator approval.

---

## Prompt T-125: Run Final Full E2E Regression Suite

**Context:** Final verification of the entire modernized stack.

**Objective:** Execute all Playwright E2E specs in sequence: homepage → product → cart → checkout → orders → logout → login.

**Current Project State:** Database consolidation complete or skipped (T-124 complete or T-123 if skipped).

**Inputs:** All E2E spec files, running services.

**Constraints:** Execute every E2E spec. Document results. No console errors allowed. 100% pass rate required.

**Expected Output:** Final E2E report at `docs/migration/final-e2e-report.md`.

**Acceptance Criteria:**
- [ ] All E2E specs execute (homepage, product-catalog, cart, checkout, order-history)
- [ ] 100% pass rate
- [ ] No console errors in browser
- [ ] Report file exists with results documented

**Stopping Condition:** After running full suite and documenting results, STOP. This is the final task. The project is complete. Report to Orchestrator for final sign-off.

---

## End of Prompt Library

**Total Prompts:** 125
**Execution Mode:** Strictly sequential
**Approval Required:** After every prompt
**No Auto-Continuation:** Every prompt stops and waits for Orchestrator approval