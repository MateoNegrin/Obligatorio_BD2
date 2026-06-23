namespace Ticketing.Domain;

public sealed class Entrada
{
    public int Id { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string EstadoSeed { get; set; } = string.Empty;
    public string? QrUsado { get; set; }
    public decimal Costo { get; set; }
    public int IdSector { get; set; }
    public int IdEventoDeportivo { get; set; }
    public string NumeroDocumentoAdministrador { get; set; } = string.Empty;
    public int IdVenta { get; set; }
    public string NumeroDocumentoPropietarioActual { get; set; } = string.Empty;

    // Campos derivados (no se persisten): provienen de JOINs en la query.
    public string NombreLocal { get; set; } = string.Empty;
    public string NombreVisitante { get; set; } = string.Empty;
    public DateOnly FechaEvento { get; set; }
    public string NombreEstadio { get; set; } = string.Empty;
    public string NombreSector { get; set; } = string.Empty;
}
