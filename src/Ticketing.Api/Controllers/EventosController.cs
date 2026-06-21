using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Eventos;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EventosController : ControllerBase
{
    private readonly IEventoService _service;

    public EventosController(IEventoService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var evento = await _service.GetByIdAsync(id, ct);
        return evento is null ? NotFound() : Ok(evento);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearEventoRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, ActualizarEventoRequest request, CancellationToken ct)
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
    [ProducesResponseType(typeof(IReadOnlyList<SectorHabilitadoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSectoresHabilitados(int id, CancellationToken ct)
        => Ok(await _service.GetSectoresHabilitadosAsync(id, ct));

    [HttpGet("{id:int}/sectores-disponibles")]
    [ProducesResponseType(typeof(IReadOnlyList<SectorDisponibleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSectoresDisponibles(int id, CancellationToken ct)
        => Ok(await _service.GetSectoresDisponiblesAsync(id, ct));

    [HttpPost("{id:int}/sectores")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> HabilitarSector(int id, HabilitarSectorRequest request, CancellationToken ct)
    {
        await _service.HabilitarSectorAsync(id, request, ct);
        return NoContent();
    }
}
