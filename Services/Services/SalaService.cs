using AutoMapper;
using Infrastructure.Repositories;
using Services.Models.ModelosComputador;
using Services.Models.ModelosSala;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SalaService : ISalaService
    {
        private readonly ISalaRepository repo;
        private readonly IMapper mapper;

        public SalaService(ISalaRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<IList<ModeloSala>> GetSalas()
        {
            var salas = await repo.GetSalas();
            return mapper.Map<IList<ModeloSala>>(salas);
        }

        public async Task<ModeloSala> GetSala(Guid id)
        {
            var sala = await repo.GetSala(id);
            return mapper.Map<ModeloSala>(sala);
        }

        public async Task AddSala(AñadirModeloSala model)
        {
            if (model.NumeroSalon <= 0)
            {
                throw new InvalidOperationException("El número de salón debe ser mayor que 0.");
            }

            if (model.Capacidad < 1 || model.Capacidad > 30)
            {
                throw new InvalidOperationException("La capacidad debe estar entre 1 y 30 dispositivos.");
            }

            var salasExistentes = await repo.GetSalas();
            if (salasExistentes.Any(s => s.NumeroSalon == model.NumeroSalon))
            {
                throw new InvalidOperationException($"Ya existe una sala con el número {model.NumeroSalon}.");
            }

            var entity = mapper.Map<Domain.Sala>(model);
            await repo.Save(entity);
        }

        public async Task UpdateSala(ModeloSala model)
        {
            if (model.Capacidad < 1 || model.Capacidad > 30)
            {
                throw new InvalidOperationException("La capacidad debe estar entre 1 y 30 dispositivos.");
            }

            var existente = await repo.GetSala(model.Id);
            if (existente == null)
            {
                throw new InvalidOperationException("Sala no encontrada.");
            }

            existente.Capacidad = model.Capacidad;
            existente.Estado = model.Estado;

            await repo.Update(existente);
        }

        public async Task DeleteSala(Guid id)
        {
            await repo.Delete(id);
        }

        public async Task<IList<ModeloComputador>> GetComputadoresBySala(Guid salaId)
        {
            var comps = await repo.GetComputadoresBySala(salaId);
            return mapper.Map<IList<ModeloComputador>>(comps);
        }
    }
}