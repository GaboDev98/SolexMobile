namespace SolexMobileApp.Models
{
    public class RequestDatosOrden
    {
        public decimal OrdenId { get; set; }
        public string EntregaNombre { get; set; }
        public decimal DocumentosRecogidos { get; set; }
        public decimal UnidadesRecogidas { get; set; }
        public decimal UsuarioId { get; set; }
        public string UsuarioLogin { get; set; }
        public RequestEstadoOrden UltimoEstado { get; set; }
    }
}
