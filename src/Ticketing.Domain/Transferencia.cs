namespace Ticketing.Domain;

public sealed class Transferencia
{
    public string NumeroDocumentoEmisor { get; set; } = string.Empty;
    public string NumeroDocumentoReceptor { get; set; } = string.Empty;
    public int IdEntrada { get; set; }
    public DateTime Fecha { get; set; }
}
