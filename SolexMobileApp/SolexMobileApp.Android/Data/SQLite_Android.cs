using System.IO;
using Xamarin.Forms;
using SolexMobile.Data;
using SolexMobileApp.Droid.Data;

[assembly: Dependency(typeof(SQLite_Android))]

namespace SolexMobileApp.Droid.Data
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }
        public SQLite.SQLiteConnection GetConnection()
        {
            string sqliteFileName = "SolexMobileDB.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFileName);
            var conn = new SQLite.SQLiteConnection(path);
            return conn;
        }
    }
}