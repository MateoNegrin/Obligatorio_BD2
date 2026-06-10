namespace Ticketing.Contracts.Estadios;

public sealed record EstadioResponse(int Id, string NombreSede, int CapacidadMaxima, string Ubicacion);

public sealed record CrearEstadioRequest(string NombreSede, int CapacidadMaxima, string Ubicacion);

public sealed record ActualizarEstadioRequest(string NombreSede, int CapacidadMaxima, string Ubicacion);

public sealed record SectorResponse(int Id, int IdEstadio, string Nombre, int Capacidad);

public sealed record CrearSectorRequest(int IdEstadio, string Nombre, int Capacidad);
