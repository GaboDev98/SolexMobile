using Xamarin.Forms;
using SolexMobileApp.Models;

namespace SolexMobileApp.Views.Modules
{
    public class TabbedDetailPage : TabbedPage
    {
        private Guia guiaSelected = new Guia();

        // Constructor sin parámetros
        public TabbedDetailPage(Guia guia)
        {
            Title = "Opciones de la Guía";
            Children.Add(new DetailGuiaPage(guia));
            Children.Add(new DetailGuiaPage(guia));
        }
    }
}
