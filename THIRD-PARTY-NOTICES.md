# Third-Party Notices

EntitySpaces itself is licensed under BSD 3-Clause (see `LICENSE`). This file lists the third-party
components it depends on or redistributes, under which terms, and what each one obliges us to do.

Third-party binaries are **redistributed deliberately**. The alternative — shipping without the
database drivers and the Microsoft interop assemblies — would force every user to source and build
those dependencies themselves, which defeats the purpose of the packages and the installer. Each
component below is redistributed **unmodified**, under its own license, with the obligations that
license imposes.

---

## 1. Runtime dependencies — NuGet packages

Declared as package dependencies; NuGet resolves them on the consumer's machine. They are **not**
bundled inside the `.nupkg`.

| Component | Version | License | Obligation |
|---|---|---|---|
| Microsoft.Data.SqlClient | 7.1.1 | MIT | Retain copyright and permission notice |
| Npgsql | 10.0.3 | PostgreSQL License | Retain copyright and permission notice |
| MySqlConnector | 2.6.2 | MIT | Retain copyright and permission notice |
| Oracle.ManagedDataAccess(.Core) | 23.26.400 | Oracle Free Distribution, Hosting, and Use Terms and Conditions | Ship the Oracle text with the distribution; do not modify the assemblies |
| System.Data.SQLite.Core | 1.0.119 | Public domain | None |
| FirebirdSql.Data.FirebirdClient | 10.3.4 | Initial Developer's Public License (IDPL) 1.0 | See §4 |
| System.Configuration.ConfigurationManager | 10.0.14 | MIT | Retain notice |
| System.Security.Cryptography.Pkcs | 10.0.14 | MIT | Retain notice |

## 2. Components redistributed inside the Studio MSI

The installer ships these binaries in its output directory. Their licenses apply to those files
regardless of the BSD 3-Clause license of EntitySpaces.

| Component | License | Obligation |
|---|---|---|
| Microsoft.Data.SqlClient + SNI | MIT | Retain notice |
| Npgsql | PostgreSQL License | Retain notice |
| MySqlConnector | MIT | Retain notice |
| Oracle.ManagedDataAccess | Oracle Free Use Terms and Conditions | Include the Oracle license text (§3) |
| System.Data.SQLite + `SQLite.Interop.dll` | Public domain | None |
| FirebirdSql.Data.FirebirdClient | IDPL 1.0 | Retain notice; see §4 |
| Microsoft.CodeAnalysis / Roslyn | MIT | Retain notice |
| Krypton.Toolkit | BSD 3-Clause | Retain notice |
| WeifenLuo.WinFormsUI.Docking (DockPanel Suite) | MIT | Retain notice |
| System.Data.OleDb, System.Management, System.DirectoryServices.Protocols | MIT | .NET runtime components |
| Microsoft.Extensions.* | MIT | Retain notice |

## 3. Oracle driver

`Oracle.ManagedDataAccess` is redistributed inside the Studio MSI, unmodified. Oracle's terms permit
redistribution of the unmodified assemblies **provided the Oracle license terms accompany the
distribution**.

That obligation is met: the installer places `THIRD-PARTY-NOTICES.txt` in its `Documentation`
folder, which reproduces verbatim the license text published with the NuGet package
`Oracle.ManagedDataAccess 23.26.400`.

The `Oracle.ManagedDataAccess.Core` package, used by the NuGet provider, is resolved by NuGet on the
consumer's machine and is not redistributed by us.

Two rules follow from the Oracle terms:

1. Never modify, repack, rename or strip the Oracle assemblies.
2. Keep the Oracle text in the installer whenever the Oracle driver is included.

## 4. Firebird client — copyleft by file

`FirebirdSql.Data.FirebirdClient` is distributed under the **IDPL 1.0**, a file-level copyleft
license. This matters for a corporate user who embeds the driver:

- The driver is consumed as a package reference or as an unmodified binary. That is permitted.
- Modifications to the driver's own source must be published under the IDPL.
- EntitySpaces code that merely *calls* the driver is not a derivative work of it.

This is stated here so that a customer's legal review does not have to discover it mid-project.

## 5. Microsoft interop assemblies

The Studio MSI includes assemblies generated from Microsoft type libraries:

| File | Origin |
|---|---|
| `Interop.ADOX.dll` | Microsoft ADO Extensions for DDL and Security |
| `Interop.MSDASC.dll` | Microsoft Data Access Components |
| `Microsoft.VisualStudio.Interop.dll` | Visual Studio extensibility |
| `Microsoft.VisualStudio.Imaging.Interop.14.0.DesignTime.dll` | Visual Studio imaging |
| `stdole.dll` | OLE automation |
| `Microsoft.Windows.SDK.NET.dll`, `WinRT.Runtime.dll` | Windows SDK projections |

They are kept because the Studio depends on them at runtime: the metadata engine uses the ADO/OLEDB
stack, and the UI uses VS imaging types for its iconography. Removing them would leave the product
unable to connect to the databases it lists, which is not a trade-off we are willing to make for a
packaging preference.

Redistribution is **unmodified**, in the form these assemblies are produced by the Microsoft tools
and SDKs that generate them. No Microsoft code is patched, repackaged, or renamed.

## 6. Build-time only — not redistributed

| Component | License | Notes |
|---|---|---|
| WiX Toolset (via WixSharp) | Version-dependent: MS-RL for v4, OSMF EULA for v5+ | Used to build the MSI; not shipped inside it. Confirm the license of the exact version in use before publishing the installer. |
| Microsoft.CodeAnalysis (compiler, build time) | MIT | |

## 7. Keeping this file current

- The dependency table in §1 mirrors `Directory.Packages.props`. Adding a package means adding a row.
- The list in §2 mirrors the published output of the Studio installer. Adding a driver means adding a
  row and, if its license requires it, shipping its text.
- The machine-readable artifact of this file is `Studio/Documentation/THIRD-PARTY-NOTICES.txt`, which
  is what the installer ships.

A practical way to keep §1 honest is to generate it from the package manifest at release time rather
than editing it by hand.
