using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Rbac;

namespace Ticketing.Api.Controllers;

// TODO: el RBAC dinámico no está presente en el esquema actual (sección 9).
[ApiController]
[Route("api/[controller]")]
public sealed class AsignacionesController : ControllerBase
{
    private readonly IAsignacionService _service;

    public AsignacionesController(IAsignacionService service) => _service = service;

    [HttpPost("permiso-a-rol")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AsignarPermisoARol(AsignarPermisoARolRequest request, CancellationToken ct)
    {
        await _service.AsignarPermisoARolAsync(request, ct);
        return NoContent();
    }

    [HttpPost("rol-a-usuario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AsignarRolAUsuario(AsignarRolAUsuarioRequest request, CancellationToken ct)
    {
        await _service.AsignarRolAUsuarioAsync(request, ct);
        return NoContent();
    }
}
