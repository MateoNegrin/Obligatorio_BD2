using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Dispositivos;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DispositivosController : ControllerBase
{
    private readonly IDispositivoRepository _repo;

    public DispositivosController(IDispositivoRepository repo) => _repo = repo;

    [HttpGet("funcionario/{numeroDocumento}")]
    [ProducesResponseType(typeof(IReadOnlyList<DispositivoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByFuncionario(string numeroDocumento, CancellationToken ct)
    {
        var dispositivos = await _repo.GetByFuncionarioAsync(numeroDocumento, ct);
        var response = dispositivos.Select(d => new DispositivoResponse(d.Id, d.Nombre, d.NumeroDocumento)).ToList();
        return Ok(response);
    }
}
