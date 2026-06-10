namespace Ticketing.Domain;

public sealed class EventoDeportivo
{
    public int Id { get; set; }
    public int IdEquipoLocal { get; set; }
    public int IdEquipoVisitante { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public int CantidadEntradas { get; set; }
}
