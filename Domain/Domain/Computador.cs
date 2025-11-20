using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Computador
    {
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; } // Nombre del computador
        public string Estado { get; set; } // Ejemplo: Disponible, Ocupado, Mantenimiento
        public Guid? SalaId { get; set; } // Puede ser nulo si el computador no está asignado a ninguna sala
        public Sala? Sala { get; set; }
    }
}
