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

    public Task<IReadOnlyList<EntradaResponse>> GetDeUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<EntradaResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
