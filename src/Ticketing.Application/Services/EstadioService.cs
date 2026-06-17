using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Estadios;
using Ticketing.Domain;

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

    public async Task<IReadOnlyList<EstadioResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var estadios = await _repository.GetAllAsync(ct);
        return estadios.Select(e => new EstadioResponse(e.Id, e.NombreSede, e.CapacidadMaxima, e.Ubicacion)).ToList();
    }

    public async Task<EstadioResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var estadio = await _repository.GetByIdAsync(id, ct);
        return estadio is null ? null : new EstadioResponse(estadio.Id, estadio.NombreSede, estadio.CapacidadMaxima, estadio.Ubicacion);
    }

    public async Task<int> CreateAsync(CrearEstadioRequest request, CancellationToken ct = default)
    {
        var estadio = new Estadio
        {
            NombreSede = request.NombreSede,
            CapacidadMaxima = request.CapacidadMaxima,
            Ubicacion = request.Ubicacion
        };
        return await _repository.CreateAsync(estadio, ct);
    }

    public async Task UpdateAsync(int id, ActualizarEstadioRequest request, CancellationToken ct = default)
    {
        var estadio = new Estadio
        {
            Id = id,
            NombreSede = request.NombreSede,
            CapacidadMaxima = request.CapacidadMaxima,
            Ubicacion = request.Ubicacion
        };
        await _repository.UpdateAsync(estadio, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(id, ct);
    }

    public async Task<IReadOnlyList<SectorResponse>> GetSectoresAsync(int idEstadio, CancellationToken ct = default)
    {
        var sectores = await _repository.GetSectoresAsync(idEstadio, ct);
        return sectores.Select(s => new SectorResponse(s.Id, s.IdEstadio, s.Nombre, s.Capacidad)).ToList();
    }

    public async Task<int> CreateSectorAsync(CrearSectorRequest request, CancellationToken ct = default)
    {
        var sector = new Sector
        {
            IdEstadio = request.IdEstadio,
            Nombre = request.Nombre,
            Capacidad = request.Capacidad
        };
        return await _repository.CreateSectorAsync(sector, ct);
    }
}
