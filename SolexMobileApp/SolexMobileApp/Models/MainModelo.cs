using System;
using Xamarin.Forms;
using SolexMobileApp.Interfaces;

namespace SolexMobileApp.Models
{
    public class MainModelo
    {
        public string IdMaquina { get; set; }
        public string VersionName { get; set; }

        public MainModelo()
        {
            GetVersionName();
            GetDeviceId();
        }

        public void GetDeviceId()
        {
            try
            {
                IdMaquina = DependencyService.Get<IDispositivo>().IdDispositivo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("<Error> No se pudo leer el Id del dispositivo. " + ex.Message);
            }
        }

        public void GetVersionName()
        {
            try
            {
                VersionName = DependencyService.Get<IDispositivo>().VersionName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("<Error> No se pudo obtener la versión de la aplicación. " + ex.Message);
            }
        }
    }
}
