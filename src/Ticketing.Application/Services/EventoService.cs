using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Eventos;
using Ticketing.Domain;

namespace Ticketing.Application.Services;

public interface IEventoService
{
    Task<IReadOnlyList<EventoResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EventoResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CrearEventoRequest request, CancellationToken ct = default);
    Task UpdateAsync(int id, ActualizarEventoRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<SectorHabilitadoResponse>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default);
    Task HabilitarSectorAsync(int idEvento, HabilitarSectorRequest request, CancellationToken ct = default);
}

public sealed class EventoService : IEventoService
{
    private readonly IEventoRepository _repository;

    public EventoService(IEventoRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<EventoResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var eventos = await _repository.GetAllAsync(ct);
        return eventos.Select(e => new EventoResponse(
            e.Id,
            e.IdEquipoLocal,
            e.IdEquipoVisitante,
            e.Fecha,
            e.Hora,
            e.CantidadEntradas
        )).ToList();
    }

    public async Task<EventoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var evento = await _repository.GetByIdAsync(id, ct);
        if (evento is null)
            return null;

        return new EventoResponse(
            evento.Id,
            evento.IdEquipoLocal,
            evento.IdEquipoVisitante,
            evento.Fecha,
            evento.Hora,
            evento.CantidadEntradas
        );
    }

    public async Task<int> CreateAsync(CrearEventoRequest request, CancellationToken ct = default)
    {
        var evento = new EventoDeportivo
        {
            IdEquipoLocal = request.IdEquipoLocal,
            IdEquipoVisitante = request.IdEquipoVisitante,
            Fecha = request.Fecha,
            Hora = request.Hora,
            CantidadEntradas = request.CantidadEntradas
        };

        return await _repository.CreateAsync(evento, ct);
    }

    public async Task UpdateAsync(int id, ActualizarEventoRequest request, CancellationToken ct = default)
    {
        var evento = new EventoDeportivo
        {
            Id = id,
            IdEquipoLocal = request.IdEquipoLocal,
            IdEquipoVisitante = request.IdEquipoVisitante,
            Fecha = request.Fecha,
            Hora = request.Hora,
            CantidadEntradas = request.CantidadEntradas
        };

        await _repository.UpdateAsync(evento, ct);
    }

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);

    public async Task<IReadOnlyList<SectorHabilitadoResponse>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default)
    {
        var sectores = await _repository.GetSectoresHabilitadosAsync(idEvento, ct);
        return sectores.Select(s => new SectorHabilitadoResponse(
            s.IdSector,
            s.IdEventoDeportivo,
            s.NumeroDocumentoAdministrador
        )).ToList();
    }

    public async Task HabilitarSectorAsync(int idEvento, HabilitarSectorRequest request, CancellationToken ct = default)
    {
        var info = new InformacionEntrada
        {
            IdSector = request.IdSector,
            IdEventoDeportivo = idEvento,
            NumeroDocumentoAdministrador = request.NumeroDocumentoAdministrador
        };

        await _repository.HabilitarSectorAsync(info, ct);
    }
}
