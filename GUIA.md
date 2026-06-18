# Guía para trabajar con el código

Esta guía explica **cómo está organizado el proyecto y cómo operar sobre él**. Para
levantarlo (Docker, API, front) está el [README.md](README.md); acá nos centramos en la
estructura y en cómo implementar la lógica que hoy es esqueleto.

---

## Idea general

El backend está dividido en **capas** con una regla simple: cada capa solo conoce a la de
abajo. Una petición HTTP viaja así:

```
Front (Blazor)  ->  Api (Controller)  ->  Application (Service)  ->  Infrastructure (Repository)  ->  MySQL
                         DTO  <----------------  DTO   <-----  mapea -----  Domain (entidad)
```

- El **Controller** recibe el request, llama al **Service** y devuelve la respuesta. Es delgado.
- El **Service** orquesta la lógica de negocio y usa uno o más **Repositories**.
- El **Repository** habla con la base por **ADO.NET/MySql.Data** y devuelve **entidades de dominio**.
- Los **DTOs** (Contracts) son lo que entra y sale por HTTP; el **Domain** es interno.

> **Status actual:** Funcionalidades de **Equipos**, **Estadios** y **Eventos** están completamente implementadas
> con CRUD completo. Las demás entidades aún lanzan `NotImplementedException`.

---

## Qué hace cada proyecto

| Proyecto | Rol | Depende de |
|----------|-----|------------|
| **Ticketing.Domain** | Entidades de dominio (POCOs) que reflejan las tablas del esquema. Sin lógica ni dependencias. | — |
| **Ticketing.Contracts** | DTOs de request/response (records) que se comparten entre API y Front. | — |
| **Ticketing.Application** | Interfaces de repositorios (`Abstractions/`) e interfaces + implementación de los services (`Services/`). Acá va la **lógica de negocio**. | Domain, Contracts |
| **Ticketing.Infrastructure** | Implementación de los repositorios con ADO.NET/**MySql.Data** y la fábrica de conexión (`Persistence/`). Acá va el **SQL**. | Application |
| **Ticketing.Api** | Web API: controllers, Swagger, CORS y el wiring de DI (`Program.cs`). | Application, Infrastructure, Contracts |
| **Ticketing.Front** | Blazor WebAssembly: clientes HTTP (`ApiClients/`) y páginas (`Pages/`). | Contracts |

Registro de dependencias (DI):
- `AddApplicationServices()` en `Application/DependencyInjection.cs` → registra los services.
- `AddInfrastructure()` en `Infrastructure/DependencyInjection.cs` → registra la conexión y los repositorios.
- Ambos se invocan en `Api/Program.cs`.

---

## Convenciones

- **C#** en `PascalCase`; **MySQL** en `snake_case`. El mapeo
  `columna_snake → PropiedadPascal` se hace **a mano** al leer el `MySqlDataReader`.
- Todo método que toca la base es **async** y recibe `CancellationToken ct = default`.
- **Siempre con parámetros** (`MySqlParameter` / `AddWithValue`), nunca concatenando
  strings (anti SQL injection).
- **Prohibido** Entity Framework, Dapper o cualquier ORM. Solo `MySql.Data` (driver ADO.NET).
- Cadena de conexión en `appsettings.json` (`ConnectionStrings:MySQL`).
- **MySQL TIME:** Se almacena como TimeSpan y se convierte a TimeOnly con `TimeOnly.FromTimeSpan()` o `TimeOnly.Parse()`.

---

## El molde: cómo está hecho "Equipo" (implementado ✅)

Cada área sigue el mismo molde. **Equipo, Estadio y Evento ya están completamente implementados**. Tomá **Evento** como referencia para implementar nuevas entidades:

1. **Domain** — `Ticketing.Domain/EventoDeportivo.cs` (POCO).
2. **Contracts** — `Ticketing.Contracts/Eventos/EventoDtos.cs` (Response, Crear, Actualizar).
3. **Application**
   - Interfaz de repo: `Abstractions/IEventoRepository.cs`.
   - Service: `Services/EventoService.cs` (IEventoService + EventoService).
4. **Infrastructure** — `Repositories/EventoRepository.cs` (implementación completa con MySql.Data).
5. **Api** — `Controllers/EventosController.cs` (endpoints GET, POST, PUT, DELETE).

---

## Cómo implementar un método (paso a paso)

Ejemplo: implementar `GET /api/Eventos` (ya implementado, acá como referencia).

### 1. Repositorio (Infrastructure) — el SQL

En `EventoRepository.GetAllAsync`:

```csharp
public async Task<IReadOnlyList<EventoDeportivo>> GetAllAsync(CancellationToken ct = default)
{
    await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
    await using var cmd = new MySqlCommand(
        "SELECT id, id_equipo_local, id_equipo_visitante, fecha, hora, cantidad_entradas FROM evento_deportivo",
        (MySqlConnection)conn);
    await using var reader = await cmd.ExecuteReaderAsync(ct);

    var result = new List<EventoDeportivo>();
    while (await reader.ReadAsync(ct))
    {
        var horaValue = reader.GetValue(4);
        var hora = horaValue is TimeSpan ts ? TimeOnly.FromTimeSpan(ts) : TimeOnly.Parse(horaValue.ToString()!);
        
        result.Add(new EventoDeportivo
        {
            Id = reader.GetInt32(0),
            IdEquipoLocal = reader.GetInt32(1),
            IdEquipoVisitante = reader.GetInt32(2),
            Fecha = DateOnly.FromDateTime(reader.GetDateTime(3)),
            Hora = hora,
            CantidadEntradas = reader.GetInt32(5)
        });
    }
    return result;
}
```

Para queries con parámetros (siempre que entre un valor del usuario):

```csharp
await using var cmd = new MySqlCommand(
    "SELECT id, nombre FROM equipo WHERE id = @id", 
    (MySqlConnection)conn);
cmd.Parameters.AddWithValue("@id", id);
```

Para un `INSERT` que devuelve el id generado, usá `LAST_INSERT_ID()` + `ExecuteScalarAsync` con conversión `ulong` → `int`:

```csharp
await using var cmd = new MySqlCommand(
    "INSERT INTO equipo (nombre) VALUES (@nombre); SELECT LAST_INSERT_ID();",
    (MySqlConnection)conn);
cmd.Parameters.AddWithValue("@nombre", equipo.Nombre);
return (int)(ulong)await cmd.ExecuteScalarAsync(ct);
```

### 2. Service (Application) — mapear a DTO

En `EquipoService.GetAllAsync`:

```csharp
public async Task<IReadOnlyList<EquipoResponse>> GetAllAsync(CancellationToken ct = default)
{
    var equipos = await _repository.GetAllAsync(ct);
    return equipos.Select(e => new EquipoResponse(e.Id, e.Nombre)).ToList();
}
```

### 3. Listo

El controller ya está hecho y delega en el service. Probá el endpoint desde **Swagger**
(`/swagger`). Repetí para el resto de los métodos.

---

## Cómo agregar un área nueva

1. Entidad en **Domain** (si tiene tabla).
2. DTOs en **Contracts** (carpeta por área).
3. Interfaz de repo en **Application/Abstractions** + implementación esqueleto en
   **Infrastructure/Repositories** (registrala en `AddInfrastructure`).
4. `IXxxService` + `XxxService` en **Application/Services** (registralo en
   `AddApplicationServices`).
5. `XxxController` en **Api/Controllers** con los endpoints decorados.
6. (Opcional) Cliente y página en **Front**.

---

## El esquema de base manda

`database/01_schema_mysql.sql` es la **fuente de verdad** de las tablas. Si vas a implementar un
endpoint y la tabla/columna no existe, no inventes: está marcado con `// TODO` en el código
y con bloques comentados en el `.sql`. Hay **huecos conocidos** documentados al final de
`01_schema.sql` (dueño actual de la entrada, entidad de validación, vínculo evento→estadio,
RBAC dinámico). Coordinar con el equipo antes de tocar el esquema.

Áreas que hoy **no** tienen tabla en el esquema (Auth, Roles, Permisos, Asignaciones,
LogsUsuario, Paises, Localidades, Validacion, Qr): el controller y el service existen para
que aparezcan en Swagger, pero quedan en `NotImplementedException` con su `// TODO`.

---

## Comandos útiles

```bash
dotnet build Ticketing.sln                              # compilar todo
dotnet run --project src/Ticketing.Api                  # API + Swagger (http://localhost:5284/swagger)
dotnet run --project src/Ticketing.Front                # front Blazor
curl -X POST http://localhost:5284/api/Admin/initialize-schema  # inicializar schema (si es necesario)
```