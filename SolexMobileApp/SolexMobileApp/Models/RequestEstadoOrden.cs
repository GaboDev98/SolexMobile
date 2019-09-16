namespace SolexMobileApp.Models
{
    public class RequestEstadoOrden
    {
        public decimal OrdenId { get; set; }
        public decimal EstadoId { get; set; }
        public string Descripcion { get; set; }
        public string NombreEntrega { get; set; }
        public decimal DocumentosRecogidos { get; set; }
        public decimal UnidadesRecogidas { get; set; }
        public string Fecha { get; set; }
        public decimal OperadorUsuarioId { get; set; }
    }
}
