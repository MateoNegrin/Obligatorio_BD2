using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ticketing.Application.Services;

public sealed class FirebaseTokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FirebaseTokenValidationMiddleware> _logger;

    public FirebaseTokenValidationMiddleware(RequestDelegate next, ILogger<FirebaseTokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IFirebaseAuthService firebaseAuthService)
    {
        var token = ExtractTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            var validationResult = await firebaseAuthService.ValidateTokenAsync(token);

            if (validationResult.IsValid && !string.IsNullOrEmpty(validationResult.UserId))
            {
                // Crear claims con la información del usuario de Firebase
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, validationResult.UserId),
                    new(ClaimTypes.Email, validationResult.Email ?? string.Empty),
                };

                var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);

                context.User = principal;
                _logger.LogInformation("Token Firebase validado para usuario: {UserId}", validationResult.UserId);
            }
            else
            {
                _logger.LogWarning("Validación de token fallida: {ErrorMessage}", validationResult.ErrorMessage);
            }
        }

        await _next(context);
    }

    private static string? ExtractTokenFromHeader(HttpContext context)
    {
        const string bearerScheme = "Bearer ";
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith(bearerScheme, StringComparison.OrdinalIgnoreCase))
        {
            return authHeader[bearerScheme.Length..];
        }

        return null;
    }
}
