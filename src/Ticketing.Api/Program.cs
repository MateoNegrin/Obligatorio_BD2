using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ticketing.Api.Middleware;
using Ticketing.Application;
using Ticketing.Application.Firebase;
using Ticketing.Application.Services;
using Ticketing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

const string FrontCorsPolicy = "AllowFront";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Capas de la solución.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure();

// Firebase Authentication
var firebaseJsonPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-credentials.json");
if (File.Exists(firebaseJsonPath))
{
    var firebaseJson = File.ReadAllText(firebaseJsonPath);
    builder.Services.AddFirebaseAuthentication(firebaseJson);
}
else
{
    Console.WriteLine("ADVERTENCIA: Archivo firebase-credentials.json no encontrado. Firebase no está inicializado.");
}

// Configurar autenticación (esquema JWT para que [Authorize] funcione)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// CORS para el front Blazor WASM (en Development abrimos cualquier origen).
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontCorsPolicy, policy =>
        policy
            .WithOrigins("http://localhost:5199", "https://localhost:5199")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Convierte errores de validación de dominio en respuestas 400 con el mensaje.
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(FrontCorsPolicy);

// Middleware de validación de tokens Firebase (DEBE estar antes de UseAuthorization)
app.UseMiddleware<FirebaseTokenValidationMiddleware>();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
