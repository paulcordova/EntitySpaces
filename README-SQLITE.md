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

This package provides the SQLite runtime only — it does not generate code. Entity and Collection classes are generated from your database schema using **EntitySpaces Studio**, a separate WinForms tool.

- Get it from the repo: [EntitySpaces.Studio](https://github.com/paulcordova/EntitySpaces/tree/master/EntitySpaces.Studio)
- Download the **most recent** `.zip` — avoid any file tagged `-deprecated`, that build is kept for reference only.
- Connect to your database under Settings → Connection, then run the Generated and Custom class templates.

## New Features

- **Culture-independent DECIMAL storage** — Decimal values are now stored as `REAL` (via `DbType.Double` and `DataColumn double`). This prevents silent truncation (e.g., `123.45` becoming `123`) on OS cultures that use a comma as the decimal separator (like Spanish or German).
- **Robust parameter binding on INSERT** — Fixed a bug where dynamic INSERTs with modified columns or explicit PKs would fail because the generated SQL referenced a parameter that was never registered in the `SQLiteCommand.Parameters` collection.
- Auto-increment detection for both SQLite forms — the rowid alias (`INTEGER PRIMARY KEY`) and the explicit `AUTOINCREMENT` keyword — both now correctly generate `IsAutoIncrement = true` metadata
- Foreign key enforcement is now automatic — the provider appends `Foreign Keys=True` to the connection string on every connection, no configuration change needed
- Concurrency exception detection — `SQLITE_CONSTRAINT` (code 19) violations (PK/UNIQUE/FK/CHECK/NOT NULL) are translated to `esConcurrencyException`, consistent with the other providers
- Navigation properties generated in Studio from FK relationships defined in the DDL
- Studio metadata engine rewritten to use native SQLite pragmas (`PRAGMA table_info`, `PRAGMA foreign_key_list`) instead of `information_schema`, which SQLite does not support
- **Tolerant column/property name resolution** — the provider accepts both the database column name and the generated property name when deciding whether a column is part of an INSERT/UPDATE and when mapping server-returned values back to the entity. This covers:
  - `snake_case` → PascalCase conversion: `order_id` ↔ `OrderId`, `customer_id` ↔ `CustomerId`.
  - Arbitrary divergence between a FK column and the PK column it references, including differences in case (`invoiceid` ↔ `Invoiceid`) and the presence of underscores (`Invoice_ID` ↔ `InvoiceID`).
  - No code changes required on your side; the tolerant resolution is applied uniformly to INSERT, UPDATE, DELETE, and server-returned value mapping.
- **Identity and Default Read-Back** — The provider fetches identity PKs via `last_insert_rowid()` and retrieves `DEFAULT`-populated columns (including concurrency tokens) through a read-back `SELECT` within the `OnRowUpdated` event. Server-side special columns (e.g., `DateAdded`) are excluded from the INSERT and read back in this same step, ensuring a seamless round-trip.
- **Server-side special columns** (`DateAdded`, `DateModified`, `AddedBy`, `ModifiedBy`) — excluded from the INSERT column list and the UPDATE SET clause when `IsServerSide` is set; the DB `DEFAULT` fills them and the value is read back through the read-back `SELECT`.
- **Client-side special columns** — When Studio is configured to generate `DateAdded` and `DateModified` with `DateType.ClientSide` (and `ClientType.Now`), the Core framework populates these fields before the INSERT/UPDATE. The provider correctly binds them as parameters. Auditing columns (`AddedBy`, `ModifiedBy`) with `UseEventHandler = true` remain null if no event handler is implemented, without failing the save operation.
- **`DEFAULT` columns not explicitly assigned send `DBNull.Value` instead of `null`** — the driver raises a parameter exception on `null`, this is now handled automatically.

## Fixes

- **Decimal truncation on non-English cultures** — Fixed an issue where `decimal` columns were serialized to text using `CurrentCulture` before being bound to SQLite. The provider now forces binary double binding, preserving precision across all platforms.
- **Missing parameter registration in INSERT** — Fixed `BuildDynamicInsertCommand` to correctly call `cmd.Parameters.Add(clone)` in the `isModified` and `hasExplicitPkValue` branches. Previously, the SQL was generated correctly but the parameter collection was empty, causing a runtime exception.
- FK violations now correctly raise `esConcurrencyException` instead of failing silently — a direct result of foreign key enforcement now being applied by default
- Explicit PK inserts on auto-increment tables are now detected correctly, so an explicitly assigned `Id` is inserted directly instead of being overwritten by a generated rowid
- **Tolerant name resolution in dynamic INSERT/UPDATE/DELETE** — the provider now accepts both the canonical DB column name (`invoiceid`) and the EntitySpaces property name (`Invoiceid`) when matching modified columns, reading values, and building WHERE clauses. SQLite preserves whatever casing was used at schema-creation time, so schemas that use lowercase columns with PascalCase property names would previously fail to bind the FK parameter (bound as `DBNull`) or produce no match in the WHERE clause. Aligned with the pattern in the MySQL, PostgreSQL, and SQL Server providers.
- **Hierarchical save propagates a user-supplied parent PK to child FKs** across all three construction patterns: `parent.Collection.Add(child)`, `parent.Collection.AddNew()`, and direct `Collection.Save()` calls. Previously only auto-generated parent PKs (rowid / `AUTOINCREMENT`) propagated reliably; parents with user-supplied keys (e.g. `Customers.CustomerId` of type `TEXT`) required an explicit FK assignment on each child. Aligned cross-provider.
- **In-memory entity state after a hierarchical save reflects the propagated FK** — `SyncCurrentValueKeys` merges any property-name-keyed entries into the canonical column-name slot inside `AcceptChanges`, so the typed getter (`child.Invoiceid`) returns the correct value without requiring a reload. `GetExtraColumns` was updated to accept property names as valid identifiers for real columns, preventing the merged keys from being misclassified as extra columns in serialization payloads.
- **`esEntityCollection.Save()` applies the collection-level `fks` dictionary to Added children before the PreSave loop** — covers the parent-initiated pattern where the parent is saved before the children are added to the collection. Previously the FK was only carried to the SQL parameters (correct DB value) but never written to the child's `CurrentValues` (null getter after save).
- **`esEntityCollection.Combine()` writes the target collection's `fks` using `SetProperty`** (comparing values through `SetValue`) instead of `SetColumn(..., false)`. The prior unconditional `MarkFieldAsModified` flipped `Unchanged` entities to `Modified` even when the FK value already matched, causing spurious UPDATEs on subsequent `Save()` calls.
- **Dynamic INSERT no longer emits bareword `ServerSideText` for special columns** — the generated BO stores `ProviderMetadata["DateAdded.ServerSideText"]` (and siblings) as the bareword column name (`"DateAdded"`), which the provider was concatenating directly into the INSERT values. SQLite resolved the bareword to `NULL` on the new row, tripping the `NOT NULL` constraint. Special columns are now excluded from the INSERT values and read back through `RETURNING` / `SELECT`.
- **Dynamic UPDATE resolves special-column `ServerSideText` barewords to valid SQL expressions** — the UPDATE SET clause emitted `SET "DateModified" = DateModified`, which SQLite resolved as a column reference on the new row. A `ResolveServerSideExpression` helper translates the bareword in three steps: (1) if the value already looks like a real expression, use it as-is; (2) use the column's metadata `Default`; (3) otherwise fall back by well-known column name (`DateAdded` / `DateModified` → `CURRENT_TIMESTAMP`, `AddedBy` / `ModifiedBy` → `NULL`). If nothing applies, the `SET` is omitted.
- **Explicit `ROLLBACK` removed from error paths** — the provider no longer issues `ROLLBACK` on enlisted connections. On an `esTransactionScope`-enlisted connection, `ROLLBACK` aborts the *ambient* transaction, not just the current command. Transaction rollback is now delegated entirely to `esTransactionScope`, which is the correct owner. Aligned with the MySQL, PostgreSQL, and SQL Server providers.

## ⚠️ Transaction Management

**Use `esTransactionScope` for transactions — not raw `SQLiteConnection.BeginTransaction()`.**

The provider participates correctly in EntitySpaces' native `esTransactionScope`, which is the supported mechanism across all EntitySpaces providers:

```csharp
using (var scope = new esTransactionScope())
{
    var employee = new Employees { FirstName = "Joe", LastName = "Smith" };
    employee.Save();

    var product = new Products { ProductName = "Some Gadget" };
    product.Save();

    scope.Complete();   // commit; omit to roll back
}
```

Nested `esTransactionScope` instances are supported — the inner scope votes on the outer transaction. Omitting `Complete()` on any scope that owns the root transaction rolls back everything created inside it.

> **Connection pool behavior on error:** All save and load operations release connections safely. On error, the connection is closed by `CleanupCommand` (or by the reader being disposed when the `RETURNING` result set was opened). **No explicit `ROLLBACK` is issued** — that responsibility belongs to the transaction scope owner. This avoids the class of bugs where a child insert failure would abort an otherwise recoverable transaction.

> **Scope poisoning on error:** When a constraint violation occurs (e.g., NOT NULL), the provider's `SaveDynamicEntity` catch block closes the ambient connection to prevent connection pool corruption. As a result, the current `esTransactionScope` becomes unusable — any subsequent command will fail with `SQLiteException: database is locked`. This mirrors SQL Server's `XACT_ABORT` behavior. 
> **Recovery pattern:** Dispose the poisoned scope without calling `Complete()` (which rolls back the transaction cleanly), and open a new `esTransactionScope` for subsequent operations.

> **SQLite file locking:** SQLite serializes writers at the file level. In single-process scenarios `esTransactionScope` provides the correct isolation. If multiple processes write to the same database file, expect `SQLITE_BUSY` responses under contention. Combine `WAL` journaling with `busy_timeout` in the connection string for shared-file scenarios.

## ⚠️ Hierarchical Save

Hierarchical parent-child saves work correctly on SQLite when using the pattern that EntitySpaces was designed for — **adding the child to the parent's collection before calling `parent.Save()`**:

```csharp
var order = new Orders
{
    CustomerId = "ALFKI",
    EmployeeId = 4,
    ShipVia = 1,
    OrderDate = DateTime.Now
};

// Recommended — add to the parent's collection
order.OrderDetailsCollectionByOrderId.Add(new OrderDetails
{
    ProductId = 10,
    UnitPrice = 15.50m,
    Quantity = 5,
    Discount = 0m
});

order.Save();   // saves the parent first, then each child with the correct FK
```

`AddNew()` works the same way:

```csharp
var detail = order.OrderDetailsCollectionByOrderId.AddNew();
detail.ProductId = 11;
detail.UnitPrice = 10.00m;
detail.Quantity = 2;
detail.Discount = 0m;

order.Save();
```

The **child-initiated pattern** (save from the child with `UpTo` set) is also supported:

```csharp
var detail = new OrderDetails { ProductId = 1, UnitPrice = 25m, Quantity = 3, Discount = 0m };
detail.UpToOrdersByOrderId = order;
detail.Save();   // saves the parent first to obtain the PK, then the child
```

### One-to-One and Self-Referencing Relationships

The provider fully supports **One-to-One (1:1)** relationships with shared primary keys, and **Self-Referencing** tables (e.g., parent-child hierarchies) via the `PostOneSave` and `UpTo` mechanisms generated by Studio.

**One-to-One (Shared PK):**
```csharp
var site = new Site { Name = "Main Site" };
var info = new SiteInfo { SalesCode = "S123" };

info.UpToSite = site;
info.Save(); // Inserts the Site, obtains the Id, and assigns it to SiteInfo.SiteId
```

### One-to-One and Self-Referencing Relationships

The provider fully supports **One-to-One (1:1)** relationships with shared primary keys, and **Self-Referencing** tables (e.g., parent-child hierarchies) via the `PostOneSave` and `UpTo` mechanisms generated by Studio.

**One-to-One (Shared PK):**
```csharp
var site = new Site { Name = "Main Site" };
var info = new SiteInfo { SalesCode = "S123" };

info.UpToSite = site;
info.Save(); // Inserts the Site, obtains the Id, and assigns it to SiteInfo.SiteId
```

**Self-Referencing (Hierarchy):**
```csharp
var parentCategory = new CategorySelf { Name = "Electronics" };
var childCategory = new CategorySelf { Name = "Laptops" };

parentCategory.CategorySelfCollectionByParentId.Add(childCategory);
parentCategory.Save(); // Propagates the parent Id to the child's ParentId
```

### Atomicity — implicit transaction

`esEntity.Save()` automatically wraps hierarchical saves in an `esTransactionScope` when the entity has any pre-saves, post-saves, or post-one-saves (i.e. `NeedsTransactionDuringSave()` returns true). On any exception during the save phases, the scope is rolled back — the parent insert is reverted along with the failed child. **No explicit `esTransactionScope` is required for the common hierarchical case.**

For single-entity saves without hierarchy, no scope is created; the operation is a single SQL statement and therefore inherently atomic.

An explicit `esTransactionScope` around a hierarchical save is optional but recommended when multiple independent entities must succeed or fail together — the framework's internal scope joins the ambient one transparently via `Required` semantics.

### Non-identity parent PKs

FK propagation from parent to child works identically for auto-increment PKs and user-supplied PKs:

- **Auto-increment parent** — `Orders.OrderId` (`INTEGER PRIMARY KEY` / `AUTOINCREMENT`): the parent is inserted, the generated rowid is retrieved via `RETURNING` (or `last_insert_rowid()` on older SQLite), and the FK is written into the child before the child's INSERT.
- **User-supplied parent** — `Customers.CustomerId` (`TEXT`, no auto-increment): the parent PK is already known before the INSERT, so the FK is available immediately and the parent-child ordering is still preserved for referential integrity.
- **Explicit value on an auto-increment parent** — `Orders.OrderId` assigned explicitly by the application: SQLite accepts the explicit value; the provider detects it and skips the auto-generated id path, and the FK is propagated the same way.

Both paths are exercised by the test suite, covering all five save patterns: persist-then-add, save-together, `AddNew()`, child-initiated via `UpTo`, and collection-initiated via `collection.Save()`.

> **⚠️ Do not use bidirectional linking.** Setting both `parent.Children.Add(child)` **and** `child.UpToParent = parent` causes infinite recursion between the parent and child navigation properties. In practice this manifests as a hang or a `StackOverflowException` depending on the runtime. This is a limitation of the current EntitySpaces Core, not of this provider. Use one direction — the collection when saving from the parent, or `UpTo` when saving from the child.

### Collection `Combine()`

`esEntityCollection<T>.Combine(source)` moves entities from a source collection into a target collection. When the target has `fks` populated by a parent navigation getter (e.g. `Customers.OrdersCollectionByCustomerId`), the FK values are written into each combined entity's `CurrentValues` **and marked as modified** so they are included in the subsequent INSERT. The operation preserves the `RowState` of `Unchanged` entities whose FK already matches, and does not duplicate the FK entry in `ModifiedColumns`.

```csharp
var source = new OrdersCollection();
var order = source.AddNew();
order.EmployeeId = 1;
order.OrderDate = DateTime.Now;

var target = customer.OrdersCollectionByCustomerId;
target.Combine(source);
target.Save();   // FK is written and persisted
```

### Cross-provider parity

The fixes documented in this README were validated against the SQL Server, MySQL, and PostgreSQL providers and ported to SQLite without provider-specific changes. The following behaviours are now aligned across the four providers:

- Hierarchical save for auto-increment and non-identity parent PKs.
- **One-to-One (1:1) relationships and Self-referencing hierarchies.**
- FK propagation across all five save patterns.
- **Tolerant column/property name resolution, including divergent FK names (case and underscore variations).**
- `Combine()` FK propagation and `RowState` preservation.
- Transaction scope, rollback, and atomicity.
- Dynamic INSERT/UPDATE handling of **both Server-Side and Client-Side special columns**.
- No explicit `ROLLBACK` issued by the provider — delegated entirely to `esTransactionScope`.
- **Advanced SQLite semantics validated by 50 tests** — The suite now covers case-sensitive equality vs. `LIKE`, NULL three-valued logic, `CAST` truncation, rowid identity/reuse, explicit PK inserts, and FK `NO ACTION` constraints (deleting parents with active children).

### SQLite-specific notes

- **No stored procedures.** SQLite has no `CREATE PROCEDURE` equivalent, so the `StoredProcedure` save path (`esSqlAccessType.StoredProcedure`) is not supported. All saves go through the dynamic SQL path.
- **`RETURNING` availability.** SQLite 3.35 (March 2021) introduced `INSERT ... RETURNING` and `UPDATE ... RETURNING`. On builds without it, the provider falls back to a two-step pattern (`last_insert_rowid()` + a read-back `SELECT`). Update the SQLite native bundle if `RETURNING` is available — it halves the round-trips for INSERTs with server-generated columns.
- **File-level locking.** SQLite is a file, not a server. Writers are serialized at the file level. For applications with multiple concurrent writers, use `journal_mode=WAL` in the connection string to allow concurrent readers during a write.
- **Rowid reuse:** SQLite reuses the rowid of the last deleted row. If your audit trail depends on stable IDs, add `AUTOINCREMENT` to your DDL (at the cost of a non-transactional `sqlite_sequence`).
- **String collation:** `=` is strictly case-sensitive. `LIKE` is case-insensitive, but only for ASCII characters.
- **Foreign Key `NO ACTION`:** The provider does not cascade deletes. You must delete children before parents, or define `ON DELETE CASCADE` in your DDL.

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
