using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Ventas;

namespace Ticketing.Application.Services;

public interface IVentaService
{
    // Comprar entradas: una venta con múltiples entradas (máx. 5 por transacción).
    Task<VentaResponse> ComprarAsync(CrearVentaRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<VentaResponse>> GetComprasDeUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default);
}

public sealed class VentaService : IVentaService
{
    private readonly IVentaRepository _repository;

    public VentaService(IVentaRepository repository) => _repository = repository;

    public Task<VentaResponse> ComprarAsync(CrearVentaRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<VentaResponse>> GetComprasDeUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
        => throw new NotImplementedException();
}
