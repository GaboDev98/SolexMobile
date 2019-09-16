using SQLite;
using System;

namespace SolexMobileApp.Models
{
    public class DetalleUnidades
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public decimal IdGuia { get; set; }
        public double Alto { get; set; }
        public double Ancho { get; set; }
        public double Largo { get; set; }
        public double Volumen { get; set; }
        public double PesoVolumen { get; set; }
        public double Peso { get; set; }
        public double Unidades { get; set; }
        public string Path { get; set; }
        public string Imagen { get; set; }
        public decimal UsuarioId { get; set; }
        public string UsuarioLogin { get; set; }
        public decimal ControlledUserId { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}
