using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using SolexMobileApp.Models;
using SolexMobileApp.Views.Menu;

namespace SolexMobileApp.Views.DetailViews.SettingsViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsScreen : ContentPage
    {
        // Objeto de configuración
        private Settings settings = new Settings();

        // Url seleccionada
        string SelectedUrlSolex { get; set; }

        // Indice de la url seleccionada
        private int selectedIndexUrlSolex = -1;

        // Constructor de la clase
        public SettingsScreen()
        {
            // Inicialización de componentes de la UI
            InitializeComponent();
            // Obtenemos la url actual
            var settings = App.SettingsDatabase.GetSettings();
            // Se verifica que exista registro de la configuración
            if (settings != null)
            {
                // Seleccionamos la url registrada
                switch (settings.UrlName)
                {
                    case "Solex Pro":
                        PickerListUrlsSolex.SelectedIndex = 0;
                        break;
                    case "Solex Test":
                        PickerListUrlsSolex.SelectedIndex = 1;
                        break;
                    case "Solex Pre":
                        PickerListUrlsSolex.SelectedIndex = 2;
                        break;
                    case "Solex 2":
                        PickerListUrlsSolex.SelectedIndex = 3;
                        break;
                    default:
                        PickerListUrlsSolex.SelectedIndex = -1;
                        break;
                }
            }
        }

        void OnPickerSelectedUrlSolexIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            selectedIndexUrlSolex = picker.SelectedIndex;

            if (selectedIndexUrlSolex != -1)
            {
                SelectedUrlSolex = (string)picker.ItemsSource[selectedIndexUrlSolex];
            }
        }

        async void SaveSettings(object sender, EventArgs e)
        {
            // Verificamos la url seleccionada
            switch (SelectedUrlSolex)
            {
                case "Solex Pro":
                    settings.UrlSolex = Constants.UrlSolexProd;
                    break;
                case "Solex Test":
                    settings.UrlSolex = Constants.UrlSolexTest;
                    break;
                case "Solex Pre":
                    settings.UrlSolex = Constants.UrlSolexPre;
                    break;
                case "Solex 2":
                    settings.UrlSolex = Constants.UrlSolex2;
                    break;
                default:
                    settings.UrlSolex = Constants.UrlSolexProd;
                    break;
            }
            // Actualizamos el nombre de la url seleccionada
            settings.UrlName = SelectedUrlSolex;
            // Método que guarda la configuración
            var response = App.SettingsDatabase.SaveSettings(settings);
            // Verificamos si se guardo la información
            if (response == 1)
            {
                // Se muestra el mensaje de alerta en pantalla
                await DisplayAlert("Configuración", "La configuración fue registrada correctamente.", "OK");
            }
            else
            {
                // Se muestra el mensaje de alerta en pantalla
                await DisplayAlert("Configuración", "La configuración no se guardo correctamente.", "OK");
            }
            // Redireccionamos al menú principal
            MainListOpen();
        }

        // Función o método que nos redirige al listado principal
        async void MainListOpen()
        {
            try
            {
                // Se verifica la plataforma para definir el tipo de transición
                if (Device.RuntimePlatform == Device.Android)
                    await Navigation.PushAsync(new DashboardPage());
                else if (Device.RuntimePlatform == Device.iOS)
                    await Navigation.PushModalAsync(new DashboardPage());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}