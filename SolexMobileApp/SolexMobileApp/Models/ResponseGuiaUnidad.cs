namespace SolexMobileApp.Models
{
    public class ResponseGuiaUnidad
    {
        public int NumeroUnidad { get; set; }
        public long IdUnidad { get; set; }
        public long IdGuia { get; set; }
        public bool Sorter { get; set; }
        public bool Cubicada { get; set; }
        public string Empresa { get; set; }
        public int Auditadas { get; set; }
        public int Total { get; set; }
    }
}
