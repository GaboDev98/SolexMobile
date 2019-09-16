using SQLite;
using System.Linq;
using Xamarin.Forms;
using SolexMobile.Data;
using System.Diagnostics;
using SolexMobileApp.Models;
using System.Collections.Generic;

namespace SolexMobileApp.Controllers
{
    public class EstadoGuiaDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;

        public EstadoGuiaDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Estado>();
        }

        public int SaveEstado(Estado estado)
        {
            lock (locker)
            {
                try
                {
                    if (estado.Id > 0)
                    {
                        database.Update(estado);
                        return estado.Id;
                    }
                    else
                    {
                        return database.Insert(estado);
                    }
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return 0;
                }
            }
        }

        public List<Estado> GetAllEstados()
        {
            lock (locker)
            {
                List<Estado> listEstados = new List<Estado>();
                try
                {
                    listEstados = (from i in database.Table<Estado>() select i).OrderBy(i => i.IdSolex).ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listEstados;
            }
        }

        public Estado GetEstadoByIdSolex(decimal IdSolex)
        {
            lock (locker)
            {
                Estado estadoGuia = new Estado();
                try
                {
                    estadoGuia = (from i in database.Table<Estado>() select i).Where(i => i.IdSolex == IdSolex).FirstOrDefault();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return estadoGuia;
            }
        }

        public bool DeleteAllEstados()
        {
            lock (locker)
            {
                List<Estado> listEstados = (from i in database.Table<Estado>() select i)
                    .OrderBy(i => i.IdSolex).ToList();

                foreach (var item in listEstados)
                {
                    database.Delete<Estado>(item.Id);
                }
                return true;
            }
        }
    }
}
