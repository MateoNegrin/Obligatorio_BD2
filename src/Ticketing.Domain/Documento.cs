namespace Ticketing.Domain;

public sealed class Documento
{
    public string UsuarioNumeroDocumento { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}
