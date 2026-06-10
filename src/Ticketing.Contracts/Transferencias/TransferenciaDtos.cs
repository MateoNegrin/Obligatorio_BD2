namespace Ticketing.Contracts.Transferencias;

public sealed record TransferirEntradaRequest(
    int IdEntrada,
    string NumeroDocumentoEmisor,
    string NumeroDocumentoReceptor);

public sealed record AceptarTransferenciaRequest(int IdEntrada, string NumeroDocumentoReceptor);

public sealed record TransferenciaResponse(
    int IdEntrada,
    string NumeroDocumentoEmisor,
    string NumeroDocumentoReceptor,
    DateTime Fecha);
