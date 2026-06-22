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

> **Status actual:** **Equipos**, **Estadios**, **Eventos**, **Usuarios**, **Entradas**, **Ventas** (compra) y **Transferencias** están
> implementadas de punta a punta (front + API), incluyendo el flujo de compra con disponibilidad por sector y la transferencia entre
> usuarios. **Métricas** queda excluida del alcance. El resto de entidades lanzan `NotImplementedException`.

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
`01_schema.sql` (entidad de validación, vínculo evento→estadio, RBAC dinámico). Coordinar con
el equipo antes de tocar el esquema.

> El **dueño actual de una entrada** ya no es un hueco: no se persiste como columna, se calcula
> al leer (`EntradaRepository`) tomando el receptor de la **última transferencia** y, si no hubo
> ninguna, el comprador original de la `venta` (`COALESCE(... última transferencia ..., venta.numero_documento_usuario)`).

Áreas que hoy **no** tienen tabla en el esquema (Auth, Roles, Permisos, Asignaciones,
LogsUsuario, Paises, Localidades, Validacion, Qr): el controller y el service existen para
que aparezcan en Swagger, pero quedan en `NotImplementedException` con su `// TODO`.

### Entidades ya implementadas:
- ✅ **Equipo** — CRUD completo
- ✅ **Estadio** (con Sector) — CRUD completo
- ✅ **EventoDeportivo** — CRUD completo + sectores habilitados y disponibilidad por sector (`/sectores`, `/sectores-disponibles`)
- ✅ **Usuario** — CRUD completo + listado de usuarios generales (`/generales`)
- ✅ **Venta** — Compra transaccional (1-5 entradas por venta, valida disponibilidad/habilitación del sector) y compras por usuario
- ✅ **Entrada** — Lectura por usuario (resolviendo el **dueño actual**) y por id, con datos de evento/estadio/sector
- ✅ **Transferencia** — Crear, contar e historial (máx. 3 transferencias por entrada)

### Entidades excluidas del alcance:
- ❌ **Métricas** — Excluida

---

## Flujo de compra y transferencia (front + API)

**Compra (`Pages/Venta.razor` → `POST /api/Ventas`):**
1. El front carga eventos (`GET /api/Eventos`) y, al elegir uno, sus sectores con disponibilidad (`GET /api/Eventos/{id}/sectores-disponibles`). Los sectores agotados se muestran deshabilitados.
2. Se arma un `CrearVentaRequest` con un item por entrada (mismo evento/sector), entre 1 y 5.
3. El **service** valida la cantidad y delega en el **repository**, que crea `venta` + `estado_venta` + las `entrada`s **en una sola transacción** validando disponibilidad/habilitación del sector. Precio fijo de `$100` por entrada.

**Transferencia (`Pages/MisEntradas.razor` → `POST /api/Transferencias`):**
1. Se listan las entradas del usuario (`GET /api/Entradas/usuario/{doc}`), que incluye las recibidas por transferencia (ver "dueño actual").
2. Se elige un destinatario de la lista de usuarios generales (`GET /api/Usuarios/generales`, excluyendo al propio usuario).
3. Antes de transferir se verifica el tope con `GET /api/Transferencias/entrada/{id}/historial` (máx. **3** por entrada).

### Toasts en el front
Las confirmaciones y errores se muestran con **toasts**, no con `alert`/excepciones a la vista:
- `Services/ToastService.cs` — `ShowSuccess` / `ShowError` disparan un evento.
- `Components/ToastContainer.razor` — vive en `Layout/MainLayout.razor`, se suscribe al evento y renderiza los mensajes.
- Se inyecta con `@inject ToastService Toast` en cualquier página.

---

## Autenticación Firebase (✨ NUEVO)

El proyecto usa **Firebase Authentication** para gestionar usuarios. El flujo es:

### En el Backend (API)

1. **Validación de tokens:** Middleware `FirebaseTokenValidationMiddleware` valida el JWT recibido en el header `Authorization: Bearer <token>`
2. **Servicios:** `IFirebaseAuthService` valida tokens usando `FirebaseAdmin` SDK
3. **Autorización:** Endpoints con `[Authorize]` solo aceptan tokens válidos

**Archivos:**
- `src/Ticketing.Application/Firebase/FirebaseDependencyInjection.cs` — Registro de servicios
- `src/Ticketing.Application/Services/FirebaseAuthService.cs` — Validación de tokens
- `src/Ticketing.Application/Services/FirebaseTokenValidationMiddleware.cs` — Middleware

**Cómo proteger un endpoint:**
```csharp
[HttpGet]
[Authorize]  // ← Solo usuarios autenticados
public async Task<IActionResult> GetUserData()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    // ...
}
```

### En el Frontend (Blazor)

1. **Login/Signup:** Usuario crea o inicia sesión en Firebase (páginas `/login` y `/signup`)
2. **Token storage:** ID token se guarda en memoria (seguro, no localStorage)
3. **AuthenticationStateProvider:** `FirebaseAuthenticationStateProvider` cascada el estado de autenticación
4. **API calls:** Token se agrega automáticamente al header de todas las solicitudes

**Archivos:**
- `src/Ticketing.Front/Services/FirebaseAuthenticationService.cs` — Orquestación de auth
- `src/Ticketing.Front/Authentication/FirebaseAuthenticationStateProvider.cs` — State cascading
- `src/Ticketing.Front/wwwroot/firebase-auth.js` — Bridge con Firebase SDK
- `src/Ticketing.Front/Pages/Login.razor` — Página de login
- `src/Ticketing.Front/Pages/Signup.razor` — Página de registro

**Cómo proteger una página:**
```razor
@page "/admin"
@attribute [Authorize]

<p>Solo usuarios autenticados ven esto</p>
```

**Datos de prueba:**
```
Email: test@example.com
Password: password123
```

### Flujo de una solicitud autenticada

```
Usuario inicia sesión en /login
↓
FirebaseAuthenticationService.LoginAsync()
↓
JS interop → window.firebaseAuth.login()
↓
Firebase retorna ID token
↓
Se guarda en memoria + se notifica AuthenticationStateProvider
↓
Usuario navegua a página protegida
↓
En cada solicitud HTTP:
  - Token se extrae de memoria
  - Se agrega header: Authorization: Bearer <token>
↓
Backend recibe solicitud
↓
FirebaseTokenValidationMiddleware valida token
↓
Se crean Claims con userId y email
↓
Endpoint [Authorize] acepta la solicitud
↓
Respuesta al usuario
```

---

## Comandos útiles

```bash
dotnet build Ticketing.sln                              # compilar todo
dotnet run --project src/Ticketing.Api                  # API + Swagger (http://localhost:5284/swagger)
dotnet run --project src/Ticketing.Front                # front Blazor
curl -X POST http://localhost:5284/api/Admin/initialize-schema  # inicializar schema (si es necesario)
```