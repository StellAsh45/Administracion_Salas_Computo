using System;
using System.Collections.Generic;

namespace Services.Models.ModelosComputador
{
    public class OcupacionDiariaModel
    {
        public DateTime Fecha { get; set; } = DateTime.Today;
        public IList<OcupacionSalaDetalle> Detalles { get; set; } = new List<OcupacionSalaDetalle>();
    }

    public class OcupacionSalaDetalle
    {
        public string SalaNombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public int EquiposRegistrados { get; set; }
        public int EquiposDisponibles { get; set; }
        public int EquiposEnUso { get; set; }
        public int EquiposOcupados { get; set; } // Equipos en mantenimiento/bloqueados

        public double PorcentajeOcupacion => EquiposRegistrados == 0
            ? 0
            : Math.Round((double)EquiposEnUso / EquiposRegistrados * 100, 2);
    }
}



