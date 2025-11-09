using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var c = configuration.GetConnectionString("DefaultConnection");

            // Repositorios
            services.AddScoped<IFarmRepository, FarmRepository>();
            services.AddScoped<ISalaRepository, SalaRepository>();

            // Registros a implementar (crear archivos): IComputadorRepository, IUsuarioRepository, ISolicitudRepository, IReporteRepository
            // services.AddScoped<IComputadorRepository, ComputadorRepository>();
            // services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            // services.AddScoped<ISolicitudRepository, SolicitudRepository>();
            // services.AddScoped<IReporteRepository, ReporteRepository>();

            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(c);
            });

            return services;
        }
    }
}
