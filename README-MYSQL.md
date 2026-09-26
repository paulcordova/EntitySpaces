<img src="https://es-banner.paul-netstep.workers.dev?src=nuget_readme" alt="EntitySpaces" width="531" height="268">

# EntitySpaces.ORM.MySQL.NET

Part of the modernized EntitySpaces ORM — MySQL / MariaDB data provider. Actively maintained fork with full .NET Framework 4.8 to .NET 10 support.

## Supportability

| | |
|---|---|
| **.NET targets** | .NET Framework 4.8 - .NET 8 - .NET 9 - .NET 10 |
| **MySQL / MariaDB** | MySQL 8.0.14+ (validated with 8.0.28) - MariaDB 10.2+ (validated with 10.2 / 10.6 / 10.11) |
| **Driver** | MySqlConnector 2.6.2 |

## ❤️ Support this project

If this provider has saved you time on a project, [support its development](https://netstep.cl/entityspaces/support/) — every contribution helps keep it free and maintained.

You can also sponsor via the ❤️ Sponsor link on this NuGet package (powered by Ko-fi).

## 📋 Help Shape the Roadmap

> [Take the 3-minute survey](https://docs.google.com/forms/d/e/1FAIpQLSd-FVQiC3deoaIarYnsOCH4pdj-4zjGKznN68uUtyx9CpuKgA/viewform) — your feedback keeps EntitySpaces alive and evolving.

## 🛠️ Requires EntitySpaces Studio

This package provides the MySQL runtime only — it does not generate code. Entity and Collection classes are generated from your database schema using **EntitySpaces Studio**, a separate WinForms tool.

- Get it from the repo: [EntitySpaces.Studio](https://github.com/paulcordova/EntitySpaces/tree/master/EntitySpaces.Studio)
- Download the **most recent** `.zip` — avoid any file tagged `-deprecated`, that build is kept for reference only.
- Connect to your database under Settings → Connection, then run the Generated and Custom class templates. 

---

## What's New

### Hierarchical parent-child save

Fixed a silent failure mode where children were inserted with a NULL foreign key on schemas with lowercase column names (a common MySQL/MariaDB convention):

```
Parent.Id        = 22       (parent inserted)
Child.Id         = 9        (child inserted)
Child.Invoiceid  = 22       (FK correctly propagated)
Child.IsDirty    = False    (framework cleared the entity)
```

The fix has two parts:

1. **Tolerant column/property name resolution** — the INSERT/UPDATE/DELETE builders now accept both the DB column name (`invoiceid`) and the property name (`Invoiceid`) when deciding whether a column participates in the statement.
2. **Server-side key sync** — after a successful INSERT/UPDATE, the provider synchronizes both keys (`invoiceid` and `Invoiceid`) in the entity's `CurrentValues`, so the in-memory getter sees the value that was persisted.

Both mechanisms are provider-agnostic and match the behavior of the PostgreSQL provider on snake_case schemas.

### Explicit `AUTO_INCREMENT` PK insert

Inserting an entity with an explicit PK value now correctly uses that value instead of letting MySQL generate one:

```csharp
var invoice = new Invoice { Id = 999990, Blablabla = "Explicit" };
invoice.Save();      // INSERTs with id=999990
```

MySQL accepts explicit values in `AUTO_INCREMENT` columns by default — the fix detects this case and adjusts the INSERT accordingly.

### Server-side column sync — AutoInc, Timestamp, Defaults

Previously, `OnRowUpdated` silently swallowed exceptions during server-side column retrieval (auto-increment, `LastTimestamp()`, defaults), and looked up parameters by `"?" + columnName` when the actual parameter name was `"?" + propertyName`. Both issues are fixed:

- Exceptions in `OnRowUpdated` are now surfaced via `Debug.WriteLine` instead of `catch { }`
- Parameter lookup iterates by `SourceColumn` (which is the DB column name) instead of constructing the name
- AutoInc and Defaults values are written to both key names (`id` and `Id`) so entity getters see the value immediately

### Transaction handling — no explicit `ROLLBACK` on enlisted connections

The provider no longer issues `ROLLBACK` inside its `Save()` and `Load()` methods. On an enlisted connection, `ROLLBACK` aborts the ambient `esTransactionScope` — not just the current command. Rollback is now delegated to the scope owner (`esTransactionScope.Dispose()`), which is the correct design and matches the pattern already applied to the PostgreSQL provider.

This eliminates intermittent failures during hierarchical saves where an error in a child insert would otherwise abort the entire transaction before the framework could surface the actual error.

### Implicit vs Explicit Transactions — clarified and documented

See the "Implicit vs Explicit Transactions" section below for the full behavior matrix. Summary:

- **Single entity `Save()` / `Delete()`** — implicit atomicity (single SQL statement).
- **Hierarchical parent-child save** — requires an explicit `esTransactionScope` for full parent+children atomicity. This is a framework-level constraint, not a provider constraint.
- **Multi-entity workflows** — always use `esTransactionScope`.

### Driver migration

Migrated from the legacy `MySql.Data` driver to **MySqlConnector 2.6.2** — the actively maintained, .NET-8+-first ADO.NET provider for MySQL/MariaDB. Both drivers are wire-compatible, but `MySqlConnector` has:

- First-class support for `System.Transactions` with explicit opt-in (previously automatic)
- Consistent behavior across .NET Framework 4.8 and .NET 8–10
- Faster, allocation-light parameter handling

---

## Features

### Automatic engine detection

The provider queries `SELECT VERSION()` on the first call per connection string and caches the result. No configuration required to distinguish MySQL from MariaDB.

### Automatic `APPLY` → correct SQL strategy per engine

MySQL 8.0.14+ generates native `LEFT JOIN LATERAL` / `JOIN LATERAL`; MariaDB 10.2+ generates an equivalent `ROW_NUMBER() OVER (PARTITION BY)` pattern. Same C# query, correct SQL either way.

| EntitySpaces | MySQL 8.0.14+ | MariaDB 10.2+ |
|---|---|---|
| `OuterApply` | `LEFT JOIN LATERAL` | `LEFT JOIN` + `ROW_NUMBER() OVER (PARTITION BY)` |
| `CrossApply` | `JOIN LATERAL` | `JOIN` + `ROW_NUMBER() OVER (PARTITION BY)` |
| `Top(n)` | `LIMIT n` inside LATERAL | `es_rn <= n` in outer `ON` clause |
| Without `Top()` | No LIMIT | All rows per partition returned |

### Concurrency exception detection

Duplicate entry, deadlock, and lock wait timeout all translate to `esConcurrencyException`:

| Code | Condition | Translated To |
|---|---|---|
| `1062` | Duplicate entry — PK or unique key violation | `esConcurrencyException` |
| `1205` | Lock wait timeout exceeded | `esConcurrencyException` |
| `1213` | Deadlock detected | `esConcurrencyException` |

```csharp
try
{
    category.Save();
}
catch (esConcurrencyException ex)
{
    // Duplicate key, deadlock, or lock wait timeout
    Console.WriteLine(ex.Message);
}
```

### Connection pool safety

Save and load operations release connections safely on error:

- `ExecuteReader` opens its own raw connection and closes it via `CleanupCommand` when `ExecuteReader()` itself fails
- Other operations use `esTransactionScope.DeEnlist()` in `finally` blocks
- **No explicit `ROLLBACK` is issued** — rollback is delegated to the transaction scope owner (`esTransactionScope.Dispose()`), which is the correct design and matches the pattern already applied to the PostgreSQL provider

This eliminates the class of bugs where an error in a child insert would abort an ambient transaction, not just the current command.

### Tolerant column/property name resolution

The INSERT/UPDATE/DELETE builders accept both the database column name (`invoiceid`) and the property name (`Invoiceid`) when deciding whether a column participates in a statement, and when mapping server-returned values back to the entity. This makes hierarchical saves and explicit-PK inserts work on schemas with lowercase column names without any code changes.

### Thread-safe parameter cache

The parameter cache is a `ConcurrentDictionary` keyed by `DataID`. No lock contention in multi-threaded environments.

### Studio metadata engine

The Studio metadata engine extracts the `EXTRA` column (`VIRTUAL GENERATED` / `STORED GENERATED`) for computed-column detection and `TIMESTAMP` concurrency detection. See the main README's Studio Modernization section for the full metadata matrix (character max length, numeric precision/scale, computed columns, auto-increment, defaults).

### Documented and handled MySQL 8.0's `caching_sha2_password` default

MySQL 8.0 with `caching_sha2_password` requires either `SslMode=Required` (or `SslMode=None` combined with `AllowPublicKeyRetrieval=True`). Connections without this previously failed against default MySQL 8.0 configurations with an authentication error. See the connection string section below.

### Clarified case-sensitivity handling across platforms

MySQL on Linux is case-sensitive for table/schema names (`lower_case_table_names=0`); MySQL on Windows and all MariaDB platforms are case-insensitive. Class generation is now documented to always run against the target server to guarantee consistent metadata. See the case-sensitivity section below.

---

## Hierarchical Save — Usage

EntitySpaces supports two patterns for hierarchical saves. **Choose one, never both.**

**Parent-initiated:**

```csharp
var order = new Salesorder { CustId = 1, ShipperId = 1, OrderDate = DateTime.Now };

order.OrderdetailCollectionByOrderId.Add(new Orderdetail
{
    ProductId = 1, UnitPrice = 15.50m, Quantity = 5, Discount = 0m
});

order.Save();
```

**Child-initiated:**

```csharp
var detail = new Orderdetail { ProductId = 1, UnitPrice = 25m, Quantity = 3, Discount = 0m };
detail.UpToSalesorderByOrderId = order;
detail.Save();
```

> **⚠️ Do not use bidirectional linking** — setting both `parent.Children.Add(child)` and `child.UpToParent = parent` triggers a `StackOverflowException` in the framework's navigation property recursion. This is a limitation of the current EntitySpaces Core, not of the provider. Choose one direction.

---

## Implicit vs Explicit Transactions

The provider supports two mechanisms.

### Single entity — implicit atomicity

A single-entity `Save()` or `Delete()` is executed as one `INSERT`, `UPDATE`, or `DELETE` statement. Within an ambient `esTransactionScope`, it participates in the ambient transaction. Without one, it runs in autocommit mode — but since it is a single statement, the operation is inherently atomic.

```csharp
// Atomic — single statement
var customer = new Customer { CompanyName = "Acme" };
customer.Save();
```

### Hierarchical save — atomicity requires an explicit scope

When you call `parent.Save()` on an entity with children in its collection, the framework processes the operation in three phases:

1. **Pre-saves** (for `UpTo` parent references)
2. **Parent insert/update**
3. **Post-saves** (children collection)

The **children collection** is saved by the provider inside an internally created `esTransactionScope`. The **parent** is saved by the provider outside any implicit scope.

**Consequence**: without an ambient `esTransactionScope`, a failure in a child insert will not revert the parent — the parent stays persisted.

```csharp
// NO implicit atomicity for parent+children without a scope
var order = new Salesorder { CustId = 1, ShipperId = 1, OrderDate = DateTime.Now };
order.OrderdetailCollectionByOrderId.Add(new Orderdetail { ProductId = -999, ... });
order.Save();   // ← parent persisted, child fails, parent stays
```

**Recommended pattern** for hierarchical saves — wrap in `esTransactionScope` for full atomicity:

```csharp
using (var scope = new esTransactionScope())
{
    var order = new Salesorder { CustId = 1, ShipperId = 1, OrderDate = DateTime.Now };
    order.OrderdetailCollectionByOrderId.Add(new Orderdetail { ProductId = 1, ... });
    order.Save();
    scope.Complete();   // commit; omit to roll back the entire hierarchy
}
```

**Framework behavior:** `esEntity.Save()` automatically wraps hierarchical saves in an `esTransactionScope` when the entity has any pre-saves, post-saves, or post-one-saves (i.e. `NeedsTransactionDuringSave()` returns true). On any exception during the phases, the scope is rolled back — the parent insert is reverted along with the failed child. For single-entity saves without hierarchy, no scope is created; the operation is a single SQL statement and therefore inherently atomic. An explicit `esTransactionScope` around a hierarchical save is optional but recommended when multiple independent entities must succeed or fail together, as it joins the ambient scope transparently.

### Multi-entity workflows — explicit scope

For two or more unrelated entities that must succeed or fail together, always use `esTransactionScope`:

```csharp
using (var scope = new esTransactionScope())
{
    var customer = new Customer { CompanyName = "Acme" };
    customer.Save();

    var order = new Salesorder { CustId = customer.CustId.Value, ShipperId = 1, OrderDate = DateTime.Now };
    order.Save();

    scope.Complete();
}
```

---

## Transaction Scope — Driver Compatibility

Use `esTransactionScope` — not `System.Transactions.TransactionScope`:

```csharp
using (var scope = new esTransactionScope())
{
    var invoice = new Invoice { Blablabla = "Test" };
    invoice.Save();

    var detail = new Invoicedetail { Description = "Line 1" };
    detail.Save();

    scope.Complete();   // omit to roll back
}
```

> **Why not `TransactionScope`?** `MySqlConnector` no longer enlists connections into ambient `System.Transactions.TransactionScope` transactions automatically. The connection opened by the provider runs in autocommit mode, and `TransactionScope.Dispose()` does not revert the data. `esTransactionScope` is connection-based and works correctly on all supported .NET versions.

> **`AutoEnlist` connection string setting** is irrelevant for `esTransactionScope`. It only affects `System.Transactions` enlistment and can be left at either value.

Nested scopes are supported. The inner scope votes on the outer transaction; omitting `Complete()` on the scope that owns the root rolls back everything created inside it.

---

## Concurrency and Connection Pool Safety

All save and load operations release connections safely. On error, the connection is closed by `CleanupCommand` (or by `CommandBehavior.CloseConnection` when a reader was opened). **No explicit `ROLLBACK` is issued** — that responsibility belongs to the transaction scope owner. This matches the pattern applied across the PostgreSQL provider and avoids the class of bugs where a child insert failure would abort an otherwise recoverable transaction.

---

## Quick Samples

**Load a collection:**

```csharp
var customers = new CustomerCollection();
customers.LoadAll();

foreach (var customer in customers)
{
    Console.WriteLine(customer.CompanyName);
}
```

**Create, update, delete an entity:**

```csharp
var employee = new Employee();
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

> **SslMode note:** MySQL 8.0 with `caching_sha2_password` (default) requires either `SslMode=Required` or `SslMode=None` combined with `AllowPublicKeyRetrieval=True`. Use `SslMode=Required` for remote servers.

**APPLY query (works identically on MySQL 8 and MariaDB):**

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

More usage examples (joins, paging, transactions, and the full Fluent SQL API): see the [main EntitySpaces README](https://github.com/paulcordova/EntitySpaces).

### Generating your entity classes

See **Requires EntitySpaces Studio** above.

---

## Dependency Updates

- **MySqlConnector 2.6.2** — migrated from the legacy `MySql.Data` driver. See "Driver migration" in the What's New section above for the rationale.

- **System.Configuration.ConfigurationManager** — updated from `10.0.10` to `10.0.11`. Maintenance release. No API changes affecting EntitySpaces.