using System;
using System.Collections.Generic;
using Services.Models.ModelosUsuario;

namespace Services.Models.ModelosComputador
{
    public class AsignarComputadorModel
    {
        public Guid UsuarioId { get; set; }
        public Guid? SalaId { get; set; }
        public Guid ComputadorId { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today.AddHours(1);

        // Listas que rellena el controlador y que consume la vista
        public IList<SelectOption> Usuarios { get; set; } = new List<SelectOption>();
        public IList<SelectOption> Salas { get; set; } = new List<SelectOption>();
        public IList<SelectOption> Computadores { get; set; } = new List<SelectOption>();
    }
}

