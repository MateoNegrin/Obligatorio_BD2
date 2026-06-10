using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEquipoRepository
{
    Task<IReadOnlyList<Equipo>> GetAllAsync(CancellationToken ct = default);
    Task<Equipo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Equipo equipo, CancellationToken ct = default);
    Task UpdateAsync(Equipo equipo, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
