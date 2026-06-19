using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;

string connectionString = "Server=mysql.reto-ucu.net;Port=50006;Database=IC_Grupo3;Uid=ic_g3_admin;Pwd=BD2ObligatorioG32026;";

try
{
    using (var conn = new MySqlConnection(connectionString))
    {
        await conn.OpenAsync();
        Console.WriteLine("✓ Conectado a MySQL");

        // Leer el archivo SQL
        string sqlPath = "database/01_schema_mysql.sql";
        if (!File.Exists(sqlPath))
        {
            Console.WriteLine($"✗ Archivo no encontrado: {sqlPath}");
            Environment.Exit(1);
        }

        string sqlScript = File.ReadAllText(sqlPath);
        
        // Dividir por sentencias (simple, asume ; como delimitador)
        string[] statements = sqlScript.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        int count = 0;
        foreach (var statement in statements)
        {
            string trimmed = statement.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            
            try
            {
                using (var cmd = new MySqlCommand(trimmed + ";", conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                    count++;
                    Console.WriteLine($"✓ Sentencia {count} ejecutada");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error en sentencia {count}: {ex.Message}");
            }
        }

        Console.WriteLine($"\n✓ {count} sentencias SQL ejecutadas correctamente");
        await conn.CloseAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
    Environment.Exit(1);
}
