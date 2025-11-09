using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReporteRepository : BaseRepository, IReporteRepository
    {
        public ReporteRepository(AppDbContext context) : base(context) { }

        public async Task<IList<Reporte>> GetReportes()
        {
            return await context.Reportes
                .Include(r => r.Usuario)
                .ToListAsync();
        }

        public async Task<Reporte> GetReporte(Guid id)
        {
            return await context.Reportes
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task Save(Reporte reporte)
        {
            try
            {
                await Beguin();
                await context.Reportes.AddAsync(reporte);
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
                var entity = await context.Reportes.FirstOrDefaultAsync(r => r.Id == id);
                if (entity != null)
                {
                    context.Reportes.Remove(entity);
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

        public async Task<IList<Reporte>> GetByUsuario(Guid usuarioId)
        {
            return await context.Reportes
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Usuario)
                .ToListAsync();
        }
    }
}
