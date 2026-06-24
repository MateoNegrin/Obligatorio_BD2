using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class TransferenciaRepository : ITransferenciaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferenciaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<Transferencia>> GetHistorialAsync(int idEntrada, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT numero_documento_emisor, numero_documento_receptor, id_entrada, fecha FROM transferencia WHERE id_entrada = @id_entrada ORDER BY fecha DESC",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id_entrada", idEntrada);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Transferencia>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new Transferencia
            {
                NumeroDocumentoEmisor = reader.GetString(0),
                NumeroDocumentoReceptor = reader.GetString(1),
                IdEntrada = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3)
            });
        }
        return result;
    }

    public async Task<int> ContarTransferenciasAsync(int idEntrada, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM transferencia WHERE id_entrada = @id_entrada",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id_entrada", idEntrada);
        var result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    // Registra la transferencia y traspasa la propiedad de la entrada al receptor.
    // Ambas operaciones van en una transacción: o se aplican las dos, o ninguna.
    public async Task CreateAsync(Transferencia transferencia, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        var mysqlConn = (MySqlConnection)conn;
        await using var transaction = await mysqlConn.BeginTransactionAsync(ct);

        try
        {
            await using var cmdInsert = new MySqlCommand(
                "INSERT INTO transferencia (numero_documento_emisor, numero_documento_receptor, id_entrada, fecha) VALUES (@numero_documento_emisor, @numero_documento_receptor, @id_entrada, NOW())",
                mysqlConn, transaction);
            cmdInsert.Parameters.AddWithValue("@numero_documento_emisor", transferencia.NumeroDocumentoEmisor);
            cmdInsert.Parameters.AddWithValue("@numero_documento_receptor", transferencia.NumeroDocumentoReceptor);
            cmdInsert.Parameters.AddWithValue("@id_entrada", transferencia.IdEntrada);
            await cmdInsert.ExecuteNonQueryAsync(ct);

            await using var cmdUpdate = new MySqlCommand(
                "UPDATE entrada SET numero_documento_propietario_actual = @receptor WHERE id = @id_entrada",
                mysqlConn, transaction);
            cmdUpdate.Parameters.AddWithValue("@receptor", transferencia.NumeroDocumentoReceptor);
            cmdUpdate.Parameters.AddWithValue("@id_entrada", transferencia.IdEntrada);
            await cmdUpdate.ExecuteNonQueryAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<IReadOnlyList<Transferencia>> GetByUsuarioAsync(string numeroDocumento, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            @"SELECT numero_documento_emisor, numero_documento_receptor, id_entrada, fecha 
              FROM transferencia 
              WHERE numero_documento_emisor = @numero_documento OR numero_documento_receptor = @numero_documento
              ORDER BY fecha DESC",
            (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@numero_documento", numeroDocumento);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var result = new List<Transferencia>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new Transferencia
            {
                NumeroDocumentoEmisor = reader.GetString(0),
                NumeroDocumentoReceptor = reader.GetString(1),
                IdEntrada = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3)
            });
        }
        return result;
    }
}
