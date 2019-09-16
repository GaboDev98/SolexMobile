namespace SolexMobileApp.Models
{
    public class RequestDatosGuia
    {
        public string Guia { get; set; }
        public decimal GuiaId { get; set; }
        public string RecibeNombre { get; set; }
        public string RecibeDocumento { get; set; }
        public string Imagen { get; set; }
        public decimal UsuarioId { get; set; }
        public string UsuarioLogin { get; set; }
        public RequestEstadoGuia UltimoEstado { get; set; }
    }
}
