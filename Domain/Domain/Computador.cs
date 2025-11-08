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
        public string Estado { get; set; } // Ejemplo: Disponible, Ocupado, Mantenimiento

        public Guid SalaId { get; set; }
        public Sala Sala { get; set; }

        public Guid? UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
