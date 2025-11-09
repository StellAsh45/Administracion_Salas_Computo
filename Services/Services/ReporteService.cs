using AutoMapper;
using Infrastructure.Repositories;
using Services.Models.ModelosReporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository repo;
        private readonly IMapper mapper;

        public ReporteService(IReporteRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<IList<ModeloReporte>> GetReportes()
        {
            return mapper.Map<IList<ModeloReporte>>(await repo.GetReportes());
        }

        public async Task<ModeloReporte> GetReporte(Guid id)
        {
            return mapper.Map<ModeloReporte>(await repo.GetReporte(id));
        }

        public async Task AddReporte(AñadirModeloReporte model)
        {
            await repo.Save(mapper.Map<Domain.Reporte>(model));
        }

        public async Task DeleteReporte(Guid id)
        {
            await repo.Delete(id);
        }

        public async Task<IList<ModeloReporte>> GetByUsuario(Guid usuarioId)
        {
            return mapper.Map<IList<ModeloReporte>>(await repo.GetByUsuario(usuarioId));
        }
    }
}

