using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Entradas;

namespace Ticketing.Application.Services;

public interface IEntradaService
{
    Task<IReadOnlyList<EntradaResponse>> GetDeUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default);
    Task<EntradaResponse?> GetByIdAsync(int id, CancellationToken ct = default);
}

public sealed class EntradaService : IEntradaService
{
    private readonly IEntradaRepository _repository;

    public EntradaService(IEntradaRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<EntradaResponse>> GetDeUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
    {
        var entradas = await _repository.GetByUsuarioAsync(numeroDocumentoUsuario, ct);
        return entradas.Select(e => new EntradaResponse(
            e.Id,
            e.Estado,
            e.Fecha,
            e.QrUsado,
            e.Costo,
            e.IdSector,
            e.IdEventoDeportivo,
            e.IdVenta,
            e.NombreLocal,
            e.NombreVisitante,
            e.FechaEvento,
            e.NombreEstadio,
            e.NombreSector)).ToList();
    }

    public async Task<EntradaResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entrada = await _repository.GetByIdAsync(id, ct);
        if (entrada is null) return null;
        return new EntradaResponse(
            entrada.Id,
            entrada.Estado,
            entrada.Fecha,
            entrada.QrUsado,
            entrada.Costo,
            entrada.IdSector,
            entrada.IdEventoDeportivo,
            entrada.IdVenta,
            entrada.NombreLocal,
            entrada.NombreVisitante,
            entrada.FechaEvento,
            entrada.NombreEstadio,
            entrada.NombreSector);
    }
}
