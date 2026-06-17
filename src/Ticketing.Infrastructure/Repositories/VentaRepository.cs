using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class VentaRepository : IVentaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public VentaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<Venta>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Venta?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    // La venta + sus entradas deben crearse dentro de una transacción (MySqlTransaction).
    public Task<int> CreateAsync(Venta venta, IReadOnlyList<Entrada> entradas, CancellationToken ct = default)
        => throw new NotImplementedException();
}
