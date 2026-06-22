using System.Data;
using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EventoRepository : IEventoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EventoRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<EventoDeportivo>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT
                  e.id,
                  e.id_equipo_local,
                  e.id_equipo_visitante,
                  el.nombre AS nombre_local,
                  ev.nombre AS nombre_visitante,
                  e.fecha,
                  e.hora,
                  e.cantidad_entradas,
                  e.cantidad_entradas - (SELECT COUNT(*) FROM entrada en WHERE en.id_evento_deportivo = e.id) AS entradas_disponibles,
                  (SELECT es.ubicacion
                     FROM informacion_entrada ie
                     JOIN sector s ON s.id = ie.id_sector
                     JOIN estadio es ON es.id = s.id_estadio
                     WHERE ie.id_evento_deportivo = e.id
                     LIMIT 1) AS nombre_estadio
              FROM evento_deportivo e
              JOIN equipo el ON el.id = e.id_equipo_local
              JOIN equipo ev ON ev.id = e.id_equipo_visitante",
            (MySqlConnection)conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<EventoDeportivo>();
        while (await reader.ReadAsync(ct))
        {
            var horaValue = reader.GetValue(6);
            var hora = horaValue is TimeSpan ts ? TimeOnly.FromTimeSpan(ts) : TimeOnly.Parse(horaValue.ToString()!);

            result.Add(new EventoDeportivo
            {
                Id = reader.GetInt32(0),
                IdEquipoLocal = reader.GetInt32(1),
                IdEquipoVisitante = reader.GetInt32(2),
                NombreLocal = reader.GetString(3),
                NombreVisitante = reader.GetString(4),
                Fecha = DateOnly.FromDateTime(reader.GetDateTime(5)),
                Hora = hora,
                CantidadEntradas = reader.GetInt32(7),
                EntradasDisponibles = reader.GetInt32(8),
                NombreEstadio = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
            });
        }

        return result;
    }

    public async Task<EventoDeportivo?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT
                  e.id,
                  e.id_equipo_local,
                  e.id_equipo_visitante,
                  el.nombre AS nombre_local,
                  ev.nombre AS nombre_visitante,
                  e.fecha,
                  e.hora,
                  e.cantidad_entradas,
                  e.cantidad_entradas - (SELECT COUNT(*) FROM entrada en WHERE en.id_evento_deportivo = e.id) AS entradas_disponibles,
                  (SELECT es.ubicacion
                     FROM informacion_entrada ie
                     JOIN sector s ON s.id = ie.id_sector
                     JOIN estadio es ON es.id = s.id_estadio
                     WHERE ie.id_evento_deportivo = e.id
                     LIMIT 1) AS nombre_estadio
              FROM evento_deportivo e
              JOIN equipo el ON el.id = e.id_equipo_local
              JOIN equipo ev ON ev.id = e.id_equipo_visitante
              WHERE e.id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            return null;

        var horaValue = reader.GetValue(6);
        var hora = horaValue is TimeSpan ts ? TimeOnly.FromTimeSpan(ts) : TimeOnly.Parse(horaValue.ToString()!);

        return new EventoDeportivo
        {
            Id = reader.GetInt32(0),
            IdEquipoLocal = reader.GetInt32(1),
            IdEquipoVisitante = reader.GetInt32(2),
            NombreLocal = reader.GetString(3),
            NombreVisitante = reader.GetString(4),
            Fecha = DateOnly.FromDateTime(reader.GetDateTime(5)),
            Hora = hora,
            CantidadEntradas = reader.GetInt32(7),
            EntradasDisponibles = reader.GetInt32(8),
            NombreEstadio = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
        };
    }

    public async Task<int> CreateAsync(EventoDeportivo evento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"INSERT INTO evento_deportivo (id_equipo_local, id_equipo_visitante, fecha, hora, cantidad_entradas)
              VALUES (@id_equipo_local, @id_equipo_visitante, @fecha, @hora, @cantidad_entradas);
              SELECT LAST_INSERT_ID();",
            (MySqlConnection)conn);
        
        cmd.Parameters.AddWithValue("@id_equipo_local", evento.IdEquipoLocal);
        cmd.Parameters.AddWithValue("@id_equipo_visitante", evento.IdEquipoVisitante);
        cmd.Parameters.AddWithValue("@fecha", evento.Fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@hora", evento.Hora.ToTimeSpan());
        cmd.Parameters.AddWithValue("@cantidad_entradas", evento.CantidadEntradas);

        return (int)(ulong)await cmd.ExecuteScalarAsync(ct);
    }

    public async Task UpdateAsync(EventoDeportivo evento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"UPDATE evento_deportivo 
              SET id_equipo_local = @id_equipo_local, 
                  id_equipo_visitante = @id_equipo_visitante, 
                  fecha = @fecha, 
                  hora = @hora, 
                  cantidad_entradas = @cantidad_entradas 
              WHERE id = @id",
            (MySqlConnection)conn);

        cmd.Parameters.AddWithValue("@id", evento.Id);
        cmd.Parameters.AddWithValue("@id_equipo_local", evento.IdEquipoLocal);
        cmd.Parameters.AddWithValue("@id_equipo_visitante", evento.IdEquipoVisitante);
        cmd.Parameters.AddWithValue("@fecha", evento.Fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@hora", evento.Hora.ToTimeSpan());
        cmd.Parameters.AddWithValue("@cantidad_entradas", evento.CantidadEntradas);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "DELETE FROM evento_deportivo WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<InformacionEntrada>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id_sector, id_evento_deportivo, numero_documento_administrador FROM informacion_entrada WHERE id_evento_deportivo = @id_evento_deportivo",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id_evento_deportivo", idEvento);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<InformacionEntrada>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new InformacionEntrada
            {
                IdSector = reader.GetInt32(0),
                IdEventoDeportivo = reader.GetInt32(1),
                NumeroDocumentoAdministrador = reader.GetString(2)
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<SectorDisponibilidad>> GetSectoresDisponiblesAsync(int idEvento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT
                  s.id,
                  s.nombre,
                  s.capacidad,
                  (SELECT COUNT(*) FROM entrada en
                     WHERE en.id_evento_deportivo = ie.id_evento_deportivo
                       AND en.id_sector = s.id) AS vendidas
              FROM informacion_entrada ie
              JOIN sector s ON s.id = ie.id_sector
              WHERE ie.id_evento_deportivo = @id_evento
              ORDER BY s.nombre",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id_evento", idEvento);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<SectorDisponibilidad>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new SectorDisponibilidad
            {
                IdSector = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Capacidad = reader.GetInt32(2),
                EntradasVendidas = reader.GetInt32(3)
            });
        }

        return result;
    }

    public async Task HabilitarSectorAsync(InformacionEntrada info, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"INSERT INTO informacion_entrada (id_sector, id_evento_deportivo, numero_documento_administrador)
              VALUES (@id_sector, @id_evento_deportivo, @numero_documento_administrador)",
            (MySqlConnection)conn);

        cmd.Parameters.AddWithValue("@id_sector", info.IdSector);
        cmd.Parameters.AddWithValue("@id_evento_deportivo", info.IdEventoDeportivo);
        cmd.Parameters.AddWithValue("@numero_documento_administrador", info.NumeroDocumentoAdministrador);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}
