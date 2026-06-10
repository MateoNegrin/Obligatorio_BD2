namespace Ticketing.Contracts.Rbac;

// TODO: el RBAC dinámico (Rol, Permiso, RolPermiso, UsuarioRol, LogUsuario)
// no está presente en el esquema actual de la sección 9. DTOs declarados como base.

public sealed record RolResponse(int Id, string Nombre);
public sealed record CrearRolRequest(string Nombre);
public sealed record ActualizarRolRequest(string Nombre);

public sealed record PermisoResponse(int Id, string Nombre);
public sealed record CrearPermisoRequest(string Nombre);
public sealed record ActualizarPermisoRequest(string Nombre);

public sealed record AsignarPermisoARolRequest(int IdRol, int IdPermiso);
public sealed record AsignarRolAUsuarioRequest(string NumeroDocumentoUsuario, int IdRol);

public sealed record LogUsuarioResponse(long Id, string NumeroDocumentoUsuario, string Accion, DateTime Fecha);
