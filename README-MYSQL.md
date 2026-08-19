<img src="https://es-banner.paul-netstep.workers.dev?src=nuget_readme" alt="EntitySpaces" width="531" height="268">

# EntitySpaces.ORM.MySQL.NET

Part of the modernized EntitySpaces ORM — MySQL / MariaDB data provider. Actively maintained fork with full .NET Framework 4.8 to .NET 10 support.

## Supportability

| | |
|---|---|
| **.NET targets** | .NET Framework 4.8 · .NET 8 · .NET 9 · .NET 10 |
| **MySQL / MariaDB** | MySQL 8.0.14+ (validated with 8.0.28) · MariaDB 10.2+ (validated with 10.2 / 10.6 / 10.11) |
| **Driver** | MySql.Data 9.x |

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

- Automatic engine detection — the provider queries `SELECT VERSION()` on the first call per connection string and caches the result, no configuration required to distinguish MySQL from MariaDB
- Automatic `APPLY` → correct SQL strategy per engine: MySQL 8.0.14+ generates native `LEFT JOIN LATERAL` / `JOIN LATERAL`, MariaDB 10.2+ generates an equivalent `ROW_NUMBER() OVER (PARTITION BY)` pattern — same C# query, correct SQL either way
- Concurrency exception detection — duplicate entry, deadlock, and lock wait timeout all translate to `esConcurrencyException`
- Connection pool safety — `hasError` flag + `Transaction.Rollback()` in `finally`, preventing dirty connections from being reused
- Studio metadata engine now extracts the `EXTRA` column (`VIRTUAL GENERATED` / `STORED GENERATED`) for computed-column detection and `TIMESTAMP` concurrency detection

## Fixes

- Documented and handled MySQL 8.0's `caching_sha2_password` default, which requires `SslMode=Required` (or `SslMode=None` with `AllowPublicKeyRetrieval=True`) — connections without this previously failed against default MySQL 8.0 configurations
- Clarified case-sensitivity handling across platforms — MySQL on Linux is case-sensitive for table/schema names while Windows and MariaDB are not; class generation now documented to always run against the target server to guarantee consistent metadata

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

**MySQL connection string:**

```csharp
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.MySqlProvider";
conn.ConnectionString = "Server=myserver;Port=3306;Database=mydb;Uid=myuser;" +
                        "Pwd=mypassword;SslMode=Required;AllowPublicKeyRetrieval=True;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);
```

More usage examples (joins, paging, transactions, and the full Fluent SQL API): see the [main EntitySpaces README](https://github.com/paulcordova/EntitySpaces).

### Generating your entity classes

See **Requires EntitySpaces Studio** above.
