using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Equipos;
using Ticketing.Domain;

namespace Ticketing.Application.Services;

public interface IEquipoService
{
    Task<IReadOnlyList<EquipoResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EquipoResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CrearEquipoRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, ActualizarEquipoRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class EquipoService : IEquipoService
{
    private readonly IEquipoRepository _repository;

    public EquipoService(IEquipoRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<EquipoResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var equipos = await _repository.GetAllAsync(ct);
        return equipos.Select(e => new EquipoResponse(e.Id, e.Nombre)).ToList();
    }

    public async Task<EquipoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var equipo = await _repository.GetByIdAsync(id, ct);
        return equipo is null ? null : new EquipoResponse(equipo.Id, equipo.Nombre);
    }

    public async Task<int> CreateAsync(CrearEquipoRequest request, CancellationToken ct = default)
    {
        var equipo = new Equipo { Nombre = request.Nombre };
        return await _repository.CreateAsync(equipo, ct);
    }

    public async Task UpdateAsync(int id, ActualizarEquipoRequest request, CancellationToken ct = default)
    {
        var equipo = new Equipo { Id = id, Nombre = request.Nombre };
        await _repository.UpdateAsync(equipo, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(id, ct);
    }
}
