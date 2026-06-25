namespace Ticketing.Api.Middleware;

// Convierte las excepciones de validación de dominio (InvalidOperationException) en
// respuestas 400 con el mensaje, en lugar de dejar que lleguen como stacktrace al front.
public sealed class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(ex.Message);
        }
    }
}
