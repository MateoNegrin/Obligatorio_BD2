using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IVentaRepository
{
    Task<IReadOnlyList<Venta>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default);
    Task<Venta?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Venta venta, IReadOnlyList<Entrada> entradas, int idEstadoVenta, CancellationToken ct = default);
}
