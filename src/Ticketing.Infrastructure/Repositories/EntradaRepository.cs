using System.Data.Common;
using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EntradaRepository : IEntradaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EntradaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<Entrada>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT e.id, e.estado, e.fecha, e.estado_seed, e.qr_usado, e.costo, e.id_sector, e.id_evento_deportivo, e.numero_documento_administrador, e.id_venta,
                     el.nombre AS nombre_local, ev.nombre AS nombre_visitante, ed.fecha AS fecha_evento, es.ubicacion AS nombre_estadio, s.nombre AS nombre_sector, e.numero_documento_propietario_actual
              FROM entrada e
              INNER JOIN evento_deportivo ed ON ed.id = e.id_evento_deportivo
              INNER JOIN equipo el ON el.id = ed.id_equipo_local
              INNER JOIN equipo ev ON ev.id = ed.id_equipo_visitante
              INNER JOIN sector s ON s.id = e.id_sector
              INNER JOIN estadio es ON es.id = s.id_estadio
              WHERE e.numero_documento_propietario_actual = @numero_documento_usuario",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento_usuario", numeroDocumentoUsuario);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Entrada>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(MapEntrada(reader));
        }
        return result;
    }

    public async Task<Entrada?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT e.id, e.estado, e.fecha, e.estado_seed, e.qr_usado, e.costo, e.id_sector, e.id_evento_deportivo, e.numero_documento_administrador, e.id_venta,
                     el.nombre AS nombre_local, ev.nombre AS nombre_visitante, ed.fecha AS fecha_evento, es.ubicacion AS nombre_estadio, s.nombre AS nombre_sector, e.numero_documento_propietario_actual
              FROM entrada e
              INNER JOIN evento_deportivo ed ON ed.id = e.id_evento_deportivo
              INNER JOIN equipo el ON el.id = ed.id_equipo_local
              INNER JOIN equipo ev ON ev.id = ed.id_equipo_visitante
              INNER JOIN sector s ON s.id = e.id_sector
              INNER JOIN estadio es ON es.id = s.id_estadio
              WHERE e.id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            return MapEntrada(reader);
        }
        return null;
    }

    private static Entrada MapEntrada(DbDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Estado = reader.GetString(1),
        Fecha = reader.GetDateTime(2),
        EstadoSeed = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
        QrUsado = reader.IsDBNull(4) ? null : reader.GetString(4),
        Costo = reader.GetDecimal(5),
        IdSector = reader.GetInt32(6),
        IdEventoDeportivo = reader.GetInt32(7),
        NumeroDocumentoAdministrador = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
        IdVenta = reader.GetInt32(9),
        NombreLocal = reader.GetString(10),
        NombreVisitante = reader.GetString(11),
        FechaEvento = DateOnly.FromDateTime(reader.GetDateTime(12)),
        NombreEstadio = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
        NombreSector = reader.GetString(14),
        NumeroDocumentoPropietarioActual = reader.GetString(15)
    };

    public async Task ValidarEntradaAsync(int id, string codigoQr, int idDispositivo, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "UPDATE entrada SET estado = 'Validada', qr_usado = @codigo WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@codigo", codigoQr);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<Entrada>> GetSinValidarAsync(CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT e.id, e.estado, e.fecha, e.estado_seed, e.qr_usado, e.costo, e.id_sector, e.id_evento_deportivo, e.numero_documento_administrador, e.id_venta,
                     el.nombre AS nombre_local, ev.nombre AS nombre_visitante, ed.fecha AS fecha_evento, es.ubicacion AS nombre_estadio, s.nombre AS nombre_sector, e.numero_documento_propietario_actual
              FROM entrada e
              INNER JOIN evento_deportivo ed ON ed.id = e.id_evento_deportivo
              INNER JOIN equipo el ON el.id = ed.id_equipo_local
              INNER JOIN equipo ev ON ev.id = ed.id_equipo_visitante
              INNER JOIN sector s ON s.id = e.id_sector
              INNER JOIN estadio es ON es.id = s.id_estadio
              WHERE e.qr_usado IS NULL AND e.estado != 'Anulada'
              ORDER BY ed.fecha DESC, s.nombre ASC",
            (MySqlConnection)conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Entrada>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(MapEntrada(reader));
        }
        return result;
    }
}
