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

// El evento se crea habilitando un subconjunto de los sectores de un estadio (mínimo 1).
// La sede se valida contra la del administrador; cantidad_entradas se calcula sumando
// la capacidad de los sectores habilitados.
public sealed record CrearEventoRequest(
    string NumeroDocumentoAdministrador,
    int IdEquipoLocal,
    int IdEquipoVisitante,
    DateOnly Fecha,
    TimeOnly Hora,
    IReadOnlyList<int> IdSectoresHabilitados);

// Al modificar solo se cambian los datos básicos: los sectores habilitados no se tocan
// (podrían existir entradas vendidas para ellos).
public sealed record ActualizarEventoRequest(
    int IdEquipoLocal,
    int IdEquipoVisitante,
    DateOnly Fecha,
    TimeOnly Hora);

// Sectores habilitados por evento (InformacionEntrada).
public sealed record SectorHabilitadoResponse(int IdSector, int IdEventoDeportivo, string NumeroDocumentoAdministrador);

public sealed record HabilitarSectorRequest(int IdSector, string NumeroDocumentoAdministrador);
