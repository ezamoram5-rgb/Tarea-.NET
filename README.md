# Administración de clientes · API .NET 10 y Blazor

## Tarea Blazor

La solución incluye dos aplicaciones independientes: la API original y `ClientesBlazor`, una Blazor Web App con interactividad Server. Blazor consume `/api/clientes` mediante `HttpClient`; no referencia Entity Framework ni accede a la base de datos.

El módulo incluye listado con búsqueda, alta y edición en un modal Bootstrap, confirmación de eliminación, validación visual de campos y recarga del listado después de cada operación. Los errores de la API, incluido el CUI duplicado, se muestran en el modal sin perder los datos ingresados.

### Ejecutar las dos aplicaciones

Desde la raíz del repositorio, restaurar y compilar:

```powershell
dotnet restore ClientesAPI.slnx
dotnet build ClientesAPI.slnx
```

En una terminal, iniciar la API:

```powershell
dotnet run --project Programacion2ClientesAPI.csproj --launch-profile http
```

En una segunda terminal, iniciar Blazor:

```powershell
dotnet run --project ClientesBlazor/ClientesBlazor.csproj --launch-profile http
```

- Aplicación: [http://localhost:5170](http://localhost:5170).
- Swagger: [http://localhost:5169/swagger](http://localhost:5169/swagger).
- En Visual Studio, abrir `ClientesAPI.slnx` y configurar ambos proyectos como proyectos de inicio.

La URL de la API se configura en `ClientesBlazor/appsettings.json`, sección `Api:BaseUrl`; debe terminar en `/`. También puede configurarse con `Api__BaseUrl`. Las llamadas salen desde el servidor Blazor, por lo que este modo no requiere habilitar CORS. Mantener ambos procesos en ejecución durante la demostración.

### Bootstrap y validación

Bootstrap 5.3.8 se incluye localmente en `wwwroot/lib/bootstrap` con su aviso de licencia. Sus modales gestionan el fondo y el foco. `EditForm`, `DataAnnotationsValidator` y `BootstrapValidation` aplican `is-invalid` e `is-valid`, junto con mensajes junto a cada campo. Se validan campos obligatorios, longitudes, CUI de 13 dígitos, teléfono y fecha no futura. La API vuelve a validar los datos antes de persistirlos.

El modelo de Blazor es un contrato de entrada/salida independiente; la fecha es nullable en el formulario para permitir un campo inicialmente vacío. El envío exige una fecha válida.

### Archivos principales de Blazor

- `Components/Pages/Home.razor`: listado, formularios y operaciones.
- `Models/Cliente.cs`: contrato y validaciones del formulario.
- `Services/ClientesApi.cs`: peticiones HTTP y mensajes de error de la API.
- `Services/BootstrapValidation.cs`: estilos de validación.
- `wwwroot/js/clientes.js`: apertura y cierre de modales Bootstrap.

### Entrega

El código de ambas tareas se conserva en este repositorio. Si se revisa una rama mediante un pull request, integrar sus cambios en `main` antes de entregar al docente el enlace principal: https://github.com/ezamoram5-rgb/Tarea-.NET.

Las instrucciones de la API original se conservan a continuación.

# API REST de clientes

**Autor: Eutimio Zamora**

Solución para la tarea de administración de clientes, adaptada del proyecto base Programacion2ClientesAPI. Implementa una API basada en controladores de ASP.NET Core con .NET 10, Entity Framework Core 10 y una base de datos SQL SQLite persistente.

## Requisitos

- SDK de .NET 10.
- Internet para restaurar los paquetes NuGet la primera vez.
- PowerShell 7 únicamente si se ejecuta el script de verificación.

SQLite almacena los registros en `clientes.db`. No requiere instalar MySQL ni configurar credenciales. La base y la tabla se crean al iniciar mediante la migración de Entity Framework Core incluida en el proyecto.

## Ejecución

Abra una terminal dentro de la carpeta del proyecto:

```powershell
dotnet restore ClientesAPI.slnx
dotnet run --project Programacion2ClientesAPI.csproj --launch-profile http
```

Abra [Swagger](http://localhost:5169/swagger) para probar los endpoints. La API está en `http://localhost:5169/api/clientes`. Para detenerla, presione Ctrl+C en la terminal.

## Modelo Cliente

| Campo | Tipo | Validación |
| --- | --- | --- |
| Id_cliente | int | Clave primaria generada automáticamente |
| CUI | string | Obligatorio, único y de 13 dígitos |
| NIT | string | Obligatorio, máximo 15 caracteres |
| Nombres | string | Obligatorio, máximo 100 caracteres |
| Apellidos | string | Obligatorio, máximo 100 caracteres |
| Direccion | string | Obligatorio, máximo 250 caracteres |
| Telefono | string | Obligatorio, entre 7 y 20 caracteres; admite prefijo +, espacios, paréntesis y guiones |
| Fecha_Nacimiento | DateOnly | Obligatoria, formato AAAA-MM-DD, sin fechas futuras |

Los nombres JSON conservan la escritura de esta tabla. Dirección y teléfono se representan como `Direccion` y `Telefono`, sin tildes, igual que en el proyecto base. Los valores sí admiten tildes.

## Operaciones REST

| Método | Ruta | Resultado correcto | Errores esperados |
| --- | --- | --- | --- |
| GET | /api/clientes | 200, listado completo ordenado por ID | — |
| GET | /api/clientes/{id} | 200, cliente solicitado | 404 si no existe |
| POST | /api/clientes | 201, cliente y cabecera Location | 400 por datos inválidos; 409 por CUI duplicado |
| PUT | /api/clientes/{id} | 204, sin contenido | 400 por datos o ID distintos; 404 si no existe; 409 por CUI duplicado |
| DELETE | /api/clientes/{id} | 204, sin contenido | 404 si no existe |

POST genera el ID. PUT reemplaza todos los datos del cliente y exige `Id_cliente` igual al ID de la ruta. Las validaciones se ejecutan antes de guardar. El índice único de la base impide duplicar el CUI, incluso ante solicitudes simultáneas.

## Ejemplo de registro

En Swagger, seleccione POST, pulse **Try it out**, pegue este cuerpo y pulse **Execute**. Todos los datos del ejemplo son ficticios.

```json
{
  "CUI": "1000000000001",
  "NIT": "1234567-8",
  "Nombres": "Ana",
  "Apellidos": "Prueba",
  "Direccion": "Dirección ficticia de prueba",
  "Telefono": "55555555",
  "Fecha_Nacimiento": "2000-01-15"
}
```

Copie el `Id_cliente` devuelto. Consulte ese ID con GET; para actualizarlo con PUT, agregue `Id_cliente` al JSON, cambie los campos deseados y use el mismo ID en la ruta. Finalmente ejecute DELETE con ese ID y compruebe que GET devuelve 404.

## Migraciones y configuración

La conexión se define en `appsettings.json`, en `ConnectionStrings:DefaultConnection`. La ruta predeterminada del archivo es relativa al directorio de ejecución. Puede sustituirse mediante la variable de entorno `ConnectionStrings__DefaultConnection`.

Para aplicar las migraciones manualmente:

```powershell
dotnet tool restore
dotnet ef database update
```

Después de modificar el modelo, cree una migración y actualice la base:

```powershell
dotnet ef migrations add CambioClientes
dotnet ef database update
```

Las migraciones generadas corresponden a SQLite. Si se cambia de motor SQL, se debe cambiar el proveedor y generar las migraciones correspondientes.

## Verificación automática

```powershell
pwsh -File scripts/Verificar-API.ps1
```

El script compila en Release y levanta una instancia aislada con una base temporal. Verifica el CRUD, los códigos HTTP, las validaciones, el rechazo de duplicados, Swagger y la persistencia después de reiniciar. No modifica `clientes.db`. Puede elegir otro puerto con `-Puerto 5190`. Los registros y la base de prueba quedan en la carpeta temporal indicada al terminar.

## Estructura

```text
ClientesAPI/
  Controllers/ClientesController.cs
  Data/AppDbContext.cs
  Models/Cliente.cs
  Migrations/
  Properties/launchSettings.json
  scripts/Verificar-API.ps1
  .config/dotnet-tools.json
  .gitignore
  Program.cs
  Programacion2ClientesAPI.csproj
  appsettings.json
  README.md
```

## Entrega en GitHub

Cree un repositorio vacío en su cuenta y suba el contenido de esta carpeta, incluyendo `.config` y `.gitignore`. No suba `bin`, `obj`, bases de datos ni registros. El archivo `.gitignore` ya excluye esos elementos. Entregue al docente el enlace de la página principal del repositorio y asegúrese de que tenga permiso para verlo.

La configuración está preparada para una demostración académica local. Swagger se habilita en Development. No incluye autenticación ni autorización para exponer datos reales en Internet.

## Referencias

- [Introducción a Entity Framework Core y SQLite](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app).
- [Migraciones de Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/).
- [Video de apoyo proporcionado en el enunciado](https://www.youtube.com/watch?v=xa93DX4yyjg).
