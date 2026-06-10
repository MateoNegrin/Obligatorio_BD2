using Ticketing.Contracts.Rbac;

namespace Ticketing.Application.Services;

// TODO: LogUsuario no está presente en el esquema actual (sección 9).
public interface ILogUsuarioService
{
    Task<IReadOnlyList<LogUsuarioResponse>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default);
}

public sealed class LogUsuarioService : ILogUsuarioService
{
    public Task<IReadOnlyList<LogUsuarioResponse>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
        => throw new NotImplementedException();
}
