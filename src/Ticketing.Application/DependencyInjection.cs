using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.Services;

namespace Ticketing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEquipoService, EquipoService>();
        services.AddScoped<IEstadioService, EstadioService>();
        services.AddScoped<IEventoService, EventoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IVentaService, VentaService>();
        services.AddScoped<IEntradaService, EntradaService>();
        services.AddScoped<ITransferenciaService, TransferenciaService>();
        services.AddScoped<IValidacionService, ValidacionService>();
        services.AddScoped<IMetricaService, MetricaService>();
        services.AddScoped<IQrService, QrService>();
        services.AddScoped<IRolService, RolService>();
        services.AddScoped<IPermisoService, PermisoService>();
        services.AddScoped<IAsignacionService, AsignacionService>();
        services.AddScoped<ILogUsuarioService, LogUsuarioService>();
        services.AddScoped<IPaisService, PaisService>();
        services.AddScoped<ILocalidadService, LocalidadService>();
        return services;
    }
}
