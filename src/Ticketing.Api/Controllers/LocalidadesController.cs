using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Ubicaciones;

namespace Ticketing.Api.Controllers;

// TODO: Localidad no es una tabla en el esquema actual (es columna de Usuario).
[ApiController]
[Route("api/[controller]")]
public sealed class LocalidadesController : ControllerBase
{
    private readonly ILocalidadService _service;

    public LocalidadesController(ILocalidadService service) => _service = service;

    [HttpGet("pais/{idPais:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<LocalidadResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPais(int idPais, CancellationToken ct)
        => Ok(await _service.GetByPaisAsync(idPais, ct));

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearLocalidadRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return Created(string.Empty, id);
    }
}
