using System.Net.Http.Json;
using Ticketing.Contracts.Usuarios;

namespace Ticketing.Front.ApiClients;

public sealed class UsuariosApiClient
{
    private readonly HttpClient _http;

    public UsuariosApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<UsuarioResponse>> GetAllAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<UsuarioResponse>>("api/Usuarios", ct) ?? [];

    public async Task<IReadOnlyList<UsuarioResponse>> GetGeneralesAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<UsuarioResponse>>("api/Usuarios/generales", ct) ?? [];

    public async Task CreateAsync(CrearUsuarioRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Usuarios", request, ct);
        response.EnsureSuccessStatusCode();
    }
}
