namespace Ticketing.Domain;

// Sectores habilitados por evento (PK compuesta: sector + evento).
public sealed class InformacionEntrada
{
    public int IdSector { get; set; }
    public int IdEventoDeportivo { get; set; }
    public string NumeroDocumentoAdministrador { get; set; } = string.Empty;
}
