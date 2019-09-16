using System;
using SQLite;
using System.Linq;
using Xamarin.Forms;
using SolexMobile.Data;
using System.Diagnostics;
using SolexMobileApp.Models;
using System.Collections.Generic;

namespace SolexMobileApp.Controllers
{
    public class GuiaDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;

        public GuiaDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Guia>();
        }

        public List<Guia> GetAllGuias()
        {
            lock (locker)
            {
                List<Guia> listGuias = new List<Guia>();
                try
                {
                    listGuias = (from i in database.Table<Guia>() select i)
                            .Where(x => x.Created_at.Equals(DateTime.Now.Date.ToString()))
                            .OrderByDescending(i => i.Updated_at)
                            .ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listGuias;
            }
        }

        public List<Guia> GetAllGuias(bool controlled, decimal userId)
        {
            lock (locker)
            {
                List<Guia> listGuias = new List<Guia>();
                try
                {
                    listGuias = (from i in database.Table<Guia>() select i)
                        .Where(x => x.Controlled == controlled
                        && x.ControlledUserId == userId)
                        .OrderBy(i => i.FechaAsignacion)
                        .ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listGuias;
            }
        }

        public int GetCountGuias(bool controlleds = false)
        {
            lock (locker)
            {
                int cantidad = 0;
                try
                {
                    cantidad = (from i in database.Table<Guia>() select i)
                        .Where(x => x.Controlled == controlleds)
                        .Count();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return cantidad;
            }
        }

        public Guia GetGuia()
        {
            lock (locker)
            {
                if (database.Table<Guia>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<Guia>().First();
                }
            }
        }

        public Guia GetGuiaById(int id)
        {
            lock (locker)
            {
                return database.Table<Guia>().FirstOrDefault(x => x.Id == id);
            }
        }

        public Guia GetGuiaByIdSolex(decimal idGuia)
        {
            lock (locker)
            {
                return database.Table<Guia>().FirstOrDefault(x => x.IdGuia == idGuia);
            }
        }

        public Guia GetGuiaByNumero(string numero)
        {
            lock (locker)
            {
                return database.Table<Guia>()
                    .FirstOrDefault(x => x.GuiaNumero == numero);
            }
        }
        
        public int SaveGuia(Guia guia)
        {
            lock (locker)
            {
                try
                {
                    if (guia.Id > 0)
                    {
                        database.Update(guia);
                        return guia.Id;
                    }
                    else
                    {
                        return database.Insert(guia);
                    }
                }
                catch(SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return 0;
                }
            }
        }

        public int DeleteGuia(int id)
        {
            lock (locker)
            {
                return database.Delete<Guia>(id);
            }
        }

        public bool DeleteAllGuias(decimal userId, bool controlled = false)
        {
            try
            {
                lock (locker)
                {
                    List<Guia> listGuias = (from i in database.Table<Guia>() select i)
                            .Where(x => x.Controlled == controlled
                            && x.ControlledUserId == userId)
                            .ToList();

                    foreach (var item in listGuias)
                    {
                        database.Delete<Guia>(item.Id);
                    }

                    return true;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}
