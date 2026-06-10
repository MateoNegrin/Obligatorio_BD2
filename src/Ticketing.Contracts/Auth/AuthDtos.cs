namespace Ticketing.Contracts.Auth;

public sealed record RegistrarUsuarioRequest(
    string NumeroDocumento,
    string Mail,
    string Password,
    string Pais,
    string Localidad,
    string Calle,
    string NumeroDireccion,
    string CodigoPostal);

public sealed record LoginRequest(string Mail, string Password);

public sealed record LoginResponse(string NumeroDocumento, string Mail, string Token);
