using Ticketing.Domain;

namespace Ticketing.Application.Abstractions;

public interface IDispositivoRepository
{
    Task<IReadOnlyList<DispositivoAutorizado>> GetByFuncionarioAsync(string numeroDocumento, CancellationToken ct = default);
}
