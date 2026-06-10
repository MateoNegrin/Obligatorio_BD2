using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Ventas;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VentasController : ControllerBase
{
    private readonly IVentaService _service;

    public VentasController(IVentaService service) => _service = service;

    // Comprar entradas: una venta con múltiples entradas (máx. 5 por transacción).
    [HttpPost]
    [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Comprar(CrearVentaRequest request, CancellationToken ct)
    {
        var venta = await _service.ComprarAsync(request, ct);
        return Created(string.Empty, venta);
    }

    // Listar compras de un usuario.
    [HttpGet("usuario/{numeroDocumento}")]
    [ProducesResponseType(typeof(IReadOnlyList<VentaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComprasDeUsuario(string numeroDocumento, CancellationToken ct)
        => Ok(await _service.GetComprasDeUsuarioAsync(numeroDocumento, ct));
}
