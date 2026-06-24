using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Transferencias;
using Ticketing.Domain;

namespace Ticketing.Application.Services;

public interface ITransferenciaService
{
    // Transferir entrada (máx. 3 transferencias antes de validar).
    Task TransferirAsync(TransferirEntradaRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TransferenciaResponse>> GetHistorialAsync(int idEntrada, CancellationToken ct = default);
    Task<IReadOnlyList<TransferenciaResponse>> GetByUsuarioAsync(string numeroDocumento, CancellationToken ct = default);
}

public sealed class TransferenciaService : ITransferenciaService
{
    private readonly ITransferenciaRepository _repository;
    private readonly IEntradaRepository _entradaRepository;

    public TransferenciaService(ITransferenciaRepository repository, IEntradaRepository entradaRepository)
    {
        _repository = repository;
        _entradaRepository = entradaRepository;
    }

    public async Task TransferirAsync(TransferirEntradaRequest request, CancellationToken ct = default)
    {
        // No se puede transferir a uno mismo
        if (request.NumeroDocumentoEmisor == request.NumeroDocumentoReceptor)
            throw new InvalidOperationException("No se puede transferir una entrada a uno mismo.");

        // La entrada debe existir.
        var entrada = await _entradaRepository.GetByIdAsync(request.IdEntrada, ct);
        if (entrada is null)
            throw new InvalidOperationException($"La entrada {request.IdEntrada} no existe.");

        // Solo el dueño actual puede transferirla.
        if (entrada.NumeroDocumentoPropietarioActual != request.NumeroDocumentoEmisor)
            throw new InvalidOperationException("Solo el dueño actual de la entrada puede transferirla.");

        // Una entrada ya validada (leída por el supervisor) o anulada no se puede transferir.
        if (entrada.Estado is "Validada" or "Anulada")
            throw new InvalidOperationException($"No se puede transferir una entrada en estado '{entrada.Estado}'.");

        // Validar que no supere 3 transferencias
        var cantTransferencias = await _repository.ContarTransferenciasAsync(request.IdEntrada, ct);
        if (cantTransferencias >= 3)
            throw new InvalidOperationException("La entrada ha alcanzado el límite de 3 transferencias. Requiere validación.");

        var transferencia = new Transferencia
        {
            NumeroDocumentoEmisor = request.NumeroDocumentoEmisor,
            NumeroDocumentoReceptor = request.NumeroDocumentoReceptor,
            IdEntrada = request.IdEntrada,
            Fecha = DateTime.UtcNow
        };
        await _repository.CreateAsync(transferencia, ct);
    }

    public async Task<IReadOnlyList<TransferenciaResponse>> GetHistorialAsync(int idEntrada, CancellationToken ct = default)
    {
        var transferencias = await _repository.GetHistorialAsync(idEntrada, ct);
        return transferencias.Select(t => new TransferenciaResponse(
            t.IdEntrada,
            t.NumeroDocumentoEmisor,
            t.NumeroDocumentoReceptor,
            t.Fecha)).ToList();
    }

    public async Task<IReadOnlyList<TransferenciaResponse>> GetByUsuarioAsync(string numeroDocumento, CancellationToken ct = default)
    {
        var transferencias = await _repository.GetByUsuarioAsync(numeroDocumento, ct);
        return transferencias.Select(t => new TransferenciaResponse(
            t.IdEntrada,
            t.NumeroDocumentoEmisor,
            t.NumeroDocumentoReceptor,
            t.Fecha)).ToList();
    }
}
