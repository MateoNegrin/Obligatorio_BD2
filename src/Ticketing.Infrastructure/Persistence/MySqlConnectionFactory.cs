using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Ticketing.Infrastructure.Persistence;

public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration config)
        => _connectionString = config.GetConnectionString("MySQL")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'MySQL'.");

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken ct = default)
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }
}
