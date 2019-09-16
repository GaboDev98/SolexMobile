namespace SolexMobileApp.Models
{
    public class RequestEstadoGuia
    {
        public decimal GuiaId { get; set; }
        public decimal EstadoId { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public decimal OperadorUsuarioId { get; set; }
    }
}
