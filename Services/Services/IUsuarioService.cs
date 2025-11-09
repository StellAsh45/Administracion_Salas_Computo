using Services.Models.ModelosUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUsuarioService
    {
        Task<IList<ModeloUsuario>> GetUsuarios();
        Task<ModeloUsuario> GetUsuario(Guid id);
        Task<ModeloUsuario> GetByEmail(string correo);
        Task AddUsuario(AñadirModeloUsuario model);
        Task UpdateUsuario(ModeloUsuario model);
        Task DeleteUsuario(Guid id);
        Task AssignRole(Guid usuarioId, string role);
    }
}

