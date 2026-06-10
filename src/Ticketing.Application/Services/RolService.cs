using Ticketing.Contracts.Rbac;

namespace Ticketing.Application.Services;

// TODO: el RBAC dinámico no está presente en el esquema actual (sección 9).
public interface IRolService
{
    Task<IReadOnlyList<RolResponse>> GetAllAsync(CancellationToken ct = default);
    Task<RolResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CrearRolRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, ActualizarRolRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class RolService : IRolService
{
    public Task<IReadOnlyList<RolResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<RolResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearRolRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(int id, ActualizarRolRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
