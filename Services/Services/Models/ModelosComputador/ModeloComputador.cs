using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosComputador
{
    public class ModeloComputador
    {
        public Guid Id { get; set; }
        public string Estado { get; set; }
        public Guid SalaId { get; set; }
        public Guid? UsuarioId { get; set; }
    }
}
