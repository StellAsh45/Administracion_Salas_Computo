using AutoMapper;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var existente = await repo.GetComputador(model.Id);
            if (existente == null)
            {
                throw new InvalidOperationException("Computador no encontrado.");
            }

            existente.SalaId = model.SalaId;
            existente.Estado = model.Estado;

            await repo.Update(existente);
        }

        public async Task DeleteComputador(Guid id)
        {
            await repo.Delete(id);
        }
    }
}

