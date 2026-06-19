using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EquipoRepository : IEquipoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EquipoRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<Equipo>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, nombre FROM equipo", (MySqlConnection)conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Equipo>();
        while (await reader.ReadAsync(ct))
            result.Add(new Equipo
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1)
            });
        return result;
    }

    public async Task<Equipo?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, nombre FROM equipo WHERE id = @id", (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return new Equipo { Id = reader.GetInt32(0), Nombre = reader.GetString(1) };
    }

    public async Task<int> CreateAsync(Equipo equipo, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "INSERT INTO equipo (nombre) VALUES (@nombre); SELECT LAST_INSERT_ID();",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@nombre", equipo.Nombre);
        var result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(Equipo equipo, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "UPDATE equipo SET nombre = @nombre WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@nombre", equipo.Nombre);
        cmd.Parameters.AddWithValue("@id", equipo.Id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "DELETE FROM equipo WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
