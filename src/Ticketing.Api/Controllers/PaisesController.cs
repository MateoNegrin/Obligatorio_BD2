using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Ubicaciones;

namespace Ticketing.Api.Controllers;

// TODO: País no es una tabla en el esquema actual (es columna de Usuario).
[ApiController]
[Route("api/[controller]")]
public sealed class PaisesController : ControllerBase
{
    private readonly IPaisService _service;

    public PaisesController(IPaisService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PaisResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearPaisRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return Created(string.Empty, id);
    }
}
