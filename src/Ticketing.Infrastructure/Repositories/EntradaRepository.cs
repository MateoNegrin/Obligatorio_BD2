using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EntradaRepository : IEntradaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EntradaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<Entrada>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Entrada?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task MarcarConsumidaAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
