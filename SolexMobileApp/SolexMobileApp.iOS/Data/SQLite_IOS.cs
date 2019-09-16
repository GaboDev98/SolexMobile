using System;
using System.Collections.Generic;
using System.Text;
using SolexMobile.Data;
using System.IO;
using Xamarin.Forms;
using SolexMobileApp.iOS.Data;

[assembly: Dependency(typeof(SQLite_IOS))]

namespace SolexMobileApp.iOS.Data
{
    public class SQLite_IOS : ISQLite
    {
        public SQLite_IOS() { }
        public SQLite.SQLiteConnection GetConnection()
        {
            var fileName = "Testbd.db3";
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "Library");
            var path = Path.Combine(libraryPath, fileName);
            var connection = new SQLite.SQLiteConnection(path);
            return connection;
        }
    }
}
