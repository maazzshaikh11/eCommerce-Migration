# ADR-003: ADO.NET Direct SQL over ORM

## Status

Accepted

## Context

The legacy application uses a custom `DatabaseHelper` class that wraps `SqlConnection`/`SqlCommand` with parameterized queries. The migration needs to decide whether to:
1. Port `DatabaseHelper` and continue with direct ADO.NET.
2. Adopt Entity Framework Core (EF Core) as the ORM.
3. Adopt Dapper as a micro-ORM.

The database schema already exists (`LegacyEcommerceDb`) with 9 tables. The legacy application's repository pattern is built on `DatabaseHelper` and `SqlDataReader`. Rewriting all data access to EF Core models would add significant migration overhead with no immediate business value.

## Decision

Port `DatabaseHelper` to the new `Ecommerce.Api.Data` project using `Microsoft.Data.SqlClient` (replacing `System.Data.SqlClient`). Continue the repository pattern with hand-written SQL queries.

```csharp
// DatabaseHelper wraps SqlConnection/SqlCommand with parameterized queries
public DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
public object ExecuteScalar(string sql, params SqlParameter[] parameters)
public SqlTransaction BeginTransaction()
```

## Consequences

**Positive:**
- Minimal migration effort — repository SQL ports directly.
- Full control over query performance.
- No ORM impedance mismatch.
- Existing stored procedures and complex queries work unchanged.

**Negative:**
- More boilerplate than EF Core for simple CRUD.
- Manual mapping of `DataRow` to domain models.
- No automatic migrations (schema changes require manual SQL scripts).

`# ponytail: direct SQL over EF Core — existing schema, existing queries, no ORM learning curve for this team. Upgrade to Dapper if mapping boilerplate becomes painful.`

## Related

- [Architecture Migration Plan — Data Access Layer](../architecture/architecture-migration-plan.md)
- [Reference — Legacy Codebase Notes](../reference/legacy-codebase-notes.md)