using Ticketing.Contracts.Qr;

namespace Ticketing.Application.Services;

public interface IQrService
{
    // QR dinámico simulado (baja prioridad).
    Task<QrTokenResponse> GenerarTokenAsync(int idEntrada, CancellationToken ct = default);
}

public sealed class QrService : IQrService
{
    // TODO: no existe histórico de QR ni tokens en el esquema actual. Placeholder.
    public Task<QrTokenResponse> GenerarTokenAsync(int idEntrada, CancellationToken ct = default)
        => throw new NotImplementedException();
}
