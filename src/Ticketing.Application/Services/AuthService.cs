using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Auth;

namespace Ticketing.Application.Services;

public interface IAuthService
{
    Task<string> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken ct = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
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
}
