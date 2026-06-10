namespace Ticketing.Contracts.Eventos;

public sealed record EventoResponse(
    int Id,
    int IdEquipoLocal,
    int IdEquipoVisitante,
    DateOnly Fecha,
    TimeOnly Hora,
    int CantidadEntradas);

public sealed record CrearEventoRequest(
    int IdEquipoLocal,
    int IdEquipoVisitante,
    DateOnly Fecha,
    TimeOnly Hora,
    int CantidadEntradas);

public sealed record ActualizarEventoRequest(
    int IdEquipoLocal,
    int IdEquipoVisitante,
    DateOnly Fecha,
    TimeOnly Hora,
    int CantidadEntradas);

// Sectores habilitados por evento (InformacionEntrada).
public sealed record SectorHabilitadoResponse(int IdSector, int IdEventoDeportivo, string NumeroDocumentoAdministrador);

public sealed record HabilitarSectorRequest(int IdSector, string NumeroDocumentoAdministrador);
