<img src="https://es-banner.paul-netstep.workers.dev?src=nuget_readme" alt="EntitySpaces" width="531" height="268">

# EntitySpaces.ORM.SqlServer.NET

Part of the modernized EntitySpaces ORM — SQL Server data provider. Actively maintained fork with full .NET Framework 4.8 to .NET 10 support.

## Supportability

| | |
|---|---|
| **.NET targets** | .NET Framework 4.8 · .NET 8 · .NET 9 · .NET 10 |
| **SQL Server** | SQL Server 2016 – 2025 (17.x) |
| **Driver** | Microsoft.Data.SqlClient 6.x / 7.x |

## ❤️ Support this project

If this provider has saved you time on a project, [support its development](https://netstep.cl/entityspaces/support/) — every contribution helps keep it free and maintained.

You can also sponsor via the ❤️ Sponsor link on this NuGet package (powered by Ko-fi).

## 📋 Help Shape the Roadmap

> [Take the 3‑minute survey](https://docs.google.com/forms/d/e/1FAIpQLSd-FVQiC3deoaIarYnsOCH4pdj-4zjGKznN68uUtyx9CpuKgA/viewform) – your feedback keeps EntitySpaces alive and evolving.

## 🛠️ Requires EntitySpaces Studio

This package provides the SQL Server runtime only — it does not generate code. Entity and Collection classes are generated from your database schema using **EntitySpaces Studio**, a separate WinForms tool.

- Get it from the repo: [EntitySpaces.Studio](https://github.com/paulcordova/EntitySpaces/tree/master/EntitySpaces.Studio)
- Download the **most recent** `.zip` — avoid any file tagged `-deprecated`, that build is kept for reference only.
- Connect to your database under Settings → Connection, then run the Generated and Custom class templates. 

## New Features

- Concurrency exception detection covering the full set of SQL Server runtime errors (timestamp/rowversion mismatch, CHECK/FK violation, deadlock, lock timeout, unique/PK violation), all translated to `esConcurrencyException`
- Connection pool safety — `hasError` flag + conditional rollback in `finally`, preventing broken-transaction connections from being reused
- Unified `OUTPUT INSERTED...INTO @output_vals` pattern — retrieves Identity, Computed, Concurrency (`rowversion`/`timestamp`), Defaults (`GETDATE()`, etc.), `newsequentialid()`, and special columns in a single round-trip
- `SET XACT_ABORT ON` on all generated INSERT/UPDATE/DELETE statements, ensuring consistent transaction abort behavior on any runtime error
- Server version auto-detection, cached per connection string — `conn.DatabaseVersion` is now optional
- Studio metadata engine now extracts `sys.columns.is_computed`, precision/scale/max_length, and `rowversion`/`timestamp` concurrency detection

## Fixes

- Corrected `IsComputed` metadata — no longer mis-identifies `timestamp` columns as computed
- Error 547 (CHECK/FK violation) now reads the constraint name from `SqlError.Message` instead of `SqlException.Message`, ensuring it's available on every SQL Server version including 2025
- Worked around a SQL Server 2025 regression where `OUTPUT...INTO @table_variable` inside a parameterized batch could silently suppress CHECK constraint exceptions — columns with `HasDefault` (non-GUID) now use a post-insert `SELECT ... WHERE pk = SCOPE_IDENTITY()` instead

> **Known limitation — SQL Server 2025 Express:** CHECK/FK constraint violations inside parameterized batches with `Output` parameters may not propagate as `SqlException` (confirmed on Express 17.0.4040). Does not affect SQL Server 2016 Express or any non-Express edition.

## Dependency Updates

- Updated `System.Configuration.ConfigurationManager` from `10.0.10` to `10.0.11`
  - Maintenance release. No API changes affecting EntitySpaces.

## Quick Samples

**Load a collection:**

```csharp
var customers = new CustomersCollection();
customers.LoadAll();

foreach (var customer in customers)
{
    Console.WriteLine(customer.CompanyName);
}
```

**Create, update, delete an entity:**

```csharp
var employee = new Employees();
employee.FirstName = "Joe";
employee.LastName = "Smith";
employee.Save();               // Create

employee.LastName = "Doe";
employee.Save();               // Update

employee.MarkAsDeleted();
employee.Save();               // Delete
```

**SQL Server connection string:**

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SqlClientProvider";
conn.ConnectionString = "User ID=sa;Password=mypassword;Initial Catalog=Northwind;" +
                        "Data Source=localhost;TrustServerCertificate=True;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

More usage examples (joins, paging, transactions, and the full Fluent SQL API): see the [main EntitySpaces README](https://github.com/paulcordova/EntitySpaces).

### Generating your entity classes

See **Requires EntitySpaces Studio** above.
