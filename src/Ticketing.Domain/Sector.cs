namespace Ticketing.Domain;

public sealed class Sector
{
    public int Id { get; set; }
    public int IdEstadio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Capacidad { get; set; }
}
