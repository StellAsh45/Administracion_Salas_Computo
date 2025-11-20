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
            var computadoresDomain = await repo.GetComputadores();
            var computadores = mapper.Map<IList<ModeloComputador>>(computadoresDomain);
            
            // Poblar SalaDisplay para cada computador usando la relación ya cargada
            foreach (var comp in computadores)
            {
                var compDomain = computadoresDomain.FirstOrDefault(c => c.Id == comp.Id);
                if (compDomain?.Sala != null)
                {
                    comp.SalaDisplay = $"Sala {compDomain.Sala.NumeroSalon}";
                }
                else
                {
                    comp.SalaDisplay = "Sin sala";
                }
            }
            
            return computadores;
        }

        public async Task<ModeloComputador> GetComputador(Guid id)
        {
            var computadorDomain = await repo.GetComputador(id);
            var computador = mapper.Map<ModeloComputador>(computadorDomain);
            
            // Poblar SalaDisplay
            if (computadorDomain?.Sala != null)
            {
                computador.SalaDisplay = $"Sala {computadorDomain.Sala.NumeroSalon}";
            }
            else
            {
                computador.SalaDisplay = "Sin sala";
            }
            
            return computador;
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

            existente.Nombre = model.Nombre;
            existente.SalaId = model.SalaId;
            existente.Estado = model.Estado;

            await repo.Update(existente);
        }

        public async Task DeleteComputador(Guid id)
        {
            await repo.Delete(id);
        }

        public async Task SetEstado(Guid computadorId, string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                throw new ArgumentException("El estado no puede estar vacío.", nameof(estado));
            }

            await repo.SetEstado(computadorId, estado);
        }
    }
}

