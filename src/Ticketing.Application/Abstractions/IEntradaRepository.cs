using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEntradaRepository
{
    Task<IReadOnlyList<Entrada>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default);
    Task<Entrada?> GetByIdAsync(int id, CancellationToken ct = default);
    Task MarcarConsumidaAsync(int id, CancellationToken ct = default);
}
