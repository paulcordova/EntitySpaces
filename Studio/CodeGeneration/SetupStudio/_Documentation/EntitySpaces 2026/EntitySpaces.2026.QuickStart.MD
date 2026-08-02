![ES-logo](ES-Studio.jpg)

# EntitySpaces 2026 – Quick Start Guide
*For developers who want to get started in less than 15 minutes.*

*Last updated: July 30, 2026*

---

## 📑 Table of Contents

- [EntitySpaces 2026 – Quick Start Guide](#entityspaces-2026--quick-start-guide)
  - [📑 Table of Contents](#-table-of-contents)
  - [🚀 What's New in EntitySpaces Studio 2026](#-whats-new-in-entityspaces-studio-2026)
  - [1. Requirements](#1-requirements)
  - [2. Step-by-Step Installation](#2-step-by-step-installation)
    - [2.1. Download the Installer](#21-download-the-installer)
    - [2.2. Run the Installer](#22-run-the-installer)
    - [2.3. Welcome Screen](#23-welcome-screen)
    - [2.4. Select the Installation Folder](#24-select-the-installation-folder)
    - [2.5. Installation Confirmation](#25-installation-confirmation)
    - [2.6. Installation Progress](#26-installation-progress)
    - [2.7. Installation Complete](#27-installation-complete)
  - [3. First Launch and Configuration](#3-first-launch-and-configuration)
    - [3.1. Access the Studio](#31-access-the-studio)
    - [3.2. Verify the Installed Version](#32-verify-the-installed-version)
  - [4. Connect to Your Database](#4-connect-to-your-database)
  - [5. Generate C# or VB.NET Classes](#5-generate-c-or-vbnet-classes)
    - [5.1. Open the Templates Panel](#51-open-the-templates-panel)
    - [5.2. Select the "Generated - Classes Master" Template](#52-select-the-generated---classes-master-template)
    - [5.3. Configure Generation Parameters](#53-configure-generation-parameters)
    - [5.4. Run the Generation](#54-run-the-generation)
    - [5.5. Verify the Generated Code](#55-verify-the-generated-code)
  - [6. Generate Custom Classes (Generated Once)](#6-generate-custom-classes-generated-once)
    - [6.1. Run the "Custom - Classes Master" Template](#61-run-the-custom---classes-master-template)
    - [6.2. Review the Generated Structure](#62-review-the-generated-structure)
    - [6.3. Explore the Custom Class Code](#63-explore-the-custom-class-code)
  - [7. Folder Structure in Visual Studio](#7-folder-structure-in-visual-studio)
  - [8. Unit Testing with EntitySpaces and PostgreSQL](#8-unit-testing-with-entityspaces-and-postgresql)
    - [8.1. Configure the Loader and Connection in Your Test](#81-configure-the-loader-and-connection-in-your-test)
    - [8.2. Load and Query Tests](#82-load-and-query-tests)
    - [8.3. Run the Tests](#83-run-the-tests)
  - [9. Add NuGet Packages to Your Project](#9-add-nuget-packages-to-your-project)
  - [10. Configure the Loader and Connection in Your Code](#10-configure-the-loader-and-connection-in-your-code)
  - [11. Write Your First Query](#11-write-your-first-query)
  - [12. Using with Wisej.NET](#12-using-with-wisejnet)
  - [13. Common Troubleshooting](#13-common-troubleshooting)
  - [14. Additional Resources](#14-additional-resources)
    - [Ready! With these steps, your environment is up and running, and you can start developing with EntitySpaces 2026.](#ready-with-these-steps-your-environment-is-up-and-running-and-you-can-start-developing-with-entityspaces-2026)

---

## 🚀 What's New in EntitySpaces Studio 2026

This version of the Studio includes important maintenance and performance improvements:

- **Metadata Engine Modernization** – Advanced schema properties are now extracted (`CharacterMaxLength`, `NumericPrecision/Scale`, `IsComputed`, `IsConcurrency`, `IsAutoIncrement`, `HasDefault` and `DataTypeNameComplete`) for all providers (SQL Server, PostgreSQL, MySQL, SQLite, Oracle and Firebird). The generated code accurately reflects your database structure.
- **Fixed x86 Architecture** – The `StandAlone.exe` executable is always compiled for x86, ensuring compatibility with native assemblies (`SQLite.Interop.dll`, ODP.NET). No more architecture errors when opening connections.

---

## 1. Requirements

- **.NET Framework 4.8** or **.NET 8/9/10** (depending on your project).
- **Visual Studio 2022** or later with Desktop/Web development workloads.
- **EntitySpaces Studio 2026** (standalone code generator).
- Administrator privileges for installation (optional but recommended).

---

## 2. Step-by-Step Installation

### 2.1. Download the Installer

1. Go to the GitHub repository:  
   [https://github.com/paulcordova/EntitySpaces](https://github.com/paulcordova/EntitySpaces)  
   and download the `EntitySpacesStudio_2026.7.0030.0.exe` file (or the latest version) from the **Releases** section.

2. Save the file to your computer (e.g., `Downloads`).

### 2.2. Run the Installer

![Installer - Run](images/02-Studio-Setup-Run.png)

Double-click the downloaded `.exe` file. If Windows shows a security warning, click **"Run anyway"**.

### 2.3. Welcome Screen

![Installer - Welcome](images/03-Studio-Setup-Step01.png)

The installation wizard will open. Click **"Next"** to continue.

### 2.4. Select the Installation Folder

![Installer - Installation folder](images/04-Studio-Setup-Step01-Installfolder.png)

By default, the installer suggests `C:\Program Files (x86)\EntitySpaces 2026\`. You can change it if you wish.  
Click **"Next"** after choosing the location.

### 2.5. Installation Confirmation

![Installer - Confirmation](images/04-Studio-Setup-Step01-InstallConfirmation.png)

Review the destination folder and the components to be installed. Click **"Install"** to begin copying files.

### 2.6. Installation Progress

![Installer - Progress](images/05-Studio-Setup-Step01-InstallRunning.png)

Wait while the installer copies the files and registers the necessary components.

### 2.7. Installation Complete

![Installer - Complete](images/06-Studio-Setup-Step01-InstallComplete.png)

Once the installation is finished, check **"Launch EntitySpaces Studio"** if you want to open the Studio immediately.  
Click **"Finish"** to close the wizard.

---

## 3. First Launch and Configuration

### 3.1. Access the Studio

![Studio - Shortcut](images/07-Studio-Run-Step02-DirectAccess.png)

You can open the Studio from the shortcut created on the **Desktop** or from the **Start Menu** → **EntitySpaces 2026**.

> **⚠️ Important:** Run the Studio **as administrator** the first time (right-click → "Run as administrator"). This allows the Studio to create the configuration folders in `C:\ProgramData\EntitySpaces\ES2026` with the correct permissions for all users.

### 3.2. Verify the Installed Version

![Studio - Verify version](images/08-Studio-Run-Step02-CkechVersion.png)

In the main window, go to **Help → About...** and confirm that the version is the expected one (e.g., `2026.7.0030.0`). This ensures the installation was successful.

---

## 4. Connect to Your Database

> ⚠️ **Important:** This section is a prerequisite for code generation; you need an active connection so the Studio can read the schema.

1. Go to the **Settings → Connection** tab.
2. Select the **Driver** corresponding to your database engine (SQL Server, PostgreSQL, MySQL, SQLite, Oracle, Firebird).
3. Click **Configure Connection** to build the connection string, or type it manually.
4. Test the connection with the **Test Connection** button.

![Studio - Configure SQL Server Local connection](images/09-Studio-Run-Step02-SettingsSqlServerLocal.png)

*Example: Connection setup for SQL Server Local.*

**Sample connection strings:**

| Engine | Example string |
|--------|-----------------|
| SQL Server | `User ID=sa;Password=...;Initial Catalog=Northwind;Data Source=localhost;TrustServerCertificate=True;` |
| PostgreSQL | `Host=localhost;Port=5432;Database=Northwind;Username=postgres;Password=...;SSL Mode=Require;` |
| MySQL | `Server=localhost;Port=3306;Database=Northwind;Uid=root;Pwd=...;SslMode=Required;` |
| SQLite | `Data Source=C:\Northwind.db3;Version=3;Foreign Keys=True;` |
| Oracle | `Data Source=...;User Id=...;Password=...;` (plus wallet configuration for ATP) |

---

## 5. Generate C# or VB.NET Classes

### 5.1. Open the Templates Panel

In the left panel of the Studio, expand your project tree (or create a new one in **File → New Project**).  
Right-click on the folder where you want to generate the code and select **"Add Recorded Template"** or **"Execute Template"**.

![Studio - Templates context menu](images/10-Studio-Code-Step03-ContextMenu-Template.png)

### 5.2. Select the "Generated - Classes Master" Template

The template selection dialog will appear. Navigate to the **C#** (or VB) folder and choose **"Generated - Classes Master"**. Click **"OK"**.

### 5.3. Configure Generation Parameters

In the configuration window:

- Define the **Namespace** (e.g., `BusinessObjects`).
- In the **"Input"** tab, select the tables or views you want to generate.
- You can use the **"Select All"** button to choose all of them.
- In the **"Advanced"** tab, review options such as `Generate a Single File` (recommended) and `Metadata Class Should Ignore Schema` (to avoid including the schema name in properties).

![Studio - Table selection](images/12-Studio-Code-Step03-DbTablesToGenerate.png)

### 5.4. Run the Generation

Click **"OK"**. The Studio will generate the files in the specified output folder (by default, a `Generated` subfolder within your project folder).  
You will see the progress at the bottom of the window.

![Studio - Run generation](images/13-Studio-Code-Step03-ContextMenu-ExceuteGenerationCode.png)

### 5.5. Verify the Generated Code

In less than a minute, you will have all the files ready. You can open them with your preferred editor or directly from File Explorer.

![Studio - Code generated in less than 1 minute](images/14-Studio-Code-Step03-CodeGenerated-InLessThanOneMinute.png)

---

## 6. Generate Custom Classes (Generated Once)

The **Custom** classes are generated only once and are the place where you add your business logic, validations, or additional methods. **They are not overwritten** when regenerating.

### 6.1. Run the "Custom - Classes Master" Template

Follow the same process as in the previous step, but select the **"Custom - Classes Master"** template.

![Studio - Custom Classes](images/16-Studio-Code-Step04-CodeGenerated-CustomClassGenerateOnlyOnce.png)

### 6.2. Review the Generated Structure

For each table, three classes are generated:

- **`{TableName}`** – the entity (Custom).
- **`{TableName}Collection`** – the collection (Custom).
- **`{TableName}Query`** – the fluent query (Custom).

![Studio - Three classes per table](images/17-Studio-Code-Step04-CodeGenerated-ThreeBussinessClassForEachTable-Entity-Collection-Query.png)

### 6.3. Explore the Custom Class Code

You can freely open and modify these classes to add calculated properties, business methods, or overrides of `Save()`.

![Studio - Custom Entity](images/18-Studio-Code-Step04-CodeGenerated-Checking-Custom-Entity.png)

![Studio - Custom Collection](images/19-Studio-Code-Step04-CodeGenerated-Checking-Custom-Collection.png)

![Studio - Custom Query](images/20-Studio-Code-Step04-CodeGenerated-Checking-Custom-Query.png)

---

## 7. Folder Structure in Visual Studio

Once the files are generated, add them to your Visual Studio project. The recommended structure is to separate the `Generated` classes (which are regenerated) from the `Custom` classes (which contain your logic).

![Visual Studio - Folder structure](images/21-Studio-Code-Step05-VisualStudio-SampleFolderStructureForCustomAndGeneratedClasses.png)

- **`Generated` folder**: Contains automatically generated files. **Do not edit them** (they are overwritten on regeneration).
- **`Custom` folder**: Contains your business classes. **This is where you work**.

---

## 8. Unit Testing with EntitySpaces and PostgreSQL

To verify that your configuration and classes work correctly, you can write simple unit tests.

### 8.1. Configure the Loader and Connection in Your Test

```csharp
[SetUp]
public void Setup()
{
    esProviderFactory.Factory = new EntitySpaces.LoaderMT.esDataProviderFactory();
    esConnectionElement conn = new esConnectionElement();
    conn.Provider = "EntitySpaces.PostgreSQLProvider";
    conn.ConnectionString = "Host=localhost;Port=5432;Database=Northwind;Username=postgres;Password=...;SSL Mode=Require;";
    esConfigSettings.ConnectionInfo.Connections.Add(conn);
    esConfigSettings.ConnectionInfo.Default = "Northwind";
}
```

### 8.2. Load and Query Tests

```csharp
[Test]
public void CanLoadAllCustomers()
{
    var customers = new CustomersCollection();
    Assert.IsTrue(customers.LoadAll());
    Assert.Greater(customers.Count, 0);
}

[Test]
public void CanQueryCustomersByCountry()
{
    var coll = new CustomersQuery("c", out var c)
        .Where(c.Country == "USA")
        .ToCollection<CustomersCollection>();
    Assert.Greater(coll.Count, 0);
}
```

![Visual Studio - Unit tests](images/22-Studio-Code-Step05-VisualStudio-SampleTestingCaseUsingEntityClassesIntoCode.png)

### 8.3. Run the Tests

Make sure the PostgreSQL provider is correctly referenced (NuGet package `EntitySpaces.ORM.PostgreSQL.NET`).

![Visual Studio - Tests with PostgreSQL](images/23-Studio-Code-Step05-VisualStudio-SampleTestingCaseForPostgreProvider.png)

If everything is correct, the tests will run successfully.

![Visual Studio - Tests successful](images/24-Studio-Code-Step05-VisualStudio-SampleTestingCaseExecutedSuccesfully.png)

---

## 9. Add NuGet Packages to Your Project

Open your project in Visual Studio and add the package for the provider you need.

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

# For Firebird (coming soon)
Install-Package EntitySpaces.ORM.Firebird.NET
```

These packages include all necessary dependencies (`EntitySpaces.Core`, `EntitySpaces.Interfaces`, `EntitySpaces.DynamicQuery`, the Loader, and the specific provider).

---

## 10. Configure the Loader and Connection in Your Code

EntitySpaces requires you to register the **Loader** (medium trust or standard) and the **connection string**.

**At the start of your application (e.g., `Program.Main`, `Global.asax`, or the main form constructor):**

```csharp
using EntitySpaces.Interfaces;

// 1. Register the Loader (medium trust version, recommended)
esProviderFactory.Factory = new EntitySpaces.LoaderMT.esDataProviderFactory();

// 2. Configure the connection
esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SqlClientProvider";   // Change according to your provider
conn.ConnectionString = "User ID=...;Password=...;Initial Catalog=Northwind;Data Source=localhost;";
conn.SqlAccessType = esSqlAccessType.DynamicSQL;    // or StoredProcedure
conn.DatabaseVersion = "2016";                      // Optional, auto-detected

// 3. Add the connection and set it as default
esConfigSettings.ConnectionInfo.Connections.Add(conn);
esConfigSettings.ConnectionInfo.Default = "Northwind"; // name of your choice
```

**Alternative:** use an `app.config` / `web.config` file with the `<EntitySpaces>` section. The complete guide is available in the repository's README.

---

## 11. Write Your First Query

With the generated classes and configuration ready, you can load data:

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

// Query with filter (fluent API)
CustomersCollection coll = new CustomersQuery("c", out var c)
    .Where(c.Country == "USA")
    .OrderBy(c.CompanyName.Ascending)
    .ToCollection<CustomersCollection>();

// Basic CRUD
Customers newCust = new Customers();
newCust.CustomerID = "NEWCO";
newCust.CompanyName = "New Company";
newCust.Save();                     // INSERT

newCust.CompanyName = "New Company Inc.";
newCust.Save();                     // UPDATE

newCust.MarkAsDeleted();
newCust.Save();                     // DELETE
```

---

## 12. Using with Wisej.NET

[**Wisej.NET**](https://wisej.com) is a framework that lets you build web applications with the same logic and event model as Windows Forms. EntitySpaces integrates seamlessly:

- You can reuse all your generated classes and existing business logic.
- The connection and transaction patterns are identical to those of a desktop application.
- The ORM correctly handles pooling and concurrency in the web environment (thanks to thread‑safe parameter caching and transaction handling in `finally` blocks).

**Example in a Wisej control:**

```csharp
// In a button Click event
private void buttonLoad_Click(object sender, EventArgs e)
{
    var products = new ProductsCollection();
    products.Query.Where(products.Query.UnitPrice > 10);
    products.Query.Load();

    dataGridView1.DataSource = products;
}
```

Refer to the [official Wisej.NET documentation](https://wisej.com/documentation/) for more details on deployment.

---

## 13. Common Troubleshooting

| Problem | Solution |
|---------|----------|
| **"Assembly EntitySpaces.Core not found"** | Make sure you installed the correct NuGet package and that it is referenced in your project. |
| **"Error connecting to SQLite"** | Verify that the connection string includes `Version=3;Foreign Keys=True;` and that the `.db3` file exists. |
| **Studio does not see tables** | In the Studio, ensure you have selected the correct catalog/database in the dropdown after connecting. |
| **Concurrency exception when saving** | EntitySpaces throws `esConcurrencyException` for key violations, deadlocks, etc. Catch it and handle the conflict according to your business logic. |
| **Permission error in `ProgramData\EntitySpaces\ES2026`** | Make sure to run the Studio as administrator the first time, or grant write permissions to the folder for your user (or the "Authenticated Users" group). |
| **Studio does not save the MRU project list** | Verify that the registry key `HKEY_CURRENT_USER\Software\EntitySpaces 2026` exists and that your user has write permissions. If it does not exist, run the Studio as administrator once to create it. |

---

## 14. Additional Resources

- **GitHub Repository:** [https://github.com/paulcordova/EntitySpaces](https://github.com/paulcordova/EntitySpaces)  
  - Source code, full examples, change history.
- **Extended documentation:** The `README.md` file in the repository contains all technical details, including metadata migration, providers, and best practices.
- **Forum / Issues:** Use GitHub Issues to report problems or suggest improvements.
- **Consulting:** Contact `paul.cordova@netstep.cl` for professional support or training.

---

### Ready! With these steps, your environment is up and running, and you can start developing with EntitySpaces 2026.
