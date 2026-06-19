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
            @"SELECT e.id, e.estado, e.fecha, e.estado_seed, e.qr_usado, e.costo, e.id_sector, e.id_evento_deportivo, e.numero_documento_administrador, e.id_venta
              FROM entrada e
              INNER JOIN venta v ON e.id_venta = v.id
              WHERE v.numero_documento_usuario = @numero_documento_usuario",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento_usuario", numeroDocumentoUsuario);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Entrada>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new Entrada
            {
                Id = reader.GetInt32(0),
                Estado = reader.GetString(1),
                Fecha = reader.GetDateTime(2),
                EstadoSeed = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                QrUsado = reader.GetBoolean(4),
                Costo = reader.GetDecimal(5),
                IdSector = reader.GetInt32(6),
                IdEventoDeportivo = reader.GetInt32(7),
                NumeroDocumentoAdministrador = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                IdVenta = reader.GetInt32(9)
            });
        }
        return result;
    }

    public async Task<Entrada?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, estado, fecha, estado_seed, qr_usado, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta FROM entrada WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            return new Entrada
            {
                Id = reader.GetInt32(0),
                Estado = reader.GetString(1),
                Fecha = reader.GetDateTime(2),
                EstadoSeed = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                QrUsado = reader.GetBoolean(4),
                Costo = reader.GetDecimal(5),
                IdSector = reader.GetInt32(6),
                IdEventoDeportivo = reader.GetInt32(7),
                NumeroDocumentoAdministrador = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                IdVenta = reader.GetInt32(9)
            };
        }
        return null;
    }

    public async Task MarcarConsumidaAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "UPDATE entrada SET qr_usado = 1 WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
