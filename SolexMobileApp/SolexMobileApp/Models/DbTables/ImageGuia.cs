using SQLite;
using System;

namespace SolexMobileApp.Models
{
    public class ImageGuia
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }
        public DateTime RegisterDate { get; set; }
        public int Id_guia { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}
