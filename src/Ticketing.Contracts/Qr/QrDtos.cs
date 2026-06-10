namespace Ticketing.Contracts.Qr;

public sealed record QrTokenResponse(int IdEntrada, string Token, DateTime Expira);
