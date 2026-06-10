namespace Ticketing.Domain;

public sealed class Venta
{
    public int Id { get; set; }
    public string NumeroDocumentoUsuario { get; set; } = string.Empty;
    public int IdComision { get; set; }
    public DateTime Fecha { get; set; }
    public decimal MontoTotal { get; set; }
}
