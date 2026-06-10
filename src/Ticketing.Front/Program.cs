using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ticketing.Front;
using Ticketing.Front.ApiClients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// URL base de la API leída de wwwroot/appsettings.json (fallback al host del front).
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Un cliente HTTP por área principal.
builder.Services.AddScoped<EquiposApiClient>();
builder.Services.AddScoped<EstadiosApiClient>();
builder.Services.AddScoped<EventosApiClient>();
builder.Services.AddScoped<UsuariosApiClient>();
builder.Services.AddScoped<MetricasApiClient>();

await builder.Build().RunAsync();
