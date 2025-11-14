using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosUsuario
{
    public class ModeloUsuario
    {
        public Guid Id { get; set; }
        //public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public string Contrasena { get; set; }
    }
}
