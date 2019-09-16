using SQLite;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using SolexMobile.Data;
using SolexMobileApp.Models;
using System.Diagnostics;

namespace SolexMobileApp.Controllers
{
    public class ImageGuiaDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;

        public ImageGuiaDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<ImageGuia>();
        }

        public ImageGuia GetImageGuia()
        {
            lock (locker)
            {
                if (database.Table<ImageGuia>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<ImageGuia>().First();
                }
            }
        }

        public List<ImageGuia> GetImagesByIdGuia(int id_guia)
        {
            lock (locker)
            {
                List<ImageGuia> listImagesGuia = new List<ImageGuia>();
                try
                {
                    listImagesGuia = database.Table<ImageGuia>().Where(x => x.Id_guia == id_guia).ToList();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return listImagesGuia;
            }
        }

        public ImageGuia GetLastImageGuia()
        {
            lock (locker)
            {
                ImageGuia lastImageGuia = new ImageGuia();
                try
                {
                    lastImageGuia = (from i in database.Table<ImageGuia>() select i).ToList().OrderByDescending(i => i.Id).FirstOrDefault();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
                return lastImageGuia;
            }
        }

        public int SaveImageGuia(ImageGuia imageGuia)
        {
            lock (locker)
            {
                try
                {
                    if (imageGuia.Id > 0)
                    {
                        database.Update(imageGuia);
                        return imageGuia.Id;
                    }
                    else
                    {
                        return database.Insert(imageGuia);
                    }
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    return 0;
                }
            }
        }

        public int DeleteIamgeGuia(int id)
        {
            lock (locker)
            {
                return database.Delete<ImageGuia>(id);
            }
        }
    }
}
