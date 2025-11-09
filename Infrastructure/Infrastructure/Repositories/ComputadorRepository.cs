using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ComputadorRepository : BaseRepository, IComputadorRepository
    {
        public ComputadorRepository(AppDbContext context) : base(context) { }

        public async Task<IList<Computador>> GetComputadores()
        {
            return await context.Computadores
                .Include(c => c.Sala)
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        public async Task<Computador> GetComputador(Guid id)
        {
            return await context.Computadores
                .Include(c => c.Sala)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Save(Computador computador)
        {
            try
            {
                await Beguin();
                await context.Computadores.AddAsync(computador);
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task Update(Computador computador)
        {
            try
            {
                await Beguin();
                context.Computadores.Update(computador);
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
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
                var entity = await context.Computadores.FirstOrDefaultAsync(c => c.Id == id);
                if (entity != null)
                {
                    context.Computadores.Remove(entity);
                    await context.SaveChangesAsync();
                }
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task AssignToUser(Guid computadorId, Guid usuarioId)
        {
            try
            {
                await Beguin();
                var comp = await context.Computadores.FirstOrDefaultAsync(c => c.Id == computadorId);
                if (comp != null)
                {
                    comp.UsuarioId = usuarioId;
                    comp.Estado = "Ocupado";
                    context.Computadores.Update(comp);
                    await context.SaveChangesAsync();
                }
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task Release(Guid computadorId)
        {
            try
            {
                await Beguin();
                var comp = await context.Computadores.FirstOrDefaultAsync(c => c.Id == computadorId);
                if (comp != null)
                {
                    comp.UsuarioId = null;
                    comp.Estado = "Disponible";
                    context.Computadores.Update(comp);
                    await context.SaveChangesAsync();
                }
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task SetEstado(Guid computadorId, string estado)
        {
            try
            {
                await Beguin();
                var comp = await context.Computadores.FirstOrDefaultAsync(c => c.Id == computadorId);
                if (comp != null)
                {
                    comp.Estado = estado;
                    context.Computadores.Update(comp);
                    await context.SaveChangesAsync();
                }
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }
    }
}

