using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context) { }

        public async Task<IList<Usuario>> GetUsuarios()
        {
            return await context.Usuarios
                .Include(u => u.Solicitudes)
                .Include(u => u.Reportes)
                .ToListAsync();
        }

        public async Task<Usuario> GetUsuario(Guid id)
        {
            return await context.Usuarios
                .Include(u => u.Solicitudes)
                .Include(u => u.Reportes)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> GetByEmail(string correo)
        {
            return await context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task Save(Usuario usuario)
        {
            try
            {
                await Beguin();
                await context.Usuarios.AddAsync(usuario);
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task Update(Usuario usuario)
        {
            try
            {
                await Beguin();
                context.Usuarios.Update(usuario);
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
                var entity = await context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
                if (entity != null)
                {
                    context.Usuarios.Remove(entity);
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

