using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Metricas;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MetricasController : ControllerBase
{
    private readonly IMetricaService _service;

    public MetricasController(IMetricaService service) => _service = service;

    // Eventos con más entradas vendidas.
    [HttpGet("eventos-mas-vendidos")]
    [ProducesResponseType(typeof(IReadOnlyList<EventoMasVendidoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventosMasVendidos([FromQuery] int top, CancellationToken ct)
        => Ok(await _service.GetEventosMasVendidosAsync(top <= 0 ? 10 : top, ct));

    // Ranking de mayores compradores.
    [HttpGet("mayores-compradores")]
    [ProducesResponseType(typeof(IReadOnlyList<MayorCompradorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMayoresCompradores([FromQuery] int top, CancellationToken ct)
        => Ok(await _service.GetMayoresCompradoresAsync(top <= 0 ? 10 : top, ct));
}
