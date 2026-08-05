# Implementation Roadmap

Task-by-task checklist with file names, shell commands, dependencies, and completion criteria for every phase of the migration.

---

## Phase 0: Infrastructure & Proxy Setup

**Goal:** YARP routes traffic, API shell runs, Vue shell renders, JWT bridge works, database connectivity verified.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 0.1 | Create `modern/` directory | `mkdir modern` | None | `modern/` exists at repo root. |
| 0.2 | Create .NET 10 API solution & project | `cd modern; dotnet new sln -n Ecommerce; dotnet new webapi -n Ecommerce.Api -o Ecommerce.Api --framework net10.0` | 0.1 | `modern/Ecommerce.sln` and `modern/Ecommerce.Api/Ecommerce.Api.csproj` exist. `dotnet --version` prints `10.x`. |
| 0.3 | Add YARP to API | Edit `modern/Ecommerce.Api/Ecommerce.Api.csproj`: add `PackageReference Include="Yarp.ReverseProxy" Version="2.2.0"` (or latest stable). | 0.2 | `csproj` contains YARP package. `dotnet restore` succeeds. |
| 0.4 | Configure YARP routes | Create/edit `modern/Ecommerce.Api/Program.cs`: add `builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));` and `app.MapReverseProxy();`. | 0.3 | API project builds (`dotnet build` zero errors). |
| 0.5 | Add YARP `appsettings.json` routes | Edit `modern/Ecommerce.Api/appsettings.json` to add `ReverseProxy` section with clusters for `apiCluster` (`localhost:5001`), `spaCluster` (`localhost:5002`), `legacyCluster` (`localhost:5000`) and routes for `/api/*`, `/app/*`, `/*` catch-all. | 0.4 | `appsettings.json` contains valid `ReverseProxy` JSON. |
| 0.6 | Verify YARP builds & runs | `cd modern/Ecommerce.Api; dotnet run` | 0.5 | Process starts on port 5003 (or configured port). Visiting `http://localhost:5003/api/health` returns 200. Visiting `http://localhost:5003/` falls through to legacy (or 502 if legacy not running yet — acceptable). |
| 0.7 | Add shared projects to solution | `dotnet new classlib -n Ecommerce.Api.Core -o Ecommerce.Api.Core --framework net10.0`<br>`dotnet new classlib -n Ecommerce.Api.Services -o Ecommerce.Api.Services --framework net10.0`<br>`dotnet new classlib -n Ecommerce.Api.Data -o Ecommerce.Api.Data --framework net10.0`<br>`dotnet sln add Ecommerce.Api.Core/Ecommerce.Api.Core.csproj`<br>`dotnet sln add Ecommerce.Api.Services/Ecommerce.Api.Services.csproj`<br>`dotnet sln add Ecommerce.Api.Data/Ecommerce.Api.Data.csproj` | 0.2 | Solution contains 4 projects. `dotnet build` at solution level succeeds. |
| 0.8 | Wire project references | `Ecommerce.Api` → `Ecommerce.Api.Services`, `Ecommerce.Api.Core`<br>`Ecommerce.Api.Services` → `Ecommerce.Api.Core`, `Ecommerce.Api.Data`<br>`Ecommerce.Api.Data` → `Ecommerce.Api.Core` | 0.7 | `dotnet build` succeeds. No circular references. |
| 0.9 | Port `DatabaseHelper` | Copy `Ecommerce.Data/DatabaseHelper.cs` to `modern/Ecommerce.Api.Data/DatabaseHelper.cs`. Replace `using System.Data.SqlClient;` with `using Microsoft.Data.SqlClient;`. Add package `Microsoft.Data.SqlClient` to `Ecommerce.Api.Data.csproj`. | 0.8 | `DatabaseHelper.cs` compiles. `dotnet build` succeeds. |
| 0.10 | Test database connectivity | Create a quick health-check endpoint in `Ecommerce.Api/Controllers/HealthController.cs`:<br>`[HttpGet("health")] public IActionResult Get() { _db.ExecuteScalar("SELECT 1"); return Ok(); }` | 0.9 | `GET /api/health` returns 200. Logs show successful SQL connection. |
| 0.11 | Add JWT NuGet packages | In `Ecommerce.Api.csproj`: add `Microsoft.AspNetCore.Authentication.JwtBearer`.<br>In legacy `Ecommerce.Web` (via NuGet Package Manager or `packages.config`): add `System.IdentityModel.Tokens.Jwt`. | 0.2 | Both projects restore successfully. |
| 0.12 | Implement JWT token service (new API) | Create `modern/Ecommerce.Api.Core/Security/JwtTokenService.cs` with `GenerateToken(userId, customerId, email)` using symmetric key. | 0.11 | Unit test: `GenerateToken` returns non-null string; `ValidateToken` returns correct claims. |
| 0.13 | Configure JWT auth in API | Edit `modern/Ecommerce.Api/Program.cs`: add `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` with signing key from `appsettings.json`. Add `app.UseAuthentication(); app.UseAuthorization();`. | 0.12 | `[Authorize]` on a test controller returns 401 without token, 200 with valid token. |
| 0.14 | Add JWT bridge endpoint to legacy MVC | Edit `Ecommerce.Web/Controllers/AccountController.cs`: add `IssueToken()` action that reads cookie auth user, resolves `CustomerId` claim, calls legacy-side token generator, returns `{ token }` JSON. | 0.11 | Logged-in user visiting `/Account/IssueToken` returns JSON with JWT. Anonymous user gets 302 to login. |
| 0.15 | Scaffold Vue SPA | `cd modern; npm create vue@latest Ecommerce.Spa` (select Vue 3, TypeScript, Pinia, Vue Router, Vitest). OR: `npm init vite@latest Ecommerce.Spa -- --template vue-ts`. | 0.1 | `modern/Ecommerce.Spa/package.json` exists. `npm install` succeeds. `npm run dev` starts Vite dev server on `localhost:5173` (or 5002 if configured). |
| 0.16 | Add Pinia & Vue Router | `cd modern/Ecommerce.Spa; npm install pinia vue-router` | 0.15 | `package.json` lists `pinia` and `vue-router`. `npm run dev` still starts cleanly. |
| 0.17 | Configure API base URL | Edit `modern/Ecommerce.Spa/.env.development`: `VITE_API_BASE_URL=http://localhost:5001/api`. | 0.15 | `import.meta.env.VITE_API_BASE_URL` resolves correctly in dev. |
| 0.18 | Create `fetch` wrapper with JWT | Create `modern/Ecommerce.Spa/src/api/client.ts`: exports `get`, `post`, `put`, `del` functions that read token from Pinia `useAuthStore` and attach `Authorization: Bearer <token>`. | 0.16, 0.13 | API calls from Vue include correct `Authorization` header when token exists. |
| 0.19 | Create Pinia auth store | Create `modern/Ecommerce.Spa/src/stores/useAuthStore.ts`: holds `token`, `isAuthenticated`, `fetchToken()` (calls `/Account/IssueToken` via proxy), `logout()`. | 0.16, 0.14 | Store initializes, fetches token successfully, persists to `localStorage`. |
| 0.20 | Verify full proxy chain | Start all 4 services (Legacy 5000, API 5001, SPA 5002, YARP 5003). Visit `http://localhost:5003/app/`. | 0.6, 0.15, 0.19 | Vue app loads. Auth store fetches JWT. A test API call (e.g., `GET /api/health`) succeeds with Bearer token. |

**Phase 0 Gate:** All tasks 0.1–0.20 complete. `dotnet test` (if any tests written) passes. Playwright can load `http://localhost:5003/app/` and see the Vue default page.

---

## Phase 1: Product Catalog Vertical Slice

**Goal:** Product listing and detail pages fully served by Vue + API. Legacy product routes bypassed via YARP.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 1.1 | Port domain models | Copy `Ecommerce.Core/Models/Product.cs`, `Category.cs`, `ProductImage.cs`, `ProductVariant.cs` to `modern/Ecommerce.Api.Core/Models/`. | 0.8 | Models compile. No EF references (they're POCOs). |
| 1.2 | Create DTOs | Create `modern/Ecommerce.Api.Core/DTOs/ProductDto.cs`, `ProductDetailDto.cs`, `CategoryDto.cs`, `PagedResult.cs`. | 1.1 | DTOs compile. No circular references. |
| 1.3 | Port repository interfaces | Copy `Ecommerce.Core/Interfaces/IProductRepository.cs`, `ICategoryRepository.cs` to `modern/Ecommerce.Api.Core/Interfaces/`. | 0.8 | Interfaces compile. |
| 1.4 | Port repository implementations | Copy `Ecommerce.Data/Repositories/ProductRepository.cs`, `CategoryRepository.cs` to `modern/Ecommerce.Api.Data/Repositories/`. Replace `System.Data.SqlClient` → `Microsoft.Data.SqlClient`. | 0.9, 1.3 | Repositories compile. `dotnet build` succeeds. |
| 1.5 | Port service interfaces | Copy `Ecommerce.Core/Interfaces/IProductService.cs`, `ICategoryService.cs` to `modern/Ecommerce.Api.Services/Interfaces/`. | 1.3 | Interfaces compile. |
| 1.6 | Port service implementations | Copy `Ecommerce.Services/ProductService.cs`, `CategoryService.cs` to `modern/Ecommerce.Api.Services/Implementations/`. Change namespaces. Replace ViewModel usage with DTO mapping. | 1.4, 1.5 | Services compile. `dotnet build` succeeds. |
| 1.7 | Register DI services | Edit `modern/Ecommerce.Api/Program.cs`: `builder.Services.AddScoped<IProductRepository, ProductRepository>();` etc. | 1.6 | API starts. DI container resolves `IProductService` without error. |
| 1.8 | Create `ProductsController` | Create `modern/Ecommerce.Api/Controllers/ProductsController.cs`:<br>`GET /api/products` (pagination, filter, sort)<br>`GET /api/products/{id}` (detail) | 1.6, 1.2 | Controller returns JSON. Swagger/Postman test returns expected DTO shape. |
| 1.9 | Create `CategoriesController` | Create `modern/Ecommerce.Api/Controllers/CategoriesController.cs`:<br>`GET /api/categories` | 1.6 | Returns `CategoryDto[]`. |
| 1.10 | Add YARP product routes | Edit `appsettings.json` (or `Program.cs`): route `/products*` and `/product/*` → `spaCluster`. | 0.5 | Visiting `http://localhost:5003/products` serves Vue SPA (not legacy MVC). |
| 1.11 | Create `ProductListView.vue` | `modern/Ecommerce.Spa/src/views/ProductListView.vue`: fetches `GET /api/products`, renders grid, handles query params (`?categoryId=&page=`). | 0.18, 1.8 | Grid renders with real data from API. |
| 1.12 | Create `ProductDetailView.vue` | `modern/Ecommerce.Spa/src/views/ProductDetailView.vue`: fetches `GET /api/products/{id}`, renders gallery, variants, related products. | 0.18, 1.8 | Detail page renders with real data. URL `/products/123` works. |
| 1.13 | Create reusable components | `ProductCard.vue`, `CategoryFilter.vue`, `PaginationControls.vue`, `LoadingSpinner.vue`, `ErrorAlert.vue`. | 1.11 | Components render correctly in Storybook or Vitest. |
| 1.14 | Create Pinia product store | `modern/Ecommerce.Spa/src/stores/useProductStore.ts`: manages filter state, pagination, product cache. | 0.16 | Store updates URL query params reactively. Filters trigger refetch. |
| 1.15 | Configure Vue Router | `modern/Ecommerce.Spa/src/router/index.ts`: routes `/products` → `ProductListView.vue`, `/products/:id` → `ProductDetailView.vue`. | 0.16 | Router navigation works. Browser back/forward preserves state. |
| 1.16 | Backend unit tests — services | `modern/Ecommerce.Api.Tests/Services/ProductServiceTests.cs`: mock `IUnitOfWork`/`IProductRepository`, verify pagination logic. | 1.6 | `dotnet test` passes. |
| 1.17 | Backend unit tests — controllers | `modern/Ecommerce.Api.Tests/Controllers/ProductsControllerTests.cs`: mock `IProductService`, verify 200/404 responses. | 1.8 | `dotnet test` passes. |
| 1.18 | Frontend unit tests — components | `modern/Ecommerce.Spa/tests/unit/components/ProductCard.spec.ts`: verify props rendering, emit events. | 1.13 | `npm run test:unit` passes. |
| 1.19 | Frontend unit tests — store | `modern/Ecommerce.Spa/tests/unit/stores/useProductStore.spec.ts`: verify fetch calls correct URL. | 1.14 | `npm run test:unit` passes. |
| 1.20 | E2E test — product catalog | `modern/Ecommerce.Spa/tests/e2e/product-catalog.spec.ts`: visit `/products`, filter by category, click product, assert detail page. | 1.11, 1.12 | `npx playwright test` passes. |

**Phase 1 Gate:** All tasks 1.1–1.20 complete. Users can browse products entirely through Vue + API. Legacy product pages still work at `localhost:5000` but are bypassed by YARP. Test suites green.

---

## Phase 2: Shopping Cart Vertical Slice

**Goal:** Cart add/update/remove fully in Vue + API. Auth bridge required.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 2.1 | Port cart domain models | Copy `CartItem.cs`, `ProductVariant.cs` (if not already) to `modern/Ecommerce.Api.Core/Models/`. | 1.1 | Models compile. |
| 2.2 | Create cart DTOs | `CartItemDto.cs`, `CartSummaryDto.cs`, `AddCartItemRequest.cs` in `modern/Ecommerce.Api.Core/DTOs/`. | 2.1 | DTOs compile. |
| 2.3 | Port cart repository | Copy `ICartItemRepository`, `CartItemRepository` to new projects. | 1.3, 0.9 | Compiles. |
| 2.4 | Port `CartService` | Copy `Ecommerce.Services/CartService.cs` to `modern/Ecommerce.Api.Services/Implementations/`. Update namespace. Change return types to DTOs. | 2.3, 1.5 | `dotnet build` succeeds. |
| 2.5 | Create `CartController` | `modern/Ecommerce.Api/Controllers/CartController.cs` (all actions require `[Authorize]`):<br>`GET /api/cart`<br>`POST /api/cart/items`<br>`PUT /api/cart/items/{id}`<br>`DELETE /api/cart/items/{id}`<br>`GET /api/cart/summary` | 2.4, 0.13 | Authenticated requests return 200. Anonymous returns 401. |
| 2.6 | Create `AddToCartButton.vue` | `modern/Ecommerce.Spa/src/components/AddToCartButton.vue`: emits event or calls `cartStore.addItem()`. | 0.18 | Button click triggers API call with Bearer token. |
| 2.7 | Create `CartView.vue` | `modern/Ecommerce.Spa/src/views/CartView.vue`: fetches cart, renders table, handles quantity change & remove. | 0.18, 2.5 | Cart renders with real data. Quantity update persists. |
| 2.8 | Create `CartSummaryBadge.vue` | `modern/Ecommerce.Spa/src/components/CartSummaryBadge.vue`: displays `itemCount` from `useCartStore`. Placed in `TheHeader.vue`. | 2.7 | Badge updates when items added. |
| 2.9 | Create Pinia cart store | `modern/Ecommerce.Spa/src/stores/useCartStore.ts`: `fetchCart`, `addItem`, `updateItem`, `removeItem`. | 0.16 | Store methods call correct API endpoints. |
| 2.10 | Update Vue Router | Add `/cart` route → `CartView.vue`. | 1.15 | Navigation to `/cart` works. |
| 2.11 | Update YARP routes | Route `/cart*` → `spaCluster`. | 1.10 | `/cart` hits Vue SPA. |
| 2.12 | Backend tests — `CartController` | Mock `ClaimsPrincipal` with `customer_id` claim. Verify 401 without auth, 200 with auth. Verify correct `customerId` passed to service. | 2.5 | `dotnet test` passes. |
| 2.13 | Backend tests — `CartService` | Mock `ICartItemRepository`. Verify add, update, remove logic. | 2.4 | `dotnet test` passes. |
| 2.14 | Frontend tests — `AddToCartButton` | Vitest: click emits event, calls store method. | 2.6 | `npm run test:unit` passes. |
| 2.15 | Frontend tests — `CartView` | Vitest: renders items, handles quantity emit. | 2.7 | `npm run test:unit` passes. |
| 2.16 | E2E test — cart flow | Playwright: add to cart from product detail → visit cart → update qty → remove → verify empty. | 2.7 | `npx playwright test` passes. |

**Phase 2 Gate:** Cart fully functional in Vue + API. Auth bridge verified. Tests green.

---

## Phase 3: Checkout Vertical Slice

**Goal:** Multi-step checkout (address → review → place order) in Vue + API. Highest risk slice.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 3.1 | Port address/order models | Copy `Address.cs`, `Order.cs`, `OrderItem.cs`, `Customer.cs` to `modern/Ecommerce.Api.Core/Models/`. | 2.1 | Models compile. |
| 3.2 | Create checkout DTOs | `AddressDto.cs`, `OrderDto.cs`, `OrderItemDto.cs`, `PlaceOrderRequest.cs`, `OrderConfirmationDto.cs` in `DTOs/`. | 3.1 | DTOs compile. |
| 3.3 | Port address/order repositories | Copy `IAddressRepository`, `IOrderRepository`, `ICustomerRepository` + implementations. | 0.9 | Compiles. |
| 3.4 | Port `CheckoutService` verbatim | Copy `Ecommerce.Services/CheckoutService.cs` to `modern/Ecommerce.Api.Services/Implementations/`. **Do not refactor.** Change only namespaces and return DTOs at the end. | 3.3, 2.4 | `dotnet build` succeeds. Transaction and idempotency logic intact. |
| 3.5 | Port `OrderService` | Copy `Ecommerce.Services/OrderService.cs` to new project. | 3.3 | Compiles. |
| 3.6 | Create `CheckoutController` | `modern/Ecommerce.Api/Controllers/CheckoutController.cs` (all `[Authorize]`):<br>`GET /api/checkout/addresses`<br>`POST /api/checkout/addresses`<br>`GET /api/checkout/review`<br>`POST /api/checkout/orders` (reads `Idempotency-Key` header)<br>`GET /api/checkout/orders/{id}/confirmation` | 3.4, 0.13 | Endpoints return correct JSON. `PlaceOrder` enforces idempotency header. |
| 3.7 | Create `OrdersController` (read-only) | `GET /api/orders`, `GET /api/orders/{id}`. | 3.5 | Returns order data for authenticated customer. |
| 3.8 | Create `CheckoutWizard.vue` | `modern/Ecommerce.Spa/src/views/CheckoutWizard.vue`: stepper component (AddressStep → ReviewStep → ConfirmationStep). | 0.16 | Step navigation works. State persists across steps. |
| 3.9 | Create `AddressStep.vue` | Includes `AddressSelector.vue` (saved addresses) + `AddressForm.vue` (create new). | 3.8 | Can select existing or create new address. |
| 3.10 | Create `ReviewStep.vue` | Displays cart summary + selected address + total. "Place Order" button. | 3.8 | Renders order preview correctly. |
| 3.11 | Create `OrderConfirmation.vue` | Success page showing order number. | 3.8 | Displays order ID after successful placement. |
| 3.12 | Create Pinia checkout store | `modern/Ecommerce.Spa/src/stores/useCheckoutStore.ts`: holds `selectedAddress`, `idempotencyKey` (UUID generated on checkout start). | 0.16 | Store generates UUID. Persists across wizard steps. |
| 3.13 | Update Vue Router | Add `/checkout` → `CheckoutWizard.vue`, `/checkout/confirmation/:orderId` → `OrderConfirmation.vue`. | 2.10 | Routes work. Guards prevent direct access to later steps. |
| 3.14 | Update YARP routes | Route `/checkout*` → `spaCluster`. | 2.11 | `/checkout` hits Vue SPA. |
| 3.15 | Backend tests — `CheckoutService` | xUnit: mock `IUnitOfWork`, verify:<br>• Transaction committed exactly once<br>• Duplicate idempotency key returns same order without DB writes<br>• Address created when `addressId` is null | 3.4 | `dotnet test` passes. |
| 3.16 | Backend tests — `CheckoutController` | Verify 400 when `Idempotency-Key` missing. Verify 201 on first order, 200 on duplicate. | 3.6 | `dotnet test` passes. |
| 3.17 | Frontend tests — `CheckoutWizard` | Vitest: step navigation, form validation, submit sends correct headers. | 3.8 | `npm run test:unit` passes. |
| 3.18 | E2E test — full checkout | Playwright: cart → checkout → select/create address → review → place order → confirmation → verify DB has order + items + cart cleared. | 3.8 | `npx playwright test` passes. Order row exists in SQL Server. |

**Phase 3 Gate:** Complete checkout works end-to-end. Idempotency verified. Cart clears after order. Tests green.

---

## Phase 4: Order History & Account Management

**Goal:** Order list and detail in Vue. Account pages remain legacy during hybrid phase.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 4.1 | Create `OrderListView.vue` | `modern/Ecommerce.Spa/src/views/OrderListView.vue`: fetches `GET /api/orders`, renders table with pagination. | 3.7, 0.18 | Renders real order history. |
| 4.2 | Create `OrderDetailView.vue` | `modern/Ecommerce.Spa/src/views/OrderDetailView.vue`: fetches `GET /api/orders/{id}`, renders items, shipping info. | 3.7 | Renders order details correctly. |
| 4.3 | Update Vue Router | Add `/orders` → `OrderListView.vue`, `/orders/:id` → `OrderDetailView.vue`. | 3.13 | Routes work. |
| 4.4 | Update YARP routes | Route `/orders*` → `spaCluster`. | 3.14 | `/orders` hits Vue SPA. |
| 4.5 | Frontend tests — order components | Vitest for `OrderListView.vue` and `OrderDetailView.vue`. | 4.1, 4.2 | `npm run test:unit` passes. |
| 4.6 | E2E test — order history | Playwright: place order (from Phase 3 E2E) → visit `/orders` → click order → verify detail page. | 4.1 | `npx playwright test` passes. |
| 4.7 | **Account pages — STAY LEGACY** | Do NOT migrate Login/Register/Manage yet. Ensure Vue header links to `/Account/Login` (legacy route via YARP fallback). | 0.14 | Links work. JWT bridge keeps SPA authenticated after login. |

**Phase 4 Gate:** Order history fully in Vue. Account pages still legacy but seamlessly linked. Tests green.

---

## Phase 5: Home, Search & Miscellaneous

**Goal:** Remaining read-only pages migrated.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 5.1 | Create `HomeView.vue` | `modern/Ecommerce.Spa/src/views/HomeView.vue`: featured products, hero banner. Fetches `GET /api/products?featured=true` (or dedicated endpoint). | 1.11 | Homepage renders. |
| 5.2 | Create search functionality | Add `GET /api/products?searchQuery=` (already exists from Phase 1). Add search input to `TheHeader.vue`. | 1.8 | Search returns filtered results. |
| 5.3 | Update Vue Router | `/` → `HomeView.vue`. | 4.3 | Root path serves Vue homepage. |
| 5.4 | Update YARP routes | Route `/` and `/home*` → `spaCluster`. | 4.4 | `http://localhost:5003/` serves Vue SPA. |
| 5.5 | Frontend tests | Vitest for `HomeView.vue`. | 5.1 | `npm run test:unit` passes. |
| 5.6 | E2E test — homepage | Playwright: visit `/`, verify featured products render. | 5.1 | `npx playwright test` passes. |

**Phase 5 Gate:** All public read-only pages in Vue. Legacy fallback only for account/admin routes. Tests green.

---

## Phase 6: Decommission Legacy

**Goal:** Remove legacy MVC app. Migrate auth to API.

| # | Task | Files / Commands | Dependencies | Completion Criteria |
|---|------|------------------|--------------|---------------------|
| 6.1 | Verify zero legacy traffic | Check YARP logs/metrics for 1+ week: no requests hitting `legacyCluster` fallback except `/Account/*`. | 5.4 | Log analysis confirms no unexpected legacy hits. |
| 6.2 | Build Vue login page | `modern/Ecommerce.Spa/src/views/LoginView.vue`: form calling new API endpoint. | 0.16 | UI renders. |
| 6.3 | Build Vue register page | `modern/Ecommerce.Spa/src/views/RegisterView.vue`. | 0.16 | UI renders. |
| 6.4 | Add ASP.NET Core Identity to API | Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (or keep using raw ADO.NET if schema stays identical). Add `POST /api/auth/login`, `POST /api/auth/register` endpoints that issue JWTs. | 0.13 | Endpoints return valid JWTs. Password hashes compatible or migration script run. |
| 6.5 | Update auth store | `useAuthStore.ts`: switch from calling `/Account/IssueToken` to `POST /api/auth/login`. | 6.4 | Login flow works without legacy app. |
| 6.6 | Remove YARP legacy fallback | Edit YARP config: remove `legacyCluster` catch-all. Route remaining unmatched paths → `spaCluster` (Vue Router handles 404s). | 6.1 | `/*` serves Vue SPA. Legacy app can be shut down. |
| 6.7 | Shut down legacy MVC | Stop IIS/app pool for legacy site. | 6.6 | Legacy no longer running. |
| 6.8 | (Optional) Merge databases | Migrate `LegacyEcommerceIdentity` tables into `LegacyEcommerceDb` if desired. | 6.7 | Single connection string in API. |
| 6.9 | Final E2E regression | Playwright full suite: homepage → product → cart → checkout → orders → logout → login. | All prior phases | Entire suite passes against new stack only. |

**Phase 6 Gate:** Legacy app decommissioned. All traffic served by Vue + API. Full test suite green.

---

## Cross-Cutting Tasks (Ongoing)

| # | Task | Frequency | Files | Completion Criteria |
|---|------|-----------|-------|---------------------|
| CC.1 | **Code reviews** | Every phase | PRs in `modern/` branch | At least one review per controller + service + Vue view. |
| CC.2 | **Database backup** | Before Phase 3, 6 | SQL Server `.bak` | Verified restore on separate instance. |
| CC.3 | **Dependency updates** | Monthly | `package.json`, `*.csproj` | `npm audit` and `dotnet list package --vulnerable` show zero high-severity issues. |
| CC.4 | **Documentation updates** | Per phase | `README.md`, `migration.md` | Docs reflect current architecture and port status. |
| CC.5 | **CI pipeline setup** | Phase 0–1 | `.github/workflows/ci.yml` or Azure DevOps YAML | Build + test runs on every push. Backend `dotnet test`, frontend `vitest`, E2E `playwright test` all execute. |

---

## Quick Reference: Common Commands

```bash
# Start everything (4 terminals)
cd Ecommerce.Web        # IIS Express / local IIS on :5000
cd modern/Ecommerce.Api && dotnet run          # :5001
cd modern/Ecommerce.Spa && npm run dev          # :5002
cd modern/Ecommerce.Api && dotnet run           # :5003 (YARP host)

# Backend tests
cd modern/Ecommerce.Api.Tests && dotnet test --logger trx

# Frontend unit tests
cd modern/Ecommerce.Spa && npm run test:unit

# E2E tests
cd modern/Ecommerce.Spa && npx playwright test

# Build for production
cd modern/Ecommerce.Api && dotnet publish -c Release
cd modern/Ecommerce.Spa && npm run build
```

---

## Legend

- **File:** Path relative to repo root (`C:\Users\Administrator\Desktop\eCommerce\`).
- **Command:** PowerShell / cmd compatible where possible; `dotnet` and `npm` commands are cross-platform.
- **Dependency:** Task numbers that must be completed before this task starts.
- **Completion Criteria:** Concrete, verifiable condition. Usually a build success, a test pass, or a manual browser check.
