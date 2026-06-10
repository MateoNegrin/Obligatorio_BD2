using System.Net.Http.Json;
using Ticketing.Contracts.Metricas;

namespace Ticketing.Front.ApiClients;

public sealed class MetricasApiClient
{
    private readonly HttpClient _http;

    public MetricasApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<EventoMasVendidoResponse>> GetEventosMasVendidosAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<EventoMasVendidoResponse>>("api/Metricas/eventos-mas-vendidos", ct) ?? [];

    public async Task<IReadOnlyList<MayorCompradorResponse>> GetMayoresCompradoresAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<MayorCompradorResponse>>("api/Metricas/mayores-compradores", ct) ?? [];
}
