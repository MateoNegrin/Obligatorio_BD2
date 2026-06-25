using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IEventoRepository
{
    Task<IReadOnlyList<EventoDeportivo>> GetAllAsync(CancellationToken ct = default);
    Task<EventoDeportivo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(EventoDeportivo evento, CancellationToken ct = default);
    Task UpdateAsync(EventoDeportivo evento, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    // Crea el evento y habilita los sectores indicados (informacion_entrada) en una transacción.
    Task<int> CreateConSectoresAsync(EventoDeportivo evento, IReadOnlyList<int> idSectores, string numeroDocumentoAdministrador, CancellationToken ct = default);

    // Devuelve los sectores indicados con la sede de su estadio (para validación al crear eventos).
    Task<IReadOnlyList<SectorConSede>> GetSectoresConSedeAsync(IReadOnlyList<int> idSectores, CancellationToken ct = default);

    // Indica si ya existe un evento en el estadio indicado, en la misma fecha, a menos de 2hs del horario dado.
    Task<bool> ExisteEventoEnEstadioCercaDeAsync(int idEstadio, DateOnly fecha, TimeOnly hora, CancellationToken ct = default);

    Task<IReadOnlyList<InformacionEntrada>> GetSectoresHabilitadosAsync(int idEvento, CancellationToken ct = default);
    Task<IReadOnlyList<SectorDisponibilidad>> GetSectoresDisponiblesAsync(int idEvento, CancellationToken ct = default);
    Task HabilitarSectorAsync(InformacionEntrada info, CancellationToken ct = default);
}
