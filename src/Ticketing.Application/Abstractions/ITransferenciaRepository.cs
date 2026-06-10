using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface ITransferenciaRepository
{
    Task<IReadOnlyList<Transferencia>> GetHistorialAsync(int idEntrada, CancellationToken ct = default);
    Task<int> ContarTransferenciasAsync(int idEntrada, CancellationToken ct = default);
    Task CreateAsync(Transferencia transferencia, CancellationToken ct = default);
}
