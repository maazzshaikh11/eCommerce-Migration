# Target State — Modern Architecture

## Overview

The target architecture is a **decoupled SPA + API** system built with Vue 3 and .NET 10 Web API, using the **Strangler Fig pattern** for incremental migration. The legacy MVC application is fully decommissioned — all traffic routes to the modern stack.

## Topology

```
┌──────────────────────────────────────────────────┐
│                  Client Browser                  │
└──────────────────────┬───────────────────────────┘
                       │
┌──────────────────────▼───────────────────────────┐
│            YARP Reverse Proxy (:5003)            │
│   Routes all paths to SPA cluster (:5002)        │
│   API controllers handle /api/* natively         │
└──────────┬───────────────────────┬───────────────┘
           │                       │
┌──────────▼──────────┐  ┌────────▼────────────────┐
│  .NET 10 Web API    │  │    Vue 3 SPA (Vite)     │
│  (:5001)            │  │  (:5002 dev / preview)  │
│                     │  │                         │
│  • JWT Auth         │  │  • Pinia stores         │
│  • Controllers      │  │  • Vue Router           │
│  • Services         │  │  • Composition API      │
│  • Repositories     │  │  • fetch + Bearer JWT   │
│  • ADO.NET          │  │                         │
└──────────┬──────────┘  └─────────────────────────┘
           │
┌──────────▼──────────┐
│   SQL Server        │
│   LegacyEcommerceDb │
└─────────────────────┘
```

## Backend — .NET 10 Web API

### Project Structure

| Project | Purpose | Depends On |
|---------|---------|------------|
| `Ecommerce.Api` | Controllers, DI, YARP, middleware | Core, Services, Data |
| `Ecommerce.Api.Core` | Models, DTOs, Interfaces, Security | — |
| `Ecommerce.Api.Services` | Business logic implementations | Core |
| `Ecommerce.Api.Data` | ADO.NET repositories, `DatabaseHelper` | Core |
| `Ecommerce.Api.Migrations` | Database migration tool (placeholder) | Data |

### Key Files

| File | Responsibility |
|------|---------------|
| `Program.cs` | DI registration, JWT config, YARP, CORS, pipeline |
| `Controllers/AuthController.cs` | `POST /api/auth/login`, `POST /api/auth/register`, `GET /api/auth/me` |
| `Controllers/ProductsController.cs` | Product listing, detail, featured |
| `Controllers/CategoriesController.cs` | Category tree |
| `Controllers/CartController.cs` | Cart CRUD |
| `Controllers/CheckoutController.cs` | Addresses, shipping options, payment validation, place order |
| `Controllers/OrdersController.cs` | Order list, detail, confirmation |
| `Core/Security/JwtTokenService.cs` | HS256 token generation |
| `Data/DatabaseHelper.cs` | SQL connection/transaction wrapper |

### Authentication

- **JWT Bearer** with HS256 symmetric key
- Claims: `customer_id`, `email`
- Token stored in `localStorage['jwt']` on the SPA
- `Authorization: Bearer <token>` header on all authenticated requests
- Expiry checked on app init; expired tokens auto-cleared

## Frontend — Vue 3 SPA

### Project Structure

```
src/frontend/spa/src/
├── api/
│   └── client.ts          ← fetch wrapper with JWT auth
├── assets/
├── components/
│   ├── AddToCartButton.vue
│   ├── ProductCard.vue
│   ├── SiteFooter.vue
│   └── SiteHeader.vue
├── router/
│   └── index.ts           ← Vue Router routes
├── stores/
│   ├── useAuthStore.ts    ← Auth state, JWT, profile
│   ├── cartStore.ts       ← Cart items, totals
│   ├── categoryStore.ts   ← Category tree
│   ├── checkoutStore.ts   ← Checkout flow state
│   ├── orderStore.ts      ← Order history
│   └── productStore.ts    ← Product list, detail, filters
├── types/
│   └── index.ts           ← TypeScript interfaces (DTOs)
├── utils/
│   └── currency.ts        ← INR formatter (Intl.NumberFormat)
└── views/
    ├── AccountDashboard.vue
    ├── CartView.vue
    ├── CheckoutWizard.vue
    ├── HomeView.vue
    ├── LoginView.vue
    ├── OrderConfirmationView.vue
    ├── OrderDetailView.vue
    ├── OrderListView.vue
    ├── ProductDetailView.vue
    ├── ProductListView.vue
    ├── ProfileView.vue
    └── RegisterView.vue
```

### Key Patterns

- **Composition API** with `<script setup lang="ts">` everywhere
- **Pinia** for state management (setup stores)
- **Vue Router** with `createWebHistory()`
- **Native `fetch`** for HTTP (no Axios)
- **INR currency formatting** via shared `formatINR()` util
- **Auth-gated actions** — AddToCartButton redirects to login if anonymous

## Ports

| Service | Port | Environment |
|---------|------|-------------|
| .NET 10 API | 5001 | Development |
| Vue 3 SPA (Vite preview) | 5002 | Development |
| YARP Reverse Proxy | 5003 | Development |

## Related

- [Current State (Legacy)](current-state.md)
- [Data Flow](data-flow.md)
- [Auth Strategy](auth-strategy.md)
- [Deployment Model](deployment-model.md)