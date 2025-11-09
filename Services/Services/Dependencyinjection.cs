using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class Dependencyinjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
          
            services.AddTransient<IFarmService, FarmService>();
            services.AddTransient<IReporteService, ReporteService>();
            services.AddTransient<ISolicitudService, SolicitudService>();
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IComputadorService, ComputadorService>();
            services.AddTransient<ISalaService, SalaService>();

            return services;
        }
    }
}
