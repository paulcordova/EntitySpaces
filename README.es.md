<img src="https://es-banner.paul-netstep.workers.dev?src=github" alt="EntitySpaces" width="250" height="126">

# EntitySpaces ORM — .NET 4.8 a .NET 10

#### [Read this in English](README.md)

ORM .NET modernizado y mantenido activamente — seguro en concurrencia, seguro en el pool de
conexiones y preparado para bases de datos en la nube sobre SQL Server, PostgreSQL, MySQL,
SQLite y Oracle.

El código generado desde tu base de datos y tu lógica de negocio viven en clases separadas y
aisladas: puedes regenerar el esquema cuando quieras sin tocar nunca una línea de tu propio código.
Una arquitectura ligera pensada para equipos de una o dos personas que prefieren concentrarse en
lo que de verdad aporta valor.

La misma arquitectura que creó Mike Griffin, extendida y endurecida para .NET Framework 4.8
hasta .NET 10.

> Esta traducción es un **espejo del README principal**. Si hay diferencias, prevalece la versión
> en inglés: [README.md](README.md).

## 🛠️ Requiere EntitySpaces Studio

EntitySpaces genera código directamente desde el esquema de tu base de datos mediante una
herramienta local basada en plantillas llamada **EntitySpaces Studio**. Solo necesitas ejecutarla
la primera vez y cada vez que cambie una tabla.

Studio genera dos tipos de clases: las **Generated**, que se recrean en cada ejecución, y las
**Custom**, donde vive tu lógica de negocio. Las Custom se crean vacías la primera vez y nunca se
tocan en regeneraciones posteriores, así que volver a generar siempre es seguro y tu código queda
intacto.

Usa siempre la versión más reciente de **EntitySpaces Studio**.

- Descárgala del repositorio: [EntitySpaces.Studio](https://github.com/paulcordova/EntitySpaces/tree/master/EntitySpaces.Studio)
- Baja el `.zip` **más reciente** — evita cualquier archivo marcado `-Deprecated`, que se conserva
  solo como referencia.
- Conéctate a tu base de datos en *Settings → Connection* y ejecuta las plantillas de clases
  Generated y Custom.

> **Studio 2026.11 requiere .NET 10 Desktop Runtime y Windows x64** (Windows 10 2004 o superior).
> La versión 2026.08 de Studio funcionaba sobre .NET Framework 4.8.1; el cambio de plataforma es
> intencional y no hay binario alternativo.

## Plataformas soportadas

| Versión | Estado |
|---------|--------|
| .NET Framework 4.8 | ✅ Activo |
| .NET 8 | ✅ Activo |
| .NET 9 | ✅ Activo |
| .NET 10 | ✅ Activo |
| .NET Standard / .NET 6 / .NET 7 | ❌ Fuera del conjunto de destino de los paquetes ORM |

Los paquetes ORM se compilan para `net48;net8.0;net9.0;net10.0`.

## Disponible en NuGet

| Proveedor | Paquete |
|----------|---------|
| SQL Server | [EntitySpaces.ORM.SqlServer.NET](https://www.nuget.org/packages/EntitySpaces.ORM.SqlServer.NET/) |
| PostgreSQL | [EntitySpaces.ORM.PostgreSQL.NET](https://www.nuget.org/packages/EntitySpaces.ORM.PostgreSQL.NET/) |
| MySQL | [EntitySpaces.ORM.MySQL.NET](https://www.nuget.org/packages/EntitySpaces.ORM.MySQL.NET/) |
| SQLite | [EntitySpaces.ORM.SQLite.NET](https://www.nuget.org/packages/EntitySpaces.ORM.SQLite.NET/) |
| Oracle Managed Client | [EntitySpaces.ORM.OracleManagedClient.NET](https://www.nuget.org/packages/EntitySpaces.ORM.OracleManagedClient.NET/) |
| Firebird | 🚧 En preparación — no forma parte de la entrega 2026.11 |

```powershell
Install-Package EntitySpaces.ORM.SqlServer.NET
Install-Package EntitySpaces.ORM.PostgreSQL.NET
Install-Package EntitySpaces.ORM.MySQL.NET
Install-Package EntitySpaces.ORM.SQLite.NET
Install-Package EntitySpaces.ORM.OracleManagedClient.NET
```

El detalle de comportamiento de cada motor está en su propio README:
[SQL Server](README-SQLSERVER.md) · [PostgreSQL](README-POSTGRE.md) ·
[MySQL](README-MYSQL.md) · [SQLite](README-SQLITE.md) · [Oracle](README-ORACLE.md).

**Apoyo y comentarios** — [donación única vía PayPal](https://netstep.cl/entityspaces/support/) · [encuesta de 3 minutos](https://docs.google.com/forms/d/e/1FAIpQLSd-FVQiC3deoaIarYnsOCH4pdj-4zjGKznN68uUtyx9CpuKgA/viewform)

## Licencia

**BSD 3-Clause.** Libre para uso comercial y en software cerrado; conserva el aviso de copyright.

- [`LICENSE`](LICENSE) — el texto legal que rige
- [`NOTICE`](NOTICE) — procedencia, marcas, política de contribuciones y por qué el código que genera
  tu Studio es tuyo
- [`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md) — componentes incluidos y sus términos

El soporte comercial, la consultoría y la capacitación se contratan aparte: la licencia no incluye
ninguno de ellos y ninguno es necesario para usar el software.

## Inicio rápido

```csharp
// Configuración inicial
esProviderFactory.Factory = new EntitySpaces.Loader.esDataProviderFactory();

esConnectionElement conn = new esConnectionElement();
conn.Provider = "EntitySpaces.PostgreSQLClientProvider";
conn.Schema = "public";
conn.ConnectionString = "Host=miservidor;Port=5432;Database=mibd;Username=usuario;Password=clave;";
esConfigSettings.ConnectionInfo.Connections.Add(conn);

// Entidad
Users user = new Users();
user.LoadByPrimaryKey(1);
user.Name = "Nuevo nombre";
user.Save();

// Consulta con API fluida
UsersCollection users = new UsersCollection()
    .Where(u => u.Active == true)
    .OrderBy(u => u.Name)
    .Load();
```

## Gestión de transacciones

EntitySpaces incluye su propio ámbito transaccional — `esTransactionScope` — y es el mecanismo
recomendado en **todos** los proveedores:

```csharp
using (var scope = new esTransactionScope())
{
    // ... varias operaciones Save() / Load() ...
    scope.Complete();   // omitir para hacer rollback
}
```

**No uses `System.Transactions.TransactionScope` para envolver operaciones de EntitySpaces.** Los
drivers modernos (Npgsql 6+, Microsoft.Data.SqlClient 5+, MySqlConnector) ya no enlistan las
conexiones en una transacción ambiente automáticamente: si envuelves llamadas de EntitySpaces en
un `TransactionScope`, las conexiones se abren en autocommit y el rollback **no** revertirá tus
datos.

## Por qué elegir EntitySpaces

EntitySpaces está pensado para equipos que:

- Trabajan con esquemas relacionales complejos
- Necesitan control total sobre el SQL generado
- Prefieren validación en tiempo de compilación antes que abstracción en tiempo de ejecución
- Mantienen sistemas empresariales de larga vida

A diferencia de los ORM con abstracciones pesadas que intentan ocultar la complejidad del SQL,
EntitySpaces trata el SQL como un ciudadano de primera clase. El resultado es generación de SQL
predecible, alto rendimiento, construcción explícita de consultas, depuración sencilla y evolución
del esquema más segura.

Si tu equipo entiende SQL, ya entiende EntitySpaces.

## Sobre este fork

Soy Paul Córdova, usuario de EntitySpaces desde hace años y actual mantenedor de este fork para
.NET moderno, responsable de la alineación multiproveedor y de la compatibilidad entre versiones
de .NET.

EntitySpaces fue originalmente un ORM comercial ampliamente adoptado en sistemas empresariales. Su
arquitectura sigue siendo sólida, sobre todo para equipos que prefieren control explícito del SQL,
trabajan con modelos relacionales complejos, necesitan generación de consultas predecible y
valoran la seguridad en tiempo de compilación.

El objetivo no es rediseñar EntitySpaces, sino preservar y evolucionar su arquitectura probada
para los ecosistemas .NET actuales.

## Enlaces

- [Historial de cambios](CHANGELOG.md)
- [Referencia de la API fluida](README-API-REFERENCE.md)
- [Licencia BSD 3-Clause](LICENSE)
- Sitio original del proyecto: [mikegriffinreborn.github.io/EntitySpaces](https://mikegriffinreborn.github.io/EntitySpaces/)
