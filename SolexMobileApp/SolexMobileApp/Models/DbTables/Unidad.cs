using SQLite;
using System;

namespace SolexMobileApp.Models
{
    public class Unidad
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int NumeroUnidad { get; set; }
        public decimal IdUnidad { get; set; }
        public decimal IdGuia { get; set; }
        public bool Sent { get; set; } = false;
        public decimal ControlledUserId { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}
