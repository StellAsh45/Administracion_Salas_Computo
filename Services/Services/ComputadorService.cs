using AutoMapper;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Models.ModelosComputador;

namespace Services
{
    public class ComputadorService : IComputadorService
    {
        private readonly IComputadorRepository repo;
        private readonly IMapper mapper;

        public ComputadorService(IComputadorRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<IList<ModeloComputador>> GetComputadores()
        {
            return mapper.Map<IList<ModeloComputador>>(await repo.GetComputadores());
        }

        public async Task<ModeloComputador> GetComputador(Guid id)
        {
            return mapper.Map<ModeloComputador>(await repo.GetComputador(id));
        }

        public async Task AddComputador(AñadirModeloComputador model)
        {
            await repo.Save(mapper.Map<Domain.Computador>(model));
        }

        public async Task UpdateComputador(ModeloComputador model)
        {
            await repo.Update(mapper.Map<Domain.Computador>(model));
        }

        public async Task DeleteComputador(Guid id)
        {
            await repo.Delete(id);
        }

        public async Task AssignComputador(Guid computadorId, Guid usuarioId)
        {
            await repo.AssignToUser(computadorId, usuarioId);
        }

        public async Task ReleaseComputador(Guid computadorId)
        {
            await repo.Release(computadorId);
        }

        public async Task BlockComputador(Guid computadorId, string reason)
        {
            // almacena motivo en un reporte opcional o simplemente marca estado
            await repo.SetEstado(computadorId, "Mantenimiento");
        }
    }
}

