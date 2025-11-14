using AutoMapper;
using Infrastructure.Repositories;
using Services.Models.ModelosUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository repo;
        private readonly IMapper mapper;

        public UsuarioService(IUsuarioRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<IList<ModeloUsuario>> GetUsuarios()
        {
            return mapper.Map<IList<ModeloUsuario>>(await repo.GetUsuarios());
        }

        public async Task<ModeloUsuario> GetUsuario(Guid id)
        {
            return mapper.Map<ModeloUsuario>(await repo.GetUsuario(id));
        }

        public async Task<ModeloUsuario> GetByEmail(string correo)
        {
            return mapper.Map<ModeloUsuario>(await repo.GetByEmail(correo));
        }

        public async Task AddUsuario(AñadirModeloUsuario model)
        {
            var exists = await repo.GetByEmail(model.Correo);
            if (exists != null)
            {
                throw new InvalidOperationException("El correo ingresado ya ha sido registrado");
            }

            await repo.Save(mapper.Map<Domain.Usuario>(model));
        }

        public async Task UpdateUsuario(ModeloUsuario model)
        {
            await repo.Update(mapper.Map<Domain.Usuario>(model));
        }

        public async Task DeleteUsuario(Guid id)
        {
            await repo.Delete(id);
        }

        public async Task AssignRole(Guid usuarioId, string role)
        {
            var u = await repo.GetUsuario(usuarioId);
            if (u != null)
            {
                u.Rol = role;
                await repo.Update(u);
            }
        }
    }
}

