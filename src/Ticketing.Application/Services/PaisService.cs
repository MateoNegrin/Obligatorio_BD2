using Ticketing.Contracts.Ubicaciones;

namespace Ticketing.Application.Services;

// TODO: País no es una tabla en el esquema actual (es columna de Usuario).
public interface IPaisService
{
    Task<IReadOnlyList<PaisResponse>> GetAllAsync(CancellationToken ct = default);
    Task<int> CreateAsync(CrearPaisRequest request, CancellationToken ct = default);
}

public sealed class PaisService : IPaisService
{
    public Task<IReadOnlyList<PaisResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<int> CreateAsync(CrearPaisRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}
