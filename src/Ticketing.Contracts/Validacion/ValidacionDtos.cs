namespace Ticketing.Contracts.Validacion;

// Escaneo simulado: funcionario valida el acceso de una entrada.
public sealed record ValidarAccesoRequest(
    int IdEntrada,
    string CodigoQr,
    string NumeroDocumentoFuncionario,
    int IdDispositivo);

public sealed record ValidacionResponse(int IdEntrada, bool Aceptado, string Mensaje);
