using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosReporte
{
    public class ModeloReporte
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Contenido { get; set; }
    }
}
