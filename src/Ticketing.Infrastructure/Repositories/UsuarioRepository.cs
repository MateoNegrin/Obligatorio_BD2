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

    public async Task<IReadOnlyList<Usuario>> GetGeneralesAsync(CancellationToken ct = default)
    {
        // Usuarios generales: los que no están en administrador ni en funcionario.
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT u.numero_documento, u.mail, u.pais, u.localidad, u.calle, u.numero_direccion, u.codigo_postal, u.fecha_registro
              FROM usuario u
              LEFT JOIN administrador a ON a.numero_documento = u.numero_documento
              LEFT JOIN funcionario f ON f.numero_documento = u.numero_documento
              WHERE a.numero_documento IS NULL AND f.numero_documento IS NULL
              ORDER BY u.mail",
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

    public async Task CreateAsync(Usuario usuario, IReadOnlyList<string> telefonos = default, CancellationToken ct = default)
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

        // Insertar teléfonos si se proporcionan
        if (telefonos != null && telefonos.Count > 0)
        {
            foreach (var telefono in telefonos)
            {
                if (string.IsNullOrWhiteSpace(telefono)) continue;
                
                await using var cmdTel = new MySqlCommand(
                    "INSERT INTO telefono (usuario_numero_documento, telefono) VALUES (@usuario_numero_documento, @telefono)",
                    (MySqlConnection)conn);
                cmdTel.Parameters.AddWithValue("@usuario_numero_documento", usuario.NumeroDocumento);
                cmdTel.Parameters.AddWithValue("@telefono", telefono);
                await cmdTel.ExecuteNonQueryAsync(ct);
            }
        }
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

    public async Task<string?> GetUserRoleAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        
        // Verificar si es administrador
        await using var cmdAdmin = new MySqlCommand(
            "SELECT numero_documento FROM administrador WHERE numero_documento = @numero_documento",
            (MySqlConnection)conn);
        cmdAdmin.Parameters.AddWithValue("@numero_documento", numeroDocumento);
        await using var readerAdmin = await cmdAdmin.ExecuteReaderAsync(ct);
        if (await readerAdmin.ReadAsync(ct))
            return "Admin";
        
        // Verificar si es funcionario (supervisor)
        await using var cmdFunc = new MySqlCommand(
            "SELECT numero_documento FROM funcionario WHERE numero_documento = @numero_documento",
            (MySqlConnection)conn);
        cmdFunc.Parameters.AddWithValue("@numero_documento", numeroDocumento);
        await using var readerFunc = await cmdFunc.ExecuteReaderAsync(ct);
        if (await readerFunc.ReadAsync(ct))
            return "Supervisor";
        
        // Si no es admin ni funcionario, es usuario general
        return "General";
    }

    public async Task<string?> GetSedeAdministradorAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT nombre_sede FROM administrador WHERE numero_documento = @numero_documento",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento", numeroDocumento);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is null or DBNull ? null : (string)result;
    }

    public async Task<(string NumeroDocumento, string? Role)> GetUserRoleByEmailAsync(string email, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        
        // Query única que obtiene el documento y determina el rol
        await using var cmd = new MySqlCommand(
            @"SELECT 
                u.numero_documento,
                CASE 
                    WHEN a.numero_documento IS NOT NULL THEN 'Admin'
                    WHEN f.numero_documento IS NOT NULL THEN 'Supervisor'
                    ELSE 'General'
                END AS role
              FROM usuario u
              LEFT JOIN administrador a ON u.numero_documento = a.numero_documento
              LEFT JOIN funcionario f ON u.numero_documento = f.numero_documento
              WHERE u.mail = @mail",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@mail", email);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        
        if (await reader.ReadAsync(ct))
        {
            var numeroDocumento = reader.GetString(0);
            var role = reader.IsDBNull(1) ? "General" : reader.GetString(1);
            return (numeroDocumento, role);
        }
        
        return (string.Empty, null); // Usuario no existe
    }
}

