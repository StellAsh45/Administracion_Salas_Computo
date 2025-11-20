using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IReporteRepository
    {
        Task<IList<Reporte>> GetReportes();
        Task<Reporte> GetReporte(Guid id);
        Task Save(Reporte reporte);
        Task Delete(Guid id);
        Task<IList<Reporte>> GetByUsuario(Guid usuarioId);
        Task<IList<Reporte>> GetByTipo(string tipo);
    }
}

