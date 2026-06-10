using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Usuarios;

namespace Ticketing.Application.Services;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> GetAllAsync(CancellationToken ct = default);
    Task<UsuarioResponse?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default);
    Task CreateAsync(CrearUsuarioRequest request, CancellationToken ct = default);
    Task UpdateAsync(string numeroDocumento, ActualizarUsuarioRequest request, CancellationToken ct = default);
    Task DeleteAsync(string numeroDocumento, CancellationToken ct = default);
}

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository) => _repository = repository;

    public Task<IReadOnlyList<UsuarioResponse>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<UsuarioResponse?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task CreateAsync(CrearUsuarioRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(string numeroDocumento, ActualizarUsuarioRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(string numeroDocumento, CancellationToken ct = default)
        => throw new NotImplementedException();
}
