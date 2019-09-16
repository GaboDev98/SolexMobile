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
    public class OrdenCargueDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;

        public OrdenCargueDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<OrdenCargue>();
        }

        public List<OrdenCargue> GetAllOrdenes ()
        {
            lock (locker)
            {
                List<OrdenCargue> listOrdenes = new List<OrdenCargue>();
                try
                {
                    listOrdenes = (from i in database.Table<OrdenCargue>() select i)
                            .Where(x => x.Created_at.Equals(DateTime.Now.Date.ToString()))
                            .OrderByDescending(i => i.Updated_at)
                            .ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listOrdenes;
            }
        }

        public List<OrdenCargue> GetAllOrdenes(bool delivereds, decimal userId)
        {
            lock (locker)
            {
                List<OrdenCargue> listOrdenes = new List<OrdenCargue>();
                try
                {
                    listOrdenes = (from i in database.Table<OrdenCargue>() select i)
                        .Where(x => x.Delivered == delivereds
                        && x.ControlledUserId == userId)
                        .OrderBy(i => i.FechaAsignacion)
                        .ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listOrdenes;
            }
        }

        public int GetCountOrdenes(bool controlleds = false)
        {
            lock (locker)
            {
                int cantidad = 0;
                try
                {
                    cantidad = (from i in database.Table<OrdenCargue>() select i)
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

        public OrdenCargue GetOrdenCargue()
        {
            lock (locker)
            {
                if (database.Table<OrdenCargue>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<OrdenCargue>().First();
                }
            }
        }

        public OrdenCargue GetOrdenCargueById(int id)
        {
            lock (locker)
            {
                return database.Table<OrdenCargue>().FirstOrDefault(x => x.Id == id);
            }
        }

        public OrdenCargue GetOrdenCargueByIdSolex(decimal idOrden)
        {
            lock (locker)
            {
                return database.Table<OrdenCargue>().FirstOrDefault(x => x.OrdenId == idOrden);
            }
        }

        public OrdenCargue GetOrdenByNumero(string numero)
        {
            lock (locker)
            {
                return database.Table<OrdenCargue>()
                    .FirstOrDefault(x => x.OrdenId.ToString().Equals(numero));
            }
        }
        
        public int SaveOrdenCargue(OrdenCargue orden)
        {
            lock (locker)
            {
                try
                {
                    if (orden.Id > 0)
                    {
                        database.Update(orden);
                        return orden.Id;
                    }
                    else
                    {
                        return database.Insert(orden);
                    }
                }
                catch(SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return 0;
                }
            }
        }

        public int DeleteOrden(int id)
        {
            lock (locker)
            {
                return database.Delete<OrdenCargue>(id);
            }
        }

        public bool DeleteAllOrdenes(decimal userId)
        {
            try
            {
                lock (locker)
                {
                    List<OrdenCargue> listOrdenes = (from i in database.Table<OrdenCargue>() select i)
                            .Where(x => x.Controlled == false
                            && x.ControlledUserId == userId)
                            .ToList();

                    foreach (var item in listOrdenes)
                    {
                        database.Delete<OrdenCargue>(item.Id);
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
