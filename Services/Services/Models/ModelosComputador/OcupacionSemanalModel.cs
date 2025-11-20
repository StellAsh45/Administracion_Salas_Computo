using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Models.ModelosComputador
{
    public class OcupacionSemanalModel
    {
        public DateTime InicioSemana { get; set; } = DateTime.Today;
        public DateTime FinSemana { get; set; } = DateTime.Today.AddDays(6);
        public IList<DateTime> DiasSemana { get; set; } = new List<DateTime>();
        public IList<OcupacionSemanalSala> Salas { get; set; } = new List<OcupacionSemanalSala>();
    }

    public class OcupacionSemanalSala
    {
        public string SalaNombre { get; set; } = string.Empty;
        public int EquiposRegistrados { get; set; }
        public IDictionary<DateTime, OcupacionSemanalDia> OcupacionPorDia { get; set; } = new Dictionary<DateTime, OcupacionSemanalDia>();

        public double PromedioSemanal
        {
            get
            {
                if (OcupacionPorDia.Count == 0 || EquiposRegistrados == 0)
                {
                    return 0;
                }

                var promedio = OcupacionPorDia.Values.Sum(d => d.EquiposOcupados);
                return Math.Round((double)promedio / (OcupacionPorDia.Count * EquiposRegistrados) * 100, 2);
            }
        }
    }

    public class OcupacionSemanalDia
    {
        public int EquiposOcupados { get; set; }
        public int EquiposRegistrados { get; set; }

        public double PorcentajeOcupacion => EquiposRegistrados == 0
            ? 0
            : Math.Round((double)EquiposOcupados / EquiposRegistrados * 100, 2);
    }
}



