using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using SolexMobileApp.Models;
using SolexMobileApp.Views.Modules;
using SolexMobileApp.Views.DetailViews;
using SolexMobileApp.Views.DetailViews.SettingsViews;

namespace SolexMobileApp.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            // Inicializamos los componentes
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

        async void MainListOpen(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TabbedListPage(true));
        }
        
        async void HistoryPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TabbedHistoricalListPage(true));
        }

        async void CubingPage(object sender, EventArgs e)
        {
            // Chequamos si el usuario tiene el rol de cubicación manual
            if (Constants.CurrentUser.IsCubage)
            {
                // Abrimos la página de cubicación
                await Navigation.PushAsync(new MenuCubingPage());
            }
            else
            {
                await DisplayAlert("Mensaje informativo", "No tiene suficientes permisos para acceder al módulo de cubicación.", "OK");
            }
        }

        async void Settings(object sender, EventArgs e)
        {
            // Chequamos si el usuario tiene el rol de cubicación manual
            if (Constants.CurrentUser.IsAdmin)
            {
                // Abrimos la página de cubicación
                await Navigation.PushAsync(new SettingsScreen());
            }
            else
            {
                await DisplayAlert("Mensaje informativo", "Debes tener el rol de administrador para acceder al módulo de configuración.", "OK");
            }
        }

        async void ClosingOfTheDay(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClosingDayPage());
        }

        // Función para salir de la app
        void Logout(object sender, EventArgs e)
        {
            try
            {
                // Cerrramos la sesión del usuario desde la BD local
                App.UserDatabase.LoginUser();
                // Función que dirige al login
                LoginPage();
            }
            catch (Exception er)
            {
                // Imprimimos el error en la consola
                Debug.WriteLine("Error: " + er.Message);
            }
        }

        async void LoginPage()
        {
            // Verificamos la plataforma y cambiamos la página
            // usando la respectiva animación
            if (Device.RuntimePlatform == Device.Android)
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            else if (Device.RuntimePlatform == Device.iOS)
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }
    }
}