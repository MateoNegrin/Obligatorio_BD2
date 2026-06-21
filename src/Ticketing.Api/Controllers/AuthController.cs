using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ticketing.Application.Services;
using Ticketing.Contracts.Auth;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

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

    [HttpGet("role")]
    [Authorize]
    [ProducesResponseType(typeof(UserRoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserRole(CancellationToken ct)
    {
        _logger.LogInformation("GET /api/Auth/role - solicitud recibida");
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        _logger.LogInformation("Email extraído del claim: {Email}", string.IsNullOrEmpty(email) ? "NULL/VACIO" : email);
        
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Email vacío, retornando 401");
            return Unauthorized();
        }

        var response = await _service.GetUserRoleByEmailAsync(email, ct);
        if (response is null)
        {
            _logger.LogWarning("Respuesta nula del servicio para email: {Email}", email);
            return Unauthorized();
        }

        _logger.LogInformation("Rol obtenido: {Role} para email: {Email}", response.Role, email);
        return Ok(response);
    }
}
