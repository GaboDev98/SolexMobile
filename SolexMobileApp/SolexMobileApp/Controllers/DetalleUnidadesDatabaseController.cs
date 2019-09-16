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
    public class DetalleUnidadesDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;

        public DetalleUnidadesDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<DetalleUnidades>();
        }

        public List<DetalleUnidades> GetAllDetalleUnidades()
        {
            lock (locker)
            {
                List<DetalleUnidades> listDetalleUnidades = new List<DetalleUnidades>();
                try
                {
                    listDetalleUnidades = (from i in database.Table<DetalleUnidades>() select i)
                            .Where(x => x.Created_at.Equals(DateTime.Now.Date.ToString()))
                            .OrderByDescending(i => i.Updated_at)
                            .ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listDetalleUnidades;
            }
        }

        public int GetCountDetalleUnidades()
        {
            lock (locker)
            {
                int cantidad = 0;
                try
                {
                    cantidad = (from i in database.Table<DetalleUnidades>() select i)
                        .Count();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return cantidad;
            }
        }

        public DetalleUnidades GetDetalleUnidadesById(int id)
        {
            lock (locker)
            {
                return database.Table<DetalleUnidades>().FirstOrDefault(x => x.Id == id);
            }
        }

        public DetalleUnidades GetDetalleUnidadesByIdGuia(decimal idGuia)
        {
            lock (locker)
            {
                return database.Table<DetalleUnidades>().FirstOrDefault(x => x.IdGuia == idGuia);
            }
        }
        
        public int SaveDetalleUnidades(DetalleUnidades detalle_unidades)
        {
            lock (locker)
            {
                try
                {
                    if (detalle_unidades.Id > 0)
                    {
                        database.Update(detalle_unidades);
                        return detalle_unidades.Id;
                    }
                    else
                    {
                        return database.Insert(detalle_unidades);
                    }
                }
                catch(SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return 0;
                }
            }
        }

        public int DeleteDetalleUnidades(int id)
        {
            lock (locker)
            {
                return database.Delete<DetalleUnidades>(id);
            }
        }

        public bool DeleteAllDetalleUnidades(decimal userId, bool controlled = false)
        {
            try
            {
                lock (locker)
                {
                    List<DetalleUnidades> listDetalleUnidades = (from i in database.Table<DetalleUnidades>() select i)
                            .Where(x => x.ControlledUserId == userId)
                            .ToList();

                    foreach (var item in listDetalleUnidades)
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
    }
}
