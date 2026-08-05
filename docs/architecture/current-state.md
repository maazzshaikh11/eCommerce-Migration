# Current State — Legacy Application

## Overview

The legacy application is an **ASP.NET MVC 5** monolith running on **.NET Framework 4.7**. It is a server-rendered eCommerce application with Razor views, jQuery AJAX for partial updates, and ADO.NET for data access.

> **Note:** The legacy source code lives in a separate repository (`C:\Users\Administrator\Desktop\eCommerce`) and is **read-only**. The modernization monorepo contains only the new implementation. The legacy source is referenced exclusively during migration planning.

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET MVC 5 (.NET Framework 4.7) |
| View Engine | Razor (`.cshtml`) |
| Data Access | ADO.NET via custom `DatabaseHelper` |
| Auth | ASP.NET Identity 2.0 + OWIN cookies |
| DI Container | Unity |
| Client Script | jQuery, jQuery AJAX |
| Database | SQL Server — two databases: `LegacyEcommerceDb` (business) + `LegacyEcommerceIdentity` (auth) |

## Controllers

| Controller | Responsibility |
|-----------|---------------|
| `HomeController` | Landing page, featured products |
| `ProductController` | Product listing, detail, search, filter |
| `CartController` | Cart view, add/remove/update |
| `CheckoutController` | Address → Shipping → Payment → Review → Confirm |
| `OrderController` | Order list, order detail |
| `AccountController` | Login, register, profile, logout |
| `CategoryController` | Category tree (child action) |
| `ErrorController` | Error pages |

## Service Layer

| Service | Methods |
|---------|---------|
| `ProductService` | `GetProducts`, `GetProductDetail`, `GetFeaturedProducts` |
| `CartService` | `GetCartItems`, `AddToCart`, `UpdateQuantity`, `RemoveFromCart`, `ClearCart` |
| `CategoryService` | `GetCategoryTree`, `GetById` |
| `CheckoutService` | `GetShippingOptions`, `ValidateAddress`, `ValidatePayment`, `PlaceOrder` |
| `OrderService` | `GetOrders`, `GetOrderDetail` |

## Repository Layer

All repositories inherit from a generic `Repository<T>` base and use `DatabaseHelper` for SQL execution:

- `ProductRepository`, `CartItemRepository`, `CategoryRepository`
- `OrderRepository`, `CustomerRepository`, `AddressRepository`

## Database Schema

The `LegacyEcommerceDb` database has 9 tables:

| Table | Purpose |
|-------|---------|
| `Product` | Products with price, image, category |
| `Category` | Hierarchical categories (self-referencing `ParentCategoryId`) |
| `ProductImage` | Multiple images per product |
| `ProductVariant` | Size/color variants with stock and price adjustment |
| `CartItem` | Shopping cart (session-based or customer-based) |
| `Customer` | Customer profiles (linked to Identity users) |
| `Address` | Shipping/billing addresses |
| `Order` | Orders with totals, status, idempotency key |
| `OrderItem` | Line items within orders |

## Coupling Points

1. **Controller-View coupling** — ViewModels are shaped for Razor rendering.
2. **Auth in MVC pipeline** — `User.Identity.GetUserId()` resolves `CustomerId`.
3. **HttpContext leakage** — Session/cart state in form posts.
4. **Shared layout state** — Cart count, category tree computed server-side per request.
5. **`DatabaseHelper` hard-wired** — Singleton in Unity, `System.Data.SqlClient`.

## Related

- [Target State](target-state.md)
- [Architecture Migration Plan](architecture-migration-plan.md)
- [Reference — Legacy Codebase Notes](../reference/legacy-codebase-notes.md)