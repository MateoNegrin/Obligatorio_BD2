using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Entradas;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EntradasController : ControllerBase
{
    private readonly IEntradaService _service;

    public EntradasController(IEntradaService service) => _service = service;

    // Listar entradas asignadas a un usuario.
    [HttpGet("usuario/{numeroDocumento}")]
    [ProducesResponseType(typeof(IReadOnlyList<EntradaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeUsuario(string numeroDocumento, CancellationToken ct)
        => Ok(await _service.GetDeUsuarioAsync(numeroDocumento, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EntradaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var entrada = await _service.GetByIdAsync(id, ct);
        return entrada is null ? NotFound() : Ok(entrada);
    }

    [HttpGet("sin-validar")]
    [ProducesResponseType(typeof(IReadOnlyList<EntradaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSinValidar(CancellationToken ct)
        => Ok(await _service.GetSinValidarAsync(ct));
}
