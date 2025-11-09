using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SalaRepository : BaseRepository, ISalaRepository
    {
        public SalaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<Sala>> GetSalas()
        {
            return await context.Salas
                .Include(s => s.Computadores)
                .ToListAsync();
        }

        public async Task<Sala> GetSala(Guid id)
        {
            return await context.Salas
                .Include(s => s.Computadores)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task Save(Sala sala)
        {
            try
            {
                await Beguin();
                await context.Salas.AddAsync(sala);
                await Save();
                await Comit();
            }
            catch (Exception)
            {
                await RollBack();
                throw;
            }
        }

        public async Task Update(Sala sala)
        {
            try
            {
                await Beguin();
                context.Salas.Update(sala);
                await Save();
                await Comit();
            }
            catch (Exception)
            {
                await RollBack();
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                await Beguin();
                var sala = await context.Salas.FirstOrDefaultAsync(s => s.Id == id);
                if (sala != null)
                {
                    context.Salas.Remove(sala);
                    await Save();
                }
                await Comit();
            }
            catch (Exception)
            {
                await RollBack();
                throw;
            }
        }

        public async Task<IList<Computador>> GetComputadoresBySala(Guid salaId)
        {
            return await context.Computadores
                .Where(c => c.SalaId == salaId)
                .ToListAsync();
        }
    }
}
