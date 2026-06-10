namespace Ticketing.Domain;

public sealed class DispositivoAutorizado
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
}
