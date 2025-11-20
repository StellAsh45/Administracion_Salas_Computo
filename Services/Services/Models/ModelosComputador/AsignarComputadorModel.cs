using System;
using System.Collections.Generic;

namespace Services.Models.ModelosComputador
{
    public class AsignarComputadorModel
    {
        public Guid UsuarioId { get; set; }
        public Guid ComputadorId { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today.AddHours(1);

    }
}

