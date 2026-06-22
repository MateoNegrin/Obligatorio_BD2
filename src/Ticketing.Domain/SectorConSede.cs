namespace Ticketing.Domain;

// Sector con la sede del estadio al que pertenece. Se usa para validar, al crear un evento,
// que los sectores habilitados pertenezcan a un estadio de la sede del administrador.
public sealed class SectorConSede
{
    public int IdSector { get; set; }
    public int Capacidad { get; set; }
    public int IdEstadio { get; set; }
    public string NombreSede { get; set; } = string.Empty;
}
