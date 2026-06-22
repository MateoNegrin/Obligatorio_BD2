using System.Net.Http.Json;
using Ticketing.Contracts.Entradas;

namespace Ticketing.Front.ApiClients;

public sealed class EntradasApiClient
{
    private readonly HttpClient _http;

    public EntradasApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<EntradaResponse>> GetDeUsuarioAsync(string numeroDocumento, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<EntradaResponse>>($"api/Entradas/usuario/{numeroDocumento}", ct) ?? [];
}
