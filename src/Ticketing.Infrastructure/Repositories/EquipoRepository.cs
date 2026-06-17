using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EquipoRepository : IEquipoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EquipoRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public Task<IReadOnlyList<Equipo>> GetAllAsync(CancellationToken ct = default)
        // Patrón ADO.NET esperado a completar luego:
        // await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        // await using var cmd = new MySqlCommand(
        //     "SELECT id, nombre FROM equipo", (MySqlConnection)conn);
        // await using var reader = await cmd.ExecuteReaderAsync(ct);
        // var result = new List<Equipo>();
        // while (await reader.ReadAsync(ct))
        //     result.Add(new Equipo
        //     {
        //         Id = reader.GetInt32(0),
        //         Nombre = reader.GetString(1)
        //     });
        // return result;
        => throw new NotImplementedException();

    public Task<Equipo?> GetByIdAsync(int id, CancellationToken ct = default)
        // Patrón con parámetro (anti SQL injection):
        // await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        // await using var cmd = new MySqlCommand(
        //     "SELECT id, nombre FROM equipo WHERE id = @id", (MySqlConnection)conn);
        // cmd.Parameters.AddWithValue("id", id);
        // await using var reader = await cmd.ExecuteReaderAsync(ct);
        // if (!await reader.ReadAsync(ct)) return null;
        // return new Equipo { Id = reader.GetInt32(0), Nombre = reader.GetString(1) };
        => throw new NotImplementedException();

    public Task<int> CreateAsync(Equipo equipo, CancellationToken ct = default)
        // INSERT ... RETURNING id; usar cmd.ExecuteScalarAsync(ct).
        => throw new NotImplementedException();

    public Task UpdateAsync(Equipo equipo, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
