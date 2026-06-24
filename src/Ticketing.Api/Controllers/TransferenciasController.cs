using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Transferencias;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransferenciasController : ControllerBase
{
    private readonly ITransferenciaService _service;

    public TransferenciasController(ITransferenciaService service) => _service = service;

    // Transferir entrada (máx. 3 transferencias antes de validar).
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Transferir(TransferirEntradaRequest request, CancellationToken ct)
    {
        await _service.TransferirAsync(request, ct);
        return Accepted();
    }

    [HttpGet("entrada/{idEntrada:int}/historial")]
    [ProducesResponseType(typeof(IReadOnlyList<TransferenciaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorial(int idEntrada, CancellationToken ct)
        => Ok(await _service.GetHistorialAsync(idEntrada, ct));

    [HttpGet("usuario/{numeroDocumento}")]
    [ProducesResponseType(typeof(IReadOnlyList<TransferenciaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUsuario(string numeroDocumento, CancellationToken ct)
        => Ok(await _service.GetByUsuarioAsync(numeroDocumento, ct));
}
