namespace Ticketing.Contracts.Equipos;

public sealed record EquipoResponse(int Id, string Nombre);

public sealed record CrearEquipoRequest(string Nombre);

public sealed record ActualizarEquipoRequest(string Nombre);
