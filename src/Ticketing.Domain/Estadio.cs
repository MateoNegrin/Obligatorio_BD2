namespace Ticketing.Domain;

public sealed class Estadio
{
    public int Id { get; set; }
    public string NombreSede { get; set; } = string.Empty;
    public int CapacidadMaxima { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
}
