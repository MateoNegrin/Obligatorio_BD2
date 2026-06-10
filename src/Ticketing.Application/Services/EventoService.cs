using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Eventos;

namespace Ticketing.Application.Services;

public interface IEventoService
{
    Task<IReadOnlyList<EventoResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EventoResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CrearEventoRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, ActualizarEventoRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<SectorHabilitadoResponse>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default);
    Task HabilitarSectorAsync(int idEvento, HabilitarSectorRequest request, CancellationToken ct = default);
}

public sealed class EventoService : IEventoService
{
    private readonly IEventoRepository _repository;

    public EventoService(IEventoRepository repository) => _repository = repository;

    public Task<IReadOnlyList<EventoResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<EventoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearEventoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(int id, ActualizarEventoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<SectorHabilitadoResponse>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task HabilitarSectorAsync(int idEvento, HabilitarSectorRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}
