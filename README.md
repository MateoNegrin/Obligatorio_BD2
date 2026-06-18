# Ticketing — Mundial 2026 (BDII)

Sistema de venta de entradas para el Mundial 2026. Backend en .NET con arquitectura de capas
con acceso a datos en **ADO.NET puro sobre MySQL** (sin ORM) y un front mínimo en
**Blazor WebAssembly**.

**Base de datos remota:** `mysql.reto-ucu.net:50006` (IC_Grupo3)

> Status: Funcionalidades implementadas para **Equipos, Estadios y Eventos**. Endpoints disponibles
> en Swagger. La arquitectura sigue el patrón de 5 capas (Domain → Contracts → Application → Infrastructure → MySQL).
>
> Para entender cómo está organizado el código y cómo trabajar sobre él, ver **[GUIA.md](GUIA.md)**.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (LTS).
- Acceso a red remota (para conectar a `mysql.reto-ucu.net:50006`).

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

## Tecnologías

- **Backend:** .NET 10 (LTS), ASP.NET Core 10
- **Base de datos:** MySQL 8.0+ (remota en mysql.reto-ucu.net)
- **Driver:** MySql.Data v9.0.0 (ADO.NET puro, sin ORM)
- **Frontend:** Blazor WebAssembly
- **API:** OpenAPI/Swagger
