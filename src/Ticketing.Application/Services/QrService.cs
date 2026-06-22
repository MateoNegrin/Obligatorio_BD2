using System.Security.Cryptography;
using System.Text;
using Ticketing.Application.Abstractions;
using Ticketing.Contracts.Qr;

namespace Ticketing.Application.Services;

public interface IQrService
{
    Task<QrTokenResponse> GenerarTokenAsync(int idEntrada, CancellationToken ct = default);
    string GenerarCodigo(string seed);
}

public sealed class QrService : IQrService
{
    private readonly IEntradaRepository _entradas;

    public QrService(IEntradaRepository entradas) => _entradas = entradas;

    public async Task<QrTokenResponse> GenerarTokenAsync(int idEntrada, CancellationToken ct = default)
    {
        var entrada = await _entradas.GetByIdAsync(idEntrada, ct)
            ?? throw new InvalidOperationException("Entrada no encontrada");

        var token = GenerarCodigo(entrada.EstadoSeed);
        var expira = DateTime.UtcNow.AddSeconds(30 - DateTime.UtcNow.Second % 30);

        return new QrTokenResponse(idEntrada, token, expira);
    }

    public string GenerarCodigo(string seed)
    {
        // Ventana temporal de 30 segundos (igual que TOTP)
        var ventana = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        var data = Encoding.UTF8.GetBytes(ventana.ToString());
        var key = Encoding.UTF8.GetBytes(seed);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(data);
        return Convert.ToHexString(hash)[..16]; // 16 caracteres es suficiente
    }
}

