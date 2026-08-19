<img src="https://es-banner.paul-netstep.workers.dev?src=nuget_readme" alt="EntitySpaces" width="531" height="268">

# EntitySpaces.ORM.SQLite.NET

Part of the modernized EntitySpaces ORM — SQLite data provider. Actively maintained fork with full .NET Framework 4.8 to .NET 10 support.

## Supportability

| | |
|---|---|
| **.NET targets** | .NET Framework 4.8 · .NET 8 · .NET 9 · .NET 10 |
| **SQLite** | SQLite 3.x (validated with 3.46.1) |
| **Driver** | System.Data.SQLite.Core 1.0.119 |

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

- Auto-increment detection for both SQLite forms — the rowid alias (`INTEGER PRIMARY KEY`) and the explicit `AUTOINCREMENT` keyword — both now correctly generate `IsAutoIncrement = true` metadata
- Foreign key enforcement is now automatic — the provider appends `Foreign Keys=True` to the connection string on every connection, no configuration change needed
- Concurrency exception detection — `SQLITE_CONSTRAINT` (code 19) violations (PK/UNIQUE/FK/CHECK) are translated to `esConcurrencyException`, consistent with the other providers
- Navigation properties generated in Studio from FK relationships defined in the DDL
- Studio metadata engine rewritten to use native SQLite pragmas (`PRAGMA table_info`, `PRAGMA foreign_key_list`) instead of `information_schema`, which SQLite does not support

## Fixes

- FK violations now correctly raise `esConcurrencyException` instead of failing silently — a direct result of foreign key enforcement now being applied by default
- Explicit PK inserts on auto-increment tables are now detected correctly, so an explicitly assigned `Id` is inserted directly instead of being overwritten by a generated rowid

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

**SQLite connection string:**

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SQLiteProvider";
conn.ConnectionString = "Data Source=C:\\path\\to\\database.db;Version=3;Foreign Keys=True;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

More usage examples (joins, paging, transactions, and the full Fluent SQL API): see the [main EntitySpaces README](https://github.com/paulcordova/EntitySpaces).

### Generating your entity classes

See **Requires EntitySpaces Studio** above.
