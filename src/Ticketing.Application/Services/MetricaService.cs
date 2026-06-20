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

    public async Task<IReadOnlyList<EventoMasVendidoResponse>> GetEventosMasVendidosAsync(int top = 10, CancellationToken ct = default)
    {
        var eventos = await _repository.GetEventosMasVendidosAsync(top, ct);
        return eventos.Select(e => new EventoMasVendidoResponse(e.IdEventoDeportivo, e.EntradasVendidas)).ToList();
    }

    public async Task<IReadOnlyList<MayorCompradorResponse>> GetMayoresCompradoresAsync(int top = 10, CancellationToken ct = default)
    {
        var compradores = await _repository.GetMayoresCompradoresAsync(top, ct);
        return compradores.Select(c => new MayorCompradorResponse(c.NumeroDocumento, c.Mail, c.EntradasCompradas)).ToList();
    }
}
