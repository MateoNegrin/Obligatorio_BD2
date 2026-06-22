using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Ticketing.Front;
using Ticketing.Front.ApiClients;
using Ticketing.Front.Authentication;
using Ticketing.Front.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// URL base de la API leída de wwwroot/appsettings.json (fallback al host del front).
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Logging
builder.Services.AddLogging();

// Authorization
builder.Services.AddAuthorizationCore();

// Un cliente HTTP por área principal.
builder.Services.AddScoped<EquiposApiClient>();
builder.Services.AddScoped<EstadiosApiClient>();
builder.Services.AddScoped<EventosApiClient>();
builder.Services.AddScoped<UsuariosApiClient>();
builder.Services.AddScoped<MetricasApiClient>();
builder.Services.AddScoped<EntradasApiClient>();
builder.Services.AddScoped<TransferenciasApiClient>();
builder.Services.AddScoped<DispositivosApiClient>();

// Toasts (notificaciones flotantes)
builder.Services.AddScoped<ToastService>();

// Firebase Authentication
builder.Services.AddScoped<FirebaseAuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, FirebaseAuthenticationStateProvider>();

var app = builder.Build();

// Esperar a que Firebase se inicialice
await Task.Delay(1000);

await app.RunAsync();
