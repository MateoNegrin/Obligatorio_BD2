using System.Data.Common;

namespace Ticketing.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken ct = default);
}
