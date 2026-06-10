using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Metricas;

namespace Ticketing.Application.Services;

public interface IMetricaService
{
    Task<IReadOnlyList<EventoMasVendidoResponse>> GetEventosMasVendidosAsync(int top = 10, CancellationToken ct = default);
    Task<IReadOnlyList<MayorCompradorResponse>> GetMayoresCompradoresAsync(int top = 10, CancellationToken ct = default);
}

public sealed class MetricaService : IMetricaService
{
    private readonly IMetricaRepository _repository;

    public MetricaService(IMetricaRepository repository) => _repository = repository;

    public Task<IReadOnlyList<EventoMasVendidoResponse>> GetEventosMasVendidosAsync(int top = 10, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<MayorCompradorResponse>> GetMayoresCompradoresAsync(int top = 10, CancellationToken ct = default)
        => throw new NotImplementedException();
}
