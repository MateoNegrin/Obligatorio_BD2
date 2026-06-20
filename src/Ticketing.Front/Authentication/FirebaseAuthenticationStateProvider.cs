using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Ticketing.Front.Services;

namespace Ticketing.Front.Authentication;

public sealed class FirebaseAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly FirebaseAuthenticationService _authService;
    private readonly ILogger<FirebaseAuthenticationStateProvider> _logger;

    public FirebaseAuthenticationStateProvider(
        FirebaseAuthenticationService authService,
        ILogger<FirebaseAuthenticationStateProvider> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _authService.GetCurrentUserTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Usuario no autenticado");
                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var state = _authService.GetCurrentState();

            if (state.IsAuthenticated && !string.IsNullOrEmpty(state.UserId))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, state.UserId),
                    new(ClaimTypes.Email, state.Email ?? string.Empty),
                };

                var identity = new ClaimsIdentity(claims, "firebase");
                var principal = new ClaimsPrincipal(identity);

                _logger.LogInformation("Usuario autenticado: {UserId}", state.UserId);
                return new AuthenticationState(principal);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo estado de autenticación");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthenticated(string userId, string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
        };

        var identity = new ClaimsIdentity(claims, "firebase");
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        _logger.LogInformation("Usuario autenticado notificado: {UserId}", userId);
    }

    public void NotifyUserLoggedOut()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        _logger.LogInformation("Usuario desconectado notificado");
    }
}
