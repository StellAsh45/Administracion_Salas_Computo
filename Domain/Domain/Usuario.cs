using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Nombre { get; set; }
        public IList<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
        public IList<Reporte> Reportes { get; set; } = new List<Reporte>();
    }
}
