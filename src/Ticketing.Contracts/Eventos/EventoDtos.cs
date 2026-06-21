namespace Ticketing.Contracts.Eventos;

public sealed record EventoResponse(
    int Id,
    int IdEquipoLocal,
    int IdEquipoVisitante,
    string NombreLocal,
    string NombreVisitante,
    string NombreEstadio,
    DateOnly Fecha,
    TimeOnly Hora,
    int CantidadEntradas,
    int EntradasDisponibles);

// Sector habilitado para un evento con su disponibilidad de entradas.
public sealed record SectorDisponibleResponse(
    int IdSector,
    string Nombre,
    int Capacidad,
    int EntradasVendidas,
    int EntradasDisponibles);

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
