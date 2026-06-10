namespace Ticketing.Domain;

public sealed class Administrador
{
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime FechaAsignacion { get; set; }
    public string NombreSede { get; set; } = string.Empty;
}
