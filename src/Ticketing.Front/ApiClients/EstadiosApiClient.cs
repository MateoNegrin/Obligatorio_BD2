using System.Net.Http.Json;
using Ticketing.Contracts.Estadios;

namespace Ticketing.Front.ApiClients;

public sealed class EstadiosApiClient
{
    private readonly HttpClient _http;

    public EstadiosApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<EstadioResponse>> GetAllAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<EstadioResponse>>("api/Estadios", ct) ?? [];

    public async Task<IReadOnlyList<SectorResponse>> GetSectoresAsync(int idEstadio, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<SectorResponse>>($"api/Estadios/{idEstadio}/sectores", ct) ?? [];

    public async Task<HttpResponseMessage> CreateAsync(CrearEstadioRequest request, CancellationToken ct = default)
        => await _http.PostAsJsonAsync("api/Estadios", request, ct);

    public async Task<HttpResponseMessage> UpdateAsync(int id, ActualizarEstadioRequest request, CancellationToken ct = default)
        => await _http.PutAsJsonAsync($"api/Estadios/{id}", request, ct);
}
