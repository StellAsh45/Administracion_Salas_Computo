using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.ModelosSala
{
    public class AñadirModeloSala
    {
        [Required(ErrorMessage = "El número de salón es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de salón debe ser mayor que 0.")]
        public int NumeroSalon { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        [Range(1, 30, ErrorMessage = "La capacidad debe estar entre 1 y 30 dispositivos.")]
        public int Capacidad { get; set; }

        public string Estado { get; set; } = "Disponible";
    }
}


    

