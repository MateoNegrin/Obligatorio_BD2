namespace Ticketing.Domain;

public sealed class Usuario
{
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Localidad { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumeroDireccion { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
}
