using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Rbac;

namespace Ticketing.Api.Controllers;

// TODO: LogUsuario no está presente en el esquema actual (sección 9).
[ApiController]
[Route("api/[controller]")]
public sealed class LogsUsuarioController : ControllerBase
{
    private readonly ILogUsuarioService _service;

    public LogsUsuarioController(ILogUsuarioService service) => _service = service;

    [HttpGet("usuario/{numeroDocumento}")]
    [ProducesResponseType(typeof(IReadOnlyList<LogUsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUsuario(string numeroDocumento, CancellationToken ct)
        => Ok(await _service.GetByUsuarioAsync(numeroDocumento, ct));
}
