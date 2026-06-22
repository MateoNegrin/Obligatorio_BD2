using System.Net.Http.Json;
using Ticketing.Contracts.Transferencias;

namespace Ticketing.Front.ApiClients;

public sealed class TransferenciasApiClient
{
    private readonly HttpClient _http;

    public TransferenciasApiClient(HttpClient http) => _http = http;

    // Devuelve null si la transferencia fue aceptada; el mensaje de error si falló.
    public async Task<string?> TransferirAsync(TransferirEntradaRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/Transferencias", request, ct);
        if (response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync(ct);
    }

    // Cantidad de transferencias ya realizadas sobre una entrada (para validar el tope de 3).
    public async Task<int> ContarTransferenciasAsync(int idEntrada, CancellationToken ct = default)
    {
        var historial = await _http.GetFromJsonAsync<IReadOnlyList<TransferenciaResponse>>(
            $"api/Transferencias/entrada/{idEntrada}/historial", ct) ?? [];
        return historial.Count;
    }
}
