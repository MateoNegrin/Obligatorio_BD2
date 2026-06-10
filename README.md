# Ticketing — Mundial 2026 (BDII)

Sistema de venta de entradas para el Mundial 2026. Esqueleto de backend en .NET (capas)
con acceso a datos en **ADO.NET puro sobre PostgreSQL** (sin ORM) y un front mínimo en
**Blazor WebAssembly**.

> Esta etapa es **esqueleto**: los endpoints están declarados y visibles en Swagger, pero
> los métodos de services y repositorios lanzan `NotImplementedException`. Se implementan
> de a uno. La única pieza realmente implementada es la conexión a la base.
>
> Para entender cómo está organizado el código y cómo trabajar sobre él, ver **[GUIA.md](GUIA.md)**.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (LTS).
- [Docker](https://www.docker.com/) (para levantar PostgreSQL en local).

## 1. Levantar la base de datos

```bash
docker compose up -d
```

Levanta PostgreSQL en `localhost:5432` con base/usuario/clave `ticketing` (coinciden con
`src/Ticketing.Api/appsettings.json`). Los scripts de `database/` (`01_schema.sql`,
`02_seed.sql`) se ejecutan automáticamente **la primera vez** que se crea el volumen.

> Si cambiás los `.sql` y querés re-ejecutarlos, borrá el volumen:
> `docker compose down -v && docker compose up -d`.

## 2. Correr la API

```bash
dotnet run --project src/Ticketing.Api
```

- Swagger UI: **http://localhost:5284/swagger**
- La API levanta aunque la base no esté: solo falla al ejecutar un endpoint que toque la DB.

## 3. Correr el front

```bash
dotnet run --project src/Ticketing.Front
```

Abre el Blazor WASM (la URL aparece en consola). Apunta a la API según `ApiBaseUrl` en
`src/Ticketing.Front/wwwroot/appsettings.json`. En esta etapa las páginas mostrarán un
error al listar (porque el backend devuelve 500 por `NotImplementedException`).

## Estructura

```
src/
  Ticketing.Domain          Entidades de dominio (POCOs)
  Ticketing.Contracts       DTOs (compartidos API <-> Front)
  Ticketing.Application      Interfaces y services (orquestación)
  Ticketing.Infrastructure   Repositorios ADO.NET/Npgsql + conexión
  Ticketing.Api              Web API + Swagger
  Ticketing.Front            Blazor WebAssembly
database/                    Scripts SQL (schema + seed)
docker-compose.yml           PostgreSQL para desarrollo
```

## Compilar todo

```bash
dotnet build Ticketing.sln
```
