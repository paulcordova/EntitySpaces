![ES-logo](ES-Studio.jpg)
# EntitySpaces 2027 – Quick Start Guide
*Version for developers who want to get started in less than 15 minutes.*

---

## 1. Requirements

- **.NET Framework 4.8** or **.NET 8/9/10** (depending on your project).
- **Visual Studio 2022** (or 2019) with Desktop/Web development workloads installed.
- **EntitySpaces Studio 2024** (standalone code generator).


## 2. Download and Install EntitySpaces Studio

1. Go to the GitHub repository:  
   [https://github.com/paulcordova/EntitySpaces](https://github.com/paulcordova/EntitySpaces)  
   and download the `EntitySpacesStudio_2024.8.30.0.zip` file (or the latest version available).
2. Extract the contents to a local folder (e.g., `C:\EntitySpaces\Studio`).
3. Run `StandAlone.exe` as Administrator (Windows may prompt for permissions).

> ✅ **Tip:** The Studio is a WinForms application that runs independently of Visual Studio. You can use it on any machine.


## 3. Connect to your Database

Once the Studio is open:

1. Go to the **Settings → Connection** tab.
2. Select the **Driver** corresponding to your database engine (SQL Server, PostgreSQL, MySQL, SQLite, Oracle, Firebird).
3. Click **Configure Connection** to build the connection string, or enter it manually.
4. Test the connection using the **Test Connection** button.

<!-- Suggested screenshot: Studio connection screen (Settings → Connection) showing the connection string field and the Test button. Take a screenshot of the window and replace this comment with the image. -->

---

<div style="page-break-before:always">&nbsp;</div>
<p></p>

**Example connection strings:**

| Database | Example Connection String |
|----------|---------------------------|
| SQL Server | `User ID=sa;Password=...;Initial Catalog=Northwind;Data Source=localhost;TrustServerCertificate=True;` |
| PostgreSQL | `Host=localhost;Port=5432;Database=Northwind;Username=postgres;Password=...;SSL Mode=Require;` |
| MySQL | `Server=localhost;Port=3306;Database=Northwind;Uid=root;Pwd=...;SslMode=Required;` |
| SQLite | `Data Source=C:\Northwind.db3;Version=3;Foreign Keys=True;` |
| Oracle | `Data Source=...;User Id=...;Password=...;` (additional wallet configuration required for ATP) |

---

## 4. Generate C# or VB.NET Classes

1. Go to the **Templates** tab.
2. Select your preferred language (C# or VB.NET) and the **"Generated Classes Master"** template.
3. In the dialog that appears:
   - Define the **Namespace** (e.g., `BusinessObjects`).
   - Choose the tables/views you want to generate.
   - In the **Advanced** tab, review options such as `Generate a Single File` (recommended).
4. Click **OK**. The Studio will generate the files in the folder you specified (by default, in the `Generated` and `Custom` subfolders).
5. Repeat the process with the **"Custom Classes Master"** template (this is generated only once; you will place your business logic here).

<!-- Suggested screenshot: Generated Classes Master template dialog, showing table selection and the Namespace field. -->

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 5. Add NuGet Packages to your Project

Open your project in Visual Studio and add the provider package you need.

**From the Package Manager Console:**

```powershell
# Example for SQL Server
Install-Package EntitySpaces.ORM.SqlServer.NET

# For PostgreSQL
Install-Package EntitySpaces.ORM.PostgreSQL.NET

# For MySQL
Install-Package EntitySpaces.ORM.MySQL.NET

# For SQLite
Install-Package EntitySpaces.ORM.SQLite.NET

# For Oracle Managed Client
Install-Package EntitySpaces.ORM.OracleManagedClient.NET

# For Firebird
Install-Package EntitySpaces.ORM.Firebird.NET
```

These packages include all necessary dependencies (`EntitySpaces.Core`, `EntitySpaces.Interfaces`, `EntitySpaces.DynamicQuery`, the Loader, and the specific provider).

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 6. Configure the Loader and Connection in your Code

EntitySpaces requires you to register the **Loader** (Medium Trust or Standard) and the **connection string**.

**At the start of your application (e.g., `Program.Main`, `Global.asax`, or the main form constructor):**

```csharp
using EntitySpaces.Interfaces;

// 1. Register the Loader (Medium Trust version is recommended)
esProviderFactory.Factory = new EntitySpaces.LoaderMT.esDataProviderFactory();

// 2. Configure the connection
esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SqlClientProvider";   // Change according to your provider
conn.ConnectionString = "User ID=...;Password=...;Initial Catalog=Northwind;Data Source=localhost;";
conn.SqlAccessType = esSqlAccessType.DynamicSQL;    // or StoredProcedure
conn.DatabaseVersion = "2016";                      // Optional, auto-detected if omitted

// 3. Add the connection and set it as the default
esConfigSettings.ConnectionInfo.Connections.Add(conn);
esConfigSettings.ConnectionInfo.Default = "Northwind"; // any name you prefer
```

**Alternative:** Use an `app.config` / `web.config` file with the `<EntitySpaces>` section. The complete guide is available in the repository's README.

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 7. Write your First Query

With the generated classes and configuration in place, you can start loading data:

```csharp
// Load all customers
CustomersCollection customers = new CustomersCollection();
if (customers.LoadAll())
{
    foreach (Customers cust in customers)
    {
        Console.WriteLine($"{cust.CustomerID} - {cust.CompanyName}");
    }
}

// Query with filter (Fluent API)
CustomersCollection coll = new CustomersQuery("c", out var c)
    .Where(c.Country == "USA")
    .OrderBy(c.CompanyName.Ascending)
    .ToCollection<CustomersCollection>();

// Basic CRUD
Customers newCust = new Customers();
newCust.CustomerID = "NEWCO";
newCust.CompanyName = "Nueva Empresa";
newCust.Save();                     // INSERT

newCust.CompanyName = "Nueva Empresa S.A.";
newCust.Save();                     // UPDATE

newCust.MarkAsDeleted();
newCust.Save();                     // DELETE
```

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 8. Usage with Wisej.NET

[**Wisej.NET**](https://wisej.com) is a framework that allows you to build web applications using the same logic and event model as Windows Forms. EntitySpaces integrates seamlessly:

- Reuse all your generated classes and existing business logic.
- The connection and transaction patterns are identical to those of a desktop application.
- The ORM correctly handles connection pooling and concurrency in the web environment (thanks to the thread-safe parameter cache and transaction management in `finally` blocks).

**Example in a Wisej control:**

```csharp
// In a Button Click event
private void buttonLoad_Click(object sender, EventArgs e)
{
    var products = new ProductsCollection();
    products.Query.Where(products.Query.UnitPrice > 10);
    products.Query.Load();

    dataGridView1.DataSource = products;
}
```

Refer to the [official Wisej.NET documentation](https://wisej.com/documentation/) for more details on deployment.

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 9. Common Troubleshooting

| Issue | Solution |
|-------|----------|
| **"Cannot find EntitySpaces.Core assembly"** | Ensure you have installed the correct NuGet package and that it is referenced in your project. |
| **"Error connecting to SQLite"** | Verify that the connection string includes `Version=3;Foreign Keys=True;` and that the .db3 file exists. |
| **Studio doesn't show tables** | In the Studio, make sure you have selected the correct Catalog/Database from the dropdown after connecting. |
| **Architecture error when running Studio** | The Studio is compiled for **x86**; do not run it in AnyCPU mode. If using native dependencies (SQLite, Oracle), ensure you have the x86 version. |
| **Concurrency exception on save** | EntitySpaces throws `esConcurrencyException` for key violations, deadlocks, etc. Catch it and handle the conflict according to your business logic. |

---




## 10. Additional Resources

- **GitHub Repository:** [https://github.com/paulcordova/EntitySpaces](https://github.com/paulcordova/EntitySpaces)  
  - Source code, complete examples, change history.
- **Extended Documentation:** Inside the repository, you will find the `README.md` with all technical details, including metadata migration, providers, and best practices.
- **Forum / Issues:** Use GitHub Issues to report problems or suggest improvements.
- **Consulting:** Contact `paul.cordova@netstep.cl` for professional support or training.

---

That's it! With these steps, your environment is set up and you are ready to start developing with EntitySpaces 2027.

*Last updated: July 2026 – Compatible with .NET 10.*

---
