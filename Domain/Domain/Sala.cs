using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Sala
    {
        [Key]
        public Guid Id { get; set; } = Guid.Empty;
        public int NumeroSalon { get; set; }
        public int Capacidad { get; set; } 
        public string Estado { get; set; } // Disponible, Ocupada, Mantenimiento
        public IList<Computador> Computadores { get; set; } = new List<Computador>();
    }
}
