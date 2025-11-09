using Services.Models.ModelosComputador;
using Services.Models.ModelosSala;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISalaService
    {
        Task<IList<ModeloSala>> GetSalas();
        Task<ModeloSala> GetSala(Guid id);
        Task AddSala(AñadirModeloSala model);
        Task UpdateSala(ModeloSala model);
        Task DeleteSala(Guid id);

        Task<IList<ModeloComputador>> GetComputadoresBySala(Guid salaId);
    }
}
