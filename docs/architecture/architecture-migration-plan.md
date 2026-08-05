# Architecture Migration Plan: Legacy .NET MVC 5 → Vue.js 3 + .NET 10 Web API

## 1. Executive Summary

**Goal:** Migrate the tightly-coupled ASP.NET MVC 5 eCommerce monolith to a decoupled architecture using the **Strangler Fig Pattern**, with a **YARP** reverse proxy routing traffic between the legacy MVC app and the modern stack.

**Current State:**
- ASP.NET MVC 5 on .NET Framework 4.7
- Razor server-side rendering
- ADO.NET + custom `DatabaseHelper` + Repository/UnitOfWork pattern
- ASP.NET Identity 2.0 with OWIN cookie authentication
- Two SQL Server databases: `LegacyEcommerceDb` (business data) + `LegacyEcommerceIdentity` (auth tables)
- 8 controllers, 5 service classes, 7 repository interfaces

**Target State:**
- **Backend:** .NET 10 Web API, stateless, JWT bearer authentication
- **Frontend:** Vue.js 3 (Composition API + `<script setup>`) built with Vite
- **Gateway:** YARP reverse proxy handling all ingress traffic
- **Communication:** RESTful JSON exclusively
- **Testing:** xUnit + Moq (backend), Vitest + Vue Test Utils (frontend), Playwright (E2E)

**Migration Strategy:** Strangler Fig — vertical slice by vertical slice, running old and new systems side-by-side until the legacy app can be decommissioned.

---

## 2. Current Architecture Analysis

### 2.1 Layer Breakdown

| Layer | Technology | Key Files |
|-------|-----------|-----------|
| **Presentation** | Razor MVC 5 + jQuery AJAX | `HomeController`, `ProductController`, `CartController`, `CheckoutController`, `OrderController`, `AccountController`, `CategoryController`, `ErrorController` |
| **View Layer** | Razor `.cshtml` + `_Layout.cshtml` | `Views/Product/Index.cshtml`, `Views/Product/Detail.cshtml`, `Views/Cart/Index.cshtml`, `Views/Checkout/Address.cshtml` |
| **Service Layer** | C# class library (`Ecommerce.Services`) | `ProductService`, `CartService`, `CategoryService`, `CheckoutService`, `OrderService` |
| **Repository Layer** | ADO.NET (`DatabaseHelper` + `SqlDataReader`) | `ProductRepository`, `CartItemRepository`, `CategoryRepository`, `OrderRepository`, `CustomerRepository`, `AddressRepository`, `Repository<T>` |
| **Unit of Work** | Transaction management | `IUnitOfWork`, `IUnitOfWorkTransaction`, `UnitOfWork` |
| **Data Access** | Custom ADO.NET helper | `DatabaseHelper.cs` — wraps `SqlConnection`, `SqlCommand`, parameterized queries |
| **Identity/Auth** | ASP.NET Identity 2.0 + OWIN | `ApplicationDbContext` (EF6), `ApplicationUser`, `IdentityConfig.cs`, `Startup.Auth.cs` |
| **DI Container** | Unity | `UnityConfig.cs` |

### 2.2 Coupling Points (Critical for Decoupling)

1. **Controller-View Tight Coupling:** Controllers return `ViewResult` with `ViewModel` objects shaped specifically for Razor rendering. Example: `ProductController.Index()` returns `ProductListViewModel` with pagination, category filters, and sorting state all pre-computed for the view.
2. **Auth Inside MVC Pipeline:** `AccountController` uses `SignInManager`, `UserManager`, and OWIN context directly. Controllers access `User.Identity.GetUserId()` to resolve the `CustomerId` claim.
3. **HttpContext Leakage:** `CheckoutService` relies on `OrderId` from session/query string (idempotency key pattern). The `CartService` indirectly accesses cart state through the repository layer, but UI state (like "selected address") lives in form posts.
4. **Shared Layout State:** `_Layout.cshtml` renders auth-dependent navigation, cart count, and category tree — all computed server-side on every request.
5. **DatabaseHelper is Hard-Wired:** The `DatabaseHelper` uses `System.Data.SqlClient` and is registered as a singleton in Unity. It manages connection strings from `Web.config`.

### 2.3 Database Schema (No Schema Changes Required)

- **Business DB (`LegacyEcommerceDb`):** `Products`, `Categories`, `CartItems`, `Orders`, `OrderItems`, `Customers`, `Addresses`, `ProductImages`, `ProductVariants`
- **Identity DB (`LegacyEcommerceIdentity`):** Standard ASP.NET Identity 2.0 tables (`AspNetUsers`, `AspNetRoles`, etc.)
- **Key Constraint:** The `ApplicationUser` model has a `CustomerId` claim that links the Identity user to the business `Customers` table.

---

## 3. Target Architecture Design

### 3.1 High-Level Topology

```
┌─────────────────────────────────────────────────────────────┐
│                         Client Browser                      │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                     YARP Reverse Proxy                        │
│                   (ASP.NET Core 10)                           │
│  ┌─────────────────┐  ┌─────────────────┐  ┌───────────────┐  │
│  │ /api/*          │  │ /app/*          │  │ /* (fallback) │  │
│  │ → .NET 10 API   │  │ → Vue.js SPA    │  │ → Legacy MVC  │  │
│  │   (localhost:   │  │   (localhost:   │  │   (localhost:  │  │
│  │    5001)        │  │    5002)        │  │    5000)      │  │
│  └─────────────────┘  └─────────────────┘  └───────────────┘  │
└─────────────────────────────────────────────────────────────┘
         │                         │                  │
         ▼                         ▼                  ▼
┌──────────────┐         ┌─────────────────┐  ┌──────────────┐
│ .NET 10 API  │         │   Vite Dev/Prod │  │ Legacy MVC   │
│              │         │   Server        │  │              │
│ • Controllers│         │                 │  │ • Controllers│
│ • Services   │         │ • Vue 3 SPA     │  │ • Razor Views│
│ • JWT Auth   │         │ • Pinia Store   │  │ • Cookie Auth│
│ • Same ADO.NET│        │ • Vue Router    │  │ • Unity DI   │
│   (ported)   │         │                 │  │              │
└──────┬───────┘         └─────────────────┘  └──────┬───────┘
       │                                               │
       └──────────────────┬────────────────────────────┘
                          │
                  ┌───────▼────────┐
                  │  SQL Server    │
                  │                │
                  │ LegacyEcommerceDb          │
                  │ LegacyEcommerceIdentity      │
                  └────────────────┘
```

### 3.2 Authentication Architecture (Hybrid Phase — Option A)

**Decision:** The legacy MVC app remains the **authentication authority** during the hybrid phase. It keeps ASP.NET Identity 2.0 + cookie auth. When a user is authenticated in the legacy app, the legacy app can issue a short-lived JWT that the Vue SPA stores (memory/localStorage) and presents to the .NET 10 API.

**JWT Bridge Mechanism:**
- Legacy MVC app adds an action `Account/IssueToken` (decorated with `[Authorize]`) that:
  1. Reads the current cookie-authenticated user.
  2. Resolves `CustomerId` from the user claims.
  3. Generates a JWT (using `System.IdentityModel.Tokens.Jwt` or `Microsoft.IdentityModel.Tokens`) with:
     - `sub` = user ID
     - `customer_id` = linked customer ID
     - `email` = user email
     - `exp` = 1 hour
  4. Returns the JWT as JSON.
- The Vue SPA calls this endpoint immediately after loading (or after login) to obtain a JWT.
- The .NET 10 API validates JWTs via `Microsoft.AspNetCore.Authentication.JwtBearer` using the same signing key.

**Why Option A?**
- No need to migrate Identity tables or password hashes immediately.
- The legacy app is the only system performing logins/registrations during the hybrid phase.
- The new API never needs to read the `LegacyEcommerceIdentity` database directly.
- Once the legacy app is decommissioned, the login/registration pages are rebuilt in Vue and the API can adopt Identity Core directly.

### 3.3 Technology Stack Details

| Component | Technology | Version/Package |
|-----------|-----------|-----------------|
| Reverse Proxy | YARP (Yet Another Reverse Proxy) | `Microsoft.ReverseProxy` / YARP NuGet |
| API Framework | ASP.NET Core | .NET 10 |
| API Auth | JWT Bearer | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| API Data Access | ADO.NET | `Microsoft.Data.SqlClient` (port of existing helper) |
| API DI | Built-in | `Microsoft.Extensions.DependencyInjection` |
| API Testing | xUnit + Moq | Latest stable |
| Frontend Framework | Vue.js | 3.4+ |
| Frontend Build | Vite | 5.x+ |
| Frontend State | Pinia | 2.x+ |
| Frontend Router | Vue Router | 4.x+ |
| Frontend HTTP | Native `fetch` or `axios` | Native preferred (one less dependency) |
| Frontend Testing | Vitest + Vue Test Utils | Latest stable |
| E2E Testing | Playwright | Latest stable |

---

## 4. Project Structure & Folder Layout

**Decision:** Create a new top-level solution folder `modern/` alongside the existing `Ecommerce.Web/`, `Ecommerce.Services/`, etc. The legacy code remains untouched until its slice is fully strangulated.

```
eCommerce/
├── Ecommerce.Web/                    # Legacy MVC 5 (untouched during migration)
├── Ecommerce.Services/               # Legacy services (will be ported)
├── Ecommerce.Data/                   # Legacy repositories + DatabaseHelper
├── Ecommerce.Core/                   # Legacy domain models + interfaces
├── Ecommerce.Tests/                  # Legacy tests (if any)
├── modern/                           # NEW: Modern stack
│   ├── Ecommerce.Api/                # .NET 10 Web API
│   │   ├── Controllers/              # API controllers (one per vertical slice)
│   │   ├── Middleware/               # JWT validation, exception handling
│   │   ├── appsettings.json          # Connection strings (same DBs)
│   │   ├── Program.cs
│   │   └── Ecommerce.Api.csproj
│   ├── Ecommerce.Api.Services/       # Ported + new service classes
│   │   ├── Interfaces/               # `IProductService`, `ICartService`, etc.
│   │   └── Implementations/          # `ProductService`, `CartService`, etc.
│   ├── Ecommerce.Api.Data/           # Ported repositories + DatabaseHelper
│   │   ├── DatabaseHelper.cs         # Ported to Microsoft.Data.SqlClient
│   │   ├── Repositories/
│   │   └── UnitOfWork.cs
│   ├── Ecommerce.Api.Core/           # Shared models, DTOs, enums
│   │   ├── DTOs/                     # `ProductDto`, `CartItemDto`, etc.
│   │   ├── Models/                   # Domain models (copied from Ecommerce.Core)
│   │   └── Mapping/                  # Extension methods: Model → DTO
│   ├── Ecommerce.Api.Tests/          # xUnit + Moq
│   │   ├── Controllers/
│   │   └── Services/
│   └── Ecommerce.Spa/                # Vue.js 3 + Vite frontend
│       ├── src/
│       │   ├── components/           # Vue components (one per slice)
│       │   ├── views/                # Page-level components
│       │   ├── stores/               # Pinia stores
│       │   ├── api/                  # API client functions (fetch wrappers)
│       │   ├── composables/          # Shared logic (auth, cart state)
│       │   ├── router/               # Vue Router config
│       │   └── App.vue
│       ├── tests/
│       │   ├── unit/                 # Vitest + Vue Test Utils
│       │   └── e2e/                  # Playwright
│       ├── index.html
│       ├── vite.config.ts
│       └── package.json
├── Migration_Assignment.pdf
└── README.md
```

**Rationale for this layout:**
- Keeps legacy and modern code physically separate.
- Allows the same repository to contain both systems during the hybrid phase.
- Each modern project mirrors its legacy counterpart (`Api.Services` ↔ `Services`, `Api.Data` ↔ `Data`) so porting is mechanical.
- The Vue SPA is a standard Vite project, completely decoupled from the backend build.

---

## 5. Migration Phases (Strangler Fig Pattern)

### Phase 0: Infrastructure & Proxy Setup (Week 1)
**Goal:** Lay the pipeline so the old and new systems can coexist.

1. **Create `modern/` solution structure** with .NET 10 API project shell.
2. **Port `DatabaseHelper`** from `System.Data.SqlClient` → `Microsoft.Data.SqlClient`. Verify it connects to `LegacyEcommerceDb`.
3. **Set up YARP** in a new .NET 10 host (or inside the API project):
   - Route `/api/*` → API (`localhost:5001`)
   - Route `/app/*` → Vue SPA (`localhost:5002` in dev, static files in prod)
   - Route `/*` (fallback) → Legacy MVC (`localhost:5000`)
4. **JWT Bridge on Legacy App:**
   - Add `Account/IssueToken` action to legacy `AccountController`.
   - Configure shared signing key (HMAC-SHA256, symmetric key stored in both `Web.config` and `Api/appsettings.json`).
5. **Vue SPA Shell:**
   - Scaffold Vite + Vue 3 + TypeScript (or plain JS — legacy codebase uses C#, no TypeScript precedent, but TS is recommended for API contract safety).
   - Add Pinia, Vue Router.
   - Add `fetch` wrapper that attaches JWT from Pinia auth store.
6. **Verification:** Visiting `http://localhost:5000/` still shows legacy site. Visiting `http://localhost:5000/app/` serves Vue shell. Visiting `http://localhost:5000/api/health` returns 200 from .NET 10 API.

### Phase 1: Product Catalog Vertical Slice (Weeks 2–3)
**Goal:** Migrate the **Product browsing experience** — the safest, read-heavy, no-auth-required slice.

**Legacy behavior:**
- `ProductController.Index()` — list products with category filter, sorting, pagination.
- `ProductController.Detail(int id)` — product details with variants, images.
- `CategoryController.GetCategories()` — AJAX endpoint for category tree.
- Views: `Product/Index.cshtml` (filter sidebar + grid), `Product/Detail.cshtml` (gallery + add-to-cart).

**Migration work:**
1. **API:** Create `ProductsController` in .NET 10 API:
   - `GET /api/products?page=&categoryId=&sortOrder=&searchQuery=` → returns `PagedResult<ProductDto>`
   - `GET /api/products/{id}` → returns `ProductDetailDto` (with variants, images, related products)
   - `GET /api/categories` → returns `CategoryDto[]`
2. **Service extraction:** Port `ProductService` and `CategoryService` from legacy to `Ecommerce.Api.Services`. Use the same repository interfaces; implementations stay in `Ecommerce.Api.Data`.
3. **Vue Components:**
   - `ProductListView.vue` — replaces `Product/Index.cshtml`
   - `ProductDetailView.vue` — replaces `Product/Detail.cshtml`
   - `ProductCard.vue` — reusable grid item
   - `CategoryFilter.vue` — sidebar category tree (replaces Razor partial)
   - `PaginationControls.vue` — reusable pagination
4. **Routing:** Add YARP rule: `/products*` → Vue SPA (`/app/products*`).
   - Legacy `ProductController` routes are still reachable directly at `localhost:5000`, but YARP intercepts the public ingress.
5. **State Management:** Pinia `useProductStore` manages filter state, pagination, and cached product lists.
6. **Testing:**
   - Backend: xUnit tests for `ProductsController` (mock `IProductService`), `ProductService` (mock `IUnitOfWork`).
   - Frontend: Vitest for `ProductCard.vue` (props rendering), `CategoryFilter.vue` (emit events on selection).
   - E2E: Playwright test that visits `/products`, filters by category, clicks a product, verifies detail page renders.

**Deliverable at end of Phase 1:** Users can browse the full product catalog through the Vue SPA + .NET 10 API. Legacy MVC product pages are still functional behind the proxy but no longer the primary path.

### Phase 2: Shopping Cart Vertical Slice (Weeks 4–5)
**Goal:** Migrate the **Cart** — introduces authenticated state (add to cart requires customer ID).

**Legacy behavior:**
- `CartController.Index()` — displays cart items with quantity controls.
- `CartController.AddToCart(productId, quantity, variantId)` — POST, redirects to cart.
- `CartController.UpdateQuantity(cartItemId, quantity)` — AJAX POST.
- `CartController.RemoveItem(cartItemId)` — AJAX POST.
- `CartController.GetCartSummary()` — AJAX for header mini-cart.
- View: `Cart/Index.cshtml`

**Migration work:**
1. **Auth requirement:** Cart endpoints need `customer_id` from JWT. The Vue SPA must obtain a JWT via the bridge before cart operations.
2. **API:** Create `CartController`:
   - `GET /api/cart` → returns `CartViewModelDto`
   - `POST /api/cart/items` → `{ productId, quantity, variantId }` → adds item
   - `PUT /api/cart/items/{id}` → `{ quantity }` → updates quantity
   - `DELETE /api/cart/items/{id}` → removes item
   - `GET /api/cart/summary` → `{ itemCount, totalAmount }` for header badge
3. **Service extraction:** Port `CartService`. No major logic changes.
4. **Vue Components:**
   - `CartView.vue` — replaces `Cart/Index.cshtml`
   - `AddToCartButton.vue` — reusable component (used in `ProductDetailView.vue` and `ProductCard.vue`)
   - `CartSummaryBadge.vue` — header component (replaces server-side cart count)
5. **State Management:** Pinia `useCartStore` manages cart items, persists nothing to localStorage (cart is server-side in DB), but keeps a reactive cache to avoid refetching.
6. **Testing:**
   - Backend: xUnit for `CartController` with authenticated user context (mock `ClaimsPrincipal`).
   - Frontend: Vitest for `AddToCartButton.vue` (click emits event, calls API mock).
   - E2E: Playwright test — add product to cart from detail page, verify cart page shows item, update quantity, verify total recalculates.

**Deliverable at end of Phase 2:** Full cart functionality in Vue + API. Users can add, update, remove items. Cart is no longer rendered server-side.

### Phase 3: Checkout Vertical Slice (Weeks 6–8)
**Goal:** Migrate the **Checkout** — the most complex business logic, requires full auth, addresses, idempotency.

**Legacy behavior:**
- Multi-step wizard: Address → Shipping → Payment (mock) → Review → Place Order.
- `CheckoutController.Address()` — GET/POST for address selection/creation.
- `CheckoutController.Review()` — displays order summary before placement.
- `CheckoutController.PlaceOrder()` — POST, uses `CheckoutService.PlaceOrderAsync` with:
  - Idempotency key (`OrderId` from session/query string)
  - UnitOfWork transaction
  - Address creation if new
  - Order + OrderItem creation
  - Cart clearing
- `CheckoutService` is the most complex service in the codebase.

**Migration work:**
1. **API:** Create `CheckoutController`:
   - `GET /api/checkout/addresses` → list customer addresses
   - `POST /api/checkout/addresses` → create new address
   - `GET /api/checkout/review` → order preview (from cart + selected address)
   - `POST /api/checkout/orders` → place order (idempotency key in header `Idempotency-Key`)
   - `GET /api/checkout/orders/{id}/confirmation` → order confirmation
2. **Service extraction:** Port `CheckoutService` **exactly** as-is, preserving:
   - UnitOfWork transaction boundaries
   - Idempotency check (`OrderRepository.GetByIdempotencyKeyAsync`)
   - Address creation logic
   - Order + OrderItem + Cart clearing sequence
3. **Vue Components:**
   - `CheckoutWizard.vue` — multi-step component (AddressStep → ShippingStep → ReviewStep → ConfirmationStep)
   - `AddressForm.vue` — address creation/editing
   - `AddressSelector.vue` — list of saved addresses
   - `OrderReview.vue` — summary before placing
   - `OrderConfirmation.vue` — success page
4. **Idempotency:** The legacy uses `OrderId` from session/query string. In the API, require clients to generate a UUID and send it as `Idempotency-Key` header. Store the mapping in the existing `Orders.IdempotencyKey` column.
5. **State Management:** Pinia `useCheckoutStore` holds selected address, shipping method, and generated idempotency key.
6. **Testing:**
   - Backend: xUnit for `CheckoutService` — mock `IUnitOfWork`, verify transaction commit/rollback, verify idempotency prevents duplicate orders.
   - Frontend: Vitest for `CheckoutWizard.vue` — step navigation, form validation, submit calls API with correct headers.
   - E2E: Playwright test — full checkout flow from cart to confirmation, verify order appears in database.

**Deliverable at end of Phase 3:** Complete checkout flow in Vue + API. This is the highest-risk slice; once this works, the core business value is preserved.

### Phase 4: Order History & Account Management (Weeks 9–10)
**Goal:** Migrate **Order viewing** and **Account pages**.

**Legacy behavior:**
- `OrderController.Index()` — list customer orders.
- `OrderController.Details(int id)` — order detail with items.
- `AccountController` — Login, Register, Manage profile, Change password.

**Migration work:**
1. **API:** Create `OrdersController`:
   - `GET /api/orders` → list orders for authenticated customer
   - `GET /api/orders/{id}` → order details
2. **Vue Components:**
   - `OrderListView.vue`
   - `OrderDetailView.vue`
3. **Account pages:** During hybrid phase, **keep account pages in legacy MVC**. The Vue SPA links back to `/Account/Login`, `/Account/Register`, `/Account/Manage` on the legacy app.
   - The JWT bridge ensures the SPA stays authenticated after login.
   - Only after the legacy app is fully decommissioned will Login/Register be rebuilt in Vue with ASP.NET Core Identity.
4. **Testing:** xUnit for `OrdersController`, Vitest for `OrderListView.vue`, Playwright for end-to-end order history navigation.

### Phase 5: Home, Search & Miscellaneous (Week 11)
**Goal:** Migrate remaining read-only pages.

- `HomeController.Index()` — featured products, banners.
- `CategoryController` breadcrumbs.
- `ErrorController`.
- Search functionality.

### Phase 6: Decommission Legacy (Week 12+)
**Goal:** Remove the legacy MVC app entirely.

**Prerequisites:**
- All user-facing routes are handled by Vue + API.
- No traffic hitting legacy fallback route in YARP for > 1 week (verified via logs).
- Admin/backoffice functions migrated (if any).

**Work:**
1. Remove YARP fallback route to legacy app.
2. Shut down legacy MVC app.
3. Migrate ASP.NET Identity 2.0 → ASP.NET Core Identity (if not already done during account slice).
4. Consolidate from two databases to one (optional — can keep separate indefinitely).

---

## 6. Vertical Slice Selection

### Phase 1 Slice: **Product Catalog**
**Why this slice:**
1. **Read-only / Low Risk:** No database mutations. A bug here doesn't corrupt data or lose money.
2. **No Authentication Required:** Browsing products is public. We don't need the JWT bridge fully working to ship this slice.
3. **High Visibility:** It's the homepage and primary user journey. Stakeholders see immediate value.
4. **Well-Bounded:** `ProductController` and `CategoryController` have clear inputs/outputs. The Razor views are complex but purely presentational.
5. **Proves the Pipeline:** End-to-end request flow (Browser → YARP → Vue → API → ADO.NET → SQL Server) is validated without auth complexity.

**What is included in the slice:**
- Product listing (grid, pagination, sorting, category filter, search)
- Product detail page (gallery, variants, description, related products)
- Category tree navigation
- Header search box

**What is NOT included (stays legacy during Phase 1):**
- "Add to Cart" button on product detail (can link to legacy cart or show as disabled until Phase 2)
- Customer reviews (if any)
- Wishlist

---

## 7. Business Logic Extraction Strategy

### 7.1 Principle: Controller → Service → API Controller

The legacy MVC controllers do three things:
1. **Orchestrate** services and repositories.
2. **Transform** domain models into ViewModels.
3. **Decide** which view to render.

In the new architecture:
- **Orchestration** moves to .NET 10 API Controllers.
- **Transformation** moves to API Controllers returning DTOs (instead of ViewModels).
- **View selection** disappears — the API returns JSON; the Vue component decides what to render.

### 7.2 Step-by-Step Extraction (Example: `ProductController`)

**Legacy:**
```csharp
public ActionResult Index(int? categoryId, string sortOrder, string searchQuery, int page = 1)
{
    var products = _productService.GetProducts(categoryId, sortOrder, searchQuery, page, PageSize);
    var categories = _categoryService.GetAllCategories();
    var viewModel = new ProductListViewModel
    {
        Products = products,
        Categories = categories,
        SelectedCategoryId = categoryId,
        SortOrder = sortOrder,
        SearchQuery = searchQuery,
        PageNumber = page,
        TotalPages = (int)Math.Ceiling(products.TotalCount / (double)PageSize)
    };
    return View(viewModel);
}
```

**Step 1 — Extract service method signature unchanged:**
`IProductService.GetProducts(int? categoryId, string sortOrder, string searchQuery, int page, int pageSize)` already exists. No change needed.

**Step 2 — Create DTO in `Ecommerce.Api.Core/DTOs`:**
```csharp
public record ProductDto(int Id, string Name, decimal Price, string ImageUrl, string CategoryName);
public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
public record ProductListResponseDto(
    IEnumerable<ProductDto> Products,
    IEnumerable<CategoryDto> Categories,
    int? SelectedCategoryId,
    string SortOrder,
    string SearchQuery,
    int PageNumber,
    int TotalPages
);
```

**Step 3 — New API Controller:**
```csharp
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    [HttpGet]
    public async Task<ActionResult<ProductListResponseDto>> GetProducts(
        [FromQuery] int? categoryId,
        [FromQuery] string sortOrder,
        [FromQuery] string searchQuery,
        [FromQuery] int page = 1)
    {
        // Same orchestration as legacy controller, but return DTO instead of ViewResult
    }
}
```

**Step 4 — Vue Component consumes DTO:**
`ProductListView.vue` fetches from `GET /api/products?categoryId=5&page=2` and renders the grid itself.

### 7.3 Critical Extraction: `CheckoutService`

`CheckoutService` is the most complex. It must be ported **verbatim** to preserve transactional integrity.

**Key preservation points:**
- `IUnitOfWork.BeginTransactionAsync()` → keep same interface.
- `IUnitOfWork.CommitAsync()` / `RollbackAsync()` → keep same semantics.
- `ICheckoutService.PlaceOrderAsync(int customerId, int? addressId, Address newAddress, string idempotencyKey)` → same signature, just change namespace to `Ecommerce.Api.Services`.
- The order of operations inside `PlaceOrderAsync` must not change:
  1. Check idempotency (query `Orders` table).
  2. Begin transaction.
  3. Create address if needed.
  4. Create `Order` entity.
  5. Create `OrderItem` entities from cart items.
  6. Clear cart.
  7. Commit transaction.
  8. Return `OrderConfirmationViewModel` (now `OrderConfirmationDto`).

**Note:** The `CheckoutService` is ~150 lines. Instead of refactoring it, copy-paste it into the new project and change `using` statements. It works. Don't fix what isn't broken during migration.

---

## 8. Authentication Coexistence Plan (Hybrid Phase)

### 8.1 The Problem
- Legacy: Cookie auth (`AspNet.ApplicationCookie`), ASP.NET Identity 2.0, OWIN pipeline.
- New API: JWT Bearer auth, stateless, no cookies.
- Both must recognize the same user during the hybrid phase.

### 8.2 Solution: JWT Bridge (Option A)

**Shared Secret:**
- A 32-byte symmetric key stored in:
  - Legacy `Web.config`: `<add key="JwtSigningKey" value="..."/>`
  - New API `appsettings.json`: `"JwtSigningKey": "..."`

**Legacy MVC — `AccountController.IssueToken()`:**
```csharp
[Authorize]
[HttpGet]
public ActionResult IssueToken()
{
    var userId = User.Identity.GetUserId();
    var customerId = User.Identity.GetCustomerId(); // from ApplicationUser claim
    var token = _jwtTokenService.GenerateToken(userId, customerId, User.Identity.Name);
    return Json(new { token }, JsonRequestBehavior.AllowGet);
}
```

**.NET 10 API — `Program.cs` JWT setup:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
```

**Vue SPA — Auth Flow:**
1. On app mount, check `localStorage` for existing JWT.
2. If none, call `GET /Account/IssueToken` (legacy MVC route, proxied through YARP).
   - If user is cookie-authenticated, legacy returns JWT.
   - If not authenticated, legacy redirects to login page (302). Vue detects this and redirects browser to `/Account/Login?returnUrl=/app/`.
3. Store JWT in `localStorage` (or better: memory-only Pinia store + `httpOnly` cookie if security is a concern; but assignment says JWT, so `localStorage` is acceptable for PoC).
4. Attach JWT as `Authorization: Bearer <token>` to all API calls.
5. On 401 from API, clear token and redirect to legacy login.

**Authorization in API Controllers:**
```csharp
[Authorize]
[ApiController]
public class CartController : ControllerBase
{
    protected int GetCustomerId() =>
        int.Parse(User.FindFirst("customer_id")!.Value);
}
```

### 8.3 Transition to Full JWT Auth (Post-Legacy)
Once the legacy MVC app is decommissioned:
1. Build Vue login/register pages.
2. Add ASP.NET Core Identity to the .NET 10 API (using `LegacyEcommerceIdentity` database).
3. API endpoints `POST /api/auth/login` and `POST /api/auth/register` issue JWTs directly.
4. Remove the legacy bridge.

---

## 9. REST API Design for .NET 10

### 9.1 Endpoint Inventory (All Vertical Slices)

| Method | Endpoint | Auth | Description | Legacy Equivalent |
|--------|----------|------|-------------|-------------------|
| GET | `/api/health` | No | Health check | — |
| GET | `/api/products` | No | List products (pagination, filter, sort) | `ProductController.Index` |
| GET | `/api/products/{id}` | No | Product detail | `ProductController.Detail` |
| GET | `/api/categories` | No | All categories | `CategoryController.GetCategories` |
| GET | `/api/cart` | Yes | Get cart items | `CartController.Index` |
| POST | `/api/cart/items` | Yes | Add item to cart | `CartController.AddToCart` |
| PUT | `/api/cart/items/{id}` | Yes | Update quantity | `CartController.UpdateQuantity` |
| DELETE | `/api/cart/items/{id}` | Yes | Remove item | `CartController.RemoveItem` |
| GET | `/api/cart/summary` | Yes | Mini-cart data | `CartController.GetCartSummary` |
| GET | `/api/checkout/addresses` | Yes | Customer addresses | `CheckoutController.Address` |
| POST | `/api/checkout/addresses` | Yes | Create address | `CheckoutController.Address` |
| GET | `/api/checkout/review` | Yes | Order preview | `CheckoutController.Review` |
| POST | `/api/checkout/orders` | Yes | Place order | `CheckoutController.PlaceOrder` |
| GET | `/api/orders` | Yes | Order history | `OrderController.Index` |
| GET | `/api/orders/{id}` | Yes | Order details | `OrderController.Details` |

### 9.2 DTO Design Principles

- **Flat over nested:** Product list returns a flat array; detail returns nested variants/images.
- **No circular references:** DTOs break `Product` → `Category` → `Products` cycles.
- **Consistent envelope:** All list endpoints return `{ data: [], totalCount: 0, page: 1, pageSize: 20 }`.
- **No ViewModels:** DTOs are API-facing, not view-facing. The Vue component decides how to present.

### 9.3 Error Handling

- **Global exception middleware** in .NET 10 API catches all unhandled exceptions.
- Returns `ProblemDetails` (RFC 7807):
  ```json
  {
    "type": "https://api.example.com/errors/not-found",
    "title": "Product not found",
    "status": 404,
    "detail": "No product with ID 123 exists.",
    "traceId": "00-abc123"
  }
  ```
- **Validation:** Use `FluentValidation` or built-in `ModelState` validation. Return 400 with validation errors map.

---

## 10. Vue.js 3 Frontend Component Mapping

### 10.1 Razor View → Vue Component Mapping

| Legacy Razor View | Vue Component | Notes |
|-------------------|---------------|-------|
| `_Layout.cshtml` | `App.vue` + `TheHeader.vue` + `TheFooter.vue` | Layout becomes shell component |
| `Home/Index.cshtml` | `HomeView.vue` | Featured products, hero banner |
| `Product/Index.cshtml` | `ProductListView.vue` | Grid + filters + pagination |
| `Product/Detail.cshtml` | `ProductDetailView.vue` | Gallery, variants, add-to-cart |
| `Cart/Index.cshtml` | `CartView.vue` | Table of items, quantity inputs |
| `Checkout/Address.cshtml` | `AddressStep.vue` (inside `CheckoutWizard.vue`) | Multi-step wizard |
| `Checkout/Review.cshtml` | `ReviewStep.vue` | Order summary |
| `Order/Index.cshtml` | `OrderListView.vue` | Table of orders |
| `Order/Details.cshtml` | `OrderDetailView.vue` | Order items, shipping info |
| `Account/Login.cshtml` | (Phase 6) `LoginView.vue` | Stay legacy during hybrid |
| `Account/Register.cshtml` | (Phase 6) `RegisterView.vue` | Stay legacy during hybrid |

### 10.2 Shared/Reusable Components

- `ProductCard.vue` — used in `HomeView.vue` (featured), `ProductListView.vue` (grid), `ProductDetailView.vue` (related products).
- `AddToCartButton.vue` — used in `ProductCard.vue` and `ProductDetailView.vue`.
- `CartSummaryBadge.vue` — used in `TheHeader.vue`.
- `PaginationControls.vue` — used in `ProductListView.vue`, `OrderListView.vue`.
- `LoadingSpinner.vue` — global async state indicator.
- `ErrorAlert.vue` — displays `ProblemDetails` errors.

### 10.3 Pinia Store Design

```typescript
// stores/useProductStore.ts
export const useProductStore = defineStore('products', () => {
  const products = ref<ProductDto[]>([]);
  const filters = ref({ categoryId: null as number | null, sortOrder: 'name', searchQuery: '' });
  const pagination = ref({ page: 1, pageSize: 12, totalCount: 0 });
  async function fetchProducts() { /* call API */ }
  return { products, filters, pagination, fetchProducts };
});

// stores/useCartStore.ts
export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItemDto[]>([]);
  const itemCount = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0));
  async function addItem(productId: number, quantity: number, variantId?: number) { /* POST /api/cart/items */ }
  async function fetchCart() { /* GET /api/cart */ }
  return { items, itemCount, addItem, fetchCart };
});

// stores/useAuthStore.ts
export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('jwt'));
  const isAuthenticated = computed(() => !!token.value);
  async function fetchToken() { /* GET /Account/IssueToken */ }
  function logout() { token.value = null; localStorage.removeItem('jwt'); }
  return { token, isAuthenticated, fetchToken, logout };
});
```

### 10.4 API Client (`api/index.ts`)

Use native `fetch` (no Axios needed — YAGNI for a PoC):

```typescript
const apiBase = import.meta.env.VITE_API_BASE_URL || '/api';

async function get<T>(path: string, token?: string): Promise<T> {
  const res = await fetch(`${apiBase}${path}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : {}
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}
```

---

## 11. Testing Strategy

### 11.1 Backend Testing (xUnit + Moq)

**Unit Tests — Service Layer:**
- Mock `IUnitOfWork`, `IProductRepository`, etc.
- Test `ProductService.GetProducts` returns correct pagination.
- Test `CheckoutService.PlaceOrderAsync` commits transaction once and only once.
- Test `CheckoutService.PlaceOrderAsync` with duplicate idempotency key returns existing order without creating new one.

**Unit Tests — API Controllers:**
- Mock `IProductService`.
- Verify `ProductsController.GetProducts` returns `Ok<PagedResult<ProductDto>>`.
- Verify `CartController.GetCart` returns 401 when no JWT.
- Verify `CartController.GetCart` extracts `customer_id` claim and passes to service.

**Integration Tests (Optional, Post-Phase 3):**
- Spin up API in-memory with `WebApplicationFactory`.
- Hit real endpoints against a **test SQL Server database** (or LocalDB).
- Verify end-to-end `POST /api/checkout/orders` creates rows in `Orders` and `OrderItems`.

### 11.2 Frontend Testing (Vitest + Vue Test Utils)

**Component Unit Tests:**
- `ProductCard.vue`: Given `product` prop, renders name, price, image. Clicking "Add to Cart" emits `add-to-cart` event.
- `CategoryFilter.vue`: Given `categories` prop, renders tree. Clicking category emits `select` with category ID.
- `PaginationControls.vue`: Given `page=2, totalPages=5`, renders correct buttons. Clicking "Next" emits `change-page(3)`.
- `CartView.vue`: Given `items` array, renders table rows. Clicking "+" calls `cartStore.updateQuantity`.

**Store Tests:**
- `useProductStore.fetchProducts` calls `fetch` with correct query string.
- `useAuthStore.fetchToken` stores token in `localStorage` on success.

### 11.3 E2E Testing (Playwright)

**Test Scenarios:**
1. **Product Catalog E2E:**
   - Visit `/products`.
   - Select category from filter.
   - Verify URL updates (`?categoryId=3`).
   - Verify product grid updates.
   - Click product.
   - Verify detail page shows product name.

2. **Cart E2E:**
   - Visit product detail.
   - Click "Add to Cart".
   - Navigate to `/cart`.
   - Verify item appears.
   - Change quantity to 3.
   - Verify total price updates.
   - Remove item.
   - Verify empty cart message.

3. **Checkout E2E:**
   - Add item to cart.
   - Navigate to checkout.
   - Select address (or create new).
   - Proceed to review.
   - Click "Place Order".
   - Verify redirect to confirmation page with order number.
   - Verify cart is empty.

4. **Auth E2E:**
   - Visit `/app/` unauthenticated.
   - Attempt to access cart → redirected to legacy login.
   - Log in via legacy form.
   - Redirected back to `/app/`.
   - SPA obtains JWT via bridge.
   - Cart is now accessible.

### 11.4 Test Execution Reports

- **Backend:** `dotnet test --logger trx` generates `.trx` files. Convert to HTML with `trxer`.
- **Frontend:** `vitest run --reporter junit` generates JUnit XML.
- **E2E:** `npx playwright test --reporter html` generates `playwright-report/`.

**Deliverable:** All three test suites must pass before each phase is considered complete. Include screenshot of green CI/build output in final submission.

---

## 12. Deployment & Rollback Plan with YARP

### 12.1 YARP Configuration

```csharp
// YARP routes in appsettings.json or Program.cs
var routes = new[]
{
    new RouteConfig
    {
        RouteId = "api",
        ClusterId = "apiCluster",
        Match = new RouteMatch { Path = "/api/{**catch-all}" }
    },
    new RouteConfig
    {
        RouteId = "app",
        ClusterId = "spaCluster",
        Match = new RouteMatch { Path = "/app/{**catch-all}" }
    },
    new RouteConfig
    {
        RouteId = "legacy",
        ClusterId = "legacyCluster",
        Match = new RouteMatch { Path = "{**catch-all}" }
    }
};
```

**Cluster destinations:**
- `apiCluster`: `http://localhost:5001`
- `spaCluster`: `http://localhost:5002` (dev) or serve static files directly from YARP host.
- `legacyCluster`: `http://localhost:5000`

**Priority:** `/api/*` and `/app/*` are checked before the catch-all fallback. This means a request to `/products` (legacy route) hits the legacy app **unless** we add an explicit route for `/products* → spaCluster` after migrating that slice.

### 12.2 Per-Phase Route Activation

| Phase | YARP Routes Active |
|-------|-------------------|
| 0 | `/api/*` → API, `/app/*` → SPA, `/*` → Legacy |
| 1 | Add `/products*` → SPA. Product browsing routes now hit Vue. |
| 2 | Add `/cart*`, `/api/cart*` → API. Cart UI served by Vue. |
| 3 | Add `/checkout*` → SPA. Checkout wizard served by Vue. |
| 4 | Add `/orders*` → SPA. |
| 5 | Add `/`, `/home*` → SPA (homepage now Vue). |
| 6 | Remove legacy fallback. `/*` → SPA (with Vue Router handling 404s). |

### 12.3 Rollback Strategy

- **YARP is the kill switch.** If a modernized slice has a critical bug:
  - Comment out the specific route rule (e.g., `/products* → SPA`).
  - Traffic immediately falls back to the legacy catch-all.
  - Zero downtime rollback.
- **Database is shared.** No data migration risk during rollback. A bug in the Vue cart doesn't corrupt the cart table; reverting to legacy MVC cart reads the same rows.
- **Feature flags (optional):** For riskier slices (checkout), add a query-string gate (`?modern=1`) before making the route permanent.

### 12.4 Local Development Setup

**Launch configuration (3 terminals):**
1. **Legacy MVC:** `iisexpress` or local IIS on port `5000`.
2. **.NET 10 API:** `dotnet run` on port `5001`.
3. **Vue SPA:** `npm run dev` on port `5002`.
4. **YARP:** `dotnet run` on port `80` or `5003`, proxying to the three above.

**CORS:** During local dev, the API must allow `http://localhost:5002` and `http://localhost:5003`. In production, YARP terminates SSL and rewrites headers; CORS is not needed because the browser sees same-origin.

---

## 13. Risk Register & Mitigation

| Risk | Impact | Likelihood | Mitigation |
|------|--------|-----------|------------|
| **Auth bridge failure** | High | Medium | Build auth bridge in Phase 0 before any protected slices. Test with Playwright E2E. Keep legacy login as fallback. |
| **DatabaseHelper port issues** | High | Low | `Microsoft.Data.SqlClient` is API-compatible with `System.Data.SqlClient`. Test connectivity in Phase 0 with a simple `SELECT 1`. |
| **Checkout transaction bugs** | High | Low | Port `CheckoutService` verbatim. Write xUnit tests that verify transaction commit/rollback and idempotency. Run E2E checkout test on staging data. |
| **SEO loss from SSR→SPA** | Medium | Medium | Product catalog is the risk. Mitigation: Add SSR later (Nuxt.js) if SEO matters, or keep product pages server-rendered via legacy fallback longer. For PoC, accept SEO trade-off. |
| **Team learning curve (Vue 3)** | Medium | High | Pick Product Catalog first — no auth, no complex state. Use `<script setup>` consistently. Write component tests early to enforce patterns. |
| **YARP performance** | Low | Low | YARP is production-grade (used by Azure). For PoC, default config is sufficient. |

---

## 14. Deliverables Checklist (Per Assignment)

| Deliverable | Location | Verification |
|-------------|----------|------------|
| **Architecture Diagram** | `modern/docs/architecture-diagram.png` (or embedded in README) | Shows Browser → YARP → (Legacy / API / SPA) |
| **Fully Migrated Vertical Slice** | `modern/Ecommerce.Api/Controllers/ProductsController.cs` + `modern/Ecommerce.Spa/src/views/ProductListView.vue` | Product catalog browsable via Vue + API |
| **Technical Summary** | `modern/docs/vertical-slice-summary.md` | Explains decoupling of `ProductController` → `ProductService` → API Controller + DTO + Vue component |
| **Backend Test Execution Report** | `modern/Ecommerce.Api.Tests/TestResults/` | `dotnet test` output showing all green |
| **Frontend Test Execution Report** | `modern/Ecommerce.Spa/tests/unit/results/` | `vitest run` output showing all green |
| **E2E Test Execution Report** | `modern/Ecommerce.Spa/tests/e2e/playwright-report/` | Playwright HTML report showing passed scenarios |
---

## Decision Log

1. **Vertical slice order:** Product Catalog → Cart → Checkout → Orders → Account → Decommission. *Rationale: Read-only first, auth-required next, most complex last.*
2. **Auth strategy:** Option A — Legacy issues JWTs during hybrid phase. *Rationale: Avoids migrating Identity tables upfront; de-risks the migration.*
3. **Data access:** Port `DatabaseHelper` using `Microsoft.Data.SqlClient`; keep ADO.NET. *Rationale: Existing code works. EF Core migration is a separate project.*
4. **Vue HTTP client:** Native `fetch`. *Rationale: No need for Axios; `fetch` + wrapper function is sufficient.*
5. **TypeScript vs. JavaScript:** **Recommend TypeScript** for the Vue SPA. *Rationale: API contracts change frequently during migration; TS catches DTO shape mismatches at build time.*
6. **Testing frameworks:** xUnit + Moq (backend), Vitest + Vue Test Utils (frontend), Playwright (E2E). *Rationale: Standard, well-documented, assignment explicitly names these.*
