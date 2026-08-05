# API Endpoint Catalog

## Overview

The .NET 10 Web API exposes RESTful JSON endpoints under `/api/`. All authenticated endpoints require a `Authorization: Bearer <JWT>` header.

## Base URL

```
Development: http://localhost:5001/api
Production:  https://<domain>/api
```

## Authentication

| Method | Path | Auth | Request Body | Response |
|--------|------|------|-------------|----------|
| `POST` | `/api/auth/login` | None | `{ email, password }` | `{ token, customerId, email }` |
| `POST` | `/api/auth/register` | None | `{ email, password, firstName, lastName, phoneNumber }` | `{ token, customerId, email }` |
| `GET` | `/api/auth/me` | Bearer | — | `{ id, firstName, lastName, email, phoneNumber }` |

## Products

| Method | Path | Auth | Query Params | Response |
|--------|------|------|-------------|----------|
| `GET` | `/api/products` | None | `categoryId`, `searchQuery`, `minPrice`, `maxPrice`, `sortBy`, `page`, `pageSize` | `PagedResult<ProductListResponseDto>` |
| `GET` | `/api/products/{id}` | None | — | `ProductDetailDto` |
| `GET` | `/api/products/featured` | None | `count` | `ProductListResponseDto[]` |

### Sort Options

| `sortBy` Value | Behavior |
|----------------|---------|
| `Newest` (default) | Order by `Id DESC` |
| `price_asc` | Order by `Price ASC` |
| `price_desc` | Order by `Price DESC` |
| `name` | Order by `Name ASC` |

## Categories

| Method | Path | Auth | Response |
|--------|------|------|----------|
| `GET` | `/api/categories` | None | `CategoryDto[]` (tree with `Children`) |
| `GET` | `/api/categories/{id}` | None | `CategoryDto` |

## Cart

All cart endpoints require authentication. The `customerId` is extracted from the JWT `customer_id` claim.

| Method | Path | Auth | Request Body | Response |
|--------|------|------|-------------|----------|
| `GET` | `/api/cart` | Bearer | — | `CartItemDto[]` |
| `POST` | `/api/cart/items` | Bearer | `{ productId, quantity, variantId? }` | `CartItemDto` |
| `PUT` | `/api/cart/items/{id}` | Bearer | `{ quantity }` | No content |
| `DELETE` | `/api/cart/items/{id}` | Bearer | — | No content |
| `DELETE` | `/api/cart` | Bearer | — | No content |

## Checkout

All checkout endpoints require authentication.

| Method | Path | Auth | Request Body | Response |
|--------|------|------|-------------|----------|
| `GET` | `/api/checkout/addresses` | Bearer | — | `AddressDto[]` |
| `POST` | `/api/checkout/addresses` | Bearer | `AddressDto` | `{ id }` |
| `GET` | `/api/checkout/shipping-options` | Bearer | — | `ShippingOptionDto[]` |
| `POST` | `/api/checkout/validate-payment` | Bearer | `PaymentRequest` | `{ valid }` |
| `POST` | `/api/checkout/orders` | Bearer | `PlaceOrderRequest` | `OrderConfirmationDto` (201) |

> **Idempotency:** `POST /api/checkout/orders` requires an `Idempotency-Key` HTTP header. Duplicate keys return the original order.

## Orders

All order endpoints require authentication.

| Method | Path | Auth | Response |
|--------|------|------|----------|
| `GET` | `/api/orders` | Bearer | `OrderDto[]` |
| `GET` | `/api/orders/{id}` | Bearer | `OrderConfirmationDto` |
| `GET` | `/api/checkout/orders/{id}/confirmation` | Bearer | `OrderConfirmationDto` |

## Health

| Method | Path | Auth | Response |
|--------|------|------|----------|
| `GET` | `/api/health` | None | `200 OK` |

## DTO Reference

### ProductListResponseDto
```json
{
  "id": 1,
  "name": "Wireless Mouse",
  "description": "...",
  "price": 1999.00,
  "thumbnailUrl": "/images/...",
  "categoryName": "Electronics"
}
```

### ProductDetailDto
```json
{
  "id": 1,
  "name": "Wireless Mouse",
  "description": "...",
  "price": 1999.00,
  "imageUrl": "/images/...",
  "thumbnailUrl": "/images/thumb/...",
  "isActive": true,
  "createdDate": "2026-01-01T00:00:00Z",
  "categoryName": "Electronics",
  "images": [ProductImageDto],
  "variants": [ProductVariantDto]
}
```

### ShippingOptionDto
```json
{
  "method": "Standard",
  "label": "Standard Shipping (5-7 business days)",
  "cost": 99.00
}
```

### OrderConfirmationDto
```json
{
  "orderId": 1,
  "orderNumber": "RF-20260805-1234",
  "orderDate": "2026-08-05T12:00:00Z",
  "orderStatus": "Paid",
  "subTotal": 1999.00,
  "shippingTotal": 99.00,
  "taxTotal": 377.64,
  "grandTotal": 2475.64,
  "shippingAddressFormatted": "123 Main St, Mumbai, MH 400001, India",
  "items": [OrderItemDto]
}
```

## Related

- [Architecture — Data Flow](../architecture/data-flow.md)
- [OpenAPI Spec](openapi.yaml)