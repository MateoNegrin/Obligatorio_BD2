using System.Net.Http.Json;

namespace Ticketing.Front.ApiClients;

public sealed class DispositivosApiClient
{
    private readonly HttpClient _http;

    public DispositivosApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<DispositivoDto>> GetByFuncionarioAsync(string numeroDocumento, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<IReadOnlyList<DispositivoDto>>($"api/Dispositivos/funcionario/{numeroDocumento}", ct) ?? [];

    public record DispositivoDto(int Id, string Nombre, string NumeroDocumento);
}
