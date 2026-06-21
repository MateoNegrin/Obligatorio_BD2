using System.Net.Http.Json;
using Ticketing.Contracts.Eventos;

namespace Ticketing.Front.ApiClients;

public sealed class EventosApiClient
{
    private readonly HttpClient _http;

    public EventosApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<EventoResponse>> GetAllAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<EventoResponse>>("api/Eventos", ct) ?? [];

    public async Task<IReadOnlyList<SectorDisponibleResponse>> GetSectoresDisponiblesAsync(int idEvento, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<SectorDisponibleResponse>>($"api/Eventos/{idEvento}/sectores-disponibles", ct) ?? [];
}
