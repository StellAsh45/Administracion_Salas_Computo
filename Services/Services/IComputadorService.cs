using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Models.ModelosComputador;

namespace Services
{
    public interface IComputadorService
    {
        Task<IList<ModeloComputador>> GetComputadores();
        Task<ModeloComputador> GetComputador(Guid id);
        Task AddComputador(AñadirModeloComputador model);
        Task UpdateComputador(ModeloComputador model);
        Task DeleteComputador(Guid id);

        Task AssignComputador(Guid computadorId, Guid usuarioId);
        Task ReleaseComputador(Guid computadorId);
        Task BlockComputador(Guid computadorId, string reason);
    }
}
