using MySql.Data.MySqlClient;
using Ticketing.Application.Abstractions;
using Ticketing.Domain;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Repositories;

public sealed class VentaRepository : IVentaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public VentaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<Venta>> GetByUsuarioAsync(string numeroDocumentoUsuario, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            """
            SELECT id, numero_documento_usuario, id_comision, fecha, monto_total
            FROM venta
            WHERE numero_documento_usuario = @numeroDocumento
            ORDER BY fecha DESC
            """,
            (MySqlConnection)conn);
        
        cmd.Parameters.AddWithValue("@numeroDocumento", numeroDocumentoUsuario);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        
        var result = new List<Venta>();
        while (await reader.ReadAsync(ct))
        {
            result.Add(new Venta
            {
                Id = reader.GetInt32(0),
                NumeroDocumentoUsuario = reader.GetString(1),
                IdComision = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                MontoTotal = reader.GetDecimal(4)
            });
        }
        
        return result;
    }

    public async Task<Venta?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        await using var cmd = new MySqlCommand(
            """
            SELECT id, numero_documento_usuario, id_comision, fecha, monto_total
            FROM venta
            WHERE id = @id
            """,
            (MySqlConnection)conn);
        
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        
        if (await reader.ReadAsync(ct))
        {
            return new Venta
            {
                Id = reader.GetInt32(0),
                NumeroDocumentoUsuario = reader.GetString(1),
                IdComision = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                MontoTotal = reader.GetDecimal(4)
            };
        }
        
        return null;
    }

    // La venta + sus entradas deben crearse dentro de una transacción (MySqlTransaction).
    public async Task<int> CreateAsync(Venta venta, IReadOnlyList<Entrada> entradas, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        var mysqlConn = (MySqlConnection)conn;
        
        await using var transaction = await mysqlConn.BeginTransactionAsync(ct);
        
        try
        {
            // Insertar venta
            await using var cmdVenta = new MySqlCommand(
                """
                INSERT INTO venta (numero_documento_usuario, id_comision, fecha, monto_total)
                VALUES (@numeroDocumento, @idComision, @fecha, @montoTotal);
                SELECT LAST_INSERT_ID();
                """,
                mysqlConn,
                transaction);
            
            cmdVenta.Parameters.AddWithValue("@numeroDocumento", venta.NumeroDocumentoUsuario);
            cmdVenta.Parameters.AddWithValue("@idComision", venta.IdComision);
            cmdVenta.Parameters.AddWithValue("@fecha", venta.Fecha);
            cmdVenta.Parameters.AddWithValue("@montoTotal", venta.MontoTotal);
            
            var ventaId = Convert.ToInt32(await cmdVenta.ExecuteScalarAsync(ct));
            
            // Insertar entradas asociadas
            foreach (var entrada in entradas)
            {
                await using var cmdEntrada = new MySqlCommand(
                    """
                    INSERT INTO entrada (estado, fecha, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, qr_usado)
                    VALUES (@estado, @fecha, @costo, @idSector, @idEvento, @numeroDocumentoAdmin, @idVenta, 0)
                    """,
                    mysqlConn,
                    transaction);
                
                cmdEntrada.Parameters.AddWithValue("@estado", entrada.Estado);
                cmdEntrada.Parameters.AddWithValue("@fecha", entrada.Fecha);
                cmdEntrada.Parameters.AddWithValue("@costo", entrada.Costo);
                cmdEntrada.Parameters.AddWithValue("@idSector", entrada.IdSector);
                cmdEntrada.Parameters.AddWithValue("@idEvento", entrada.IdEventoDeportivo);
                cmdEntrada.Parameters.AddWithValue("@numeroDocumentoAdmin", (object?)entrada.NumeroDocumentoAdministrador ?? DBNull.Value);
                cmdEntrada.Parameters.AddWithValue("@idVenta", ventaId);
                
                await cmdEntrada.ExecuteNonQueryAsync(ct);
            }
            
            await transaction.CommitAsync(ct);
            return ventaId;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
