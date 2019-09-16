namespace SolexMobileApp.Models
{
    public class ResponseGuia
    {
        public decimal GuiaId { get; set; }
        public string Placa { get; set; }
        public decimal Planilla { get; set; }
        public string Sucursal { get; set; }
        public string Guia { get; set; }
        public string DANE { get; set; }
        public string Ciudad { get; set; }
        public string Destinatario { get; set; }
        public string DireccionDestino { get; set; }
        public string TelefonoCliente { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public decimal Unidades { get; set; }
        public decimal UltimoEstadoId { get; set; }
        public string FechaAsignacion { get; set; }
        public string FechaLlegoAlPunto { get; set; }
        public string FechaEntrega { get; set; }
        public string RecibeNombre { get; set; }
        public string RecibeDocumento { get; set; }
    }
}