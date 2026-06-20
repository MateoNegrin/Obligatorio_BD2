namespace Ticketing.Contracts.Usuarios;

public sealed record UsuarioResponse(
    string NumeroDocumento,
    string Mail,
    string Pais,
    string Localidad,
    string Calle,
    string NumeroDireccion,
    string CodigoPostal,
    DateTime FechaRegistro);

public sealed record CrearUsuarioRequest(
    string NumeroDocumento,
    string Mail,
    string Pais,
    string Localidad,
    string Calle,
    string NumeroDireccion,
    string CodigoPostal);

public sealed record SignUpUsuarioRequest(
    string NumeroDocumento,
    string Mail,
    string Pais,
    string Localidad,
    string Calle,
    string NumeroDireccion,
    string CodigoPostal,
    string Password);

public sealed record ActualizarUsuarioRequest(
    string Mail,
    string Pais,
    string Localidad,
    string Calle,
    string NumeroDireccion,
    string CodigoPostal);
