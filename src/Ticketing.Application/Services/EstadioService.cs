using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Estadios;

namespace Ticketing.Application.Services;

public interface IEstadioService
{
    Task<IReadOnlyList<EstadioResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EstadioResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CrearEstadioRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, ActualizarEstadioRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<SectorResponse>> GetSectoresAsync(int idEstadio, CancellationToken ct = default);
    Task<int> CreateSectorAsync(CrearSectorRequest request, CancellationToken ct = default);
}

public sealed class EstadioService : IEstadioService
{
    private readonly IEstadioRepository _repository;

    public EstadioService(IEstadioRepository repository) => _repository = repository;

    public Task<IReadOnlyList<EstadioResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<EstadioResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearEstadioRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(int id, ActualizarEstadioRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<SectorResponse>> GetSectoresAsync(int idEstadio, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateSectorAsync(CrearSectorRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}
