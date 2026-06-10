using Ticketing.Contracts.Ubicaciones;

namespace Ticketing.Application.Services;

// TODO: Localidad no es una tabla en el esquema actual (es columna de Usuario).
public interface ILocalidadService
{
    Task<IReadOnlyList<LocalidadResponse>> GetByPaisAsync(int idPais, CancellationToken ct = default);
    Task<int> CreateAsync(CrearLocalidadRequest request, CancellationToken ct = default);
}

public sealed class LocalidadService : ILocalidadService
{
    public Task<IReadOnlyList<LocalidadResponse>> GetByPaisAsync(int idPais, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearLocalidadRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}
