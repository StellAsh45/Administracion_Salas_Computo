using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosReporte
{
    public class AñadirModeloReporte
    {
        public Guid UsuarioId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }
}
