using System;
using SQLite;

namespace SolexMobileApp.Models
{
    public class Settings
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string UrlName;
        public string UrlSolex;
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public Settings() { }
        public Settings(string url_name, string url_solex)
        {
            UrlName = url_name;
            UrlSolex = url_solex;
        }
    }
}
