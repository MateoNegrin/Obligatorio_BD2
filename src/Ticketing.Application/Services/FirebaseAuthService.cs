using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace Ticketing.Application.Services;

public interface IFirebaseAuthService
{
    /// <summary>
    /// Valida un token JWT de Firebase y retorna el ID del usuario
    /// </summary>
    Task<FirebaseTokenValidationResult> ValidateTokenAsync(string idToken, CancellationToken ct = default);
}

public sealed record FirebaseTokenValidationResult(bool IsValid, string? UserId, string? Email, string? ErrorMessage);

public sealed class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _firebaseAuth;
    private readonly ILogger<FirebaseAuthService> _logger;

    public FirebaseAuthService(FirebaseAuth firebaseAuth, ILogger<FirebaseAuthService> logger)
    {
        _firebaseAuth = firebaseAuth;
        _logger = logger;
    }

    public async Task<FirebaseTokenValidationResult> ValidateTokenAsync(string idToken, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(idToken))
            {
                return new(false, null, null, "Token vacío o nulo");
            }

            // Verificar el token de Firebase
            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken, false, ct);

            var userId = decodedToken.Uid;
            var email = decodedToken.Claims.TryGetValue("email", out var emailClaim) 
                ? emailClaim.ToString() 
                : null;

            _logger.LogInformation("Token validado exitosamente para usuario: {UserId}", userId);
            return new(true, userId, email, null);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning("Error al validar token Firebase: {Message}", ex.Message);
            return new(false, null, null, $"Token inválido: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al validar token Firebase");
            return new(false, null, null, $"Error al validar token: {ex.Message}");
        }
    }
}
