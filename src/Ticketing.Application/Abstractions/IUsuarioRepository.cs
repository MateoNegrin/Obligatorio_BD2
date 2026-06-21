using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default);
    Task<Usuario?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default);
    Task CreateAsync(Usuario usuario, CancellationToken ct = default);
    Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
    Task DeleteAsync(string numeroDocumento, CancellationToken ct = default);
    Task<string?> GetUserRoleAsync(string numeroDocumento, CancellationToken ct = default);
    Task<(string NumeroDocumento, string? Role)> GetUserRoleByEmailAsync(string email, CancellationToken ct = default);
}
