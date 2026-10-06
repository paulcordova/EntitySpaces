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

This package provides the MySQL / MariaDB runtime only — it does not generate code. Entity and Collection classes are generated from your database schema using **EntitySpaces Studio**, a separate WinForms tool.

- Get it from the repo: [EntitySpaces.Studio](https://github.com/paulcordova/EntitySpaces/tree/master/EntitySpaces.Studio)
- Download the **most recent** `.zip` — avoid any file tagged `-deprecated`, that build is kept for reference only.
- Connect to your database under Settings → Connection, then run the Generated and Custom class templates.

## New Features

- **Tolerant column/property name resolution** — the provider accepts both the database column name and the generated property name when deciding whether a column is part of an INSERT/UPDATE/DELETE and when mapping server-returned values back to the entity. This covers:
  - Lowercase column names (`invoiceid` column ↔ `Invoiceid` property) — the common MySQL/MariaDB convention in Northwind-derived schemas.
  - Arbitrary divergence between a FK column and the PK column it references, without splitting on an underscore.
  - No code changes required; the tolerant resolution is applied uniformly to INSERT, UPDATE, DELETE, and server-returned value mapping.

- **Server-side key sync after INSERT/UPDATE** — after a successful INSERT or UPDATE, the provider synchronizes both keys (column name `invoiceid` and property name `Invoiceid`) in the entity's `CurrentValues`, so the in-memory getter sees the value that was persisted. This is what makes hierarchical saves and explicit-PK inserts return the expected value from the FK getter.

- **Explicit `AUTO_INCREMENT` PK insert** — when a user assigns a value to an auto-increment PK, the column is sent as an input parameter and included in the INSERT; MySQL uses the explicit value instead of generating a new one. Combined with the key sync, the entity is marked clean after save and the value is preserved.

- **Automatic engine detection** — the provider queries `SELECT VERSION()` on the first call per connection string and caches the result. No configuration required to distinguish MySQL from MariaDB.

- **Automatic `APPLY` → correct SQL strategy per engine** — write `OuterApply`/`CrossApply` once; the provider emits native `LEFT JOIN LATERAL` / `JOIN LATERAL` on MySQL 8.0.14+, and an equivalent `ROW_NUMBER() OVER (PARTITION BY)` pattern on MariaDB 10.2+ and older MySQL.

- **Concurrency exception detection and translation to `esConcurrencyException`** — duplicate entry, deadlock, and lock-wait timeout are translated to the same exception type used by every other EntitySpaces provider.

- **Removed explicit `ROLLBACK` from error paths** — the provider no longer issues `ROLLBACK` on enlisted connections. On an `esTransactionScope`-enlisted connection, `ROLLBACK` aborts the *ambient* transaction, not just the current command. Rollback is now delegated entirely to `esTransactionScope`, which is the correct owner and matches the pattern already applied to the PostgreSQL provider.

- **`OnRowUpdated` no longer swallows exceptions** — exceptions during server-side value retrieval (`LAST_INSERT_ID()`, `LastTimestamp()`, defaults) are now surfaced via `Debug.WriteLine` and `Trace.WriteLine`. Previously a `catch { }` block silently hid failures during post-insert processing.

- **AutoInc and Defaults values written to both key names** (`id` and `Id`) — the entity getter and the provider now see the same value after an INSERT with server-generated columns.

- **Thread-safe parameter cache** — the parameter cache is a `ConcurrentDictionary` keyed by `DataID`. No lock contention in multi-threaded environments; the cache is built once per entity type and reused safely across threads.

- **Studio metadata engine now extracts the `EXTRA` column** (`VIRTUAL GENERATED` / `STORED GENERATED`) for computed-column detection and `TIMESTAMP` concurrency detection. See the main README's Studio Modernization section for the full metadata matrix (character max length, numeric precision/scale, computed columns, auto-increment, defaults).

## Fixes

- **Hierarchical parent-child save with lowercase column names** — children were inserted with a NULL foreign key on schemas where the FK column uses a lowercase convention (`invoiceid` column vs `Invoiceid` property). The INSERT builder now resolves the FK column by either name and includes it in the statement. Combined with the server-side key sync, both the persisted value and the in-memory getter reflect the propagated parent PK:

    ```
    Parent.Id        = 22       (parent inserted)
    Child.Id         = 9        (child inserted)
    Child.Invoiceid  = 22       (FK correctly propagated)
    Child.IsDirty    = False    (framework cleared the entity)
    ```

- **Corrected parameter lookup in `OnRowUpdated`** — the parameter name for the auto-increment value is `?PropertyName`, not `?ColumnName`. The old code constructed `"?" + columnName` and silently failed to find the parameter; the fix iterates by `SourceColumn` (which is the DB column name) to locate the correct parameter.

- **Connection pool safety on error** — all save and load operations release connections safely:

    - `ExecuteReader` opens its own raw connection and closes it via `CleanupCommand` when `ExecuteReader()` itself fails — `CommandBehavior.CloseConnection` only fires when `ExecuteReader()` succeeds and the reader is disposed.
    - Other operations use `esTransactionScope.DeEnlist()` in `finally` blocks.
    - **No explicit `ROLLBACK` is issued** — that responsibility belongs to the transaction scope owner.

    This eliminates the class of bugs where an error in a child insert would abort an ambient transaction, not just the current command.

- **Documented and handled MySQL 8.0's `caching_sha2_password` default** — MySQL 8.0 with `caching_sha2_password` requires either `SslMode=Required` or `SslMode=None` combined with `AllowPublicKeyRetrieval=True`. Connections without this previously failed against default MySQL 8.0 configurations with an authentication error. See the connection string section below.

- **Clarified case-sensitivity handling across platforms** — MySQL on Linux is case-sensitive for table/schema names (`lower_case_table_names=0`); MySQL on Windows and all MariaDB platforms are case-insensitive. Class generation is now documented to always run against the target server to guarantee consistent metadata. See the case-sensitivity section below.

## ⚠️ Transaction Management

**Use `esTransactionScope` for transactions — not `System.Transactions.TransactionScope`.**

`MySqlConnector`, like modern Npgsql and Microsoft.Data.SqlClient, does not enlist connections into ambient `System.Transactions.TransactionScope` transactions automatically. If your application relies on `TransactionScope` for rollback, the INSERT/UPDATE will run in autocommit mode and the rollback will not revert your data.

The provider participates correctly in EntitySpaces' native `esTransactionScope`, which is the supported mechanism across all EntitySpaces providers:

```csharp
using (var scope = new esTransactionScope())
{
    var employee = new Employee { FirstName = "Joe", LastName = "Smith" };
    employee.Save();

    var product = new Product { ProductName = "Some Gadget" };
    product.Save();

    scope.Complete();   // commit; omit to roll back
}
```

Nested `esTransactionScope` instances are supported — the inner scope votes on the outer transaction. Omitting `Complete()` on any scope that owns the root transaction rolls back everything created inside it.

> **`AutoEnlist` connection string setting** is irrelevant for `esTransactionScope`. It only affects `System.Transactions` enlistment and can be left at either value.

> **Connection pool behavior on error:** All save and load operations release connections safely. On error, the connection is closed by `CleanupCommand` (or by `CommandBehavior.CloseConnection` when a reader was opened). **No explicit `ROLLBACK` is issued** — that responsibility belongs to the transaction scope owner. This matches the pattern applied across the PostgreSQL provider and avoids the class of bugs where a child insert failure would abort an otherwise recoverable transaction.

> **Note:** the previous implementation issued `ROLLBACK` on enlisted connections inside `finally` blocks. Under `MySqlConnector` / ambient `esTransactionScope`, that aborted the ambient transaction, not just the current command. This was actively harmful and is no longer done.

## ⚠️ Hierarchical Save

Hierarchical parent-child saves work correctly on MySQL and MariaDB when using the pattern that EntitySpaces was designed for — **adding the child to the parent's collection before calling `parent.Save()`**:

```csharp
var order = new Salesorder
{
    CustId = 65,
    ShipperId = 3,
    EmployeeId = 1,
    OrderDate = DateTime.Now
};

// Recommended — add to the parent's collection
order.OrderdetailCollectionByOrderId.Add(new Orderdetail
{
    ProductId = 1,
    UnitPrice = 15.50m,
    Quantity = 5,
    Discount = 0m
});

order.Save();   // saves the parent first, then each child with the correct FK
```

`AddNew()` works the same way:

```csharp
var detail = order.OrderdetailCollectionByOrderId.AddNew();
detail.ProductId = 2;
detail.UnitPrice = 10.00m;
detail.Quantity = 2;
detail.Discount = 0m;

order.Save();
```

The **child-initiated pattern** (save from the child with `UpTo` set) is also supported:

```csharp
var detail = new Orderdetail { ProductId = 1, UnitPrice = 25m, Quantity = 3, Discount = 0m };
detail.UpToSalesorderByOrderId = order;
detail.Save();   // saves the parent first to obtain the PK, then the child
```

### Atomicity — implicit transaction

`esEntity.Save()` automatically wraps hierarchical saves in an `esTransactionScope` when the entity has any pre-saves, post-saves, or post-one-saves (i.e. `NeedsTransactionDuringSave()` returns true). On any exception during the save phases, the scope is rolled back — the parent insert is reverted along with the failed child. **No explicit `esTransactionScope` is required for the common hierarchical case.**

For single-entity saves without hierarchy, no scope is created; the operation is a single SQL statement and therefore inherently atomic.

An explicit `esTransactionScope` around a hierarchical save is optional but recommended when multiple independent entities must succeed or fail together — the framework's internal scope joins the ambient one transparently via `Required` semantics.


### Non-identity parent PKs

FK propagation from parent to child works identically for auto-increment PKs and user-supplied PKs:

- **Auto-increment parent** — `Salesorder.OrderId` (`AUTO_INCREMENT`): the parent is inserted, the generated value is retrieved via `LAST_INSERT_ID()`, and the FK is written into the child before the child's INSERT.
- **User-supplied parent** — `Region.RegionId` (`INT NOT NULL`, no `AUTO_INCREMENT`): the parent PK is already known before the INSERT, so the FK is available immediately and the parent-child ordering is still preserved for referential integrity.
- **Explicit value on `AUTO_INCREMENT` parent** — `Salesorder.OrderId` assigned explicitly by the application: MySQL accepts the explicit value; the provider routes it as an input parameter and the FK is propagated the same way.

All three paths are exercised by the test suite against MySQL 8 and MariaDB 10.11, covering all six save patterns: persist-then-add, save-together, `AddNew()`, child-initiated via `UpTo`, collection-initiated via `collection.Save()`, and `collection.BulkInsert()`.

> **⚠️ Do not use bidirectional linking.** Setting both `parent.Children.Add(child)` **and** `child.UpToParent = parent` causes infinite recursion between the parent and child navigation properties. In practice this manifests as a hang or a `StackOverflowException` depending on the runtime. This is a limitation of the current EntitySpaces Core, not of this provider. Use one direction — the collection when saving from the parent, or `UpTo` when saving from the child.

### Collection `Combine()`

`esEntityCollection<T>.Combine(source)` moves entities from a source collection into a target collection. When the target has `fks` populated by a parent navigation getter (e.g. `Customer.SalesorderCollectionByCustId`), the FK values are written into each combined entity's `CurrentValues` **and marked as modified** so they are included in the subsequent INSERT. The operation preserves the `RowState` of `Unchanged` entities whose FK already matches, and does not duplicate the FK entry in `ModifiedColumns`.

```csharp
var source = new SalesorderCollection();
var order = source.AddNew();
order.ShipperId = 3;
order.EmployeeId = 1;
order.OrderDate = DateTime.Now;

var target = customer.SalesorderCollectionByCustId;
target.Combine(source);
target.Save();   // FK is written and persisted
```

### Cross-provider parity

The fixes documented in this README were originally validated against SQL Server, then ported to PostgreSQL, and now to MySQL/MariaDB without provider-specific changes. The following behaviours are validated against MySQL 8.0.28 and MariaDB 10.11:

- Hierarchical save for auto-increment, user-supplied, and explicit-value PKs.
- FK propagation across all six save patterns (including `BulkInsert`).
- Tolerant column/property name resolution, including divergent FK names.
- `Combine()` FK propagation and `RowState` preservation.
- Transaction scope, rollback, and atomicity — including CHECK-constraint-triggered rollback of an in-flight hierarchy.
- Implicit transaction created by `esEntity.Save()` on the `NeedsTransactionDuringSave()` path.

## MySQL / MariaDB APPLY Support

EntitySpaces automatically translates `OuterApply` / `CrossApply` into the correct SQL strategy based on the detected server engine and version — no code changes required when switching between MySQL and MariaDB.

| EntitySpaces | MySQL 8.0.14+ | MariaDB 10.2+ |
|---|---|---|
| `OuterApply` | `LEFT JOIN LATERAL` | `LEFT JOIN` + `ROW_NUMBER() OVER (PARTITION BY)` |
| `CrossApply` | `JOIN LATERAL` | `JOIN` + `ROW_NUMBER() OVER (PARTITION BY)` |
| `Top(n)` | `LIMIT n` inside LATERAL | `es_rn <= n` in outer `ON` clause |
| Without `Top()` | No LIMIT | All rows per partition returned |

Engine detection is automatic — the provider queries `SELECT VERSION()` on the first call per connection string and caches the result. No configuration required.

**C# query (provider-agnostic):**

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

## Concurrency Exception Detection

The provider translates MySQL and MariaDB-specific error codes into `esConcurrencyException`, consistent with all other EntitySpaces providers:

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

## Case Sensitivity

MySQL on **Linux** is case-sensitive for table and schema names (`lower_case_table_names=0`).
MySQL on **Windows** is case-insensitive. MariaDB is case-insensitive on all platforms.

EntitySpaces generates class metadata (`meta.Source`, `meta.Destination`) using the exact table names as they exist in the database at generation time. **Always generate your EntitySpaces classes directly against the target server** to guarantee case consistency.

| Platform | Behavior | Recommendation |
|---|---|---|
| MySQL on Linux | Case-sensitive | Generate classes against Linux MySQL |
| MySQL on Windows | Case-insensitive | No special action needed |
| MariaDB (all) | Case-insensitive | No special action needed |

## Dependency Updates

- **MySqlConnector 2.6.2** — migrated from the legacy `MySql.Data` driver. `MySqlConnector` is the actively maintained, .NET-8+-first ADO.NET provider for MySQL/MariaDB. Both drivers are wire-compatible, but `MySqlConnector` has:
  - First-class support for `System.Transactions` with explicit opt-in (previously automatic).
  - Consistent behavior across .NET Framework 4.8 and .NET 8–10.
  - Faster, allocation-light parameter handling.

- **System.Configuration.ConfigurationManager** — updated from `10.0.10` to `10.0.11`. Maintenance release. No API changes affecting EntitySpaces.

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

