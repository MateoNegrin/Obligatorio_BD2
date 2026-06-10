using Ticketing.Application;
using Ticketing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

const string FrontCorsPolicy = "AllowFront";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Capas de la solución.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure();

// CORS para el front Blazor WASM (en Development abrimos cualquier origen).
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontCorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(FrontCorsPolicy);
app.MapControllers();

app.Run();
