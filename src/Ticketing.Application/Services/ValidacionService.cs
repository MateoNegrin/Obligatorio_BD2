using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Validacion;

namespace Ticketing.Application.Services;

public interface IValidacionService
{
    // Validar acceso (escaneo simulado): registra funcionario + código aceptado y marca consumida.
    Task<ValidacionResponse> ValidarAccesoAsync(ValidarAccesoRequest request, CancellationToken ct = default);
}

public sealed class ValidacionService : IValidacionService
{
    private readonly IEntradaRepository _entradas;

    public ValidacionService(IEntradaRepository entradas) => _entradas = entradas;

    // TODO: el esquema actual no modela la entidad de validación/acceso
    // (funcionario validador + código QR aceptado + dispositivo + marca "consumida").
    public Task<ValidacionResponse> ValidarAccesoAsync(ValidarAccesoRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}
