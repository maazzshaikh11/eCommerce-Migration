# Database Seeding

## Overview

The application uses an existing SQL Server database (`LegacyEcommerceDb`). This runbook describes how to seed the database with initial data for development and testing.

## Scripts

| Script | Platform | Purpose |
|--------|----------|---------|
| `scripts/setup.sh` | Linux/macOS | Create database + tables + seed data |
| `scripts/setup.ps1` | Windows | Same, PowerShell |
| `scripts/db-seed.sh` | Linux/macOS | Seed data only (assumes tables exist) |
| `scripts/db-seed.ps1` | Windows | Same, PowerShell |
| `scripts/migrate.sh` | Linux/macOS | Run database migrations |
| `scripts/migrate.ps1` | Windows | Same, PowerShell |

## Database Schema

The `LegacyEcommerceDb` database has 9 tables:

```sql
-- Core tables
Product          (Id, Name, Description, Price, CategoryId, ImageUrl, ThumbnailUrl, IsActive, CreatedDate)
Category         (Id, Name, Description, ThumbnailUrl, ParentCategoryId)
ProductImage     (Id, ProductId, ImageUrl, IsPrimary, DisplayOrder)
ProductVariant   (Id, ProductId, VariantName, VariantValue, PriceAdjustment, StockQuantity)
CartItem         (Id, CustomerId, SessionId, ProductId, VariantId, Quantity, UnitPrice)
Customer         (Id, FirstName, LastName, Email, PasswordHash, PhoneNumber)
Address          (Id, CustomerId, AddressType, StreetAddress, City, StateProvince, PostalCode, Country, IsDefault)
Order            (Id, CustomerId, OrderDate, OrderStatus, OrderNumber, SubTotal, ShippingTotal, TaxTotal, GrandTotal, ShippingAddressId, BillingAddressId, IdempotencyKey)
OrderItem        (Id, OrderId, ProductId, ProductName, Quantity, UnitPrice, LineTotal)
```

## Key Constraints

- `Category.ParentCategoryId` — self-referencing FK for hierarchy
- `Order.IdempotencyKey` — unique index for idempotent order placement
- `CartItem` — supports both session-based (anonymous) and customer-based (authenticated) carts

## Verification

After seeding, verify the database:

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LegacyEcommerceDb -Q "SELECT COUNT(*) FROM Product"
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LegacyEcommerceDb -Q "SELECT COUNT(*) FROM Category"
```

## Related

- [Local Setup](local-setup.md)
- [Architecture — Data Flow](../architecture/data-flow.md)