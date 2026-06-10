using Ticketing.Application.Abstractions;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class MetricaRepository : IMetricaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MetricaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<(int IdEventoDeportivo, int EntradasVendidas)>> GetEventosMasVendidosAsync(int top, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<(string NumeroDocumento, string Mail, int EntradasCompradas)>> GetMayoresCompradoresAsync(int top, CancellationToken ct = default)
        => throw new NotImplementedException();
}
