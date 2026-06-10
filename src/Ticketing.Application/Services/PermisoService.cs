using Ticketing.Contracts.Rbac;

namespace Ticketing.Application.Services;

// TODO: el RBAC dinámico no está presente en el esquema actual (sección 9).
public interface IPermisoService
{
    Task<IReadOnlyList<PermisoResponse>> GetAllAsync(CancellationToken ct = default);
    Task<PermisoResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CrearPermisoRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, ActualizarPermisoRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class PermisoService : IPermisoService
{
    public Task<IReadOnlyList<PermisoResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<PermisoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearPermisoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(int id, ActualizarPermisoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
