using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosSolicitud
{
    public class AñadirModeloSolicitud
    {
        public Guid UsuarioId { get; set; }
        public Guid ComputadorId { get; set; }
        public Guid? SalaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string Tipo { get; set; } = "Asignacion";
        public string Descripcion { get; set; } = string.Empty;
    }
}
