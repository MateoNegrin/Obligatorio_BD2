namespace Ticketing.Contracts.Ventas;

// Una venta agrupa varias entradas (máx. 5 por transacción).
public sealed record CrearVentaRequest(
    string NumeroDocumentoUsuario,
    IReadOnlyList<ItemVentaRequest> Items);

public sealed record ItemVentaRequest(int IdSector, int IdEventoDeportivo);

public sealed record VentaResponse(
    int Id,
    string NumeroDocumentoUsuario,
    DateTime Fecha,
    decimal MontoTotal,
    IReadOnlyList<EntradaVendidaResponse> Entradas);

public sealed record EntradaVendidaResponse(int IdEntrada, int IdSector, int IdEventoDeportivo, decimal Costo);
