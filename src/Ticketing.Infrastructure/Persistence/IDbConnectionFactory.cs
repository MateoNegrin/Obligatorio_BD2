using Npgsql;

namespace Ticketing.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateOpenConnectionAsync(CancellationToken ct = default);
}
