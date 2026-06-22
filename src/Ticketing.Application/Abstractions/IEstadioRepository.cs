using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEstadioRepository
{
    Task<IReadOnlyList<Estadio>> GetAllAsync(CancellationToken ct = default);
    Task<Estadio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Estadio estadio, CancellationToken ct = default);
    Task UpdateAsync(Estadio estadio, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    // Inserta el estadio junto con sus sectores en una única transacción.
    Task<int> CreateConSectoresAsync(Estadio estadio, IReadOnlyList<Sector> sectores, CancellationToken ct = default);

    // Actualiza ubicación y capacidad del estadio + sus sectores (por id) en una transacción.
    Task UpdateConSectoresAsync(Estadio estadio, IReadOnlyList<Sector> sectores, CancellationToken ct = default);

    Task<IReadOnlyList<Sector>> GetSectoresAsync(int idEstadio, CancellationToken ct = default);
    Task<int> CreateSectorAsync(Sector sector, CancellationToken ct = default);
}
