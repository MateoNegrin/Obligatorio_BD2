using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.Services;

namespace Ticketing.Application.Firebase;

public static class FirebaseDependencyInjection
{
    /// <summary>
    /// Registra los servicios de Firebase en el contenedor de DI
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="serviceAccountJson">JSON con las credenciales de la cuenta de servicio de Firebase</param>
    /// <returns>La colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddFirebaseAuthentication(
        this IServiceCollection services,
        string serviceAccountJson)
    {
        if (string.IsNullOrWhiteSpace(serviceAccountJson))
        {
            throw new ArgumentException("El JSON de credenciales de Firebase no puede estar vacío", nameof(serviceAccountJson));
        }

        try
        {
            // Inicializar Firebase con las credenciales
            if (FirebaseApp.DefaultInstance == null)
            {
                var credential = GoogleCredential.FromJson(serviceAccountJson);
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                });
            }

            // Registrar los servicios
            services.AddSingleton(FirebaseAuth.DefaultInstance);
            services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();

            return services;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Error al inicializar Firebase. Verifica que el JSON de credenciales sea válido.",
                ex);
        }
    }
}
