namespace Ticketing.Contracts.Estadios;

public sealed record EstadioResponse(int Id, string NombreSede, int CapacidadMaxima, string Ubicacion);

// Sector indicado al crear un estadio (nombre + capacidad). La capacidad del estadio
// se calcula sumando la capacidad de los 4 sectores.
public sealed record SectorInput(string Nombre, int Capacidad);

// La sede se toma del administrador (su sede asignada), no se recibe del cliente.
public sealed record CrearEstadioRequest(
    string NumeroDocumentoAdministrador,
    string Ubicacion,
    IReadOnlyList<SectorInput> Sectores);

// Sector existente a modificar (por id).
public sealed record SectorUpdate(int Id, string Nombre, int Capacidad);

// Modificación: ubicación + nombres/capacidades de los sectores. La capacidad del estadio
// se recalcula sumando los sectores. La sede no se modifica.
public sealed record ActualizarEstadioRequest(
    string Ubicacion,
    IReadOnlyList<SectorUpdate> Sectores);

public sealed record SectorResponse(int Id, int IdEstadio, string Nombre, int Capacidad);

public sealed record CrearSectorRequest(int IdEstadio, string Nombre, int Capacidad);
