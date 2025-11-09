using Services.Models.ModelosComputador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosSala
{
    public class ModeloSala
    {
        public Guid Id { get; set; }
        public int NumeroSalon { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; }
        public IList<ModeloComputador> Computadores { get; set; } = new List<ModeloComputador>();
    }
}
