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

    // La venta + su estado + sus entradas se crean dentro de una transacción (MySqlTransaction):
    // o se confirma todo, o no se confirma nada.
    public async Task<int> CreateAsync(Venta venta, IReadOnlyList<Entrada> entradas, int idEstadoVenta, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        var mysqlConn = (MySqlConnection)conn;

        await using var transaction = await mysqlConn.BeginTransactionAsync(ct);

        try
        {
            // Validar por (sector, evento): sector habilitado + disponibilidad. Dentro de la
            // transacción para que el conteo de vendidas sea consistente con los INSERT siguientes.
            foreach (var grupo in entradas.GroupBy(e => (e.IdSector, e.IdEventoDeportivo)))
            {
                var idSector = grupo.Key.IdSector;
                var idEvento = grupo.Key.IdEventoDeportivo;
                var solicitadas = grupo.Count();

                // Sector habilitado para el evento -> además devuelve el admin que lo habilitó.
                await using var cmdInfo = new MySqlCommand(
                    "SELECT numero_documento_administrador FROM informacion_entrada WHERE id_sector = @idSector AND id_evento_deportivo = @idEvento",
                    mysqlConn, transaction);
                cmdInfo.Parameters.AddWithValue("@idSector", idSector);
                cmdInfo.Parameters.AddWithValue("@idEvento", idEvento);
                var adminObj = await cmdInfo.ExecuteScalarAsync(ct);
                if (adminObj is null or DBNull)
                    throw new InvalidOperationException($"El sector {idSector} no está habilitado para el evento {idEvento}.");
                var numeroDocumentoAdmin = (string)adminObj;

                // Disponibilidad = capacidad del sector - entradas ya vendidas para ese evento+sector.
                await using var cmdDisp = new MySqlCommand(
                    @"SELECT s.capacidad - (SELECT COUNT(*) FROM entrada en WHERE en.id_sector = s.id AND en.id_evento_deportivo = @idEvento)
                      FROM sector s WHERE s.id = @idSector",
                    mysqlConn, transaction);
                cmdDisp.Parameters.AddWithValue("@idSector", idSector);
                cmdDisp.Parameters.AddWithValue("@idEvento", idEvento);
                var disponibles = Convert.ToInt32(await cmdDisp.ExecuteScalarAsync(ct));
                if (solicitadas > disponibles)
                    throw new InvalidOperationException($"No hay entradas suficientes en el sector {idSector}: disponibles {disponibles}, solicitadas {solicitadas}.");

                // El comprobante de cada entrada queda a nombre del admin que habilitó el sector.
                foreach (var entrada in grupo)
                    entrada.NumeroDocumentoAdministrador = numeroDocumentoAdmin;
            }

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

            // Estado inicial de la venta
            await using var cmdEstado = new MySqlCommand(
                "INSERT INTO venta_tiene_estado (id_venta, id_estado_venta) VALUES (@idVenta, @idEstado)",
                mysqlConn, transaction);
            cmdEstado.Parameters.AddWithValue("@idVenta", ventaId);
            cmdEstado.Parameters.AddWithValue("@idEstado", idEstadoVenta);
            await cmdEstado.ExecuteNonQueryAsync(ct);

            // Insertar entradas asociadas, capturando el id generado de cada una.
            foreach (var entrada in entradas)
            {
                await using var cmdEntrada = new MySqlCommand(
                    """
                    INSERT INTO entrada (estado, fecha, estado_seed, costo, id_sector, id_evento_deportivo, numero_documento_administrador, id_venta, qr_usado, numero_documento_propietario_actual)
                    VALUES (@estado, @fecha, @estadoSeed, @costo, @idSector, @idEvento, @numeroDocumentoAdmin, @idVenta, NULL, @propietario);
                    SELECT LAST_INSERT_ID();
                    """,
                    mysqlConn,
                    transaction);

                cmdEntrada.Parameters.AddWithValue("@estado", entrada.Estado);
                cmdEntrada.Parameters.AddWithValue("@fecha", entrada.Fecha);
                cmdEntrada.Parameters.AddWithValue("@estadoSeed", entrada.EstadoSeed ?? string.Empty);
                cmdEntrada.Parameters.AddWithValue("@costo", entrada.Costo);
                cmdEntrada.Parameters.AddWithValue("@idSector", entrada.IdSector);
                cmdEntrada.Parameters.AddWithValue("@idEvento", entrada.IdEventoDeportivo);
                cmdEntrada.Parameters.AddWithValue("@numeroDocumentoAdmin", (object?)entrada.NumeroDocumentoAdministrador ?? DBNull.Value);
                cmdEntrada.Parameters.AddWithValue("@idVenta", ventaId);
                // El comprador queda como dueño actual de la entrada al momento de la compra.
                cmdEntrada.Parameters.AddWithValue("@propietario", venta.NumeroDocumentoUsuario);

                entrada.Id = Convert.ToInt32(await cmdEntrada.ExecuteScalarAsync(ct));
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
