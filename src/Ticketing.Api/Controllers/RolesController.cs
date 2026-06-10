using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Rbac;

namespace Ticketing.Api.Controllers;

// TODO: el RBAC dinámico no está presente en el esquema actual (sección 9).
[ApiController]
[Route("api/[controller]")]
public sealed class RolesController : ControllerBase
{
    private readonly IRolService _service;

    public RolesController(IRolService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RolResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RolResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var rol = await _service.GetByIdAsync(id, ct);
        return rol is null ? NotFound() : Ok(rol);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearRolRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, ActualizarRolRequest request, CancellationToken ct)
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
