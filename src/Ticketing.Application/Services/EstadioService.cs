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
    private readonly IUsuarioRepository _usuarios;

    public EstadioService(IEstadioRepository repository, IUsuarioRepository usuarios)
    {
        _repository = repository;
        _usuarios = usuarios;
    }

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
        if (request.Sectores is null || request.Sectores.Count != 4)
            throw new InvalidOperationException("El estadio debe crearse con exactamente 4 sectores.");
        if (request.Sectores.Any(s => string.IsNullOrWhiteSpace(s.Nombre) || s.Capacidad <= 0))
            throw new InvalidOperationException("Cada sector debe tener un nombre y una capacidad mayor a cero.");

        // La sede del estadio es la sede asignada al administrador; no se puede crear en otra.
        var sede = await _usuarios.GetSedeAdministradorAsync(request.NumeroDocumentoAdministrador, ct)
            ?? throw new InvalidOperationException("El usuario indicado no es un administrador válido.");

        var estadio = new Estadio
        {
            NombreSede = sede,
            CapacidadMaxima = request.Sectores.Sum(s => s.Capacidad),
            Ubicacion = request.Ubicacion
        };
        var sectores = request.Sectores
            .Select(s => new Sector { Nombre = s.Nombre, Capacidad = s.Capacidad })
            .ToList();

        return await _repository.CreateConSectoresAsync(estadio, sectores, ct);
    }

    public async Task UpdateAsync(int id, ActualizarEstadioRequest request, CancellationToken ct = default)
    {
        if (request.Sectores is null || request.Sectores.Count == 0)
            throw new InvalidOperationException("Debe indicar los sectores del estadio.");
        if (request.Sectores.Any(s => string.IsNullOrWhiteSpace(s.Nombre) || s.Capacidad <= 0))
            throw new InvalidOperationException("Cada sector debe tener un nombre y una capacidad mayor a cero.");

        var estadio = new Estadio
        {
            Id = id,
            CapacidadMaxima = request.Sectores.Sum(s => s.Capacidad),
            Ubicacion = request.Ubicacion
        };
        var sectores = request.Sectores
            .Select(s => new Sector { Id = s.Id, IdEstadio = id, Nombre = s.Nombre, Capacidad = s.Capacidad })
            .ToList();

        await _repository.UpdateConSectoresAsync(estadio, sectores, ct);
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
