![ES-logo](ES-Studio.jpg)
# EntitySpaces 2026 – Guía Rápida 
*Versión para desarrolladores que quieren empezar en menos de 15 minutos.*


## 1. Requisitos

- **.NET Framework 4.8** o **.NET 8/9/10** (según tu proyecto).
- **Visual Studio 2022** (o 2019) con cargas de desarrollo de escritorio/Web.
- **EntitySpaces Studio 2024** (generador de código standalone).


## 2. Descargar e instalar EntitySpaces Studio

1. Ve al repositorio GitHub:  
   [https://github.com/paulcordova/EntitySpaces](https://github.com/paulcordova/EntitySpaces)  
   y descarga el archivo `EntitySpacesStudio_2024.8.30.0.zip` (o la versión más reciente).
2. Extrae el contenido en una carpeta local (ej. `C:\EntitySpaces\Studio`).
3. Ejecuta `StandAlone.exe` como administrador (Windows puede pedir permisos).

> ✅ **Consejo:** El Studio es una aplicación WinForms que funciona sin Visual Studio. Puedes usarlo en cualquier máquina.

---

## 3. Conectar a tu base de datos

Una vez abierto el Studio:

1. Ve a la pestaña **Settings → Connection**.
2. Selecciona el **Driver** correspondiente a tu motor (SQL Server, PostgreSQL, MySQL, SQLite, Oracle, Firebird).
3. Haz clic en **Configure Connection** para construir la cadena de conexión, o escríbela manualmente.
4. Prueba la conexión con el botón **Test Connection**.

<!-- Captura sugerida: Pantalla de conexión del Studio (Settings → Connection) con el campo de cadena y el botón Test. Puedes tomar una captura de la ventana y sustituir este comentario por la imagen. -->

<div style="page-break-before:always">&nbsp;</div>
<p></p>

**Ejemplos de cadenas de conexión:**

| Motor | Cadena de ejemplo |
|-------|-------------------|
| SQL Server | `User ID=sa;Password=...;Initial Catalog=Northwind;Data Source=localhost;TrustServerCertificate=True;` |
| PostgreSQL | `Host=localhost;Port=5432;Database=Northwind;Username=postgres;Password=...;SSL Mode=Require;` |
| MySQL | `Server=localhost;Port=3306;Database=Northwind;Uid=root;Pwd=...;SslMode=Required;` |
| SQLite | `Data Source=C:\Northwind.db3;Version=3;Foreign Keys=True;` |
| Oracle | `Data Source=...;User Id=...;Password=...;` (más configuración de wallet para ATP) |


## 4. Generar las clases C# o VB.NET

1. Ve a la pestaña **Templates**.
2. Selecciona el idioma (C# o VB.NET) y la plantilla **"Generated Classes Master"**.
3. En la ventana que aparece:
   - Define el **Namespace** (ej. `BusinessObjects`).
   - Elige las tablas/vistas que quieras generar.
   - En la pestaña **Advanced**, revisa opciones como `Generate a Single File` (recomendado).
4. Haz clic en **OK**. El Studio generará los archivos en la carpeta que definiste (por defecto, subcarpetas `Generated` y `Custom`).
5. Repite el paso con la plantilla **"Custom Classes Master"** (se genera una sola vez; ahí pondrás tu lógica de negocio).

<!-- Captura sugerida: Diálogo de la plantilla Generated Classes Master, con la selección de tablas y el campo Namespace. -->

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 5. Agregar los paquetes NuGet a tu proyecto

Abre tu proyecto en Visual Studio y agrega el paquete del proveedor que necesites.

**Desde la consola de Package Manager:**

```powershell
# Ejemplo para SQL Server
Install-Package EntitySpaces.ORM.SqlServer.NET

# Para PostgreSQL
Install-Package EntitySpaces.ORM.PostgreSQL.NET

# Para MySQL
Install-Package EntitySpaces.ORM.MySQL.NET

# Para SQLite
Install-Package EntitySpaces.ORM.SQLite.NET

# Para Oracle Managed Client
Install-Package EntitySpaces.ORM.OracleManagedClient.NET

# Para Firebird
Install-Package EntitySpaces.ORM.Firebird.NET
```

Estos paquetes incluyen todas las dependencias necesarias (`EntitySpaces.Core`, `EntitySpaces.Interfaces`, `EntitySpaces.DynamicQuery`, el Loader y el proveedor específico).

---

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 6. Configurar el Loader y la conexión en tu código

EntitySpaces necesita que registres el **Loader** (medio confiable o estándar) y la **cadena de conexión**.

**En el inicio de tu aplicación (ej. `Program.Main`, `Global.asax`, o constructor del formulario principal):**

```csharp
using EntitySpaces.Interfaces;

// 1. Registrar el Loader (versión medio confiable, recomendada)
esProviderFactory.Factory = new EntitySpaces.LoaderMT.esDataProviderFactory();

// 2. Configurar la conexión
esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.SqlClientProvider";   // Cambia según tu proveedor
conn.ConnectionString = "User ID=...;Password=...;Initial Catalog=Northwind;Data Source=localhost;";
conn.SqlAccessType = esSqlAccessType.DynamicSQL;    // o StoredProcedure
conn.DatabaseVersion = "2016";                      // Opcional, se auto-detecta

// 3. Agregar la conexión y establecerla como predeterminada
esConfigSettings.ConnectionInfo.Connections.Add(conn);
esConfigSettings.ConnectionInfo.Default = "Northwind"; // nombre que le quieras dar
```

**Alternativa:** usar un archivo `app.config` / `web.config` con la sección `<EntitySpaces>`. La guía completa está en el README del repositorio.

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 7. Escribir tu primera consulta

Con las clases generadas y la configuración lista, puedes cargar datos:

```csharp
// Cargar todos los clientes
CustomersCollection customers = new CustomersCollection();
if (customers.LoadAll())
{
    foreach (Customers cust in customers)
    {
        Console.WriteLine($"{cust.CustomerID} - {cust.CompanyName}");
    }
}

// Consulta con filtro (API fluida)
CustomersCollection coll = new CustomersQuery("c", out var c)
    .Where(c.Country == "USA")
    .OrderBy(c.CompanyName.Ascending)
    .ToCollection<CustomersCollection>();

// CRUD básico
Customers newCust = new Customers();
newCust.CustomerID = "NEWCO";
newCust.CompanyName = "Nueva Empresa";
newCust.Save();                     // INSERT

newCust.CompanyName = "Nueva Empresa S.A.";
newCust.Save();                     // UPDATE

newCust.MarkAsDeleted();
newCust.Save();                     // DELETE
```

---

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 8. Uso con Wisej.NET

[**Wisej.NET**](https://wisej.com) es un framework que permite desarrollar aplicaciones web con la misma lógica y modelo de eventos que Windows Forms. EntitySpaces se integra perfectamente:

- Puedes reutilizar todas tus clases generadas y tu lógica de negocio existente.
- El patrón de conexión y transacciones es idéntico al de una aplicación de escritorio.
- El ORM maneja correctamente el *pooling* y la concurrencia en el entorno web (gracias al caché de parámetros thread‑safe y al manejo de transacciones en `finally`).

**Ejemplo en un control Wisej:**

```csharp
// En el evento Click de un botón
private void buttonLoad_Click(object sender, EventArgs e)
{
    var products = new ProductsCollection();
    products.Query.Where(products.Query.UnitPrice > 10);
    products.Query.Load();

    dataGridView1.DataSource = products;
}
```

Puedes consultar la [documentación oficial de Wisej.NET](https://wisej.com/documentation/) para más detalles sobre el despliegue.

---

<div style="page-break-before:always">&nbsp;</div>
<p></p>

## 9. Solución de problemas comunes

| Problema | Solución |
|----------|----------|
| **"No se encuentra el ensamblado EntitySpaces.Core"** | Asegúrate de haber instalado el paquete NuGet correcto y que esté referenciado en el proyecto. |
| **"Error al conectar con SQLite"** | Verifica que la cadena incluya `Version=3;Foreign Keys=True;` y que el archivo .db3 exista. |
| **El Studio no ve las tablas** | En el Studio, asegúrate de haber seleccionado el catálogo/Base de datos correcto en el desplegable después de conectar. |
| **Error de arquitectura al ejecutar el Studio** | El Studio está compilado para **x86**; no lo ejecutes en modo AnyCPU. Si usas dependencias nativas (SQLite, Oracle), descarga la versión x86. |
| **Excepción de concurrencia al guardar** | EntitySpaces lanza `esConcurrencyException` para violaciones de clave, deadlocks, etc. Captúrala y maneja el conflicto según tu lógica de negocio. |

---

## 10. Recursos adicionales

- **Repositorio GitHub:** [https://github.com/paulcordova/EntitySpaces](https://github.com/paulcordova/EntitySpaces)  
  - Código fuente, ejemplos completos, historial de cambios.
- **Documentación extendida:** Dentro del repositorio encontrarás el `README.md` con todos los detalles técnicos, incluyendo la migración de metadatos, proveedores y mejores prácticas.
- **Foro / Issues:** Usa GitHub Issues para reportar problemas o sugerir mejoras.
- **Consultoría:** Contacta a `paul.cordova@netstep.cl` para soporte profesional o formación.



¡Listo! Con estos pasos ya tienes tu entorno funcionando y puedes empezar a desarrollar con EntitySpaces 2027.

*Última actualización: Julio 2026 – Compatible con .NET 10.*

---
