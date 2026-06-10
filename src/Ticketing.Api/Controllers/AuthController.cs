using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Services;
using Ticketing.Contracts.Auth;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service) => _service = service;

    [HttpPost("registrar")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar(RegistrarUsuarioRequest request, CancellationToken ct)
    {
        var numeroDocumento = await _service.RegistrarAsync(request, ct);
        return Created(string.Empty, numeroDocumento);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        => Ok(await _service.LoginAsync(request, ct));
}
