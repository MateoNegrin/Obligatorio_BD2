using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.Abstractions;
using Ticketing.Infrastructure.Persistence;
using Ticketing.Infrastructure.Repositories;

namespace Ticketing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        services.AddScoped<IEquipoRepository, EquipoRepository>();
        services.AddScoped<IEstadioRepository, EstadioRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IEntradaRepository, EntradaRepository>();
        services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
        services.AddScoped<IMetricaRepository, MetricaRepository>();

        return services;
    }
}
