# Legacy Codebase Notes

## Overview

This document captures notes, mappings, and gotchas from the legacy ASP.NET MVC 5 codebase that were relevant during migration.

> **Important:** The legacy source code lives in a separate repository (`C:\Users\Administrator\Desktop\eCommerce`). The modernization monorepo contains only the new implementation. The legacy source is read-only reference.

## Legacy → Modern Mapping

| Legacy (MVC 5) | Modern (.NET 10 + Vue 3) |
|----------------|------------------------|
| `HomeController.Index()` | `HomeView.vue` + `GET /api/products/featured` |
| `ProductController.Index()` | `ProductListView.vue` + `GET /api/products` |
| `ProductController.Detail(id)` | `ProductDetailView.vue` + `GET /api/products/{id}` |
| `CartController.Index()` | `CartView.vue` + `GET /api/cart` |
| `CheckoutController.Address()` | `CheckoutWizard.vue` Step 1 + `GET /api/checkout/addresses` |
| `CheckoutController.Shipping()` | `CheckoutWizard.vue` Step 2 + `GET /api/checkout/shipping-options` |
| `CheckoutController.Payment()` | `CheckoutWizard.vue` Step 3 + `POST /api/checkout/validate-payment` |
| `CheckoutController.Review()` | `CheckoutWizard.vue` Step 4 |
| `CheckoutController.Confirm()` | `CheckoutWizard.vue` Step 5 + `POST /api/checkout/orders` |
| `OrderController.Index()` | `OrderListView.vue` + `GET /api/orders` |
| `OrderController.Details(id)` | `OrderDetailView.vue` + `GET /api/orders/{id}` |
| `AccountController.Login()` | `LoginView.vue` + `POST /api/auth/login` |
| `AccountController.Register()` | `RegisterView.vue` + `POST /api/auth/register` |
| `CategoryController.Tree()` | `categoryStore.ts` + `GET /api/categories` |

## Gotchas

### 1. Currency: INR not USD

The legacy application uses **Indian Rupees (₹)** via `AppConstants.CurrencySymbol = "₹"`. The modern SPA uses `Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR' })` via `src/utils/currency.ts`.

### 2. Token Key in localStorage

The auth store writes the JWT to `localStorage['jwt']`. The API client must read from the same key. A mismatch (`'jwt'` vs `'token'`) caused silent auth failures during early development.

### 3. Idempotency Key as Header, Not Body

The legacy `CheckoutService.PlaceOrder` accepts the idempotency key as a method parameter. The modern API expects it as an **HTTP header** (`Idempotency-Key`), not in the request body. The `PlaceOrderRequest` DTO has `IdempotencyKey` as optional for backward compatibility.

### 4. Address Creation Persistence

The legacy checkout creates addresses as part of the order flow. The modern API's `AddAddress` endpoint was initially a stub (202 Accepted without persistence). It now properly creates `Address` rows via `IAddressRepository`.

### 5. Shipping Methods Hardcoded

Legacy `CheckoutService.GetShippingOptions()` returns Standard (₹99) and Express (₹199). The modern API mirrors this exactly. Shipping cost is selected by the user and passed to `PlaceOrder`.

### 6. GST Rate

Both legacy and modern use `GST_RATE = 0.18m` (18%). Tax is calculated as `(SubTotal + Shipping) × GST_RATE`.

### 7. Order Number Format

Both use `RF-YYYYMMDD-XXXX` where `XXXX` is a random 4-digit number.

### 8. Category Hierarchy

The legacy `Category` table has `ParentCategoryId` (self-referencing). The modern `ProductService` resolves descendants in memory when filtering by a parent category. `# ponytail: in-memory tree walk; upgrade to recursive SQL CTE if catalog grows large`

### 9. Cart Merge on Login

The legacy application merges guest cart into customer cart on login. The modern application does **not** implement this — carts are customer-based only. Anonymous users cannot add to cart (AddToCartButton redirects to login).

### 10. Legacy MVC Cannot Start Headless

The legacy MVC application could not be started via IIS Express in headless mode during testing. This was logged as a non-blocking issue (T-028). The modern stack does not depend on the legacy app running.

## Related

- [Current State (Legacy)](../architecture/current-state.md)
- [Architecture Migration Plan](../architecture/architecture-migration-plan.md)
- [Feature Parity Report](../migration/feature-parity-report.md)