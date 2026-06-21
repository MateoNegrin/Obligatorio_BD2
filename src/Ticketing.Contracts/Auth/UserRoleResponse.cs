namespace Ticketing.Contracts.Auth;

public sealed record UserRoleResponse(
    string NumeroDocumento,
    string Mail,
    string Role); // "General", "Supervisor", "Admin"
