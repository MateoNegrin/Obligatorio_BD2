using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Validacion;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ValidacionController : ControllerBase
{
    private readonly IValidacionService _service;

    public ValidacionController(IValidacionService service) => _service = service;

    // Validar acceso (escaneo simulado): registra funcionario + código aceptado y marca consumida.
    [HttpPost("validar-acceso")]
    [ProducesResponseType(typeof(ValidacionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidarAcceso(ValidarAccesoRequest request, CancellationToken ct)
        => Ok(await _service.ValidarAccesoAsync(request, ct));
}
