using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> GetGeneralesAsync(CancellationToken ct = default);
    Task<Usuario?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default);
    Task CreateAsync(Usuario usuario, IReadOnlyList<string> telefonos = default, CancellationToken ct = default);
    Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
    Task DeleteAsync(string numeroDocumento, CancellationToken ct = default);
    Task<string?> GetUserRoleAsync(string numeroDocumento, CancellationToken ct = default);
    Task<(string NumeroDocumento, string? Role)> GetUserRoleByEmailAsync(string email, CancellationToken ct = default);

    // Sede asignada al administrador; null si el documento no corresponde a un administrador.
    Task<string?> GetSedeAdministradorAsync(string numeroDocumento, CancellationToken ct = default);
}
