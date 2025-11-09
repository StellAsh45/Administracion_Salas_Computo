using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IComputadorRepository
    {
        Task<IList<Computador>> GetComputadores();
        Task<Computador> GetComputador(Guid id);
        Task Save(Computador computador);
        Task Update(Computador computador);
        Task Delete(Guid id);
        Task AssignToUser(Guid computadorId, Guid usuarioId);
        Task Release(Guid computadorId);
        Task SetEstado(Guid computadorId, string estado);
    }
}
