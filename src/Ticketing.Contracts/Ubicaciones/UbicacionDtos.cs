namespace Ticketing.Contracts.Ubicaciones;

// TODO: País y Localidad no son tablas en el esquema actual (son columnas de Usuario).
// DTOs declarados como base para los endpoints de Paises/Localidades.

public sealed record PaisResponse(int Id, string Nombre);
public sealed record CrearPaisRequest(string Nombre);

public sealed record LocalidadResponse(int Id, string Nombre, int IdPais);
public sealed record CrearLocalidadRequest(string Nombre, int IdPais);
