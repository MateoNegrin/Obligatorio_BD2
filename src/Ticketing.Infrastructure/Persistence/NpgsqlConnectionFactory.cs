using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Ticketing.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration config)
        => _connectionString = config.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'Postgres'.");

    public async Task<NpgsqlConnection> CreateOpenConnectionAsync(CancellationToken ct = default)
    {
        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }
}
