using System;
using SQLite;

namespace SolexMobileApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public decimal IdSolex { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Placa { get; set; }
        public bool IsLogin { get; set; } = false;
        public bool IsCubage { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public User() { }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        // Función para válidar campos nulos
        public bool CheckInformation()
        {
            if (!string.IsNullOrWhiteSpace(this.Username)
                && !string.IsNullOrWhiteSpace(this.Password))
                return true;
            else
                return false;
        }
    }
}
