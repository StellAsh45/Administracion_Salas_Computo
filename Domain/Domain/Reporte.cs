using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Reporte
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public Usuario Usuario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Contenido { get; set; }
    }
}
