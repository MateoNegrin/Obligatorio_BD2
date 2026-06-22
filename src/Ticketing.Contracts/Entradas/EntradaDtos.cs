namespace Ticketing.Contracts.Entradas;

public sealed record EntradaResponse(
    int Id,
    string Estado,
    DateTime Fecha,
    string? QrUsado,
    decimal Costo,
    int IdSector,
    int IdEventoDeportivo,
    int IdVenta,
    string NombreLocal,
    string NombreVisitante,
    DateOnly FechaEvento,
    string NombreEstadio,
    string NombreSector);
