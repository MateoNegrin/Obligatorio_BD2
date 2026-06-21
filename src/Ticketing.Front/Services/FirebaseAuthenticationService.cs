using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace Ticketing.Front.Services;

public sealed record FirebaseAuthenticationState(bool IsAuthenticated, string? UserId, string? Email, string? Role = null);

public sealed record LoginRequest(string Email, string Password);

public sealed record SignUpRequest(
    string Email,
    string Password,
    string DisplayName);

public sealed record SignUpCompleteRequest(
    string NumeroDocumento,
    string Email,
    string Password,
    string Pais,
    string Localidad,
    string Calle,
    string NumeroDireccion,
    string CodigoPostal);

public sealed record UserRoleResponse(
    string NumeroDocumento,
    string Mail,
    string Role);

public interface IAuthenticationService
{
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password);
    Task<(bool Success, string? ErrorMessage)> SignUpAsync(string email, string password, string displayName);
    Task<(bool Success, string? ErrorMessage)> SignUpCompleteAsync(SignUpCompleteRequest request);
    Task LogoutAsync();
    Task<string?> GetCurrentUserTokenAsync();
    FirebaseAuthenticationState GetCurrentState();
    Task<UserRoleResponse?> GetUserRoleAsync();
    void SetUserRole(string role);
}

public sealed class FirebaseAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<FirebaseAuthenticationService> _logger;
    private string? _currentToken;
    private string? _currentUserId;
    private string? _currentEmail;
    private string? _currentRole;

    public FirebaseAuthenticationService(
        HttpClient httpClient,
        IJSRuntime jsRuntime,
        ILogger<FirebaseAuthenticationService> logger)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("Iniciando sesión con email: {Email}", email);

            // Esperar a que Firebase esté completamente listo (max 10 segundos)
            try
            {
                _logger.LogInformation("Esperando a que Firebase esté listo para login...");
                bool ready = await _jsRuntime.InvokeAsync<bool>("window.firebaseAuth.waitForReady", 10000);
                if (!ready)
                {
                    _logger.LogError("Firebase no se inicializó a tiempo");
                    return (false, "Servicio de autenticación no disponible. Recarga la página e intenta de nuevo.");
                }
                _logger.LogInformation("✓ Firebase está listo para login");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error esperando Firebase: {Message}", ex.Message);
                // Continuar de todas formas, quizás ya esté listo
            }

            // Llamar a Firebase via JavaScript
            var result = await _jsRuntime.InvokeAsync<FirebaseLoginResult>("window.firebaseAuth.login", email, password);

            if (result.Success && !string.IsNullOrEmpty(result.IdToken))
            {
                _currentToken = result.IdToken;
                _currentUserId = result.UserId;
                _currentEmail = email;
                _logger.LogInformation("Sesión iniciada exitosamente para: {Email}", email);
                return (true, null);
            }

            var errorMsg = result.ErrorMessage ?? "Error desconocido al iniciar sesión";
            _logger.LogWarning("Error al iniciar sesión: {Error}", errorMsg);
            return (false, errorMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al iniciar sesión");
            return (false, $"Error al iniciar sesión: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> SignUpAsync(string email, string password, string displayName)
    {
        try
        {
            _logger.LogInformation("Registrando nuevo usuario: {Email}", email);

            // Esperar a que Firebase esté completamente listo (max 10 segundos)
            try
            {
                _logger.LogInformation("Esperando a que Firebase esté listo para signup...");
                bool ready = await _jsRuntime.InvokeAsync<bool>("window.firebaseAuth.waitForReady", 10000);
                if (!ready)
                {
                    _logger.LogError("Firebase no se inicializó a tiempo");
                    return (false, "Servicio de autenticación no disponible. Recarga la página e intenta de nuevo.");
                }
                _logger.LogInformation("✓ Firebase está listo para signup");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error esperando Firebase: {Message}", ex.Message);
                // Continuar de todas formas, quizás ya esté listo
            }

            var result = await _jsRuntime.InvokeAsync<FirebaseLoginResult>(
                "window.firebaseAuth.signup", email, password, displayName);

            if (result.Success && !string.IsNullOrEmpty(result.IdToken))
            {
                _currentToken = result.IdToken;
                _currentUserId = result.UserId;
                _currentEmail = email;
                _logger.LogInformation("Usuario registrado exitosamente: {Email}", email);
                return (true, null);
            }

            var errorMsg = result.ErrorMessage ?? "Error desconocido al registrar";
            _logger.LogWarning("Error al registrar usuario: {Error}", errorMsg);
            return (false, errorMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al registrar usuario");
            return (false, $"Error al registrar: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> SignUpCompleteAsync(SignUpCompleteRequest request)
    {
        try
        {
            _logger.LogInformation("Registrando usuario completo: {Email}", request.Email);

            // 0. Esperar a que Firebase esté completamente listo (max 10 segundos)
            try
            {
                _logger.LogInformation("Esperando a que Firebase esté listo...");
                bool ready = await _jsRuntime.InvokeAsync<bool>("window.firebaseAuth.waitForReady", 10000);
                if (!ready)
                {
                    _logger.LogError("Firebase no se inicializó a tiempo");
                    return (false, "Servicio de autenticación no disponible. Recarga la página e intenta de nuevo.");
                }
                _logger.LogInformation("✓ Firebase está listo");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error esperando Firebase: {Message}", ex.Message);
                // Continuar de todas formas, quizás ya esté listo
            }

            // 1. Registrar en Firebase primero
            var firebaseResult = await _jsRuntime.InvokeAsync<FirebaseLoginResult>(
                "window.firebaseAuth.signup", request.Email, request.Password, request.NumeroDocumento);

            if (!firebaseResult.Success || string.IsNullOrEmpty(firebaseResult.IdToken))
            {
                var errorMsg = firebaseResult.ErrorMessage ?? "Error al registrar en Firebase";
                _logger.LogWarning("Error Firebase: {Error}", errorMsg);
                return (false, errorMsg);
            }

            // 2. Guardar token y datos locales
            _currentToken = firebaseResult.IdToken;
            _currentUserId = firebaseResult.UserId;
            _currentEmail = request.Email;

            // 3. Crear usuario en la base de datos
            try
            {
                var crearUsuarioRequest = new
                {
                    request.NumeroDocumento,
                    Mail = request.Email,  // Backend espera "Mail", no "Email"
                    request.Pais,
                    request.Localidad,
                    request.Calle,
                    request.NumeroDireccion,
                    request.CodigoPostal
                };

                var response = await _httpClient.PostAsJsonAsync("api/Usuarios", crearUsuarioRequest);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al crear usuario en BD: {Status} - {Error}", response.StatusCode, errorContent);
                    return (false, "Error al registrar usuario en base de datos");
                }

                _logger.LogInformation("Usuario registrado completamente: {Email}", request.Email);
                return (true, null);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al crear usuario en BD");
                return (false, "Error al conectar con el servidor");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al registrar usuario completo");
            return (false, $"Error al registrar: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Cerrando sesión");
            await _jsRuntime.InvokeVoidAsync("window.firebaseAuth.logout");
            _currentToken = null;
            _currentUserId = null;
            _currentEmail = null;
            _currentRole = null;
            _logger.LogInformation("Sesión cerrada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar sesión");
        }
    }

    public async Task<string?> GetCurrentUserTokenAsync()
    {
        if (!string.IsNullOrEmpty(_currentToken))
        {
            return _currentToken;
        }

        try
        {
            _currentToken = await _jsRuntime.InvokeAsync<string?>("window.firebaseAuth.getCurrentToken");
            return _currentToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo token actual");
            return null;
        }
    }

    public FirebaseAuthenticationState GetCurrentState()
    {
        return new(
            !string.IsNullOrEmpty(_currentToken),
            _currentUserId,
            _currentEmail,
            _currentRole);
    }

    public async Task<UserRoleResponse?> GetUserRoleAsync()
    {
        try
        {
            _logger.LogInformation("GetUserRoleAsync: Iniciando obtención de rol");
            var token = await GetCurrentUserTokenAsync();
            _logger.LogInformation("GetUserRoleAsync: Token obtenido: {Token}", 
                string.IsNullOrEmpty(token) ? "NULL/VACIO" : $"presente ({token.Length} chars)");
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("GetUserRoleAsync: Token vacío, no se puede obtener rol");
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "api/Auth/role");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            _logger.LogInformation("GetUserRoleAsync: Enviando solicitud a api/Auth/role");
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roleResponse = System.Text.Json.JsonSerializer.Deserialize<UserRoleResponse>(
                    content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (roleResponse != null)
                {
                    _currentRole = roleResponse.Role;
                    _logger.LogInformation("Rol obtenido exitosamente: {Role}", roleResponse.Role);
                }
                return roleResponse;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("GetUserRoleAsync: Error obteniendo rol: {StatusCode} {Content}", 
                response.StatusCode, errorContent);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo rol del usuario");
            return null;
        }
    }

    public void SetUserRole(string role)
    {
        _currentRole = role;
        _logger.LogInformation("Rol establecido: {Role}", role);
    }
}

public sealed record FirebaseLoginResult(
    bool Success,
    string? IdToken,
    string? UserId,
    string? ErrorMessage);
