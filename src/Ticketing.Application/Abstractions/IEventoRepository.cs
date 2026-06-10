using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEventoRepository
{
    Task<IReadOnlyList<EventoDeportivo>> GetAllAsync(CancellationToken ct = default);
    Task<EventoDeportivo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(EventoDeportivo evento, CancellationToken ct = default);
    Task UpdateAsync(EventoDeportivo evento, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<InformacionEntrada>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default);
    Task HabilitarSectorAsync(InformacionEntrada info, CancellationToken ct = default);
}
