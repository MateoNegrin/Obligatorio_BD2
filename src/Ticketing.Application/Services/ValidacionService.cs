using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Validacion;

namespace Ticketing.Application.Services;

public interface IValidacionService
{
    Task<ValidacionResponse> ValidarAccesoAsync(ValidarAccesoRequest request, CancellationToken ct = default);
}

public sealed class ValidacionService : IValidacionService
{
    private readonly IEntradaRepository _entradas;
    private readonly IQrService _qrService;

    public ValidacionService(IEntradaRepository entradas, IQrService qrService)
    {
        _entradas = entradas;
        _qrService = qrService;
    }

    public async Task<ValidacionResponse> ValidarAccesoAsync(ValidarAccesoRequest request, CancellationToken ct = default)
    {
        var entrada = await _entradas.GetByIdAsync(request.IdEntrada, ct);

        if (entrada is null)
            return new ValidacionResponse(request.IdEntrada, false, "Entrada no encontrada.", null);

        if (entrada.QrUsado is not null)
            return new ValidacionResponse(request.IdEntrada, false, "La entrada ya fue validada.", null);

        var codigo = _qrService.GenerarCodigo(entrada.EstadoSeed);

        await _entradas.ValidarEntradaAsync(request.IdEntrada, codigo, request.IdDispositivo, ct);

        return new ValidacionResponse(request.IdEntrada, true, "Acceso validado correctamente.", codigo);
    }
}
