using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class MetricaRepository : IMetricaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MetricaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<(int IdEventoDeportivo, int EntradasVendidas)>> GetEventosMasVendidosAsync(int top, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            """
            SELECT e.id_evento_deportivo, COUNT(*) as cantidad_entradas
            FROM entrada e
            WHERE e.estado != 'cancelada'
            GROUP BY e.id_evento_deportivo
            ORDER BY cantidad_entradas DESC
            LIMIT @top
            """,
            (MySqlConnection)conn);
        
        cmd.Parameters.AddWithValue("@top", top);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        
        var result = new List<(int, int)>();
        while (await reader.ReadAsync(ct))
        {
            result.Add((
                reader.GetInt32(0),  // IdEventoDeportivo
                reader.GetInt32(1)   // EntradasVendidas
            ));
        }
        
        return result;
    }

    public async Task<IReadOnlyList<(string NumeroDocumento, string Mail, int EntradasCompradas)>> GetMayoresCompradoresAsync(int top, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            """
            SELECT u.numero_documento, u.mail, COUNT(e.id) as cantidad_entradas
            FROM usuario u
            INNER JOIN venta v ON u.numero_documento = v.numero_documento_usuario
            INNER JOIN entrada e ON v.id = e.id_venta
            WHERE e.estado != 'cancelada'
            GROUP BY u.numero_documento, u.mail
            ORDER BY cantidad_entradas DESC
            LIMIT @top
            """,
            (MySqlConnection)conn);
        
        cmd.Parameters.AddWithValue("@top", top);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        
        var result = new List<(string, string, int)>();
        while (await reader.ReadAsync(ct))
        {
            result.Add((
                reader.GetString(0),  // NumeroDocumento
                reader.GetString(1),  // Mail
                reader.GetInt32(2)    // EntradasCompradas
            ));
        }
        
        return result;
    }
}
