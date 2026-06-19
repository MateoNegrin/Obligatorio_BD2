using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Ticketing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AdminController : ControllerBase
{
    private readonly IConfiguration _config;

    public AdminController(IConfiguration config) => _config = config;

    [HttpPost("initialize-schema")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InitializeSchema(CancellationToken ct)
    {
        try
        {
            var connectionString = _config.GetConnectionString("MySQL")
                ?? throw new InvalidOperationException("Falta la conexión MySQL");

            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync(ct);

            var schemaFile = Path.Combine(Directory.GetCurrentDirectory(), "../../database/01_schema_mysql.sql");
            if (!System.IO.File.Exists(schemaFile))
                return BadRequest(new { error = $"Archivo no encontrado: {schemaFile}" });

            var sqlScript = await System.IO.File.ReadAllTextAsync(schemaFile, ct);
            var statements = sqlScript.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            foreach (var statement in statements)
            {
                var trimmed = statement.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                await using var cmd = new MySqlCommand(trimmed + ";", conn);
                await cmd.ExecuteNonQueryAsync(ct);
                count++;
            }

            await conn.CloseAsync();
            return Ok(new { message = $"Esquema inicializado: {count} sentencias ejecutadas" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }
}
