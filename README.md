# Ticketing — Mundial 2026 (BDII)

Sistema de venta de entradas para el Mundial 2026. Backend en .NET con arquitectura de capas
con acceso a datos en **ADO.NET puro sobre MySQL** (sin ORM) y un front mínimo en
**Blazor WebAssembly**.

**Base de datos remota:** `mysql.reto-ucu.net:50006` (IC_Grupo3)

> Status: Funcionalidades completamente implementadas para **Equipos, Estadios, Eventos, Usuarios, Entradas y Transferencias** (CRUD completo). Endpoints disponibles
> en Swagger. La arquitectura sigue el patrón de 5 capas (Domain → Contracts → Application → Infrastructure → MySQL).
>
> Para entender cómo está organizado el código y cómo trabajar sobre él, ver **[GUIA.md](GUIA.md)**.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (LTS).
- Acceso a red remota (para conectar a `mysql.reto-ucu.net:50006`).
- **Autenticación Firebase:** Acceso al proyecto Firebase `obligatoriobd2` con credenciales configuradas.

## 1. Verificar conexión a la base de datos

La base de datos remota está preconfigurada en `src/Ticketing.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "MySQL": "Server=mysql.reto-ucu.net;Port=50006;Database=IC_Grupo3;Uid=ic_g3_admin;Pwd=BD2ObligatorioG32026;"
}
```

La inicialización del schema (tablas) se realiza automáticamente a través del endpoint:
```bash
POST http://localhost:5284/api/Admin/initialize-schema
```

## 2. Compilar el proyecto

```bash
dotnet build Ticketing.sln
```

## 3. Correr la API

```bash
dotnet run --project src/Ticketing.Api
```

- Swagger UI: **http://localhost:5284/swagger**
- Health check: `GET http://localhost:5284/health`
- Admin endpoint: `POST http://localhost:5284/api/Admin/initialize-schema` (crear tablas)

### Endpoints implementados:
- ✅ `GET /api/Equipos` — Listar equipos
- ✅ `POST /api/Equipos` — Crear equipo
- ✅ `GET /api/Estadios` — Listar estadios
- ✅ `POST /api/Estadios` — Crear estadio
- ✅ `GET /api/Eventos` — Listar eventos deportivos
- ✅ `POST /api/Eventos` — Crear evento
- ✅ `GET /api/Usuarios` — Listar usuarios
- ✅ `GET /api/Usuarios/{numeroDocumento}` — Obtener usuario
- ✅ `POST /api/Usuarios` — Crear usuario
- ✅ `PUT /api/Usuarios/{numeroDocumento}` — Actualizar usuario
- ✅ `DELETE /api/Usuarios/{numeroDocumento}` — Eliminar usuario
- ✅ `GET /api/Entradas` — Listar entradas del usuario
- ✅ `GET /api/Entradas/{id}` — Obtener entrada
- ✅ `POST /api/Transferencias` — Transferir entrada
- ✅ `POST /api/Transferencias/aceptar` — Aceptar transferencia
- ✅ `GET /api/Transferencias/entrada/{idEntrada}/historial` — Historial de transferencias

## 4. Correr el front

```bash
dotnet run --project src/Ticketing.Front
```

Abre el Blazor WASM (la URL aparece en consola). Apunta a la API según `ApiBaseUrl` en
`src/Ticketing.Front/wwwroot/appsettings.json`. Las páginas para Equipos, Estadios y Eventos
funcionan correctamente y se sincronizan con la API.

## Estructura

```
src/
  Ticketing.Domain          Entidades de dominio (POCOs)
  Ticketing.Contracts       DTOs (compartidos API <-> Front)
  Ticketing.Application      Interfaces y services (orquestación)
  Ticketing.Infrastructure   Repositorios ADO.NET/MySql.Data + conexión MySQL
  Ticketing.Api              Web API + Swagger + Admin endpoints
  Ticketing.Front            Blazor WebAssembly
database/
  01_schema_mysql.sql        DDL para MySQL (20 tablas)
  02_seed.sql                Datos semilla (opcional)
```

## 5. Autenticación Firebase

El frontend está integrado con **Firebase Authentication** (Email/Password). No requiere configuración adicional para desarrollo.

### Credenciales Firebase (ya configuradas)
```
projectId: obligatoriobd2
apiKey: AIzaSyBzf4u_99g7889T_uJh1ZZwhntVPjtKu7E
authDomain: obligatoriobd2.firebaseapp.com
```

### Flujo
1. Usuario crea cuenta en `/signup` → Se registra en Firebase + se persiste en MySQL
2. Usuario inicia sesión en `/login` → Firebase genera ID token
3. Token se valida en backend (middleware `FirebaseTokenValidationMiddleware`)
4. Endpoints protegidos usan `[Authorize]` para validar token

### Archivos relevantes
- Backend: `src/Ticketing.Application/Firebase/` (servicios de validación)
- Frontend: `src/Ticketing.Front/Services/FirebaseAuthenticationService.cs` (orquestación)
- Frontend: `src/Ticketing.Front/wwwroot/firebase-auth.js` (integración con SDK)

## Tecnologías

- **Backend:** .NET 10 (LTS), ASP.NET Core 10
- **Base de datos:** MySQL 8.0+ (remota en mysql.reto-ucu.net)
- **Driver:** MySql.Data v9.0.0 (ADO.NET puro, sin ORM)
- **Frontend:** Blazor WebAssembly
- **API:** OpenAPI/Swagger
- **Autenticación:** Firebase Admin SDK (backend) + Firebase JS SDK (frontend)
