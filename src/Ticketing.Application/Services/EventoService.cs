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
    Task<IReadOnlyList<SectorDisponibleResponse>> GetSectoresDisponiblesAsync(int idEvento, CancellationToken ct = default);
    Task HabilitarSectorAsync(int idEvento, HabilitarSectorRequest request, CancellationToken ct = default);
}

public sealed class EventoService : IEventoService
{
    private readonly IEventoRepository _repository;
    private readonly IUsuarioRepository _usuarios;

    public EventoService(IEventoRepository repository, IUsuarioRepository usuarios)
    {
        _repository = repository;
        _usuarios = usuarios;
    }

    public async Task<IReadOnlyList<EventoResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var eventos = await _repository.GetAllAsync(ct);
        return eventos.Select(e => new EventoResponse(
            e.Id,
            e.IdEquipoLocal,
            e.IdEquipoVisitante,
            e.NombreLocal,
            e.NombreVisitante,
            e.NombreEstadio,
            e.Fecha,
            e.Hora,
            e.CantidadEntradas,
            e.EntradasDisponibles
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
            evento.NombreLocal,
            evento.NombreVisitante,
            evento.NombreEstadio,
            evento.Fecha,
            evento.Hora,
            evento.CantidadEntradas,
            evento.EntradasDisponibles
        );
    }

    public async Task<int> CreateAsync(CrearEventoRequest request, CancellationToken ct = default)
    {
        var idSectores = request.IdSectoresHabilitados?.Distinct().ToList() ?? [];
        if (idSectores.Count == 0)
            throw new InvalidOperationException("Debe habilitar al menos un sector.");

        // El evento solo puede crearse en un estadio de la sede del administrador.
        var sede = await _usuarios.GetSedeAdministradorAsync(request.NumeroDocumentoAdministrador, ct)
            ?? throw new InvalidOperationException("El usuario indicado no es un administrador válido.");

        var sectores = await _repository.GetSectoresConSedeAsync(idSectores, ct);
        if (sectores.Count != idSectores.Count)
            throw new InvalidOperationException("Alguno de los sectores indicados no existe.");
        if (sectores.Select(s => s.IdEstadio).Distinct().Count() > 1)
            throw new InvalidOperationException("Los sectores habilitados deben pertenecer al mismo estadio.");
        if (sectores.Any(s => s.NombreSede != sede))
            throw new InvalidOperationException("Solo puede crear eventos en estadios de su sede.");

        var evento = new EventoDeportivo
        {
            IdEquipoLocal = request.IdEquipoLocal,
            IdEquipoVisitante = request.IdEquipoVisitante,
            Fecha = request.Fecha,
            Hora = request.Hora,
            // Las entradas disponibles son la suma de las capacidades de los sectores habilitados.
            CantidadEntradas = sectores.Sum(s => s.Capacidad)
        };

        return await _repository.CreateConSectoresAsync(evento, idSectores, request.NumeroDocumentoAdministrador, ct);
    }

    public async Task UpdateAsync(int id, ActualizarEventoRequest request, CancellationToken ct = default)
    {
        // Solo se modifican datos básicos: los sectores habilitados no se tocan para no
        // afectar entradas ya vendidas.
        var evento = new EventoDeportivo
        {
            Id = id,
            IdEquipoLocal = request.IdEquipoLocal,
            IdEquipoVisitante = request.IdEquipoVisitante,
            Fecha = request.Fecha,
            Hora = request.Hora
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

    public async Task<IReadOnlyList<SectorDisponibleResponse>> GetSectoresDisponiblesAsync(int idEvento, CancellationToken ct = default)
    {
        var sectores = await _repository.GetSectoresDisponiblesAsync(idEvento, ct);
        return sectores.Select(s => new SectorDisponibleResponse(
            s.IdSector,
            s.Nombre,
            s.Capacidad,
            s.EntradasVendidas,
            s.EntradasDisponibles
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
