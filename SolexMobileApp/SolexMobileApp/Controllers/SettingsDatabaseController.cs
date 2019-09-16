using SQLite;
using Xamarin.Forms;
using SolexMobile.Data;
using SolexMobileApp.Models;

namespace SolexMobileApp.Controllers
{
    public class SettingsDatabaseController
    {
        static readonly object locker = new object();

        SQLiteConnection database;

        public SettingsDatabaseController()
        {
            // Conexión a la base de datos local
            database = DependencyService.Get<ISQLite>().GetConnection();
            // Creación de la estructura de la tabla
            database.CreateTable<Settings>();
        }

        public Settings GetSettings()
        {
            lock (locker)
            {
                if (database.Table<Settings>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<Settings>().First();
                }
            }
        }

        public Settings GetSettingsById(int id)
        {
            lock (locker)
            {
                return database.Table<Settings>().FirstOrDefault(x => x.Id == id);
            }
        }

        public int SaveSettings(Settings settings)
        {
            lock (locker)
            {
                if (settings.Id > 0)
                {
                    database.Update(settings);
                    return settings.Id;
                }
                else
                {
                    return database.Insert(settings);
                }
            }
        }

        public int DeleteSettings(int id)
        {
            lock (locker)
            {
                return database.Delete<Settings>(id);
            }
        }
    }
}
