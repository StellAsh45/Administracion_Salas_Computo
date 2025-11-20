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
        public int EquiposOcupados { get; set; }

        public int EquiposDisponibles => Math.Max(EquiposRegistrados - EquiposOcupados, 0);

        public double PorcentajeOcupacion => EquiposRegistrados == 0
            ? 0
            : Math.Round((double)EquiposOcupados / EquiposRegistrados * 100, 2);
    }
}

