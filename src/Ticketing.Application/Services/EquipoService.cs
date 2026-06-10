using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Equipos;

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

    public Task<IReadOnlyList<EquipoResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<EquipoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearEquipoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(int id, ActualizarEquipoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
