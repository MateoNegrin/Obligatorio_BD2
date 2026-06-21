namespace Ticketing.Domain;

// Proyección de lectura: un sector habilitado para un evento con su disponibilidad.
// No se persiste; se arma desde un JOIN entre informacion_entrada, sector y entrada.
public sealed class SectorDisponibilidad
{
    public int IdSector { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public int EntradasVendidas { get; set; }
    public int EntradasDisponibles => Capacidad - EntradasVendidas;
}
