using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IMetricaRepository
{
    // (idEventoDeportivo, entradasVendidas)
    Task<IReadOnlyList<(int IdEventoDeportivo, int EntradasVendidas)>> GetEventosMasVendidosAsync(int top, CancellationToken ct = default);

    // (numeroDocumentoUsuario, mail, entradasCompradas)
    Task<IReadOnlyList<(string NumeroDocumento, string Mail, int EntradasCompradas)>> GetMayoresCompradoresAsync(int top, CancellationToken ct = default);
}
