using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Usuarios;

namespace Ticketing.Api.Controllers;

// CRUD de usuarios. Subtipos Usuario General / Funcionario / Administrador (jerarquía ISA del esquema).
[ApiController]
[Route("api/[controller]")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    // Usuarios generales (no funcionarios ni administradores). Destinatarios válidos de una transferencia.
    [HttpGet("generales")]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGenerales(CancellationToken ct)
        => Ok(await _service.GetGeneralesAsync(ct));

    [HttpGet("{numeroDocumento}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByDocumento(string numeroDocumento, CancellationToken ct)
    {
        var usuario = await _service.GetByDocumentoAsync(numeroDocumento, ct);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CrearUsuarioRequest request, CancellationToken ct)
    {
        await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByDocumento), new { numeroDocumento = request.NumeroDocumento }, null);
    }

    [HttpPut("{numeroDocumento}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(string numeroDocumento, ActualizarUsuarioRequest request, CancellationToken ct)
    {
        await _service.UpdateAsync(numeroDocumento, request, ct);
        return NoContent();
    }

    [HttpDelete("{numeroDocumento}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string numeroDocumento, CancellationToken ct)
    {
        await _service.DeleteAsync(numeroDocumento, ct);
        return NoContent();
    }

    // Estado de verificación de identidad: true si el usuario ya fue validado.
    [HttpGet("{numeroDocumento}/identidad")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIdentidadVerificada(string numeroDocumento, CancellationToken ct)
        => Ok(await _service.IsIdentidadVerificadaAsync(numeroDocumento, ct));

    // Marca la identidad del usuario como Verificada.
    [HttpPost("{numeroDocumento}/identidad/verificar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> VerificarIdentidad(string numeroDocumento, CancellationToken ct)
    {
        await _service.VerificarIdentidadAsync(numeroDocumento, ct);
        return NoContent();
    }
}
