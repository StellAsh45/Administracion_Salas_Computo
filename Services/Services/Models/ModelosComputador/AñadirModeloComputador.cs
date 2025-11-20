using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosComputador
{
    public class AñadirModeloComputador
    {
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public Guid? SalaId { get; set; }
    }
}
