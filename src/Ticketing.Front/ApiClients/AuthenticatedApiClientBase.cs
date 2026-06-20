using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ticketing.Front.Services;

namespace Ticketing.Front.ApiClients;

/// <summary>
/// Cliente base para las llamadas a la API con autenticación Firebase
/// </summary>
public abstract class AuthenticatedApiClientBase
{
    protected readonly HttpClient HttpClient;
    protected readonly FirebaseAuthenticationService AuthService;
    protected readonly ILogger Logger;

    protected AuthenticatedApiClientBase(
        HttpClient httpClient,
        FirebaseAuthenticationService authService,
        ILogger logger)
    {
        HttpClient = httpClient;
        AuthService = authService;
        Logger = logger;
    }

    /// <summary>
    /// Realiza una solicitud GET autenticada
    /// </summary>
    protected async Task<T?> GetAuthenticatedAsync<T>(string endpoint)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await HttpClient.GetAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }

            LogError(response);
            return default;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error en solicitud GET a {Endpoint}", endpoint);
            return default;
        }
    }

    /// <summary>
    /// Realiza una solicitud POST autenticada
    /// </summary>
    protected async Task<T?> PostAuthenticatedAsync<T>(string endpoint, object data)
    {
        try
        {
            await AddAuthorizationHeader();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent);
            }

            LogError(response);
            return default;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error en solicitud POST a {Endpoint}", endpoint);
            return default;
        }
    }

    /// <summary>
    /// Agrega el token de Firebase al header Authorization
    /// </summary>
    private async Task AddAuthorizationHeader()
    {
        var token = await AuthService.GetCurrentUserTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            Logger.LogWarning("No hay token disponible para la solicitud autenticada");
        }
    }

    private void LogError(HttpResponseMessage response)
    {
        Logger.LogError(
            "Solicitud fallida. Status: {StatusCode}. Razón: {ReasonPhrase}",
            response.StatusCode,
            response.ReasonPhrase);
    }
}
