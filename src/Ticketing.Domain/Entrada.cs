namespace Ticketing.Domain;

public sealed class Entrada
{
    public int Id { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string EstadoSeed { get; set; } = string.Empty;
    public bool QrUsado { get; set; }
    public decimal Costo { get; set; }
    public int IdSector { get; set; }
    public int IdEventoDeportivo { get; set; }
    public string NumeroDocumentoAdministrador { get; set; } = string.Empty;
    public int IdVenta { get; set; }
}
