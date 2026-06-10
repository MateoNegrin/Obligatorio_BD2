using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Usuario?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task CreateAsync(Usuario usuario, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(string numeroDocumento, CancellationToken ct = default)
        => throw new NotImplementedException();
}
