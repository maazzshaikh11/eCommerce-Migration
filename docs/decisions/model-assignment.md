# Model Assignment Document

## 1. System Overview

This document governs a three-model execution system for the migration project.

| Model | Role | Scope |
|-------|------|-------|
| **GLM 5.2** | Architect / Planner | Planning, architectural decisions, documentation, task decomposition |
| **MiniMax** | Orchestrator | Execution order management, dependency validation, task assignment, completion verification, next-task gating |
| **Kimi-K2.6** | Implementation Agent | Coding, refactoring, testing, build verification, low-level documentation |

### Execution Principle
**Strictly sequential.** Task N+1 never begins until Task N passes all gates. The Orchestrator is the single authority that approves task transitions.

---

## 2. Role Definitions

### GLM 5.2 — Architect / Planner
- **Does:** Design decisions, ADR authoring, task decomposition, documentation, structural reviews
- **Does NOT:** Write implementation code, run builds, execute tests
- **Trigger:** When a task requires an architectural choice, when documentation must be created, when new tasks need decomposition

### MiniMax — Orchestrator
- **Does:** Maintain the master task queue, validate dependencies before assignment, assign tasks to GLM 5.2 or Kimi-K2.6, inspect outputs against acceptance criteria, approve/reject task completion, trigger next task
- **Does NOT:** Write code or documentation directly
- **Trigger:** Continuous; active between every task

### Kimi-K2.6 — Implementation Agent
- **Does:** Create files, write code, run commands (`dotnet build`, `npm install`, etc.), execute tests, fix compilation errors, perform refactoring
- **Does NOT:** Make architectural decisions, skip tasks, proceed without orchestrator approval
- **Trigger:** When the Orchestrator assigns an implementation task

---

## 3. Assignment Rules

| Task Category | Assigned Model | Reason |
|-------------|--------------|--------|
| Scaffolding projects | Kimi-K2.6 | Mechanical setup; no design decisions |
| Installing packages | Kimi-K2.6 | Mechanical dependency management |
| Copying/porting legacy code | Kimi-K2.6 | Mechanical port with namespace/SQL client swap |
| Creating DTOs/Models | Kimi-K2.6 | Straightforward record definitions |
| Building controllers/services | Kimi-K2.6 | Implementation of defined contracts |
| Vue component development | Kimi-K2.6 | UI implementation against specs |
| Pinia store development | Kimi-K2.6 | State management implementation |
| Running builds/tests | Kimi-K2.6 | Tool execution and verification |
| Writing unit/E2E tests | Kimi-K2.6 | Test code implementation |
| Docker/infra configuration | Kimi-K2.6 | Configuration-as-code implementation |
| Architectural decisions | GLM 5.2 | Requires design judgment |
| ADR authoring | GLM 5.2 | Documentation requiring architectural reasoning |
| Task decomposition (new) | GLM 5.2 | Planning and scoping |
| Schema/contract redesign | GLM 5.2 | Cross-service design decisions |
| Rollback planning review | GLM 5.2 | Risk assessment and strategy |
| Orchestration & gating | MiniMax | Central coordination authority |

---

## 4. Orchestrator Workflow (MiniMax)

For every task, MiniMax executes this exact workflow:

```
1. RECEIVE COMPLETION SIGNAL from previous task's assigned model
2. VERIFY OUTPUTS against task's Expected Outputs list
3. RUN VERIFICATION METHOD (e.g., dotnet build, npm test, manual check)
4. CHECK ACCEPTANCE CRITERIA — all checkboxes must pass
5. DECISION:
   ├─ PASS → Log completion, trigger Next Task Trigger, assign to target model
   └─ FAIL → Log failure, initiate Rollback Strategy, re-assign same task to fix
```

MiniMax never assigns Task N+1 until Step 5 results in PASS.

---

## 5. Master Task Assignment Table

### Phase 0: Infrastructure & Proxy Setup

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-001 | Create `modern/` directory | Kimi-K2.6 | Mechanical directory creation | Git repo access | `modern/` directory exists | `dir modern/` / `ls modern/` | Directory visible to Git | T-001 PASS |
| T-002 | Verify .NET 10 SDK | Kimi-K2.6 | Environment verification | Dev machine | Confirmed SDK version | `dotnet --version` returns 10.x | Version string validated | T-002 PASS |
| T-003 | Create .NET solution file | Kimi-K2.6 | Scaffold command execution | T-002 | `Ecommerce.sln` | `dotnet sln list` shows solution | Solution file created | T-003 PASS |
| T-004 | Create .NET 10 Web API project | Kimi-K2.6 | Template scaffolding | T-003 | API project skeleton | `dotnet build` succeeds zero errors | Project builds clean | T-004 PASS |
| T-005 | Remove default WeatherForecastController | Kimi-K2.6 | Cleanup | T-004 | Clean project | `dotnet build` succeeds; file deleted | No sample code remains | T-005 PASS |
| T-006 | Add YARP NuGet package | Kimi-K2.6 | Package installation | T-005 | Updated `.csproj` | `dotnet restore` succeeds | Package restorable | T-006 PASS |
| T-007 | Add ReverseProxy config section to appsettings.json | GLM 5.2 | Architectural routing design decision | T-006 | Valid `ReverseProxy` JSON | JSON validation + build | Routes/clusters correctly structured | T-007 PASS |
| T-008 | Register YARP services and middleware in Program.cs | Kimi-K2.6 | Configuration wiring | T-007 | Modified `Program.cs` | `dotnet build` succeeds | YARP wired in pipeline | T-008 PASS |
| T-009 | Verify YARP starts without runtime errors | Kimi-K2.6 | Runtime verification | T-008 | Running process | `dotnet run` console output | No exceptions; YARP loaded | T-009 PASS |
| T-010 | Create shared class library projects | Kimi-K2.6 | Multi-project scaffolding | T-003 | Four `.csproj` files | `dotnet sln list` shows all; build succeeds | All projects in solution | T-010 PASS |
| T-011 | Configure Central Package Management (CPM) | GLM 5.2 | Architectural standard for dependency management | T-010 | `Directory.Packages.props` | `dotnet build` succeeds | CPM active; versions centralized | T-011 PASS |
| T-012 | Create Directory.Build.props | Kimi-K2.6 | Shared MSBuild properties | T-011 | `Directory.Build.props` | `dotnet build` succeeds | Nullable/lang version set | T-012 PASS |
| T-013 | Wire inter-project references | Kimi-K2.6 | Reference configuration | T-012 | Updated `.csproj` files | `dotnet build` succeeds; no cycles | Dependency graph correct | T-013 PASS |
| T-014 | Port DatabaseHelper from legacy | Kimi-K2.6 | Code port with SQL client swap | T-013 + legacy repo | `DatabaseHelper.cs` in new project | `dotnet build` succeeds | No `System.Data.SqlClient`; uses modern client | T-014 PASS |
| T-015 | Create temporary HealthController | Kimi-K2.6 | Connectivity verification endpoint | T-014 | `HealthController.cs` | `GET /api/health` returns 200 | Database connectivity proven | T-015 PASS |
| T-016 | Add JWT NuGet package | Kimi-K2.6 | Package installation | T-015 | Updated `.csproj` | `dotnet restore` succeeds | Package restorable | T-016 PASS |
| T-017 | Implement JWT Token Service in Core | GLM 5.2 | Security architecture — token generation and validation design | T-016 | `JwtTokenService.cs` + tests | Unit test passes | Token generates with correct claims; validates | T-017 PASS |
| T-018 | Configure JWT auth in Program.cs | Kimi-K2.6 | Middleware wiring | T-017 | Modified `Program.cs` | Auth test: 401 without token, 200 with token | Middleware correctly ordered and active | T-018 PASS |
| T-019 | Add JWT signing key to appsettings.json | Kimi-K2.6 | Configuration | T-018 | Updated config files | API reads key without exception | Key present in both config files | T-019 PASS |
| T-020 | Add JWT bridge endpoint to legacy MVC | Kimi-K2.6 | Legacy code modification | T-019 + legacy repo | Modified `AccountController.cs` | Manual test: logged-in user gets JSON token; anonymous gets 302 | Bridge functional | T-020 PASS |
| T-021 | Verify Node.js installation | Kimi-K2.6 | Environment check | T-001 | Confirmed versions | `node --version` and `npm --version` | Node 20+ and npm 10+ | T-021 PASS |
| T-022 | Scaffold Vue SPA with Vite | Kimi-K2.6 | Template scaffolding | T-021 | Vite scaffold folder | `npm install` succeeds; `npm run dev` starts | Dev server runs; welcome page visible | T-022 PASS |
| T-023 | Install Pinia and Vue Router | Kimi-K2.6 | Package installation | T-022 | Updated `package.json` | `npm install` succeeds; dev server starts | Dependencies listed; no errors | T-023 PASS |
| T-024 | Configure API base URL in env file | Kimi-K2.6 | Configuration | T-023 | `.env.development` | `import.meta.env.VITE_API_BASE_URL` resolves | Base URL correctly configured | T-024 PASS |
| T-025 | Implement fetch wrapper with JWT | Kimi-K2.6 | API client implementation | T-024 | `client.ts` | Manual browser test: Bearer header present | Wrapper exports get/post/put/del with auth | T-025 PASS |
| T-026 | Create Pinia auth store | Kimi-K2.6 | Store implementation | T-025 | `useAuthStore.ts` | Manual test: token fetched and persisted | Store manages token lifecycle | T-026 PASS |
| T-027 | Configure Vue Router skeleton | Kimi-K2.6 | Router setup | T-023 | `router/index.ts` | Navigation works in browser | Router with history mode active | T-027 PASS |
| T-028 | Verify full proxy chain end-to-end | Kimi-K2.6 | Integration verification | T-027 + T-020 + T-009 | Verified running system | Manual multi-service test | All 4 services run; JWT fetch + API call succeed | T-028 PASS |

---

### Phase 1: Product Catalog Vertical Slice

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-029 | Port Product domain model | Kimi-K2.6 | Mechanical model copy | T-028 + legacy repo | `Product.cs` in new project | `dotnet build` succeeds | File compiles; no EF refs | T-029 PASS |
| T-030 | Port Category domain model | Kimi-K2.6 | Mechanical model copy | T-029 + legacy repo | `Category.cs` in new project | `dotnet build` succeeds | File compiles; no EF refs | T-030 PASS |
| T-031 | Port ProductImage and ProductVariant models | Kimi-K2.6 | Mechanical model copy | T-030 + legacy repo | Two model files | `dotnet build` succeeds | Both files compile | T-031 PASS |
| T-032 | Create ProductListResponseDto | Kimi-K2.6 | DTO definition | T-031 | `ProductListResponseDto.cs` | `dotnet build` succeeds | Record with correct fields | T-032 PASS |
| T-033 | Create ProductDetailDto | Kimi-K2.6 | DTO definition | T-032 | `ProductDetailDto.cs` | `dotnet build` succeeds | Record with nested collections; no cycles | T-033 PASS |
| T-034 | Create CategoryDto and PagedResult | Kimi-K2.6 | DTO definition | T-033 | Two DTO files | `dotnet build` succeeds | Generic pagination reusable | T-034 PASS |
| T-035 | Port IProductRepository interface | Kimi-K2.6 | Interface copy | T-034 + legacy repo | `IProductRepository.cs` | `dotnet build` succeeds | Methods use domain models | T-035 PASS |
| T-036 | Port ICategoryRepository interface | Kimi-K2.6 | Interface copy | T-035 + legacy repo | `ICategoryRepository.cs` | `dotnet build` succeeds | Methods use domain models | T-036 PASS |
| T-037 | Port ProductRepository implementation | Kimi-K2.6 | Repository port with SQL client swap | T-036 + legacy repo | `ProductRepository.cs` | `dotnet build` succeeds | Uses `Microsoft.Data.SqlClient` | T-037 PASS |
| T-038 | Port CategoryRepository implementation | Kimi-K2.6 | Repository port with SQL client swap | T-037 + legacy repo | `CategoryRepository.cs` | `dotnet build` succeeds | Uses `Microsoft.Data.SqlClient` | T-038 PASS |
| T-039 | Port IProductService interface | Kimi-K2.6 | Interface copy | T-038 + legacy repo | `IProductService.cs` | `dotnet build` succeeds | Methods return domain/DTOs | T-039 PASS |
| T-040 | Port ICategoryService interface | Kimi-K2.6 | Interface copy | T-039 + legacy repo | `ICategoryService.cs` | `dotnet build` succeeds | Methods return domain/DTOs | T-040 PASS |
| T-041 | Port ProductService implementation | Kimi-K2.6 | Service port with return type updates | T-040 + legacy repo | `ProductService.cs` | `dotnet build` succeeds | No Razor/ViewModel refs | T-041 PASS |
| T-042 | Port CategoryService implementation | Kimi-K2.6 | Service port with return type updates | T-041 + legacy repo | `CategoryService.cs` | `dotnet build` succeeds | No Razor/ViewModel refs | T-042 PASS |
| T-043 | Register Product and Category services in DI | Kimi-K2.6 | DI configuration | T-042 | Modified `Program.cs` | API starts without DI exceptions | All scoped registrations present | T-043 PASS |
| T-044 | Implement ProductsController with GET endpoints | Kimi-K2.6 | Controller implementation | T-043 | `ProductsController.cs` | Swagger/Postman test | Returns `PagedResult<ProductDto>` and `ProductDetailDto` | T-044 PASS |
| T-045 | Implement CategoriesController with GET endpoint | Kimi-K2.6 | Controller implementation | T-044 | `CategoriesController.cs` | Swagger/Postman test | Returns `CategoryDto[]` | T-045 PASS |
| T-046 | Add YARP route for product pages | Kimi-K2.6 | YARP configuration | T-045 | Updated YARP config | Manual browser test | `/products*` routes to SPA | T-046 PASS |
| T-047 | Create ProductCard Vue component | Kimi-K2.6 | UI component | T-046 | `ProductCard.vue` | Vitest renders with sample data | Emits `add-to-cart` event | T-047 PASS |
| T-048 | Create CategoryFilter Vue component | Kimi-K2.6 | UI component | T-047 | `CategoryFilter.vue` | Vitest renders tree | Emits `select` with category ID | T-048 PASS |
| T-049 | Create PaginationControls Vue component | Kimi-K2.6 | UI component | T-048 | `PaginationControls.vue` | Vitest renders correct pages | Emits `change-page` event | T-049 PASS |
| T-050 | Create LoadingSpinner and ErrorAlert components | Kimi-K2.6 | UI utility components | T-049 | Two component files | Isolated render test | Both render without errors | T-050 PASS |
| T-051 | Create Pinia product store | Kimi-K2.6 | State management | T-050 | `useProductStore.ts` | Manual test: filter change triggers refetch | Fetches with query params; manages pagination | T-051 PASS |
| T-052 | Create ProductListView Vue page | Kimi-K2.6 | View implementation | T-051 | `ProductListView.vue` | Browser test with real data | Grid renders; URL params update | T-052 PASS |
| T-053 | Create ProductDetailView Vue page | Kimi-K2.6 | View implementation | T-052 | `ProductDetailView.vue` | Browser test with real data | Detail renders for `/products/:id` | T-053 PASS |
| T-054 | Configure Vue Router for product pages | Kimi-K2.6 | Router configuration | T-053 | Updated `router/index.ts` | Browser navigation test | Routes work; back/forward preserves state | T-054 PASS |
| T-055 | Write backend unit tests for ProductService | Kimi-K2.6 | Unit test implementation | T-043 | `ProductServiceTests.cs` | `dotnet test` passes | Covers pagination and filtering | T-055 PASS |
| T-056 | Write backend unit tests for ProductsController | Kimi-K2.6 | Unit test implementation | T-044 | `ProductsControllerTests.cs` | `dotnet test` passes | Covers 200 and 404 responses | T-056 PASS |
| T-057 | Write frontend unit tests for ProductCard | Kimi-K2.6 | Unit test implementation | T-047 | `ProductCard.spec.ts` | `npm run test:unit` passes | Props render; event emits | T-057 PASS |
| T-058 | Write frontend unit tests for product store | Kimi-K2.6 | Unit test implementation | T-051 | `useProductStore.spec.ts` | `npm run test:unit` passes | Fetch calls correct URL | T-058 PASS |
| T-059 | Write E2E test for product catalog flow | Kimi-K2.6 | E2E test implementation | T-052 + T-053 | `product-catalog.spec.ts` | `npx playwright test` passes | Full flow: filter → click → verify detail | T-059 PASS |

---

### Phase 2: Shopping Cart Vertical Slice

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-060 | Port CartItem domain model | Kimi-K2.6 | Mechanical model copy | T-059 + legacy repo | `CartItem.cs` in new project | `dotnet build` succeeds | File compiles | T-060 PASS |
| T-061 | Create cart DTOs | Kimi-K2.6 | DTO definition | T-060 | Three DTO files | `dotnet build` succeeds | All DTOs compile | T-061 PASS |
| T-062 | Port ICartItemRepository interface | Kimi-K2.6 | Interface copy | T-061 + legacy repo | `ICartItemRepository.cs` | `dotnet build` succeeds | Interface compiles | T-062 PASS |
| T-063 | Port CartItemRepository implementation | Kimi-K2.6 | Repository port with SQL client swap | T-062 + legacy repo | `CartItemRepository.cs` | `dotnet build` succeeds | Uses `Microsoft.Data.SqlClient` | T-063 PASS |
| T-064 | Port ICartService interface | Kimi-K2.6 | Interface copy | T-063 + legacy repo | `ICartService.cs` | `dotnet build` succeeds | Interface compiles | T-064 PASS |
| T-065 | Port CartService implementation | Kimi-K2.6 | Service port with return type updates | T-064 + legacy repo | `CartService.cs` | `dotnet build` succeeds | No Razor/ViewModel refs | T-065 PASS |
| T-066 | Register cart services in DI | Kimi-K2.6 | DI configuration | T-065 | Modified `Program.cs` | API starts without DI exceptions | Scoped registrations present | T-066 PASS |
| T-067 | Implement CartController with authenticated endpoints | Kimi-K2.6 | Controller implementation | T-066 | `CartController.cs` | Auth test: 401 anonymous, 200 authenticated | All actions `[Authorize]`; correct DTOs | T-067 PASS |
| T-068 | Create Pinia cart store | Kimi-K2.6 | State management | T-067 | `useCartStore.ts` | Manual test: add/fetch/update/remove | Server-side cart; no localStorage | T-068 PASS |
| T-069 | Create AddToCartButton Vue component | Kimi-K2.6 | UI component | T-068 | `AddToCartButton.vue` | Manual click test | Calls `cartStore.addItem()` with Bearer token | T-069 PASS |
| T-070 | Create CartView Vue page | Kimi-K2.6 | View implementation | T-069 | `CartView.vue` | Browser test with real data | Renders items; quantity persists; remove works | T-070 PASS |
| T-071 | Create CartSummaryBadge component | Kimi-K2.6 | UI component | T-070 | `CartSummaryBadge.vue` | Browser test | Displays correct count; updates on add | T-071 PASS |
| T-072 | Add cart route to Vue Router | Kimi-K2.6 | Router configuration | T-071 | Updated `router/index.ts` | Browser navigation test | `/cart` renders `CartView` | T-072 PASS |
| T-073 | Add YARP route for cart pages | Kimi-K2.6 | YARP configuration | T-072 | Updated YARP config | Manual browser test | `/cart*` routes to SPA | T-073 PASS |
| T-074 | Write backend unit tests for CartController | Kimi-K2.6 | Unit test implementation | T-067 | `CartControllerTests.cs` | `dotnet test` passes | 401 for anonymous; 200 for authenticated | T-074 PASS |
| T-075 | Write backend unit tests for CartService | Kimi-K2.6 | Unit test implementation | T-065 | `CartServiceTests.cs` | `dotnet test` passes | Covers add/update/remove logic | T-075 PASS |
| T-076 | Write frontend unit tests for AddToCartButton | Kimi-K2.6 | Unit test implementation | T-069 | `AddToCartButton.spec.ts` | `npm run test:unit` passes | Click emits event; calls store | T-076 PASS |
| T-077 | Write frontend unit tests for CartView | Kimi-K2.6 | Unit test implementation | T-070 | `CartView.spec.ts` | `npm run test:unit` passes | Renders items; handles quantity | T-077 PASS |
| T-078 | Write E2E test for cart flow | Kimi-K2.6 | E2E test implementation | T-070 | `cart.spec.ts` | `npx playwright test` passes | Add → update → remove → verify empty | T-078 PASS |

---

### Phase 3: Checkout Vertical Slice

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-079 | Port Address domain model | Kimi-K2.6 | Mechanical model copy | T-078 + legacy repo | `Address.cs` in new project | `dotnet build` succeeds | File compiles | T-079 PASS |
| T-080 | Port Order and OrderItem domain models | Kimi-K2.6 | Mechanical model copy | T-079 + legacy repo | Two model files | `dotnet build` succeeds | Both files compile | T-080 PASS |
| T-081 | Port Customer domain model | Kimi-K2.6 | Mechanical model copy | T-080 + legacy repo | `Customer.cs` in new project | `dotnet build` succeeds | File compiles | T-081 PASS |
| T-082 | Create checkout DTOs | Kimi-K2.6 | DTO definition | T-081 | Five DTO files | `dotnet build` succeeds | No circular references | T-082 PASS |
| T-083 | Port Address repository interface and implementation | Kimi-K2.6 | Repository port with SQL client swap | T-082 + legacy repo | Two files | `dotnet build` succeeds | Uses `Microsoft.Data.SqlClient` | T-083 PASS |
| T-084 | Port Order repository interface and implementation | Kimi-K2.6 | Repository port with SQL client swap | T-083 + legacy repo | Two files | `dotnet build` succeeds | Uses `Microsoft.Data.SqlClient` | T-084 PASS |
| T-085 | Port Customer repository interface and implementation | Kimi-K2.6 | Repository port with SQL client swap | T-084 + legacy repo | Two files | `dotnet build` succeeds | Uses `Microsoft.Data.SqlClient` | T-085 PASS |
| T-086 | Port CheckoutService verbatim | Kimi-K2.6 | Critical service port — exact copy | T-085 + legacy repo | `CheckoutService.cs` | `dotnet build` succeeds | Diff shows only namespace/return type changes | T-086 PASS |
| T-087 | Port OrderService | Kimi-K2.6 | Service port | T-086 + legacy repo | `OrderService.cs` | `dotnet build` succeeds | File compiles | T-087 PASS |
| T-088 | Register checkout and order services in DI | Kimi-K2.6 | DI configuration | T-087 | Modified `Program.cs` | API starts without DI exceptions | All new registrations present | T-088 PASS |
| T-089 | Implement CheckoutController | Kimi-K2.6 | Controller implementation | T-088 | `CheckoutController.cs` | Auth + header test | All `[Authorize]`; requires `Idempotency-Key` | T-089 PASS |
| T-090 | Implement OrdersController (read-only) | Kimi-K2.6 | Controller implementation | T-089 | `OrdersController.cs` | Auth test | Returns only customer's orders | T-090 PASS |
| T-091 | Create Pinia checkout store | Kimi-K2.6 | State management | T-090 | `useCheckoutStore.ts` | Manual test | UUID generated; address persists | T-091 PASS |
| T-092 | Create AddressSelector Vue component | Kimi-K2.6 | UI component | T-091 | `AddressSelector.vue` | Browser test | Lists addresses; updates store on select | T-092 PASS |
| T-093 | Create AddressForm Vue component | Kimi-K2.6 | UI component | T-092 | `AddressForm.vue` | Browser test | Validates; posts to API | T-093 PASS |
| T-094 | Create AddressStep Vue component | Kimi-K2.6 | UI component | T-093 | `AddressStep.vue` | Browser test | Toggles selector/form; proceeds with selection | T-094 PASS |
| T-095 | Create ReviewStep Vue component | Kimi-K2.6 | UI component | T-094 | `ReviewStep.vue` | Browser test | Displays preview; calls place order | T-095 PASS |
| T-096 | Create OrderConfirmation Vue component | Kimi-K2.6 | UI component | T-095 | `OrderConfirmation.vue` | Browser test | Shows order ID from route param | T-096 PASS |
| T-097 | Create CheckoutWizard Vue page | Kimi-K2.6 | View implementation | T-096 | `CheckoutWizard.vue` | Browser test | Step nav works; state persists; guards enforce sequence | T-097 PASS |
| T-098 | Add checkout and confirmation routes to Vue Router | Kimi-K2.6 | Router configuration | T-097 | Updated `router/index.ts` | Browser navigation test | Routes exist; guards active | T-098 PASS |
| T-099 | Add YARP route for checkout pages | Kimi-K2.6 | YARP configuration | T-098 | Updated YARP config | Manual browser test | `/checkout*` routes to SPA | T-099 PASS |
| T-100 | Write backend unit tests for CheckoutService | Kimi-K2.6 | Unit test implementation | T-086 | `CheckoutServiceTests.cs` | `dotnet test` passes | Transaction once; idempotency returns existing; address created | T-100 PASS |
| T-101 | Write backend unit tests for CheckoutController | Kimi-K2.6 | Unit test implementation | T-089 | `CheckoutControllerTests.cs` | `dotnet test` passes | 400 without key; 201 first; 200 duplicate | T-101 PASS |
| T-102 | Write frontend unit tests for CheckoutWizard | Kimi-K2.6 | Unit test implementation | T-097 | `CheckoutWizard.spec.ts` | `npm run test:unit` passes | Step nav; submit headers | T-102 PASS |
| T-103 | Write E2E test for full checkout flow | Kimi-K2.6 | E2E test implementation | T-097 | `checkout.spec.ts` | `npx playwright test` + SQL check | Full flow; DB confirms order + cleared cart | T-103 PASS |

---

### Phase 4: Order History & Account Management

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-104 | Create OrderListView Vue page | Kimi-K2.6 | View implementation | T-103 | `OrderListView.vue` | Browser test with real data | Fetches orders; table with pagination | T-104 PASS |
| T-105 | Create OrderDetailView Vue page | Kimi-K2.6 | View implementation | T-104 | `OrderDetailView.vue` | Browser test with real data | Displays items, address, status | T-105 PASS |
| T-106 | Add order routes to Vue Router | Kimi-K2.6 | Router configuration | T-105 | Updated `router/index.ts` | Browser navigation test | `/orders` and `/orders/:id` work | T-106 PASS |
| T-107 | Add YARP route for order pages | Kimi-K2.6 | YARP configuration | T-106 | Updated YARP config | Manual browser test | `/orders*` routes to SPA | T-107 PASS |
| T-108 | Write frontend unit tests for order views | Kimi-K2.6 | Unit test implementation | T-104 + T-105 | Two test files | `npm run test:unit` passes | Both views tested | T-108 PASS |
| T-109 | Write E2E test for order history flow | Kimi-K2.6 | E2E test implementation | T-104 | `order-history.spec.ts` | `npx playwright test` passes | Place order → verify in history → click detail | T-109 PASS |
| T-110 | Integrate legacy account page links in Vue header | Kimi-K2.6 | Hybrid-phase integration | T-109 | Modified `TheHeader.vue` | Manual browser test | Links to legacy; JWT bridge works post-login | T-110 PASS |

---

### Phase 5: Home, Search & Miscellaneous

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-111 | Create HomeView Vue page | Kimi-K2.6 | View implementation | T-110 | `HomeView.vue` | Browser test | Featured products; hero banner | T-111 PASS |
| T-112 | Add search input to header | Kimi-K2.6 | UI enhancement | T-111 | Modified `TheHeader.vue` | Browser test | Input triggers search; updates grid | T-112 PASS |
| T-113 | Add home route to Vue Router | Kimi-K2.6 | Router configuration | T-112 | Updated `router/index.ts` | Browser navigation test | `/` renders `HomeView` | T-113 PASS |
| T-114 | Add YARP routes for homepage and search | Kimi-K2.6 | YARP configuration | T-113 | Updated YARP config | Manual browser test | `/` and `/home*` route to SPA | T-114 PASS |
| T-115 | Write frontend unit tests for HomeView | Kimi-K2.6 | Unit test implementation | T-111 | `HomeView.spec.ts` | `npm run test:unit` passes | Renders featured section | T-115 PASS |
| T-116 | Write E2E test for homepage | Kimi-K2.6 | E2E test implementation | T-111 | `homepage.spec.ts` | `npx playwright test` passes | Visits `/`; verifies featured products | T-116 PASS |

---

### Phase 6: Decommission Legacy

| Task ID | Task Name | Assigned Model | Reason | Inputs | Expected Outputs | Verification Method | Acceptance Criteria | Next Task Trigger |
|---------|-----------|---------------|--------|--------|------------------|---------------------|---------------------|-----------------|
| T-117 | Analyze YARP traffic logs for legacy usage | GLM 5.2 | Analysis and decision support | T-116 | Traffic analysis markdown | Log review | Zero unexpected legacy traffic for 7 days | T-117 PASS |
| T-118 | Create Vue LoginView page | Kimi-K2.6 | View implementation | T-117 | `LoginView.vue` | Browser test | Form with email/password | T-118 PASS |
| T-119 | Create Vue RegisterView page | Kimi-K2.6 | View implementation | T-118 | `RegisterView.vue` | Browser test | Form with registration fields | T-119 PASS |
| T-120 | Add auth endpoints to API | Kimi-K2.6 | Controller implementation | T-119 | `AuthController.cs` | Token validation test | `POST /api/auth/login` and `POST /api/auth/register` return JWTs | T-120 PASS |
| T-121 | Update auth store to use API endpoints | Kimi-K2.6 | Store modification | T-120 | Modified `useAuthStore.ts` | Manual login test | Calls new endpoints; token validates | T-121 PASS |
| T-122 | Remove legacy fallback from YARP | Kimi-K2.6 | YARP configuration | T-121 | Updated YARP config | Manual browser test | `/*` routes to SPA; Vue Router handles 404s | T-122 PASS |
| T-123 | Shut down legacy application | Kimi-K2.6 | Infrastructure operation | T-122 | Shutdown confirmation | Process/monitoring check | Legacy returns 503/refused | T-123 PASS |
| T-124 | (Optional) Consolidate databases | GLM 5.2 | Architectural database consolidation decision | T-123 | Migration script + updated config | Restore test on separate instance | Identity tables in business DB; single connection string works | T-124 PASS |
| T-125 | Run final full E2E regression suite | Kimi-K2.6 | Final verification | T-124 (or T-123) | Final E2E report | `npx playwright test` | 100% pass; no console errors | PROJECT COMPLETE |

---

## 6. Orchestrator Handoff Protocol

### For Every Task Transition

**Step 1 — Assignment (MiniMax → Target Model)**
```
TASK ASSIGNMENT
Task ID: {ID}
Task Name: {Name}
Assigned To: {GLM 5.2 | Kimi-K2.6}
Inputs: {list}
Expected Outputs: {list}
Acceptance Criteria: {checklist}
Rollback Strategy: {description}
Deadline: {optional}
```

**Step 2 — Execution (Target Model)**
- Kimi-K2.6 creates/modifies files, runs builds/tests, reports results.
- GLM 5.2 produces documentation/decisions, reports completion.

**Step 3 — Completion Signal (Target Model → MiniMax)**
```
TASK COMPLETION
Task ID: {ID}
Status: COMPLETE
Files Created: {list}
Files Modified: {list}
Evidence: {screenshot | test output | log snippet}
```

**Step 4 — Verification (MiniMax)**
- MiniMax re-runs the Verification Method independently.
- MiniMax checks every Acceptance Criteria checkbox.

**Step 5 — Gate Decision (MiniMax)**
```
GATE DECISION
Task ID: {ID}
Result: PASS | FAIL
If FAIL: Reason: {description}
If FAIL: Action: REASSIGN for fix | ROLLBACK
Next Task: {ID+1} | SAME TASK (re-fix)
```

---

## 7. Exception Handling

### When a Task Fails Verification
1. **MiniMax logs the failure** with Task ID and specific criteria that failed.
2. **MiniMax consults GLM 5.2** if the failure indicates an architectural problem.
3. **MiniMax reassigns the task to Kimi-K2.6** with specific fix instructions.
4. **Kimi-K2.6 executes the fix** and reports back.
5. **MiniMax re-verifies** before approving.

### When an Architectural Question Arises During Implementation
1. **Kimi-K2.6 pauses** and signals MiniMax.
2. **MiniMax assigns a new ad-hoc task to GLM 5.2** to resolve the architectural question.
3. **GLM 5.2 produces a decision document** (or ADR update).
4. **MiniMax appends the decision** to the current task's inputs.
5. **Kimi-K2.6 resumes** with the clarified design.

### When a Task Requires Decomposition
1. **MiniMax identifies** that a task is too large or ambiguous.
2. **MiniMax assigns decomposition to GLM 5.2**.
3. **GLM 5.2 breaks the task into sub-tasks** with their own IDs.
4. **MiniMax inserts the sub-tasks** into the master queue.
5. **Execution resumes** with the first sub-task.

---

## 8. Model Communication Matrix

| From / To | MiniMax | GLM 5.2 | Kimi-K2.6 |
|-----------|---------|---------|-----------|
| **MiniMax** | — | Assignment + architectural questions | Assignment + fix instructions |
| **GLM 5.2** | Decisions + documentation | — | Design clarifications (via MiniMax) |
| **Kimi-K2.6** | Completion signals + blockers | Blockers (routed via MiniMax) | — |

**Rule:** No model communicates directly with another model. All communication routes through MiniMax (the Orchestrator).

---

## 9. Sign-Off Log

| Task ID | Task Name | Assigned Model | Completion Date | Verified By | Gate Result |
|---------|-----------|---------------|-----------------|-------------|-------------|
| | | | | | |

*MiniMax maintains this log. It is the single source of truth for project progress.*

---

## 10. Summary

| Metric | Value |
|--------|-------|
| Total Tasks | 125 |
| Assigned to Kimi-K2.6 | 118 (implementation) |
| Assigned to GLM 5.2 | 7 (architecture + analysis) |
| Orchestrated by MiniMax | 125 (all tasks) |
| Strictly Sequential | Yes |
| No Task Skipping | Yes |
| Orchestrator Approval Required for Every Transition | Yes |

This document is active for the duration of the project. MiniMax is the sole authority for task transitions. No model proceeds without explicit Orchestrator approval.
