using AutoMapper;
using Infrastructure.Repositories;
using Services.Models.ModelosSolicitud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository repo;
        private readonly IMapper mapper;

        public SolicitudService(ISolicitudRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<IList<ModeloSolicitud>> GetSolicitudes()
        {
            return mapper.Map<IList<ModeloSolicitud>>(await repo.GetSolicitudes());
        }

        public async Task<ModeloSolicitud> GetSolicitud(Guid id)
        {
            return mapper.Map<ModeloSolicitud>(await repo.GetSolicitud(id));
        }

        public async Task AddSolicitud(AñadirModeloSolicitud model)
        {
            if (string.IsNullOrWhiteSpace(model.Estado))
            {
                model.Estado = "Pendiente";
            }
            if (string.IsNullOrWhiteSpace(model.Tipo))
            {
                model.Tipo = "Asignacion";
            }
            await repo.Save(mapper.Map<Domain.Solicitud>(model));
        }

        public async Task UpdateSolicitud(ModeloSolicitud model)
        {
            await repo.Update(mapper.Map<Domain.Solicitud>(model));
        }

        public async Task DeleteSolicitud(Guid id)
        {
            await repo.Delete(id);
        }

        public async Task<IList<ModeloSolicitud>> GetByEstado(string estado)
        {
            return mapper.Map<IList<ModeloSolicitud>>(await repo.GetByEstado(estado));
        }

        public async Task AcceptSolicitud(Guid id)
        {
            await repo.AcceptSolicitud(id);
        }

        public async Task DenySolicitud(Guid id)
        {
            await repo.DenySolicitud(id);
        }
    }
}

