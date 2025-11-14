using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosUsuario
{
    public class AñadirModeloUsuario
    {
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
        public string? Nombre { get; set; }
    }
}
