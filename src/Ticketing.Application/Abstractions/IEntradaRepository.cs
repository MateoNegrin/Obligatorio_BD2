using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEntradaRepository
{
    Task<IReadOnlyList<Entrada>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default);
    Task<Entrada?> GetByIdAsync(int id, CancellationToken ct = default);
    // Dueño actual: receptor de la última transferencia, o el comprador si nunca se transfirió.
    // Devuelve null si la entrada no existe.
    Task<string?> GetPropietarioActualAsync(int idEntrada, CancellationToken ct = default);
    Task MarcarConsumidaAsync(int id, CancellationToken ct = default);
    Task ValidarEntradaAsync(int id, string codigoQr, int idDispositivo, CancellationToken ct = default);
    Task<IReadOnlyList<Entrada>> GetSinValidarAsync(CancellationToken ct = default);
}
