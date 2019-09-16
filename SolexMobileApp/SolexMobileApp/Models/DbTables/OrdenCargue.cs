using SQLite;
using System;

namespace SolexMobileApp.Models
{
    public class OrdenCargue
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Placa { get; set; }
        public decimal PlanillaNumero { get; set; }
        public string Sucursal { get; set; }
        [Unique]
        public decimal OrdenId { get; set; }
        public string DANE { get; set; }
        public string Ciudad { get; set; }
        public string Destinatario { get; set; }
        public string DireccionDestino { get; set; }
        public string NombreCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public decimal UnidadesProgramadas { get; set; }
        public decimal DocumentosRecogidos { get; set; }
        public decimal UnidadesRecogidas { get; set; }
        public decimal UltimoEstadoId { get; set; }
        public string FechaLlegoAlPunto { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Receives { get; set; }
        public string ArrivalDate { get; set; }
        public bool Arrival { get; set; }
        public string DeliveredDate { get; set; }
        public bool Delivered { get; set; }
        public bool Controlled { get; set; }
        public decimal ControlledUserId { get; set; }
        public string ColorButtonRow { get; set; } = "#FF5252";
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}
