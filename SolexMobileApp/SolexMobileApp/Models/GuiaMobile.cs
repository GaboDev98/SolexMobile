using System;

namespace SolexMobileApp.Models
{
    public class GuiaMobile
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public decimal PlanillaNumero { get; set; }
        public string Sucursal { get; set; }
        public string GuiaNumero { get; set; }
        public string DANE { get; set; }
        public string Ciudad { get; set; }
        public string Destinatario { get; set; }
        public string DireccionDestino { get; set; }
        public string TelefonoCliente { get; set; }
        public decimal Unidades { get; set; }
        public string Receives { get; set; }
        public string Receives_Doc { get; set; }
        public string DeliveredDate { get; set; }
        public bool Delivered { get; set; }
        public string ColorButtonRow { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
