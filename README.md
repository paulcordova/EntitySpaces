<img src="https://repository-images.githubusercontent.com/194275145/55b5b080-1ccf-11ea-8609-15b9de0d2351" alt="EntitySpaces" width="531" height="268">

# EntitySpaces ORM — .NET 4.8 to .NET 10

Actively maintained ORM for .NET, focused on long-term stability and modern .NET alignment. Originally created by Mike Griffin, this fork extends support to current .NET versions while preserving the proven EntitySpaces architecture.

## Supported Targets

| Version | Status |
|---------|--------|
| .NET Framework 4.8 | ✅ Active |
| .NET 8 | ✅ Active |
| .NET 9 | ✅ Active |
| .NET 10 | ✅ Active |
| .NET 6 / .NET 7 | ⚠️ Maintained in 2025.02 branch — not included in future NuGet releases |

## Available on NuGet

| Provider | Package |
|----------|---------|
| SQL Server | [EntitySpaces.ORM.SqlServer.NET](https://www.nuget.org/packages/EntitySpaces.ORM.SqlServer.NET/) |
| PostgreSQL | [EntitySpaces.ORM.PostgreSQL.NET](https://www.nuget.org/packages/EntitySpaces.ORM.PostgreSQL.NET/) |
| MySQL | [EntitySpaces.ORM.MySQL.NET](https://www.nuget.org/packages/EntitySpaces.ORM.MySQL.NET/) |
| SQLite | [EntitySpaces.ORM.SQLite.NET](https://www.nuget.org/packages/EntitySpaces.ORM.SQLite.NET/) |
| Oracle Managed Client | [EntitySpaces.ORM.OracleManagedClient.NET](https://www.nuget.org/packages/EntitySpaces.ORM.OracleManagedClient.NET/) |
| Firebird | [EntitySpaces.ORM.Firebird.NET](https://www.nuget.org/packages/EntitySpaces.ORM.Firebird.NET/) |

Original project site: [https://mikegriffinreborn.github.io/EntitySpaces/](https://mikegriffinreborn.github.io/EntitySpaces/)

---

# About This Fork

I am Paul Córdova, long-time EntitySpaces user and current maintainer of this modern .NET fork, responsible for multi-provider alignment and .NET version compatibility.

EntitySpaces was originally a commercial ORM widely adopted in enterprise systems. Its architecture remains solid, particularly for teams that:

- Prefer explicit SQL control
- Work with complex relational models
- Need predictable query generation
- Value compile-time safety over runtime abstraction

This fork focuses on:

- Multi-targeting modern .NET versions
- Provider alignment across database engines
- SQL Server modernization for SQL Server 2016–2025 and Microsoft.Data.SqlClient
- MySQL / MariaDB LATERAL JOIN support with automatic engine detection
- PostgreSQL modernization for PG 12–17 and Npgsql 7–10
- Clean compilation with warnings resolved
- Correct exception propagation and improved debug support (`LastQuery` always available)
- Reliable NuGet packaging
- Thread-safe parameter cache using `ConcurrentDictionary` across all providers, eliminating lock contention in multi-threaded environments
- Unified connection pool safety pattern (`hasError` + conditional rollback) across all five providers, preventing connection leaks and transaction state corruption in web applications

The goal is not to redesign EntitySpaces, but to preserve and evolve its proven architecture for current .NET ecosystems.

---
## Why Now? The Bridge from Legacy to Modern Web

Transitioning legacy WinForms and WebForms applications to the web is one of the greatest challenges—and opportunities—in enterprise software today. These systems often rely on outdated threading models and explicit locking mechanisms that fail under the concurrent load of modern web servers.

This fork addresses these foundational issues head-on. By refactoring the core parameter cache to use `ConcurrentDictionary` across **every** supported database provider, EntitySpaces now delivers true thread-safety without the bottlenecks of explicit `lock` statements. This unlocks several critical advantages for modernization projects:

- **Scalability under Web Load**: Your application handles concurrent HTTP requests efficiently, eliminating contention and deadlocks in the data access layer when hosted in IIS, Kestrel, or cloud environments.

- **Cloud-Ready Connection Management**: Safe connection pool handling — now unified across all providers with a standard `hasError + ROLLBACK` pattern — ensures that errors during `Save()` or `Load()` never leave connections in a broken state. This prevents resource leaks and application crashes, a common pain point when moving legacy apps to the cloud.
  
- **Preserve Business Logic, Modernize the UI**: Paired with modern web frameworks like **Wisej.NET**—which mirrors the WinForms programming model—you can lift-and-shift your existing business logic and EntitySpaces queries directly to the web. There is no need to rewrite hundreds of stored procedures or complex SQL joins.

- **Future-Proof Multi-Provider Support**: With automatic engine detection and full support for SQL Server, PostgreSQL, MySQL, SQLite, and Oracle, your modernized application is ready for Azure, AWS, Oracle Cloud, or hybrid on-premise environments without locking you into a single vendor.

Your teams already know SQL and your existing ORM patterns. This modernization path preserves that investment while giving your applications a future-proof, web-ready architecture. Time-to-market is measured in weeks, not years.

---

# Why Choose EntitySpaces?

EntitySpaces is designed for teams that:

- Work with complex relational schemas
- Need full control over generated SQL
- Prefer compile-time validation over runtime abstraction
- Maintain long-lived enterprise systems

Unlike heavy abstraction ORMs that attempt to conceal SQL complexity, EntitySpaces embraces SQL as a first-class concern. The result is:

- Predictable SQL generation
- High performance
- Explicit query construction
- Easier debugging
- Safer schema evolution

If your team understands SQL, you already understand EntitySpaces.

---

# Database Support Matrix

| Database | Package | Status | Notes |
|----------|---------|--------|-------|
| SQL Server | EntitySpaces.ORM.SqlServer.NET | ✅ Modernized | SQL Server 2016–2025 · Concurrency exception detection · Connection pool safety |
| PostgreSQL | EntitySpaces.ORM.PostgreSQL.NET | ✅ Modernized | PG 12–17 · Npgsql 7–10 · Neon compatible |
| MySQL | EntitySpaces.ORM.MySQL.NET | ✅ Modernized | MySQL 8.0.14+ · MariaDB 10.2+ · MySql.Data 9.x · Concurrency exception detection |
| SQLite | EntitySpaces.ORM.SQLite.NET | ✅ Modernized | SQLite 3.x · System.Data.SQLite 1.0.119 · Auto-increment detection · FK enforcement · Concurrency exception detection |
| Oracle | EntitySpaces.ORM.OracleManagedClient.NET | ✅ Modernized | Oracle 12c–19c · ODP.NET Managed · Oracle Cloud ATP · Concurrency exception detection · Connection pool safety · Navigation properties · Thread-safe parameter cache · .NET 8 compatible |
| Firebird | EntitySpaces.ORM.Firebird.NET | ✅ Active | |

---

# PostgreSQL Modernization

> **Validated with:** PostgreSQL 13 · PostgreSQL 17 · Npgsql 7.x–10.x · Neon PostgreSQL cloud services

The PostgreSQL provider has been significantly modernized for compatibility with current PostgreSQL servers and modern Npgsql versions.

Validated and updated features include:

- PostgreSQL 13 through PostgreSQL 17
- Npgsql 7.x through 10.x
- `GENERATED BY DEFAULT AS IDENTITY` — auto and explicit PK insert
- `GENERATED ALWAYS AS IDENTITY`
- `INSERT ... RETURNING` — consolidated single round-trip for all output columns
- Concurrency exception detection and translation
- Server version auto-detection and cache per connection string
- `LIMIT`/`OFFSET` translation
- `APPLY` query translation using PostgreSQL `LATERAL` joins
- `JOIN LATERAL` and `LEFT JOIN LATERAL` support
- Compatible with Neon PostgreSQL cloud services

## PostgreSQL APPLY Support

EntitySpaces supports automatic translation of SQL Server APPLY semantics into native PostgreSQL LATERAL joins. Queries written for SQL Server with APPLY semantics are automatically translated — no code changes required when switching providers.

| EntitySpaces / SQL Server | PostgreSQL |
|---------------------------|------------|
| `OUTER APPLY` | `LEFT JOIN LATERAL` |
| `CROSS APPLY` | `JOIN LATERAL` |
| `TOP(n)` | `LIMIT n` |

**C# query (provider-agnostic):**

```csharp
var coll = new CustomersQuery("c", out var c)
    .OuterApply<OrdersQuery>(out var o, () =>
    {
        return new OrdersQuery("o", out var subQuery)
            .Select(subQuery.OrderId, subQuery.OrderDate)
            .Top(2)
            .Where(subQuery.CustomerId == c.CustomerId)
            .OrderBy(subQuery.OrderDate.Descending);
    })
    .Select(c.CustomerId, c.CompanyName, o.OrderId)
    .ToCollection<CustomersCollection>();
```

**Generated PostgreSQL SQL:**

```sql
SELECT
    c."customer_id",
    c."company_name",
    o."order_id"
FROM "customers" c
LEFT JOIN LATERAL
(
    SELECT
        o."order_id",
        o."order_date"
    FROM "orders" o
    WHERE o."customer_id" = c."customer_id"
    ORDER BY o."order_date" DESC
    LIMIT 2
) AS o ON TRUE
```

## PostgreSQL Identity Support

Supported identity strategies:

- `SERIAL`
- `BIGSERIAL`
- `GENERATED BY DEFAULT AS IDENTITY`
- `GENERATED ALWAYS AS IDENTITY`

Identity retrieval uses modern PostgreSQL `RETURNING` semantics. All output columns — identity PKs, columns with defaults, concurrency columns, and special columns — are returned in a single `INSERT ... RETURNING` clause, eliminating the need for a second `SELECT` round-trip.

### Columns with Default Values

When a column has a database-level default (e.g. `DEFAULT 0`) and no value is assigned by the application, the provider sends `DBNull.Value` as the parameter value and retrieves the actual default via `RETURNING`. This is required by Npgsql — passing `null` instead of `DBNull.Value` raises a parameter exception at runtime.

```csharp
// UnitPrice has DEFAULT 0 in DDL — not assigned here
var product = new Products
{
    ProductName  = "WidgetX",
    CategoryId   = cat.CategoryId,
    SupplierId   = sup.SupplierId,
    Discontinued = 0
};
product.Save();

// UnitPrice is populated from the database DEFAULT after save
Console.WriteLine(product.UnitPrice); // 0
```

### Explicit PK Insert on IDENTITY Columns

PostgreSQL `GENERATED BY DEFAULT AS IDENTITY` allows inserting an explicit value when needed. EntitySpaces detects whether a PK value was explicitly assigned and adjusts the INSERT accordingly:

```csharp
// Explicit PK — inserts value directly, sequence not used
var product = new Products { ProductId = 999, ProductName = "WidgetX" };
product.Save();

// Auto PK — sequence generates the next available ID via RETURNING
var product = new Products { ProductName = "WidgetX" };
product.Save();
Console.WriteLine(product.ProductId); // populated after save
```

> **Note:** When loading data with explicit IDs (e.g. seed scripts), run `setval` to resync sequences
> so the next auto-generated ID does not collide with existing data:
> ```sql
> SELECT setval(pg_get_serial_sequence('products', 'product_id'), MAX(product_id)) FROM products;
> ```

## PostgreSQL Concurrency Exception Detection

The provider translates PostgreSQL-specific error codes into `esConcurrencyException`, compatible with .NET Framework 4.8, .NET 8, and .NET 10:

| PostgreSQL Code | Condition | Translated To |
|----------------|-----------|---------------|
| `23505` | Unique / PK violation | `esConcurrencyException` |
| `40001` | Serialization failure | `esConcurrencyException` |
| `40P01` | Deadlock detected | `esConcurrencyException` |
| `55P03` | Lock not available | `esConcurrencyException` |

```csharp
try
{
    product.Save();
}
catch (esConcurrencyException ex)
{
    // Duplicate key, deadlock, or serialization conflict
    Console.WriteLine(ex.Message);
}
```

## PostgreSQL Server Version Cache

The provider auto-detects the PostgreSQL server version on the first operation per connection string and caches it for the lifetime of the process. No configuration required — supports multiple PostgreSQL instances of different versions running simultaneously.

```
PostgreSQL 13.23  →  detected and cached on first query
PostgreSQL 17.2   →  detected and cached on first query (separate cache entry)
```

## PostgreSQL Connection String

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.PostgreSQLClientProvider";
conn.Schema = "public";
conn.ConnectionString = "Host=myserver;Port=5432;Database=mydb;" +
                        "Username=myuser;Password=mypassword;SSL Mode=Require;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

**Neon PostgreSQL (serverless cloud):**

```csharp
conn.ConnectionString = "Host=ep-xxx.us-east-1.aws.neon.tech;Port=5432;" +
                        "Database=mydb;Username=myuser;Password=mypassword;" +
                        "SSL Mode=Require;Channel Binding=Require;";
```

> **Development tip:** Add `Include Error Detail=true` to the connection string to get full
> PostgreSQL error details in exception messages. Do not use in production — may expose
> sensitive data.
>
> ```
> Host=...;Database=...;Include Error Detail=true;
> ```

## PostgreSQL DDL Recommendations

For best compatibility across PostgreSQL 13–17, define IDENTITY columns on a single line:

```sql
-- Recommended — single line, works correctly on all versions and clients
product_id INTEGER GENERATED BY DEFAULT AS IDENTITY (START WITH 100 INCREMENT BY 1) NOT NULL,

-- Avoid — multi-line definition can be misinterpreted by some SQL clients
product_id INTEGER GENERATED BY DEFAULT AS IDENTITY
                   (START WITH 100 INCREMENT BY 1) NOT NULL,
```

---

# SQL Server Modernization

> **Validated with:** SQL Server 2016 · SQL Server 2017 · SQL Server 2025 (17.x) · Microsoft.Data.SqlClient 6.x / 7.x · .NET Framework 4.8 · .NET 8–10

The SQL Server provider has been modernized for robustness, correctness, and compatibility across all current SQL Server versions.

## Concurrency Exception Detection

`CheckForConcurrencyException` now covers the full set of SQL Server runtime errors that represent concurrency or constraint conflicts, translating them to `esConcurrencyException` for uniform handling across providers:

| Error Number | Condition | Translated To |
|---|---|---|
| `532` | Timestamp / rowversion mismatch | `esConcurrencyException` |
| `547` | CHECK or FK constraint violation | `esConcurrencyException` |
| `1205` | Deadlock victim | `esConcurrencyException` |
| `1222` | Lock request timeout | `esConcurrencyException` |
| `2601` | Unique index violation | `esConcurrencyException` |
| `2627` | PK / unique constraint violation | `esConcurrencyException` |

Error 547 uses `SqlError.Message` (which always contains the constraint name) rather than `SqlException.Message`, ensuring the constraint name is available on all SQL Server versions including SQL Server 2025.

```csharp
try
{
    product.Save();
}
catch (esConcurrencyException ex)
{
    // Duplicate key, deadlock, lock timeout, or constraint violation
    Console.WriteLine(ex.Message);
}
```

## Connection Pool Safety

All save and load operations implement safe connection pool management. If an error occurs during `Save()` or `LoadCollection()`, the provider:

- Sets a `hasError` flag on exception
- Issues `IF @@TRANCOUNT > 0 ROLLBACK` in the `finally` block before returning the connection to the pool
- Prevents connections in a broken transaction state from being reused by subsequent operations

This matches the same robustness pattern implemented in the PostgreSQL and MySQL providers.

## SET XACT_ABORT ON

All generated INSERT, UPDATE, and DELETE statements include `SET XACT_ABORT ON`. This ensures that any runtime error — CHECK constraint, FK violation, arithmetic overflow — immediately aborts the transaction and propagates a `SqlException` to the caller, with consistent behavior across all SQL Server versions.

## Server Version Auto-Detection

The provider auto-detects the SQL Server version on the first operation per connection string and caches it for the lifetime of the process. This is used internally for version-dependent SQL generation (e.g. `rowversion` comparison syntax). No configuration required.

`conn.DatabaseVersion` is still supported as an explicit override when needed, but is no longer required for correct operation.

```
SQL Server 2016 (13.x)  →  detected and cached on first query
SQL Server 2025 (17.x)  →  detected and cached on first query (separate cache entry)
```

## OUTPUT INSERTED for Server-Generated Columns

Columns with `IsComputed`, `IsConcurrency` (rowversion/timestamp), or `IsEntitySpacesConcurrency` are retrieved via `OUTPUT INSERTED ... INTO @output_vals` in a single round-trip — no second SELECT required.

Columns with `HasDefault` (non-GUID) use a post-insert `SELECT ... WHERE pk = SCOPE_IDENTITY()` instead of `OUTPUT...INTO`. This avoids a SQL Server 2025 regression where `OUTPUT...INTO @table_variable` inside a parameterized batch silently suppresses CHECK constraint exceptions.

## Known Limitation — SQL Server 2025 Express

SQL Server 2025 Express (17.0.x) has a regression where CHECK and FK constraint violations inside parameterized batches with `Output` parameters do not propagate as `SqlException`. The INSERT is silently rejected (0 rows affected) and `ExecuteNonQuery()` returns without error. This does not affect SQL Server 2016 Express or any non-Express edition.

Tests that validate constraint enforcement should use `Assert.Inconclusive` with server version detection when running against SQL Server 2025 Express:

```csharp
if (IsSqlServer2025())
    Assert.Inconclusive(
        "Skipped on SQL Server 2025 (17.x): CHECK/FK constraint violations inside " +
        "parameterized batches with Output parameters do not propagate as exceptions. " +
        "Confirmed on Express 17.0.4040 with Microsoft.Data.SqlClient 6.x/7.x. " +
        "Does not reproduce on SQL Server 2016 Express.");
```

## SQL Server Connection String

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SqlClientProvider";
conn.ConnectionString = "User ID=sa;Password=mypassword;Initial Catalog=Northwind;" +
                        "Data Source=localhost;TrustServerCertificate=True;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

> `conn.DatabaseVersion` is optional. When omitted, the provider auto-detects the server version from `SELECT @@VERSION` on the first call and caches it per connection string.

---

# SQLite Modernization

> **Validated with:** SQLite 3.46.1 · System.Data.SQLite.Core 1.0.119 · Northwind model

The SQLite provider has been significantly modernized for correct auto-increment detection, foreign key enforcement, and concurrency exception translation.

## Auto-Increment Detection

SQLite has two equivalent forms of auto-increment that EntitySpaces now correctly detects in both the provider and Studio code generator:

| Form | DDL | Notes |
|------|-----|-------|
| Rowid alias | `"Id" INTEGER PRIMARY KEY` | Most common — no keyword required |
| Explicit | `"Id" INTEGER PRIMARY KEY AUTOINCREMENT` | Stricter — never reuses deleted IDs |

Both forms are detected and generate `c.IsAutoIncrement = true` in the class metadata, enabling correct `last_insert_rowid()` retrieval after insert.

> **DDL Recommendation:** Use `INTEGER PRIMARY KEY` (without `AUTOINCREMENT`) for most tables.
> SQLite automatically assigns the next available rowid — no `sqlite_sequence` overhead.
> Use `AUTOINCREMENT` only when strict non-reuse of deleted IDs is required.

### Explicit PK Insert on Auto-Increment Tables

EntitySpaces detects whether a PK value was explicitly assigned and adjusts the INSERT accordingly:

```csharp
// Explicit PK — inserts value directly, rowid not generated
var cat = new Category { Id = 999, CategoryName = "Special" };
cat.Save();

// Auto PK — SQLite generates the next rowid
var cat = new Category { CategoryName = "Beverages" };
cat.Save();
Console.WriteLine(cat.Id); // populated after save
```

## Foreign Key Enforcement

SQLite does not enforce foreign key constraints by default — `PRAGMA foreign_keys = ON` must be issued per connection. The provider now automatically appends `Foreign Keys=True` to the connection string, ensuring FK constraints are enforced on every connection without any configuration change.

```csharp
// FK violation now correctly raises esConcurrencyException
var order = new Order { EmployeeId = 9999 }; // non-existent employee
order.Save(); // throws esConcurrencyException
```

## Concurrency Exception Detection

The provider translates SQLite constraint violations into `esConcurrencyException`, consistent with other EntitySpaces providers:

| SQLite Code | Condition | Translated To |
|-------------|-----------|---------------|
| `19` (`SQLITE_CONSTRAINT`) | PK/UNIQUE/FK/CHECK violation | `esConcurrencyException` |

```csharp
try
{
    entity.Save();
}
catch (esConcurrencyException ex)
{
    // Duplicate key, FK violation, or CHECK constraint failure
    Console.WriteLine(ex.Message);
}
```

## Navigation Properties

Foreign key relationships defined in the DDL are read by the Studio and generate navigation properties:

```csharp
// Load category with its products
var cat = new Category();
cat.LoadByPrimaryKey(1);

var products = cat.ProductCollectionByCategoryId;
// ProductCollection loaded via FK Category.Id → Product.CategoryId
```

## Studio — Standalone Code Generator

The EntitySpaces Standalone Studio is a WinForms application (net481) used to connect to databases and generate C# entity classes.

### Supported databases in Studio

| Database | Driver | Version |
|----------|--------|---------|
| SQL Server / Azure SQL | System.Data.SqlClient | — |
| PostgreSQL | Npgsql | 8.0.8 |
| MySQL | MySql.Data | 9.7.0 |
| SQLite | System.Data.SQLite | 1.0.119 |
| Oracle | Oracle.ManagedDataAccess | 23.x |
| Firebird | FirebirdSql.Data.FirebirdClient | 10.3.4 |

### Assembly copy requirements (net481)

The Studio loads database drivers via reflection at runtime. For net481 projects using the legacy `.csproj` format, NuGet references that resolve from the GAC are **not copied** to the output directory automatically — causing runtime assembly load failures when the metadata engine tries to instantiate the driver.

Affected assemblies require explicit `HintPath` and `Private=True` in the `.csproj`:

```xml
<!-- Required for Npgsql — without Private=True the DLL is not copied to bin\Debug\ -->
<Reference Include="Npgsql, Version=8.0.8.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7">
    <HintPath>packages\Npgsql.8.0.8\lib\netstandard2.0\Npgsql.dll</HintPath>
    <Private>True</Private>
</Reference>

<!-- Required for System.ValueTuple — resolves from GAC without local copy otherwise -->
<Reference Include="System.ValueTuple, Version=4.0.3.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51">
    <HintPath>packages\System.ValueTuple.4.6.2\build\net471\System.ValueTuple.targets</HintPath>
    <Private>True</Private>
</Reference>
```

### Binding redirects (app.config)

The `app.config` of both the Standalone and the `MetadataEngine` project must include binding redirects aligned with the **exact versions** of installed packages. Mismatched redirects cause silent resolution failures. Key entries that must match installed versions:

```xml
<dependentAssembly>
    <assemblyIdentity name="ZstdSharp" publicKeyToken="8d151af33a4ad5cf" culture="neutral"/>
    <bindingRedirect oldVersion="0.0.0.0-0.8.8.0" newVersion="0.8.8.0"/>
</dependentAssembly>
<dependentAssembly>
    <assemblyIdentity name="Google.Protobuf" publicKeyToken="a7d26565bac4d604" culture="neutral"/>
    <bindingRedirect oldVersion="0.0.0.0-3.35.0.0" newVersion="3.35.0.0"/>
</dependentAssembly>
<dependentAssembly>
    <assemblyIdentity name="System.ValueTuple" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral"/>
    <bindingRedirect oldVersion="0.0.0.0-4.0.3.0" newVersion="4.0.3.0"/>
</dependentAssembly>
```

## Studio Metadata Engine

The Studio metadata engine for SQLite has been rewritten to use native SQLite pragmas instead of `information_schema` (which SQLite does not support):

| Operation | Mechanism |
|-----------|-----------|
| List tables | `sqlite_master WHERE type='table'` |
| Column info | `PRAGMA table_info(table)` |
| Primary keys | `PRAGMA table_info(table)` — `pk > 0` |
| Foreign keys | `PRAGMA foreign_key_list(table)` |
| Auto-increment | DDL inspection via `sqlite_master` + rowid alias detection |

## SQLite Connection String

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SQLiteProvider";
conn.ConnectionString = "Data Source=C:\path\to\database.db;Version=3;Foreign Keys=True;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

> **Note:** `Foreign Keys=True` is automatically appended by the provider if not present.
> Including it explicitly in your connection string is recommended for clarity.

## SQLite DDL Recommendations

- Define `INTEGER PRIMARY KEY` inline on the column (not as a separate table constraint) to activate the rowid alias and enable auto-increment
- Add `CHECK` constraints at the DDL level for data validation — the provider translates violations to `esConcurrencyException`
- Always define `FOREIGN KEY` constraints to enable navigation property generation in the Studio

```sql
CREATE TABLE "Product" (
    "Id"           INTEGER PRIMARY KEY,                          -- rowid alias: auto-increment
    "ProductName"  TEXT,
    "UnitPrice"    REAL NOT NULL DEFAULT 0 CHECK ("UnitPrice" >= 0),
    "CategoryId"   INTEGER NOT NULL,
    FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id")     -- enables navigation properties
);
```


---

# Oracle Modernization

> **Validated with:** Oracle 19c · Oracle Autonomous Database (ATP) · ODP.NET Managed Client 23.x · .NET Framework 4.8 · .NET 8–10

The Oracle provider has been significantly modernized for correctness, robustness, and compatibility with Oracle Cloud and modern ODP.NET versions.

## Oracle Cloud (Autonomous Database) Connection

Oracle Autonomous Database requires a **wallet** for TLS mutual authentication. Configure the wallet path via `oracle.manageddataaccess.client` in `app.config` — it cannot be passed inline in the connection string with ODP.NET Managed:

```xml
<!-- app.config — required for Oracle Autonomous Database (ATP) -->
<oracle.manageddataaccess.client>
  <version number="*">
    <settings>
      <setting name="TNS_ADMIN" value="C:\oracle\wallet"/>
      <setting name="WALLET_LOCATION" value="C:\oracle\wallet"/>
    </settings>
  </version>
</oracle.manageddataaccess.client>
```

Connection string using the service name from `tnsnames.ora`:

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.OracleManagedClientProvider";
conn.ConnectionString =
    "Data Source=(description=(retry_count=20)(retry_delay=3)" +
    "(address=(protocol=tcps)(port=1522)(host=adb.sa-valparaiso-1.oraclecloud.com))" +
    "(connect_data=(service_name=g79d3cd3fdcb525_mydb_tp.adb.oraclecloud.com))" +
    "(security=(ssl_server_dn_match=yes)));User Id=ADMIN;Password=mypassword;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

> **Wallet location:** The wallet ZIP downloaded from Oracle Cloud Console must be extracted to a local folder. The `sqlnet.ora` inside must contain an absolute path:
> ```
> WALLET_LOCATION = (SOURCE = (METHOD = file) (METHOD_DATA = (DIRECTORY="C:\oracle\wallet")))
> SSL_SERVER_DN_MATCH=yes
> ```

## Concurrency Exception Detection

`CheckForConcurrencyException` translates Oracle-specific error numbers to `esConcurrencyException`, consistent with all other EntitySpaces providers:

| Oracle Error | Condition | Translated To |
|---|---|---|
| `ORA-20101` | Custom stored-proc concurrency error | `esConcurrencyException` |
| `ORA-00001` | Unique constraint violated (duplicate PK) | `esConcurrencyException` |
| `ORA-00060` | Deadlock detected | `esConcurrencyException` |
| `ORA-08177` | Can't serialize access for this transaction | `esConcurrencyException` |

Uses `ex.Number` (not `ex.ErrorCode`) — ODP.NET returns the raw Oracle error number in `Number`, while `ErrorCode` returns an HResult-prefixed value.

```csharp
try
{
    product.Save();
}
catch (esConcurrencyException ex)
{
    // Duplicate key, deadlock, or serialization conflict
    Console.WriteLine(ex.Message);
}
```

## Connection Pool Safety

All save and load operations implement safe connection pool management. If an error occurs, the provider:

- Sets a `hasError` flag on exception
- Issues `ROLLBACK` in the `finally` block before returning the connection to the pool
- Prevents connections in an aborted transaction state from being reused

This matches the same robustness pattern implemented in the PostgreSQL, MySQL, and SQL Server providers.

## DBNull.Value for Output Parameters

ODP.NET Managed requires `DBNull.Value` on all Output parameters — passing `null` throws a parameter exception at runtime. The provider assigns `DBNull.Value` in all cases: `HasDefault` columns, `DateModified`, `ModifiedBy`, and stored procedure output parameters.

## Oracle Identity Columns

Oracle 12c+ supports `GENERATED BY DEFAULT AS IDENTITY` — the same standard syntax as PostgreSQL. EntitySpaces detects identity columns via `ALL_TAB_COLUMNS.IDENTITY_COLUMN = 'YES'` and generates the correct `IsAutoIncrement = true` metadata for the code generator.

```sql
-- Recommended Oracle DDL for auto-increment PKs
"Id" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL
```

## Decimal Column Types

Oracle uses `NUMBER(p,s)` for all exact numeric types. The provider maps `NUMBER` with `scale > 0` to `decimal` in generated C# code. Use `NUMBER(p,s)` for monetary values — not `BINARY_FLOAT` (IEEE 754 float) which is imprecise for financial data:

```sql
-- Recommended for prices, amounts, discounts
"UnitPrice" NUMBER(18,4) DEFAULT 0 NOT NULL,
"Discount"  NUMBER(8,4)  DEFAULT 0 NOT NULL,
"Freight"   NUMBER(18,4) DEFAULT 0 NOT NULL
```

| Oracle Type | C# Type Generated |
|---|---|
| `NUMBER(p,0)` or `NUMBER(p)` | `int` / `long` |
| `NUMBER(p,s)` where `s > 0` | `decimal` |
| `BINARY_FLOAT` | `float` (mapped via provider) |
| `VARCHAR2(n)` | `string` |
| `DATE` | `DateTime` |
| `CLOB` | `string` |
| `BLOB` | `byte[]` |

## Navigation Properties

Foreign key relationships defined in the DDL are read by the Studio and generate `UpTo` navigation properties. The provider reads FK metadata from `ALL_CONSTRAINTS` and `ALL_CONS_COLUMNS`, correctly resolving both sides of each relationship:

```csharp
// Load product with its category
var product = new Product();
product.LoadByPrimaryKey(1);

var category = product.UpToCategoryByCategory;
// Navigates via FK Product.CategoryId → Category.Id

var products = category.ProductCollectionByCategoryId;
// Returns all products in this category
```

## Thread-Safe Parameter Cache

The parameter cache was upgraded from a locked `Dictionary` to a `ConcurrentDictionary`, eliminating contention in multi-threaded environments:

```csharp
// Before — required explicit lock on every access
lock (parameterCache)
{
    if (!parameterCache.ContainsKey(dataID)) { ... }
}

// After — lock-free, atomic GetOrAdd
return parameterCache.GetOrAdd(dataID, id => BuildParameterDictionary(...));
```

This improves throughput in web applications or any scenario where multiple threads execute queries against Oracle concurrently. Each `DataID` (one per entity type) is built once and reused safely across threads.

## Case-Sensitive Identifier Handling

Oracle stores quoted identifiers with their exact case. The provider uses double-quote delimiters (`"`) consistently for table and column names, ensuring queries match the schema exactly as created:

```sql
-- Generated correctly with double quotes — matches Oracle stored case
SELECT od."Id", od."OrderId" FROM "OrderDetail" od WHERE od."OrderId" = :OrderId1

-- String literals use single quotes — Oracle standard
WHERE "CategoryName" = 'Beverages'
```

The `Delimiters` class was corrected to use single quotes (`'`) for string literals and double quotes (`"`) for identifiers — previously they were both set to double quotes, which caused `ORA-00911` (invalid character) on string comparisons.

## .NET 8 Compatibility

The connection setup was adjusted for full compatibility with .NET 8. In .NET 8, `ConfigurationManager` does not automatically bind the `oracle.manageddataaccess.client` section from `app.config` without an explicit call. The recommended initialization pattern:

```csharp
// Force wallet path before any Oracle connection attempt
Environment.SetEnvironmentVariable("TNS_ADMIN", @"C:\oracle\wallet");

// Explicitly open the exe configuration so ODP.NET reads the wallet section
var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.ORM.OracleManagedClient.NET";
conn.ConnectionString = ConfigurationManager.ConnectionStrings["develop"].ConnectionString;
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

## TransactionScope and ODP.NET Managed

ODP.NET Managed does **not** support Distributed Transaction Coordinator (DTC/MSDTC). Using `TransactionScope(RequiresNew)` causes a "transaction within transaction" error when cleanup code inside a `finally` block opens a new connection after an Oracle error has aborted the current transaction.

The correct pattern for test cleanup or any post-error operation is `TransactionScopeOption.Suppress`:

```csharp
// Wrap cleanup in Suppress to exit the ambient transaction
using (var suppress = new TransactionScope(TransactionScopeOption.Suppress))
{
    var cleanup = new Product();
    if (cleanup.LoadByPrimaryKey(id))
    {
        cleanup.MarkAsDeleted();
        cleanup.Save();
    }
    suppress.Complete();
}
```

Or using the `RunWithoutTransaction` helper provided in `OracleTestBase`:

```csharp
finally
{
    RunWithoutTransaction(() =>
    {
        var cleanup = new Product();
        if (cleanup.LoadByPrimaryKey(id))
        {
            cleanup.MarkAsDeleted();
            cleanup.Save();
        }
    });
}
```

> **Root cause:** After any Oracle error (ORA-00001, ORA-02291, etc.), Oracle leaves the transaction in `ABORTED` state. No further operations are allowed on that connection until an explicit `ROLLBACK` is issued. The provider handles this automatically via `hasError` + `ROLLBACK` in `finally` blocks, but `TransactionScope` promotion to DTC must be avoided entirely.

## Unit Test Suite — NorthWind Oracle

A complete integration test suite validates all Oracle provider operations against the NorthWind database schema. Tests cover:

- CRUD operations for all entities (`Category`, `Customer`, `Employee`, `Product`, `OrderTop`, `OrderDetail`)
- Auto-increment identity column assignment via `GENERATED BY DEFAULT AS IDENTITY`
- Navigation properties (`UpTo`, collection navigation)
- FK constraint violation detection (ORA-02291)
- Unique constraint violation → `esConcurrencyException` (ORA-00001)
- Check constraint validation (ORA-02290)
- `INNER JOIN`, `GROUP BY`, `UNION`, subqueries, `OUTER APPLY`
- Transaction rollback behavior

Tests run sequentially (no parallelization) due to Oracle Cloud Free Tier connection pool limits. Each test cleans up its own data via `RunWithoutTransaction` in `finally` blocks.



The Oracle metadata engine has been rewritten to use ODP.NET Managed (`Oracle.ManagedDataAccess`) instead of the legacy OLE DB driver. Key improvements:

| Operation | Mechanism |
|-----------|-----------|
| List tables | `ALL_TABLES WHERE OWNER = schemaOwner` |
| Column info | `ALL_TAB_COLUMNS` with `DATA_PRECISION`, `DATA_SCALE` |
| Primary keys | `ALL_CONSTRAINTS` where `CONSTRAINT_TYPE = 'P'` |
| Foreign keys | `ALL_CONSTRAINTS` + `ALL_CONS_COLUMNS` JOIN on `POSITION` |
| Indexes | `ALL_INDEXES` + `ALL_IND_COLUMNS` per index |
| Auto-increment | `ALL_TAB_COLUMNS.IDENTITY_COLUMN = 'YES'` |
| Views | `ALL_VIEWS WHERE OWNER = schemaOwner` |
| Stored procedures | `ALL_PROCEDURES` + `ALL_ARGUMENTS` |

The `CreateConnection()` factory now correctly returns an `OracleConnection` (previously returned `OleDbConnection`, causing silent failures).

## Oracle DDL Recommendations

- Use `NUMBER(10)` for integer PKs — not `INTEGER` (which Oracle silently maps to `NUMBER(38)`)
- Use `NUMBER(18,4)` for monetary values — not `BINARY_FLOAT`
- Use `VARCHAR2(n)` — not `VARCHAR` (Oracle treats them identically but `VARCHAR2` is the canonical form)
- Quote identifiers with double quotes to preserve mixed case: `"ProductName"` — otherwise Oracle uppercases all names
- Define FK constraints in the DDL to enable navigation property generation in Studio

```sql
CREATE TABLE "Product" (
    "Id"          NUMBER(10)   GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    "ProductName" VARCHAR2(40),
    "UnitPrice"   NUMBER(18,4) DEFAULT 0 NOT NULL,
    "CategoryId"  NUMBER(10)   NOT NULL,
    CONSTRAINT pk_product          PRIMARY KEY ("Id"),
    CONSTRAINT fk_product_category FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id")
);
```



> **Validated with:** MySQL 8.0.28 · MariaDB 10.2 / 10.6 / 10.11 · MySql.Data 9.x

The MySQL provider has been significantly modernized with automatic engine detection and provider-specific SQL generation strategies.

## MySQL / MariaDB APPLY Support

EntitySpaces automatically translates `OuterApply` / `CrossApply` into the correct SQL strategy based on the detected server engine and version — no code changes required when switching between MySQL and MariaDB.

| EntitySpaces | MySQL 8.0.14+ | MariaDB 10.2+ |
|---|---|---|
| `OuterApply` | `LEFT JOIN LATERAL` | `LEFT JOIN` + `ROW_NUMBER() OVER (PARTITION BY)` |
| `CrossApply` | `JOIN LATERAL` | `JOIN` + `ROW_NUMBER() OVER (PARTITION BY)` |
| `Top(n)` | `LIMIT n` inside LATERAL | `es_rn <= n` in outer `ON` clause |
| Without `Top()` | No LIMIT | All rows per partition returned |

Engine detection is automatic — the provider queries `SELECT VERSION()` on the first call per connection string and caches the result. No configuration required.

**C# query (provider-agnostic) — uses new `out innerQuery` parameter:**

```csharp
var coll = new CustomerQuery("c", out var c)
    .OuterApply<SalesorderQuery>(out var o, out var oCol, () =>
    {
        return (SalesorderQuery) new SalesorderQuery("o", out var subQuery)
            .Select(subQuery.OrderId, subQuery.OrderDate)
            .Top(2)
            .Where(subQuery.CustId == c.CustId)
            .OrderBy(subQuery.OrderDate.Descending);
    })
    .Select(c.CustId, c.CompanyName, oCol.OrderId)
    .ToCollection<CustomerCollection>();
```

> **Note:** Use the `out var oCol` (second out parameter) to reference lateral subquery columns in the outer `Select()`. The first `out var o` is the join handle; `oCol` exposes the column metadata.

**Generated SQL — MySQL 8.0.14+:**

```sql
SELECT c.`custId`, c.`companyName`, o.`orderId`
FROM `customer` c
LEFT JOIN LATERAL (
    SELECT o.`orderId`, o.`orderDate`
    FROM `salesorder` o
    WHERE o.`custId` = c.`custId`
    ORDER BY o.`orderDate` DESC
    LIMIT 2
) AS o ON TRUE
```

**Generated SQL — MariaDB 10.2+:**

```sql
SELECT c.`custId`, c.`companyName`, o.`orderId`
FROM `customer` c
LEFT JOIN (
    SELECT o.`orderId`, o.`orderDate`, o.`custId`,
           ROW_NUMBER() OVER (PARTITION BY o.`custId` ORDER BY o.`orderDate` DESC) AS es_rn
    FROM `salesorder` o
) AS o ON o.`custId` = c.`custId`
      AND o.es_rn <= 2
```

## MySQL Connection String

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.MySqlProvider";
conn.ConnectionString = "Server=myserver;Port=3306;Database=mydb;Uid=myuser;" +
                        "Pwd=mypassword;SslMode=Required;AllowPublicKeyRetrieval=True;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

> **SslMode note:** MySQL 8.0 with `caching_sha2_password` (default) requires either `SslMode=Required` or `SslMode=None` combined with `AllowPublicKeyRetrieval=True`. Use `SslMode=Required` for remote servers.

## MySQL Case Sensitivity

MySQL on **Linux** is case-sensitive for table and schema names (`lower_case_table_names=0`).
MySQL on **Windows** is case-insensitive. MariaDB is case-insensitive on all platforms.

EntitySpaces generates class metadata (`meta.Source`, `meta.Destination`) using the exact table names as they exist in the database at generation time. **Always generate your EntitySpaces classes directly against the target server** to guarantee case consistency.

| Platform | Behavior | Recommendation |
|---|---|---|
| MySQL on Linux | Case-sensitive | Generate classes against Linux MySQL |
| MySQL on Windows | Case-insensitive | No special action needed |
| MariaDB (all) | Case-insensitive | No special action needed |

## MySQL / MariaDB Concurrency Exception Detection

The provider translates MySQL and MariaDB-specific error codes into `esConcurrencyException`:

| MySQL / MariaDB Code | Condition | Translated To |
|---|---|---|
| `1062` | Duplicate entry — PK or unique key violation | `esConcurrencyException` |
| `1213` | Deadlock detected | `esConcurrencyException` |
| `1205` | Lock wait timeout exceeded | `esConcurrencyException` |

```csharp
try
{
    category.Save();
}
catch (esConcurrencyException ex)
{
    // Duplicate key, deadlock, or lock timeout
    Console.WriteLine(ex.Message);
}
```

## MySQL / MariaDB Connection Pool Safety

The provider implements safe connection pool management in all save and load operations. If an error occurs during `Save()` or `LoadCollection()`, the provider:

- Sets a `hasError` flag on exception
- Issues a `Transaction.Rollback()` in the `finally` block before returning the connection to the pool
- Prevents dirty connections from being reused by subsequent operations

This matches the same robustness pattern implemented in the PostgreSQL provider.


---

# Training & Consulting

If your organization:

- Maintains a legacy EntitySpaces codebase
- Plans migration to .NET 8 through .NET 10
- Needs performance optimization or provider upgrades
- Requires architectural review of ORM usage
- Wants advanced training in Fluent SQL patterns

I offer:

- Modernization strategy consulting
- Codebase audits
- Provider migration assistance
- Advanced EntitySpaces workshops
- Architecture advisory sessions

Engagements can include remote workshops, architectural reviews, migration planning, or hands-on implementation support.

For professional inquiries:

- GitHub: [github.com/paulcordova](https://github.com/paulcordova)
- LinkedIn: *https://www.linkedin.com/in/paul-cordova-benavides/*

---

# Professional Experience

I have used EntitySpaces in legacy commercial deployments as well as modern .NET 7+ production environments, including a full production application built with Wisej on .NET 7.

Its strengths in real-world business applications include:

- Rapid project bootstrapping
- Clean separation between business logic and data access
- Strong compile-time validation when schemas change
- Robust hierarchical saves and transactional integrity
- Advanced SQL capabilities without losing control

For teams managing legacy EntitySpaces systems or evaluating Fluent SQL alternatives, the architecture remains highly competitive.

---
##################################################################
---

# EntitySpaces — Fluent SQL API Reference

> *The following documentation is based on the original EntitySpaces project by Mike Griffin,
> preserved and updated here for reference. All examples are compatible with this fork.*

---

EntitySpaces is a Fluent API for SQL Server, SQLite, MySQL, PostgreSQL and more. If you are familiar with SQL syntax then you are already an expert in EntitySpaces. EntitySpaces is high performance, transactional, and very intuitive. EntitySpaces Studio is used to generate your C# classes from your database schema.

# Quick Start

```csharp
var customers = new CustomersCollection();
customers.LoadAll();

foreach (var customer in customers)
{
    Console.WriteLine(customer.CompanyName);
}
```

# Setup

1. Install [EntitySpaces Studio 2024](https://github.com/paulcordova/EntitySpaces/blob/master/EntitySpaces.Studio/EntitySpacesStudio_2024.8.30.0.zip/)

## Connection String Examples

**SQL Server**

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SqlClientProvider";
conn.DatabaseVersion = "2012";
conn.ConnectionString = "User ID=sa;Password=blank;Initial Catalog=Northwind;Data Source=localhost";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

**SQLite**

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SQLiteProvider";
conn.DatabaseVersion = "2012";
conn.ConnectionString = @"Data Source=C:\MyFolder\Northwind.db3;Version=3;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

**MySQL**

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.MySqlProvider";
conn.DatabaseVersion = "2012";
conn.ConnectionString = "Database=Northwind;Data Source=localhost;User Id=myuser;Password=mypassword;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

**Generating your Classes via EntitySpaces Studio**

Connect to your database under the Settings → Connection tab, then execute two templates: the Generated classes (regenerated when your schema changes) and the Custom classes (generated once, where you add custom logic).

<img src="https://raw.githubusercontent.com/MikeGriffinReborn/EntitySpaces/master/docs/Studio.PNG" alt="EntitySpaces Studio" width="632" height="406">

---

# Querying Data

## Example Query

In this example we sum the total number of items for each order. Each order can have many order detail records so we group by OrderID and sum the quantity as `TotalQuantity`. Notice that derived columns are accessible through the dynamic property.

**Use of `out var`**

The `out var` syntax allows you to declare a variable inline — such as the `OrderDetailsQuery` object `od` in the `InnerJoin()` below — and reuse it throughout the query.

```csharp
OrdersCollection coll = new OrdersQuery("o", out var o)
    .InnerJoin<OrderDetailsQuery>("od", out var od).On(o.OrderID == od.OrderID)
    .Select(o.OrderID, od.Quantity.Sum().As("TotalQuantity"))
    .GroupBy(o.OrderID)
    .OrderBy(o.OrderID.Ascending)
    .ToCollection<OrdersCollection>();

foreach (Orders order in coll)
{
    Console.WriteLine(order.OrderID + " : " + order.dynamic.TotalQuantity);
}
```

SQL Generated:

```sql
SELECT o.[OrderID], SUM(od.[Quantity]) AS 'TotalQuantity'
FROM [Orders] o
INNER JOIN [Order Details] od ON o.[OrderID] = od.[OrderID]
GROUP BY o.[OrderID]
ORDER BY o.[OrderID] ASC
```

| OrderID | TotalQuantity |
|---------|---------------|
| 10248 | 27 |
| 10249 | 49 |
| 10250 | 60 |

## InnerJoin, RightJoin, LeftJoin, CrossJoin, and FullJoin

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var e)
    .InnerJoin<EmployeesQuery>("r", out var reportsTo).On(e.ReportsTo == reportsTo.EmployeeID)
    .Select(e.EmployeeID, e.LastName, reportsTo.LastName.As("SupervisorName"))
    .Where(reportsTo.LastName.Like("%a%"))
    .OrderBy(reportsTo.LastName.Descending).Distinct()
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT DISTINCT e.[EmployeeID],e.[LastName],r.[LastName] AS 'SupervisorName'
FROM [Employees] e
INNER JOIN [Employees] r ON e.[ReportsTo] = r.[EmployeeID]
WHERE r.[LastName] LIKE @LastName1
ORDER BY r.[LastName] DESC
```

## Any, All, and Some

```csharp
EmployeesCollection coll = new EmployeesQuery("q", out var q)
    .Where(q.EmployeeID > (() =>
    {
        return new EmployeesQuery("e", out var q1)
            .Select(q1.EmployeeID)
            .Where(q1.EmployeeID.IsNotNull()).Any();
    }))
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT * FROM [Employees] q
WHERE q.[EmployeeID] > ANY
(
    SELECT e.[EmployeeID]
    FROM [Employees] e
    WHERE e.[EmployeeID] IS NOT NULL
)
```

## CrossApply and OuterApply

`OuterApply` and `CrossApply` translate to the correct SQL pattern per provider automatically.

Each provider generates its native equivalent:

| Provider | OuterApply | CrossApply |
|---|---|---|
| SQL Server | `OUTER APPLY` | `CROSS APPLY` |
| PostgreSQL | `LEFT JOIN LATERAL ... ON TRUE` | `JOIN LATERAL ... ON TRUE` |
| MySQL 8.0.14+ | `LEFT JOIN LATERAL ... ON TRUE` | `JOIN LATERAL ... ON TRUE` |
| MariaDB 10.2+ | `LEFT JOIN (ROW_NUMBER() OVER ...)` | `JOIN (ROW_NUMBER() OVER ...)` |

**Single `out` parameter (original syntax — SQL Server / PostgreSQL):**

```csharp
CustomersCollection coll = new CustomersQuery("c", out var c)
    .OuterApply<OrdersQuery>(out var o, () =>
    {
        return new OrdersQuery("o", out var subQuery)
            .Select(subQuery.OrderID, subQuery.OrderDate)
            .Top(2)
            .Where(subQuery.CustomerID == c.CustomerID)
            .OrderBy(subQuery.OrderDate.Descending, subQuery.OrderID.Ascending);
    })
    .Select(c.CustomerID, c.CompanyName, o.OrderID, o.OrderDate)
    .ToCollection<CustomersCollection>();
```

**Dual `out` parameter (recommended — works on all providers including MySQL/MariaDB):**

```csharp
// Use 'out var oCol' to reference lateral subquery columns in outer Select()
CustomersCollection coll = new CustomersQuery("c", out var c)
    .OuterApply<OrdersQuery>(out var o, out var oCol, () =>
    {
        return (OrdersQuery) new OrdersQuery("o", out var subQuery)
            .Select(subQuery.OrderID, subQuery.OrderDate)
            .Top(2)
            .Where(subQuery.CustomerID == c.CustomerID)
            .OrderBy(subQuery.OrderDate.Descending, subQuery.OrderID.Ascending);
    })
    .Select(c.CustomerID, c.CompanyName, oCol.OrderID, oCol.OrderDate)
    .ToCollection<CustomersCollection>();

foreach (Customers cust in coll)
{
    Console.WriteLine(cust.CustomerID);
    Console.WriteLine(cust.CompanyName);
    Console.WriteLine(cust.dynamic.OrderID);
    Console.WriteLine(cust.dynamic.OrderDate);
}
```

SQL Generated (SQL Server):

```sql
SELECT c.[CustomerID],c.[CompanyName],o.[OrderID],o.[OrderDate]
FROM [Customers] c
OUTER APPLY
(
    SELECT TOP 2 o.[OrderID],o.[OrderDate]
    FROM [Orders] o
    WHERE o.[CustomerID] = c.[CustomerID]
    ORDER BY o.[OrderDate] DESC,o.[OrderID] ASC
) AS o
```

| CustomerID | CompanyName | OrderID | OrderDate |
|:-----------|:------------|:--------|:----------|
| ALFKI | Alfreds Futterkiste | 11011 | 04/09/1998 |
| ALFKI | Alfreds Futterkiste | 10952 | 03/16/1998 |
| ANATR | Ana Trujillo Emparedados y helados | 10926 | 03/04/1998 |
| ANATR | Ana Trujillo Emparedados y helados | 10759 | 11/28/1997 |

## Union, Intersect, and Except

```csharp
EmployeesCollection coll = new EmployeesQuery("q1", out var q1)
    .Select(q1.EmployeeID, q1.FirstName, q1.LastName)
    .Where(q1.FirstName.Like("F%"))
    .Union(() =>
    {
        return new EmployeesQuery("q2", out var q2)
            .Select(q2.EmployeeID, q2.FirstName, q2.LastName)
            .Where(q2.FirstName.Like("C%"));
    })
    .Union(() =>
    {
        return new EmployeesQuery("q3", out var q3)
            .Select(q3.EmployeeID, q3.FirstName, q3.LastName)
            .Where(q3.FirstName.Like("M%"));
    })
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT q1.[EmployeeID],q1.[FirstName],q1.[LastName]
FROM [Employees] q1 WHERE q1.[FirstName] LIKE @FirstName1
  UNION SELECT q2.[EmployeeID],q2.[FirstName],q2.[LastName]
  FROM [Employees] q2 WHERE q2.[FirstName] LIKE @FirstName2
  UNION SELECT q3.[EmployeeID],q3.[FirstName],q3.[LastName]
  FROM [Employees] q3 WHERE q3.[FirstName] LIKE @FirstName3
```

## Using In() and NotIn() via Nested Queries

```csharp
OrdersCollection coll = new OrdersQuery("o", out var oQuery)
    .Select(oQuery.OrderID, oQuery.EmployeeID)
    .InnerJoin<OrderDetailsQuery>("od", out var od).On(oQuery.OrderID == od.OrderID)
    .InnerJoin<EmployeesQuery>("e", out var e).On(e.EmployeeID == oQuery.EmployeeID
      && oQuery.EmployeeID.In(() =>
      {
          return new EmployeesQuery("ee", out var ee)
              .InnerJoin<OrdersQuery>("eo", out var eo).On(ee.EmployeeID == eo.EmployeeID)
              .InnerJoin<OrderDetailsQuery>("eod", out var eod).On(eo.OrderID == eod.OrderID)
              .Select(eo.EmployeeID)
              .Distinct();
      }))
    .ToCollection<OrdersCollection>();
```

SQL Generated:

```sql
SELECT o.[OrderID], o.[EmployeeID]
FROM [Orders] o
INNER JOIN [Order Details] od ON o.[OrderID] = od.[OrderID]
INNER JOIN [Employees] e ON (e.[EmployeeID] = o.[EmployeeID] AND o.[EmployeeID] IN
(
    SELECT DISTINCT eo.[EmployeeID]
    FROM [Employees] ee
    INNER JOIN [Orders] eo ON ee.[EmployeeID] = eo.[EmployeeID]
    INNER JOIN [Order Details] eod ON eo.[OrderID] = eod.[OrderID]
)
```

## Exists()

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var eq)
    .Select(eq.EmployeeID, eq.ReportsTo)
    .Where(eq.Exists(() =>
    {
        return new EmployeesQuery("s", out var sq)
            .Select(sq.EmployeeID).Where(sq.ReportsTo.IsNull()).Distinct();
    }))
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT e.[EmployeeID], e.[ReportsTo]
FROM [Employees] e
WHERE EXISTS (
    SELECT DISTINCT s.[EmployeeID]
    FROM [Employees] s
    WHERE s.[ReportsTo] IS NULL
)
```

## Where() with Nested Query

```csharp
TerritoriesCollection coll = new TerritoriesQuery("t", out var tq)
    .Select(tq.TerritoryID, tq.TerritoryDescription)
    .Where(tq.TerritoryID.NotIn(() =>
    {
        return new EmployeeTerritoriesQuery("et", out var etq)
            .Select(etq.TerritoryID)
            .Where(etq.EmployeeID == 1);
    }))
    .ToCollection<TerritoriesCollection>();
```

SQL Generated:

```sql
SELECT t.[TerritoryID], t.[TerritoryDescription]
FROM [dbo].[Territory] t
WHERE t.[TerritoryID] NOT IN
(
    SELECT et.[TerritoryID]
    FROM [dbo].[EmployeeTerritory] et
    WHERE et.[EmployeeID] = @EmployeeID1
)
```

## From() with Nested Query

```csharp
OrdersCollection coll = new OrdersQuery("o", out var o)
    .Select(o.CustomerID, o.OrderDate, "<sub.OrderTotal>")
    .From<OrderDetailsQuery>(out var od, () =>
    {
        return new OrderDetailsQuery("od", out var subQuery)
            .Select(subQuery.OrderID, (subQuery.UnitPrice * subQuery.Quantity).Sum().As("OrderTotal"))
            .GroupBy(subQuery.OrderID);
    }).As("sub")
    .InnerJoin(o).On(o.OrderID == od.OrderID)
    .ToCollection<OrdersCollection>();
```

SQL Generated:

```sql
SELECT o.[CustomerID], o.[OrderDate], sub.OrderTotal
FROM
(
    SELECT od.[OrderID],SUM((od.[UnitPrice] * od.[Quantity])) AS 'OrderTotal'
    FROM [Order Details] od
    GROUP BY od.[OrderID]
) AS sub
INNER JOIN [Orders] o ON o.[OrderID] = sub.[OrderID]
```

## Nested Query within Select Clause

```csharp
OrdersCollection coll = new OrdersQuery("o", out var orders)
    .Select
    (
        orders.OrderID,
        orders.OrderDate,
        new OrderDetailsQuery("oi", out var details).Select(details.UnitPrice.Max())
            .Where(orders.OrderID == details.OrderID).As("MaxUnitPrice")
    )
    .ToCollection<OrdersCollection>();
```

SQL Generated:

```sql
SELECT o.[OrderID],o.[OrderDate],
(
    SELECT MAX(oi.[UnitPrice]) AS 'UnitPrice'
    FROM [Order Details] oi
    WHERE o.[OrderID] = oi.[OrderID]
) AS MaxUnitPrice
FROM [Orders] o
```

## OVER Clause

```csharp
OrdersCollection coll = new OrdersQuery("o", out var o)
    .Select
    (
        o.Over.Sum(o.Freight).PartitionBy(o.EmployeeID).As("FreightByEmployee"),
        o.Over.Sum(o.Freight).PartitionBy(o.EmployeeID, o.ShipCountry).As("FreightByEmployeeAndCountry")
    )
    .OrderBy(o.EmployeeID.Ascending, o.ShipCountry.Ascending)
    .ToCollection<OrdersCollection>();
```

SQL Generated:

```sql
SELECT
    SUM(o.[Freight]) OVER( PARTITION BY o.[EmployeeID] ) AS 'FreightByEmployee',
    SUM(o.[Freight]) OVER( PARTITION BY o.[EmployeeID], o.[ShipCountry] ) AS 'FreightByEmployeeAndCountry'
FROM [Orders] o
ORDER BY o.[EmployeeID] ASC,o.[ShipCountry] ASC
```

## OVER Clauses with esAlias and ROWS Syntax

```csharp
esAlias aliasCompany = null, aliasPeriod = null, aliasAmount = null, aliasItemCount = null;

OrdersCollection coll = new OrdersQuery("q", out var q)
    .From<OrdersQuery>(out var sub, () =>
    {
        return new OrdersQuery("o", out var o)
            .InnerJoin<CustomersQuery>("c", out var c).On(c.CustomerID == o.CustomerID)
            .InnerJoin<OrderDetailsQuery>("od", out var od).On(od.OrderID == o.OrderID)
            .Select
            (
                o.Count().As("TotalItems", out aliasItemCount),
                c.CompanyName.As("CompanyName", out aliasCompany),
                o.OrderDate.DatePart("year").As("Period", out aliasPeriod),
                ((1.00M - od.Discount) * od.UnitPrice * od.Quantity)
                    .Cast(esCastType.Decimal, 19, 2).Sum().Round(2).As("Amount", out aliasAmount)
            )
            .GroupBy(c.CompanyName, o.OrderDate.DatePart("year"));
    }).As("sub")
    .Select(
        aliasCompany(), aliasPeriod(), aliasAmount(), aliasItemCount(),
        q.Over.Sum(aliasAmount()).PartitionBy(aliasCompany()).OrderBy(aliasPeriod().Ascending)
            .Rows.UnBoundedPreceding.As("CumulativeAmount"),
        q.Over.Sum(aliasAmount()).PartitionBy(aliasCompany()).As("TotalAmount")
    )
    .OrderBy(aliasCompany().Ascending, aliasPeriod().Ascending)
    .ToCollection<OrdersCollection>();
```

SQL Generated:

```sql
SELECT
   sub.[CompanyName], sub.[Period], sub.[Amount], sub.[TotalItems],
   SUM([Amount]) OVER( PARTITION BY [CompanyName] ORDER BY sub.[Period] ASC
      ROWS UNBOUNDED PRECEDING ) AS 'CumulativeAmount',
   SUM([Amount]) OVER( PARTITION BY [CompanyName] ) AS 'TotalAmount'
FROM
   (
      SELECT
         COUNT(*) AS 'TotalItems',
         c.[CompanyName] AS 'CompanyName',
         DATEPART(year, o.[OrderDate]) AS 'Period',
         CAST(SUM(ROUND((((1.00 - od.[Discount]) * od.[UnitPrice]) * od.[Quantity]), 2))
            AS decimal(19, 2)) AS 'Amount'
      FROM [Orders] o
         INNER JOIN [Customers] c ON c.[CustomerID] = o.[CustomerID]
         INNER JOIN [Order Details] od ON od.[OrderID] = o.[OrderID]
      GROUP BY c.[CompanyName], DATEPART(year, o.[OrderDate])
   ) AS sub
ORDER BY sub.[CompanyName] ASC, sub.[Period] ASC
```

## AND and OR and Concatenation

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var q)
    .Select(q.EmployeeID, (q.LastName + ", " + q.FirstName).As("FullName"))
    .Where(q.EmployeeID > 4 && (q.EmployeeID < 10 || q.EmployeeID == 100))
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT
   e.[EmployeeID],
   (e.[LastName] + ', ' + e.[FirstName]) AS 'FullName'
FROM [Employees] e
WHERE e.[EmployeeID] > @EmployeeID1
  AND
  (
      e.[EmployeeID] < @EmployeeID2 OR e.[EmployeeID] = @EmployeeID3
  )
```

## Select Top

```csharp
Employees emp = new EmployeesQuery("q", out var q)
    .Where(q.ReportsTo.IsNotNull())
    .OrderBy(q.LastName.Descending).Top(1)
    .ToEntity<Employees>();
```

SQL Generated:

```sql
SELECT TOP 1 *
FROM [Employees]
WHERE [ReportsTo] IS NOT NULL
ORDER BY [LastName] DESC
```

## SelectAllExcept

```csharp
EmployeesCollection coll = new EmployeesQuery("q", out var q)
    .SelectAllExcept(q.Photo)
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT q.[EmployeeID],q.[LastName],q.[FirstName],q.[Title] -- all except q.Photo
FROM [Employees] q
```

## Paging

**PageSize / PageNumber** — works on all SQL Server versions:

```csharp
EmployeesCollection coll = new EmployeesQuery("q", out var q)
    .Select(q.EmployeeID, q.LastName)
    .OrderBy(q.LastName.Ascending)
    .PageNumber(2).PageSize(20)
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
WITH [withStatement] AS
(
    SELECT [EmployeeID],[LastName],
        ROW_NUMBER() OVER( ORDER BY [LastName] ASC) AS ESRN
    FROM [Employees]
)
SELECT *
FROM [withStatement]
WHERE ESRN BETWEEN 21 AND 40
ORDER BY ESRN ASC
```

**Skip / Take** — requires SQL Server 2012 or later:

```csharp
EmployeesCollection coll = new EmployeesQuery("q", out var q)
    .Select(q.EmployeeID, q.LastName)
    .OrderBy(q.LastName.Ascending)
    .Skip(40).Take(20)
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT [EmployeeID],[LastName]
FROM [Employees]
ORDER BY [LastName] ASC
OFFSET 40 ROWS
FETCH NEXT 20 ROWS ONLY
```

## Distinct

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var e)
    .Select(e.EmployeeID)
    .InnerJoin<OrdersQuery>("o", out var o).On(e.EmployeeID == o.EmployeeID)
    .Distinct()
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT DISTINCT e.[EmployeeID]
FROM [Employees] e
INNER JOIN [Orders] o ON e.[EmployeeID] = o.[EmployeeID]
```

## With NoLock

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var e)
    .Select(e.EmployeeID)
    .InnerJoin<OrdersQuery>("o", out var o).On(e.EmployeeID == o.EmployeeID)
    .Where(o.Freight > 20)
    .es.WithNoLock()
    .ToCollection<EmployeesCollection>();
```

SQL Generated — `WITH (NOLOCK)` is applied to all tables in the query:

```sql
SELECT e.[EmployeeID]
FROM [Employees] e WITH (NOLOCK)
INNER JOIN [Orders] o WITH (NOLOCK) ON e.[EmployeeID] = o.[EmployeeID]
WHERE o.[Freight] > @Freight1
```

## Casting

```csharp
OrderDetailsCollection coll = new OrderDetailsQuery("o", out var o)
    .Select
    (
        (o.Quantity * o.UnitPrice).Cast(esCastType.Decimal, 34, 4).As("Cost")
    )
    .ToCollection<OrderDetailsCollection>();
```

SQL Generated:

```sql
SELECT CAST((o.[Quantity] * o.[UnitPrice]) AS decimal(34, 4)) AS 'Cost'
FROM [Order Details] o
```

## Having Clause

```csharp
EmployeeCollection coll = new EmployeeQuery("e", out var q)
    .Select(q.EmployeeID, q.Age.Sum().As("TotalAge"))
    .Where(q.EmployeeID.IsNotNull())
    .GroupBy(q.EmployeeID)
    .Having(q.Age.Sum() > 5)
    .OrderBy(q.EmployeeID.Descending)
    .ToCollection<EmployeeCollection>();
```

SQL Generated:

```sql
SELECT e.[EmployeeID] AS 'EmployeeID', SUM([Age]) AS 'TotalAge'
FROM [dbo].[Employee] e
WHERE e.[EmployeeID] IS NOT NULL
GROUP BY e.[EmployeeID]
HAVING SUM([Age]) > @Age2
ORDER BY e.[EmployeeID] DESC
```

## Getting the Count

```csharp
int count = new EmployeesQuery("e", out var q)
    .Select(q.Count())
    .Where(q.ReportsTo.IsNull())
    .ExecuteScalar<int>();
```

SQL Generated:

```sql
SELECT COUNT(*)
FROM [Employees] e
WHERE e.[ReportsTo] IS NULL
```

## Raw SQL Injection

For SQL features not covered by the API, pass raw SQL within `<>` angle brackets:

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var q)
    .Select("<FirstName>", q.HireDate)
    .Where("<EmployeeID = 1>")
    .GroupBy("<FirstName>", q.HireDate)
    .OrderBy("<FirstName ASC>")
    .ToCollection<EmployeesCollection>();
```

SQL Generated:

```sql
SELECT FirstName, e.[HireDate]
FROM [Employees] e
WHERE (EmployeeID = 1)
GROUP BY FirstName,[HireDate]
ORDER BY FirstName ASC
```

## The Filter Property

`Filter` allows LINQ-based in-memory filtering of a loaded collection without removing records. Setting it to `null` restores all records.

```csharp
using System.Linq;

EmployeesCollection coll = new EmployeesCollection();
if (coll.LoadAll())
{
    coll.Filter = coll.AsQueryable().Where(d => d.FirstName.Contains("a"));

    foreach (Employees employee in coll)
    {
        // Only employees whose FirstName contains 'a'
    }

    coll.Filter = null; // Restore all records
}
```

## Old School Syntax

```csharp
EmployeesQuery eQuery = new EmployeesQuery("e");
OrdersQuery o = new OrdersQuery("o");
OrderDetailsQuery od = new OrderDetailsQuery("od");

eQuery.Select(eQuery.EmployeeID)
    .InnerJoin(o).On(eQuery.EmployeeID == o.EmployeeID)
    .InnerJoin(od).On(o.OrderID == od.OrderID)
    .Where(o.Freight > 20);

EmployeesCollection coll = new EmployeesCollection();
if (coll.Load(eQuery))
{
    // Data was loaded
}
```

## JSON Serialization of Derived Columns

Derived columns from JOINs, aggregates, or concatenation serialize correctly even without a matching property on the entity:

```csharp
EmployeesCollection coll = new EmployeesQuery("e", out var e)
    .Select
    (
        e.EmployeeID, e.LastName, e.FirstName,
        (e.LastName + ", " + e.FirstName).As("fullName")
    )
    .OrderBy(e.LastName.Descending)
    .ToCollection<EmployeesCollection>();

if (coll.Count > 0)
{
    string json = JsonConvert.SerializeObject(coll);
}
```

```json
[
  {
    "EmployeeID": 6,
    "LastName": "Suyama",
    "FirstName": "Michael",
    "fullName": "Suyama, Michael"
  },
  {
    "EmployeeID": 193,
    "LastName": "Smith",
    "FirstName": "Frank",
    "fullName": "Smith, Frank"
  }
]
```

---

# Modifying Data

## CRUD Example

```csharp
// Create
Employees newEmp = new Employees();
newEmp.FirstName = "Joe";
newEmp.LastName = "Smith";
newEmp.Save();

// Load
Employees employee = new Employees();
if (employee.LoadByPrimaryKey(newEmp.EmployeeID.Value))
{
    // Update
    employee.FirstName = "Bob";
    employee.Save();

    // Delete
    employee.MarkAsDeleted();
    employee.Save();
}
```

## Transaction Support

For two unrelated objects that must succeed or fail together:

```csharp
using (esTransactionScope scope = new esTransactionScope())
{
    Employees employee = new Employees();
    employee.FirstName = "Mike";
    employee.LastName = "Griffin";
    employee.Save();

    Products product = new Products();
    product.ProductName = "Some Gadget";
    product.Save();

    scope.Complete();
}
```

For hierarchical objects, a transaction is created implicitly — no `esTransactionScope` needed:

```csharp
Orders order = new Orders
{
    OrderDate = DateTime.Now
};

order.OrderDetailsCollection.Add(new OrderDetails
{
    UnitPrice = 55.00M,
    Quantity = 4,
    ProductID = 8
});

order.Save(); // Saves hierarchically
```

## Collections

```csharp
EmployeesCollection coll = new EmployeesCollection();
if (coll.LoadAll())
{
    foreach (Employees emp in coll)
    {
        // process each employee
    }
}
```

---

# Supported Operators

## Comparison and Logical Operators

| Operator | Description |
|:---------|:------------|
| `+` | Addition |
| `-` | Subtraction |
| `*` | Multiplication |
| `/` | Division |
| `%` | Modulo |
| `>` | Greater than |
| `<` | Less than |
| `<=` | Less than or equal to |
| `>=` | Greater than or equal to |
| `==` | Equal to |
| `!=` | Not equal to |
| `&&` | AND |
| `\|\|` | OR |

## Sub Operators

| Sub Operator | Description |
|:-------------|:------------|
| `ToUpper()` | Convert to upper case |
| `ToLower()` | Convert to lower case |
| `LTrim()` | Left trim leading spaces |
| `RTrim()` | Right trim trailing spaces |
| `Trim()` | Trim both leading and trailing spaces |
| `SubString()` | Return a sub-string |
| `Coalesce()` | Return the first non-null expression |
| `Date()` | Returns only the date portion of a datetime |
| `DatePart()` | Returns a part of a datetime value |
| `Length()` | Return the length |
| `Round()` | Round to specified decimal places |
| `Avg()` | Average |
| `Count()` | Count |
| `Max()` | Maximum value |
| `Min()` | Minimum value |
| `StdDev()` | Standard deviation |
| `Var()` | Variance |
| `Sum()` | Summation |
| `Cast()` | SQL CAST |

## OVER Clause Operators

See [SELECT - OVER Clause (Transact-SQL)](https://docs.microsoft.com/en-us/sql/t-sql/queries/select-over-clause-transact-sql?view=sql-server-ver15).

**Ranking Functions**

| Operator | SQL Function |
|:---------|:-------------|
| `Over.RowNumber()` | `ROW_NUMBER()` |
| `Over.Rank()` | `RANK()` |
| `Over.DenseRank()` | `DENSE_RANK()` |
| `Over.PercentRank()` | `PERCENT_RANK()` |
| `Over.Ntile()` | `NTILE()` |

**Aggregate Functions**

| Operator | SQL Function |
|:---------|:-------------|
| `Over.Avg()` | `AVG() OVER()` |
| `Over.Count()` | `COUNT() OVER()` |
| `Over.CountBig()` | `COUNT_BIG() OVER()` |
| `Over.Max()` | `MAX() OVER()` |
| `Over.Min()` | `MIN() OVER()` |
| `Over.StdDev()` | `STDDEV() OVER()` |
| `Over.StdDevP()` | `STDDEVP() OVER()` |
| `Over.Var()` | `VAR() OVER()` |
| `Over.VarP()` | `VARP() OVER()` |

**Analytical Functions**

| Operator | SQL Function |
|:---------|:-------------|
| `Over.CumeDist()` | `CUME_DIST()` |
| `Over.FirstValue()` | `FIRST_VALUE()` |
| `Over.LastValue()` | `LAST_VALUE()` |
| `Over.Lag()` | `LAG()` |
| `Over.Lead()` | `LEAD()` |
| `Over.PercentileCont()` | `PERCENTILE_CONT()` |
| `Over.PercentileDisc()` | `PERCENTILE_DISC()` |

---

# NuGet Packages

| Provider | Package |
|----------|---------|
| SQL Server | [EntitySpaces.ORM.SqlServer.NET](https://www.nuget.org/packages/EntitySpaces.ORM.SqlServer.NET/) |
| PostgreSQL | [EntitySpaces.ORM.PostgreSQL.NET](https://www.nuget.org/packages/EntitySpaces.ORM.PostgreSQL.NET/) |
| MySQL | [EntitySpaces.ORM.MySQL.NET](https://www.nuget.org/packages/EntitySpaces.ORM.MySQL.NET/) |
| SQLite | [EntitySpaces.ORM.SQLite.NET](https://www.nuget.org/packages/EntitySpaces.ORM.SQLite.NET/) |
| Oracle Managed Client | [EntitySpaces.ORM.OracleManagedClient.NET](https://www.nuget.org/packages/EntitySpaces.ORM.OracleManagedClient.NET/) |
| Firebird | [EntitySpaces.ORM.Firebird.NET](https://www.nuget.org/packages/EntitySpaces.ORM.Firebird.NET/) |

**Install via Package Manager Console:**

```powershell
# SQL Server
Install-Package EntitySpaces.ORM.SqlServer.NET

# PostgreSQL
Install-Package EntitySpaces.ORM.PostgreSQL.NET

# MySQL
Install-Package EntitySpaces.ORM.MySQL.NET

# SQLite
Install-Package EntitySpaces.ORM.SQLite.NET

# Oracle
Install-Package EntitySpaces.ORM.OracleManagedClient.NET

# Firebird
Install-Package EntitySpaces.ORM.Firebird.NET
```

## Generating Your Classes

1. Install [EntitySpaces Studio 2024](https://github.com/paulcordova/EntitySpaces/blob/master/EntitySpaces.Studio/EntitySpacesStudio_2024.8.30.0.zip/)
2. Connect to your database under **Settings → Connection**
3. Execute two templates:
   - **Generated classes** — regenerate whenever your schema changes, never edit manually
   - **Custom classes** — generated once, add your business logic here

<img src="https://raw.githubusercontent.com/MikeGriffinReborn/EntitySpaces/master/docs/Studio.PNG" alt="EntitySpaces Studio" width="632" height="406">
