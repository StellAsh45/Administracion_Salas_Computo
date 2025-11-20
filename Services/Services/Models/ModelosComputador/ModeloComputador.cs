using System;


namespace Services.Models.ModelosComputador
{
    public class ModeloComputador
    {
        public Guid Id { get; set; }
        public string Estado { get; set; }
        public Guid? SalaId { get; set; }
        public string SalaDisplay { get; set; } = string.Empty;
    }
}
