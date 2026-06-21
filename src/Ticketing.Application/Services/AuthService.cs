using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Auth;

namespace Ticketing.Application.Services;

public interface IAuthService
{
    Task<string> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken ct = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UserRoleResponse?> GetUserRoleAsync(string numeroDocumento, CancellationToken ct = default);
    Task<UserRoleResponse?> GetUserRoleByEmailAsync(string email, CancellationToken ct = default);
}

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;

    public AuthService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    // TODO: el esquema actual no modela credenciales/password ni tokens. Placeholder.
    public Task<string> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public async Task<UserRoleResponse?> GetUserRoleAsync(string numeroDocumento, CancellationToken ct = default)
    {
        var usuario = await _usuarios.GetByDocumentoAsync(numeroDocumento, ct);
        if (usuario is null)
            return null;

        var role = await _usuarios.GetUserRoleAsync(numeroDocumento, ct);
        return new UserRoleResponse(usuario.NumeroDocumento, usuario.Mail, role ?? "General");
    }

    public async Task<UserRoleResponse?> GetUserRoleByEmailAsync(string email, CancellationToken ct = default)
    {
        var (numeroDocumento, role) = await _usuarios.GetUserRoleByEmailAsync(email, ct);
        
        if (string.IsNullOrEmpty(numeroDocumento))
            return null;

        return new UserRoleResponse(numeroDocumento, email, role ?? "General");
    }
}

