using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EventoRepository : IEventoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EventoRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<EventoDeportivo>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<EventoDeportivo?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(EventoDeportivo evento, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(EventoDeportivo evento, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<InformacionEntrada>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task HabilitarSectorAsync(InformacionEntrada info, CancellationToken ct = default)
        => throw new NotImplementedException();
}
