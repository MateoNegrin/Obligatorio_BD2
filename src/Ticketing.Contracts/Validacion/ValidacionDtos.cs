namespace Ticketing.Contracts.Validacion;

public sealed record ValidarAccesoRequest(
    int IdEntrada,
    int IdDispositivo);

public sealed record ValidacionResponse(int IdEntrada, bool Aceptado, string Mensaje, string? CodigoUsado);
