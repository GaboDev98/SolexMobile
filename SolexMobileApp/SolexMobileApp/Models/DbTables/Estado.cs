using SQLite;

namespace SolexMobileApp.Models
{
    public class Estado
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public decimal IdSolex { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}
