using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Ventas;
using Ticketing.Domain;

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
    private const int idComisionDefault = 1;       // 5% (solo referencia/FK; no se suma al monto)
    private const int idEstadoVentaPendiente = 1;  // estado_venta: 'Pendiente'
    private const decimal costoDefault = 100m;      // precio fijo por entrada

    public VentaService(IVentaRepository repository) => _repository = repository;

    public async Task<VentaResponse> ComprarAsync(CrearVentaRequest request, CancellationToken ct = default)
    {
        // Validar máximo 5 entradas por transacción
        if (request.Items.Count is < 1 or > 5)
            throw new InvalidOperationException("La venta debe tener entre 1 y 5 entradas");

        // Monto total = cantidad x precio (sin comisión).
        var montoTotal = request.Items.Count * costoDefault;

        var venta = new Venta
        {
            NumeroDocumentoUsuario = request.NumeroDocumentoUsuario,
            IdComision = idComisionDefault,
            Fecha = DateTime.UtcNow,
            MontoTotal = montoTotal
        };

        var entradas = request.Items.Select(item => new Entrada
        {
            Estado = "Disponible",
            Fecha = DateTime.UtcNow,
            QrUsado = false,
            Costo = costoDefault,
            IdSector = item.IdSector,
            IdEventoDeportivo = item.IdEventoDeportivo
        }).ToList();

        // El repo valida disponibilidad/habilitación, crea venta + estado + entradas en una
        // transacción y completa el Id real de cada entrada.
        var ventaId = await _repository.CreateAsync(venta, entradas, idEstadoVentaPendiente, ct);

        var ventaCreada = await _repository.GetByIdAsync(ventaId, ct)
            ?? throw new InvalidOperationException("Error al recuperar la venta creada");

        return new VentaResponse(
            ventaCreada.Id,
            ventaCreada.NumeroDocumentoUsuario,
            ventaCreada.Fecha,
            ventaCreada.MontoTotal,
            entradas.Select(e => new EntradaVendidaResponse(
                e.Id,
                e.IdSector,
                e.IdEventoDeportivo,
                e.Costo
            )).ToArray()
        );
    }

    public async Task<IReadOnlyList<VentaResponse>> GetComprasDeUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
    {
        var ventas = await _repository.GetByUsuarioAsync(numeroDocumentoUsuario, ct);
        
        return ventas.Select(v => new VentaResponse(
            v.Id,
            v.NumeroDocumentoUsuario,
            v.Fecha,
            v.MontoTotal,
            [] // Las entradas se cargarían en una consulta adicional si fuera necesario
        )).ToList();
    }
}
