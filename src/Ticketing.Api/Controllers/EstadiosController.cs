using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Estadios;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EstadiosController : ControllerBase
{
    private readonly IEstadioService _service;

    public EstadiosController(IEstadioService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EstadioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EstadioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var estadio = await _service.GetByIdAsync(id, ct);
        return estadio is null ? NotFound() : Ok(estadio);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearEstadioRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, ActualizarEstadioRequest request, CancellationToken ct)
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

    [HttpGet("{id:int}/sectores")]
    [ProducesResponseType(typeof(IReadOnlyList<SectorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSectores(int id, CancellationToken ct)
        => Ok(await _service.GetSectoresAsync(id, ct));

    [HttpPost("{id:int}/sectores")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSector(int id, CrearSectorRequest request, CancellationToken ct)
    {
        var sectorId = await _service.CreateSectorAsync(request with { IdEstadio = id }, ct);
        return CreatedAtAction(nameof(GetSectores), new { id }, sectorId);
    }
}
