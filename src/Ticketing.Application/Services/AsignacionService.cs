using Ticketing.Contracts.Rbac;

namespace Ticketing.Application.Services;

// TODO: el RBAC dinámico no está presente en el esquema actual (sección 9).
public interface IAsignacionService
{
    Task AsignarPermisoARolAsync(AsignarPermisoARolRequest request, CancellationToken ct = default);
    Task AsignarRolAUsuarioAsync(AsignarRolAUsuarioRequest request, CancellationToken ct = default);
}

public sealed class AsignacionService : IAsignacionService
{
    public Task AsignarPermisoARolAsync(AsignarPermisoARolRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task AsignarRolAUsuarioAsync(AsignarRolAUsuarioRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}
