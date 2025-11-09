using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SolicitudRepository : BaseRepository, ISolicitudRepository
    {
        public SolicitudRepository(AppDbContext context) : base(context) { }

        public async Task<IList<Solicitud>> GetSolicitudes()
        {
            return await context.Solicitudes
                .Include(s => s.Usuario)
                .Include(s => s.Computador)
                .ToListAsync();
        }

        public async Task<Solicitud> GetSolicitud(Guid id)
        {
            return await context.Solicitudes
                .Include(s => s.Usuario)
                .Include(s => s.Computador)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task Save(Solicitud solicitud)
        {
            try
            {
                await Beguin();
                await context.Solicitudes.AddAsync(solicitud);
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task Update(Solicitud solicitud)
        {
            try
            {
                await Beguin();
                context.Solicitudes.Update(solicitud);
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
                var entity = await context.Solicitudes.FirstOrDefaultAsync(s => s.Id == id);
                if (entity != null)
                {
                    context.Solicitudes.Remove(entity);
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

        public async Task<IList<Solicitud>> GetByEstado(string estado)
        {
            return await context.Solicitudes
                .Where(s => s.Estado == estado)
                .Include(s => s.Usuario)
                .Include(s => s.Computador)
                .ToListAsync();
        }

        public async Task AcceptSolicitud(Guid solicitudId)
        {
            try
            {
                await Beguin();

                var solicitud = await context.Solicitudes.FirstOrDefaultAsync(s => s.Id == solicitudId);
                if (solicitud == null) { await Comit(); return; }

                var computador = await context.Computadores.FirstOrDefaultAsync(c => c.Id == solicitud.ComputadorId);
                if (computador != null)
                {
                    computador.UsuarioId = solicitud.UsuarioId;
                    computador.Estado = "Ocupado";
                    context.Computadores.Update(computador);
                }

                solicitud.Estado = "Aceptado";
                context.Solicitudes.Update(solicitud);

                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task DenySolicitud(Guid solicitudId)
        {
            try
            {
                await Beguin();
                var solicitud = await context.Solicitudes.FirstOrDefaultAsync(s => s.Id == solicitudId);
                if (solicitud != null)
                {
                    solicitud.Estado = "Denegado";
                    context.Solicitudes.Update(solicitud);
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

