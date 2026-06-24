using System.Text;
using System.Text.RegularExpressions;

namespace Ticketing.Front.Utils;

/// <summary>
/// Utilidad para sanitizar inputs del usuario y proteger contra SQL injection.
/// </summary>
public static class InputSanitizer
{
    private const int MaxLength = 30;
    
    /// <summary>
    /// Caracteres peligrosos que deben ser removidos o escapados para prevenir SQL injection.
    /// </summary>
    private static readonly char[] DangerousCharacters = 
    {
        '\'',   // Comilla simple
        '"',    // Comilla doble
        ';',    // Punto y coma
        '-',    // Guion (para comentarios --)
        '*',    // Asterisco
        '/',    // Barra (para comentarios /* */)
        '\\',   // Barra invertida
        '\0',   // Null character
    };

    /// <summary>
    /// Sanitiza un input limitando su longitud y removiendo caracteres peligrosos.
    /// </summary>
    /// <param name="input">El input del usuario a sanitizar</param>
    /// <returns>El input sanitizado, o null si es nulo o vacío</returns>
    public static string? Sanitize(string? input)
    {
        // Si el input es nulo o vacío, retorna null
        if (string.IsNullOrWhiteSpace(input))
            return null;

        // Limita la longitud a MaxLength caracteres
        string trimmed = input.Trim().Substring(0, Math.Min(input.Length, MaxLength));

        // Remueve caracteres peligrosos
        string sanitized = RemoveDangerousCharacters(trimmed);

        // Si después de sanitizar queda vacío, retorna null
        return string.IsNullOrWhiteSpace(sanitized) ? null : sanitized;
    }

    /// <summary>
    /// Sanitiza un input numérico o alfanumérico (número de documento, ID, etc).
    /// Solo permite dígitos y letras.
    /// </summary>
    /// <param name="input">El input del usuario a sanitizar</param>
    /// <returns>El input sanitizado con solo dígitos y letras, o null si no contiene caracteres válidos</returns>
    public static string? SanitizeNumeric(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        // Solo mantiene dígitos y letras
        string alphanumeric = Regex.Replace(input, @"[^a-zA-Z0-9]", "");

        // Limita a MaxLength
        alphanumeric = alphanumeric.Substring(0, Math.Min(alphanumeric.Length, MaxLength));

        return string.IsNullOrEmpty(alphanumeric) ? null : alphanumeric;
    }

    /// <summary>
    /// Sanitiza un email removiendo caracteres peligrosos pero preservando caracteres válidos de email.
    /// </summary>
    /// <param name="input">El email del usuario a sanitizar</param>
    /// <returns>El email sanitizado, o null si es inválido</returns>
    public static string? SanitizeEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        string trimmed = input.Trim();

        // Remueve caracteres no válidos en emails (mantiene solo alfanuméricos, punto, guion, subrayado y @)
        string sanitized = Regex.Replace(trimmed, @"[^a-zA-Z0-9._\-@]", "");

        // Limita a MaxLength * 2 para emails (pueden ser más largos)
        sanitized = sanitized.Substring(0, Math.Min(sanitized.Length, MaxLength * 2));

        return string.IsNullOrEmpty(sanitized) ? null : sanitized;
    }

    /// <summary>
    /// Remueve caracteres peligrosos del input.
    /// </summary>
    /// <param name="input">El input a limpiar</param>
    /// <returns>El input sin caracteres peligrosos</returns>
    private static string RemoveDangerousCharacters(string input)
    {
        var sanitizedBuilder = new StringBuilder();

        foreach (char c in input)
        {
            // Si el carácter no está en la lista de peligrosos, lo incluye
            if (!DangerousCharacters.Contains(c))
            {
                sanitizedBuilder.Append(c);
            }
        }

        return sanitizedBuilder.ToString();
    }
}
