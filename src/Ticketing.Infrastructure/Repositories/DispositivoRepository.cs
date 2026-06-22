using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class DispositivoRepository : IDispositivoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DispositivoRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<DispositivoAutorizado>> GetByFuncionarioAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, nombre, numero_documento FROM dispositivo_autorizado WHERE numero_documento = @doc",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@doc", numeroDocumento);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<DispositivoAutorizado>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new DispositivoAutorizado
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                NumeroDocumento = reader.GetString(2)
            });
        }
        return result;
    }
}
