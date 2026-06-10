using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Rbac;

namespace Ticketing.Api.Controllers;

// TODO: el RBAC dinámico no está presente en el esquema actual (sección 9).
[ApiController]
[Route("api/[controller]")]
public sealed class PermisosController : ControllerBase
{
    private readonly IPermisoService _service;

    public PermisosController(IPermisoService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PermisoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PermisoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var permiso = await _service.GetByIdAsync(id, ct);
        return permiso is null ? NotFound() : Ok(permiso);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearPermisoRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, ActualizarPermisoRequest request, CancellationToken ct)
    {
        await _service.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
