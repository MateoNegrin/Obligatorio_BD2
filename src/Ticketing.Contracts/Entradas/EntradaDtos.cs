namespace Ticketing.Contracts.Entradas;

public sealed record EntradaResponse(
    int Id,
    string Estado,
    DateTime Fecha,
    bool QrUsado,
    decimal Costo,
    int IdSector,
    int IdEventoDeportivo,
    int IdVenta);
