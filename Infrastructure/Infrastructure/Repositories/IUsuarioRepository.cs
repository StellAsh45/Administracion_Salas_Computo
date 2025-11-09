using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IList<Usuario>> GetUsuarios();
        Task<Usuario> GetUsuario(Guid id);
        Task<Usuario> GetByEmail(string correo);
        Task Save(Usuario usuario);
        Task Update(Usuario usuario);
        Task Delete(Guid id);
    }
}
