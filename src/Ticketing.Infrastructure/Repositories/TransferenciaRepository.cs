using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class TransferenciaRepository : ITransferenciaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferenciaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<Transferencia>> GetHistorialAsync(int idEntrada, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> ContarTransferenciasAsync(int idEntrada, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task CreateAsync(Transferencia transferencia, CancellationToken ct = default)
        => throw new NotImplementedException();
}
