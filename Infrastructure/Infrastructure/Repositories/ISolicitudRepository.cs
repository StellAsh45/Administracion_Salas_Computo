using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ISolicitudRepository
    {
        Task<IList<Solicitud>> GetSolicitudes();
        Task<Solicitud> GetSolicitud(Guid id);
        Task Save(Solicitud solicitud);
        Task Update(Solicitud solicitud);
        Task Delete(Guid id);

        Task<IList<Solicitud>> GetByEstado(string estado);
        Task AcceptSolicitud(Guid solicitudId);
        Task DenySolicitud(Guid solicitudId);
    }
}

