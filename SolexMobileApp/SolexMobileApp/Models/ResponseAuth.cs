namespace SolexMobileApp.Models
{
    public class ResponseAuth
    {
        public decimal UsuarioId { get; set; }
        public string TipoUsuario { get; set; }
        public string Placa { get; set; }
        public Token Access_token { get; set; }
        public decimal PerfilId { get; set; }
        public string[] Roles { get; set; }
        public int Codigo { get; set; }
        public string Respuesta { get; set; }
    }
}
