using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEstadioRepository
{
    Task<IReadOnlyList<Estadio>> GetAllAsync(CancellationToken ct = default);
    Task<Estadio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Estadio estadio, CancellationToken ct = default);
    Task UpdateAsync(Estadio estadio, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<Sector>> GetSectoresAsync(int idEstadio, CancellationToken ct = default);
    Task<int> CreateSectorAsync(Sector sector, CancellationToken ct = default);
}
