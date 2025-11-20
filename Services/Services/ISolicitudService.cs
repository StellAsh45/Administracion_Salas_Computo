using Services.Models.ModelosSolicitud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISolicitudService
    {
        Task<IList<ModeloSolicitud>> GetSolicitudes();
        Task<ModeloSolicitud> GetSolicitud(Guid id);
        Task AddSolicitud(AñadirModeloSolicitud model);
        Task UpdateSolicitud(ModeloSolicitud model);
        Task DeleteSolicitud(Guid id);

        Task<IList<ModeloSolicitud>> GetByEstado(string estado);
        Task AcceptSolicitud(Guid id);
        Task DenySolicitud(Guid id);
        Task CerrarSolicitudesActivasPorEquipo(Guid computadorId);
    }
}

