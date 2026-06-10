using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EstadioRepository : IEstadioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EstadioRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<Estadio>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Estadio?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(Estadio estadio, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(Estadio estadio, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<Sector>> GetSectoresAsync(int idEstadio, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateSectorAsync(Sector sector, CancellationToken ct = default)
        => throw new NotImplementedException();
}
