using System.Net.Http.Json;
using Ticketing.Contracts.Equipos;

namespace Ticketing.Front.ApiClients;

public sealed class EquiposApiClient
{
    private readonly HttpClient _http;

    public EquiposApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<EquipoResponse>> GetAllAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<EquipoResponse>>("api/Equipos", ct) ?? [];

    public async Task<EquipoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<EquipoResponse>($"api/Equipos/{id}", ct);

    public async Task<HttpResponseMessage> CreateAsync(CrearEquipoRequest request, CancellationToken ct = default)
        => await _http.PostAsJsonAsync("api/Equipos", request, ct);
}
