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
    public class UnidadDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;

        public UnidadDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Unidad>();
        }

        public List<Unidad> GetAllUnidades()
        {
            lock (locker)
            {
                List<Unidad> listUnidades = new List<Unidad>();
                try
                {
                    listUnidades = (from i in database.Table<Unidad>() select i)
                            .Where(x => x.Created_at.Equals(DateTime.Now.Date.ToString()))
                            .OrderByDescending(i => i.Updated_at)
                            .ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listUnidades;
            }
        }

        public int GetCountUnidades()
        {
            lock (locker)
            {
                int cantidad = 0;
                try
                {
                    cantidad = (from i in database.Table<Unidad>() select i)
                        .Count();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return cantidad;
            }
        }

        public Unidad GetUnidadById(int id)
        {
            lock (locker)
            {
                return database.Table<Unidad>().FirstOrDefault(x => x.Id == id);
            }
        }

        public Unidad GetUnidadByIdSolex(decimal idUnidad)
        {
            lock (locker)
            {
                return database.Table<Unidad>().FirstOrDefault(x => x.IdUnidad == idUnidad);
            }
        }

        public List<Unidad> GetUnidadesByIdGuia(long id_guia)
        {
            lock (locker)
            {
                return database.Table<Unidad>()
                    .Where(x => x.IdGuia == id_guia)
                    .ToList();
            }
        }

        public int SaveUnidad(Unidad unidad)
        {
            lock (locker)
            {
                try
                {
                    if (unidad.Id > 0)
                    {
                        database.Update(unidad);
                        return unidad.Id;
                    }
                    else
                    {
                        return database.Insert(unidad);
                    }
                }
                catch(SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return 0;
                }
            }
        }

        public int DeleteUnidad(int id)
        {
            lock (locker)
            {
                return database.Delete<Unidad>(id);
            }
        }

        public bool DeleteAllUnidades()
        {
            try
            {
                lock (locker)
                {
                    List<Unidad> listUnidades = (from i in database.Table<Unidad>() select i).ToList();

                    foreach (var item in listUnidades)
                    {
                        database.Delete<Unidad>(item.Id);
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

        public bool DeleteAllUnidadesByUserId(decimal userId)
        {
            try
            {
                lock (locker)
                {
                    List<Unidad> listUnidades = (from i in database.Table<Unidad>() select i)
                            .Where(x => x.ControlledUserId == userId)
                            .ToList();

                    foreach (var item in listUnidades)
                    {
                        database.Delete<Unidad>(item.Id);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}
