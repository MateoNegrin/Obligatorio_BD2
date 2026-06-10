using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Transferencias;

namespace Ticketing.Application.Services;

public interface ITransferenciaService
{
    // Transferir entrada (máx. 3 transferencias antes de validar).
    Task TransferirAsync(TransferirEntradaRequest request, CancellationToken ct = default);
    Task AceptarAsync(AceptarTransferenciaRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TransferenciaResponse>> GetHistorialAsync(int idEntrada, CancellationToken ct = default);
}

public sealed class TransferenciaService : ITransferenciaService
{
    private readonly ITransferenciaRepository _repository;

    public TransferenciaService(ITransferenciaRepository repository) => _repository = repository;

    public Task TransferirAsync(TransferirEntradaRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task AceptarAsync(AceptarTransferenciaRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<TransferenciaResponse>> GetHistorialAsync(int idEntrada, CancellationToken ct = default)
        => throw new NotImplementedException();
}
