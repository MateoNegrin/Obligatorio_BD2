using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Qr;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class QrController : ControllerBase
{
    private readonly IQrService _service;

    public QrController(IQrService service) => _service = service;

    // Generar token dinámico (placeholder, baja prioridad).
    [HttpPost("entrada/{idEntrada:int}/token")]
    [ProducesResponseType(typeof(QrTokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerarToken(int idEntrada, CancellationToken ct)
        => Ok(await _service.GenerarTokenAsync(idEntrada, ct));
}
