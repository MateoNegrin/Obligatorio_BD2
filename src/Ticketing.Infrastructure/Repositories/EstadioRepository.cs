using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class EstadioRepository : IEstadioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EstadioRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<Estadio>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, nombre_sede, capacidad_maxima, ubicacion FROM estadio",
            (MySqlConnection)conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Estadio>();
        while (await reader.ReadAsync(ct))
            result.Add(new Estadio
            {
                Id = reader.GetInt32(0),
                NombreSede = reader.GetString(1),
                CapacidadMaxima = reader.GetInt32(2),
                Ubicacion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
            });
        return result;
    }

    public async Task<Estadio?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, nombre_sede, capacidad_maxima, ubicacion FROM estadio WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return new Estadio
        {
            Id = reader.GetInt32(0),
            NombreSede = reader.GetString(1),
            CapacidadMaxima = reader.GetInt32(2),
            Ubicacion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
        };
    }

    public async Task<int> CreateAsync(Estadio estadio, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "INSERT INTO estadio (nombre_sede, capacidad_maxima, ubicacion) VALUES (@nombre_sede, @capacidad_maxima, @ubicacion); SELECT LAST_INSERT_ID();",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@nombre_sede", estadio.NombreSede);
        cmd.Parameters.AddWithValue("@capacidad_maxima", estadio.CapacidadMaxima);
        cmd.Parameters.AddWithValue("@ubicacion", estadio.Ubicacion ?? string.Empty);
        var result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(Estadio estadio, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "UPDATE estadio SET nombre_sede = @nombre_sede, capacidad_maxima = @capacidad_maxima, ubicacion = @ubicacion WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@nombre_sede", estadio.NombreSede);
        cmd.Parameters.AddWithValue("@capacidad_maxima", estadio.CapacidadMaxima);
        cmd.Parameters.AddWithValue("@ubicacion", estadio.Ubicacion ?? string.Empty);
        cmd.Parameters.AddWithValue("@id", estadio.Id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "DELETE FROM estadio WHERE id = @id",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<int> CreateConSectoresAsync(Estadio estadio, IReadOnlyList<Sector> sectores, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        var mysqlConn = (MySqlConnection)conn;
        await using var transaction = await mysqlConn.BeginTransactionAsync(ct);

        try
        {
            await using var cmdEstadio = new MySqlCommand(
                "INSERT INTO estadio (nombre_sede, capacidad_maxima, ubicacion) VALUES (@nombre_sede, @capacidad_maxima, @ubicacion); SELECT LAST_INSERT_ID();",
                mysqlConn, transaction);
            cmdEstadio.Parameters.AddWithValue("@nombre_sede", estadio.NombreSede);
            cmdEstadio.Parameters.AddWithValue("@capacidad_maxima", estadio.CapacidadMaxima);
            cmdEstadio.Parameters.AddWithValue("@ubicacion", estadio.Ubicacion ?? string.Empty);
            var idEstadio = Convert.ToInt32(await cmdEstadio.ExecuteScalarAsync(ct));

            foreach (var sector in sectores)
            {
                await using var cmdSector = new MySqlCommand(
                    "INSERT INTO sector (id_estadio, nombre, capacidad) VALUES (@id_estadio, @nombre, @capacidad)",
                    mysqlConn, transaction);
                cmdSector.Parameters.AddWithValue("@id_estadio", idEstadio);
                cmdSector.Parameters.AddWithValue("@nombre", sector.Nombre);
                cmdSector.Parameters.AddWithValue("@capacidad", sector.Capacidad);
                await cmdSector.ExecuteNonQueryAsync(ct);
            }

            await transaction.CommitAsync(ct);
            return idEstadio;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateConSectoresAsync(Estadio estadio, IReadOnlyList<Sector> sectores, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        var mysqlConn = (MySqlConnection)conn;
        await using var transaction = await mysqlConn.BeginTransactionAsync(ct);

        try
        {
            await using var cmdEstadio = new MySqlCommand(
                "UPDATE estadio SET capacidad_maxima = @capacidad_maxima, ubicacion = @ubicacion WHERE id = @id",
                mysqlConn, transaction);
            cmdEstadio.Parameters.AddWithValue("@capacidad_maxima", estadio.CapacidadMaxima);
            cmdEstadio.Parameters.AddWithValue("@ubicacion", estadio.Ubicacion ?? string.Empty);
            cmdEstadio.Parameters.AddWithValue("@id", estadio.Id);
            await cmdEstadio.ExecuteNonQueryAsync(ct);

            foreach (var sector in sectores)
            {
                await using var cmdSector = new MySqlCommand(
                    "UPDATE sector SET nombre = @nombre, capacidad = @capacidad WHERE id = @id AND id_estadio = @id_estadio",
                    mysqlConn, transaction);
                cmdSector.Parameters.AddWithValue("@nombre", sector.Nombre);
                cmdSector.Parameters.AddWithValue("@capacidad", sector.Capacidad);
                cmdSector.Parameters.AddWithValue("@id", sector.Id);
                cmdSector.Parameters.AddWithValue("@id_estadio", estadio.Id);
                await cmdSector.ExecuteNonQueryAsync(ct);
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<IReadOnlyList<Sector>> GetSectoresAsync(int idEstadio, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT id, id_estadio, nombre, capacidad FROM sector WHERE id_estadio = @id_estadio",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id_estadio", idEstadio);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Sector>();
        while (await reader.ReadAsync(ct))
            result.Add(new Sector
            {
                Id = reader.GetInt32(0),
                IdEstadio = reader.GetInt32(1),
                Nombre = reader.GetString(2),
                Capacidad = reader.GetInt32(3)
            });
        return result;
    }

    public async Task<int> CreateSectorAsync(Sector sector, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "INSERT INTO sector (id_estadio, nombre, capacidad) VALUES (@id_estadio, @nombre, @capacidad); SELECT LAST_INSERT_ID();",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id_estadio", sector.IdEstadio);
        cmd.Parameters.AddWithValue("@nombre", sector.Nombre);
        cmd.Parameters.AddWithValue("@capacidad", sector.Capacidad);
        var result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }
}
