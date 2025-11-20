using Services.Models.ModelosReporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IReporteService
    {
        Task<IList<ModeloReporte>> GetReportes();
        Task<ModeloReporte> GetReporte(Guid id);
        Task AddReporte(AñadirModeloReporte model);
        Task DeleteReporte(Guid id);
        Task<IList<ModeloReporte>> GetByUsuario(Guid usuarioId);
        Task<IList<ModeloReporte>> GetByTipo(string tipo);
    }
}
