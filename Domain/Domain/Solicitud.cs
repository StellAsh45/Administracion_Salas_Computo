using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Solicitud
    {
        [Key]
        public Guid Id { get; set; } = Guid.Empty;
        public Guid UsuarioId { get; set; } = Guid.Empty;
        public Usuario Usuario { get; set; }
        public Guid ComputadorId { get; set; } = Guid.Empty;
        public Computador Computador { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } // Ejemplo: Pendiente, Aceptado, Denegado
    }
}
