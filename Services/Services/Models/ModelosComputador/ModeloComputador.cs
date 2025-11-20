using System;


namespace Services.Models.ModelosComputador
{
    public class ModeloComputador
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public Guid? SalaId { get; set; }
        public string SalaDisplay { get; set; } = string.Empty;
        public string UsuarioOcupando { get; set; } = string.Empty; // Nombre del usuario que está usando el equipo
    }
}
