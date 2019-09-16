namespace SolexMobileApp.Models
{
    public class MassiveCubic
    {
        public string Codigo_Barras { get; set; }
        public long Id_Guia { get; set; }
        public string IdsNumeros { get; set; }
        public double Alto { get; set; }
        public double Ancho { get; set; }
        public double Largo { get; set; }
        public double Volumen { get; set; }
        public double PesoVolumen { get; set; }
        public double Peso { get; set; }
        public string Imagen { get; set; }
        public string PathImage { get; set; }
        public string FileName { get; set; }
        public decimal UsuarioId { get; set; }
        public string UsuarioLogin { get; set; }
    }
}
