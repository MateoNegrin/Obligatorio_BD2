using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal, fecha_registro FROM usuario",
            (MySqlConnection)conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Usuario>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new Usuario
            {
                NumeroDocumento = reader.GetString(0),
                Mail = reader.GetString(1),
                Pais = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Localidad = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Calle = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                NumeroDireccion = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                CodigoPostal = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                FechaRegistro = reader.GetDateTime(7)
            });
        }
        return result;
    }

    public async Task<Usuario?> GetByDocumentoAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal, fecha_registro FROM usuario WHERE numero_documento = @numero_documento",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento", numeroDocumento);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            return new Usuario
            {
                NumeroDocumento = reader.GetString(0),
                Mail = reader.GetString(1),
                Pais = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Localidad = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Calle = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                NumeroDireccion = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                CodigoPostal = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                FechaRegistro = reader.GetDateTime(7)
            };
        }
        return null;
    }

    public async Task CreateAsync(Usuario usuario, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "INSERT INTO usuario (numero_documento, mail, pais, localidad, calle, numero_direccion, codigo_postal) VALUES (@numero_documento, @mail, @pais, @localidad, @calle, @numero_direccion, @codigo_postal)",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento", usuario.NumeroDocumento);
        cmd.Parameters.AddWithValue("@mail", usuario.Mail);
        cmd.Parameters.AddWithValue("@pais", usuario.Pais);
        cmd.Parameters.AddWithValue("@localidad", usuario.Localidad);
        cmd.Parameters.AddWithValue("@calle", usuario.Calle);
        cmd.Parameters.AddWithValue("@numero_direccion", usuario.NumeroDireccion);
        cmd.Parameters.AddWithValue("@codigo_postal", usuario.CodigoPostal);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "UPDATE usuario SET mail = @mail, pais = @pais, localidad = @localidad, calle = @calle, numero_direccion = @numero_direccion, codigo_postal = @codigo_postal WHERE numero_documento = @numero_documento",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento", usuario.NumeroDocumento);
        cmd.Parameters.AddWithValue("@mail", usuario.Mail);
        cmd.Parameters.AddWithValue("@pais", usuario.Pais);
        cmd.Parameters.AddWithValue("@localidad", usuario.Localidad);
        cmd.Parameters.AddWithValue("@calle", usuario.Calle);
        cmd.Parameters.AddWithValue("@numero_direccion", usuario.NumeroDireccion);
        cmd.Parameters.AddWithValue("@codigo_postal", usuario.CodigoPostal);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "DELETE FROM usuario WHERE numero_documento = @numero_documento",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento", numeroDocumento);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
