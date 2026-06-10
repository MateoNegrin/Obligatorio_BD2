namespace Ticketing.Contracts.Metricas;

public sealed record EventoMasVendidoResponse(int IdEventoDeportivo, int EntradasVendidas);

public sealed record MayorCompradorResponse(string NumeroDocumentoUsuario, string Mail, int EntradasCompradas);
