using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SolexMobileApp.Models;
using SolexMobileApp.Views.Modules;

namespace SolexMobileApp.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuCubingPage : ContentPage
    {
        public MenuCubingPage()
        {
            InitializeComponent();
            InitComponents();
        }

        // Función que permita inicializar algunos componentes 
        // de la UI de manera personalizada
        void InitComponents()
        {
            // Ocultamos la barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);
            // Seteamos las demás propiedades de la página
            BackgroundColor = Constants.BackgroundColor;
        }

        async void CubicNormal(object sender, EventArgs e)
        {
            // Chequamos si el usuario tiene el rol de cubicación manual
            if (Constants.CurrentUser.IsCubage)
            {
                // Abrimos la página de cubicación normal
                await Navigation.PushAsync(new CubingPage());
            }
            else
            {
                await DisplayAlert("Mensaje informativo", "No tiene suficientes permisos para acceder al módulo de cubicación.", "OK");
            }
        }

        async void CubicMasiva(object sender, EventArgs e)
        {
            // Chequamos si el usuario tiene el rol de cubicación manual
            if (Constants.CurrentUser.IsCubage)
            {
                // Abrimos la página de cubicación masiva
                await Navigation.PushAsync(new MassiveCubing());
            }
            else
            {
                await DisplayAlert("Mensaje informativo", "No tiene suficientes permisos para acceder al módulo de cubicación.", "OK");
            }
        }

        async void MenuPrincipal(object sender, EventArgs e)
        {
            // Abrimos la página del menú principal
            await Navigation.PushAsync(new DashboardPage());
        }
    }
}