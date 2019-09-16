using System;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;

namespace SolexMobileApp.Views.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CubicacionPage : ContentPage
    {
        public CubicacionPage()
        {
            InitializeComponent();
            InitComponents();
        }

        // Método personalizado que inicializa 
        // los componentes de la página
        void InitComponents() {
            // Posiciona el cursor
            Entry_CodeBars.Focus();
            // Seteamos las propiedades de la página
            BackgroundColor = Constants.BackgroundColor;
            // Desactivamos el spinner de carga
            ActivitySpinner.IsVisible = false;
            // Desabilitamos todas las entradas de texto menos la primera
            Entry_Alto.IsEnabled = false;
            Entry_Ancho.IsEnabled = false;
            Entry_Largo.IsEnabled = false;
            Entry_Peso.IsEnabled = false;
            Entry_Peso.IsEnabled = false;
            // Verificamos la conexión a internet
            CheckConnectivity();
            // Capturamos el evento del botón de tomar la foto
            Btn_CapturePhoto.Clicked += async (sender, args) =>
            {
                // Se inicializa el plugin
                await CrossMedia.Current.Initialize();

                // Validamos que existan los permisos necesarios
                // para poder usar la cámara del dispositivo
                var hasPermission = await Utils.Utils.CheckPermissions(Permission.Camera);
                if (!hasPermission)
                    return;

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No hay Cámara", "La Cámara del dispositivo no se encuentra disponible!", "OK");
                    return;
                }
                try
                {
                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                        Directory = "ImagesCubic",
                        Name = "cubic_mobile_" + Entry_CodeBars.Text + ".png"
                    });

                    if (file == null)
                        return;

                    await DisplayAlert("Ubicación del archivo: ", file.Path, "OK");
                }
                catch //(Exception ex)
                {
                   // Xamarin.Insights.Report(ex);
                   // await DisplayAlert("Uh oh", "Something went wrong, but don't worry we captured it in Xamarin Insights! Thanks.", "OK");
                }
            };
            // Al oprimir enter posiciona el cursor
            // en la siguiente entrada de texto
            Entry_CodeBars.Completed += async (s, e) =>
            {
                // Validamos que haya digitado el código de barras
                if (!String.IsNullOrWhiteSpace(Entry_CodeBars.Text))
                {
                    // Verificamos que exista conexión a internet
                    if (Constants.hasInternet)
                    {
                        // Creamos un hilo de ejecución para consumir el servicio del login
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            // Mostramos el spinner de carga
                            ActivitySpinner.IsVisible = true;
                            // Consumimos el servicio de autenticación de Solex
                            var result = await App.RestClient.GetExistsBarCode(Entry_CodeBars.Text);
                            // Verificamos que el servicio haya devuelto una respuesta
                            if (result != null)
                            {
                                if (result.IsSuccess)
                                {
                                    // Habilitamos los campos de medidas
                                    Entry_Alto.IsEnabled = true;
                                    Entry_Ancho.IsEnabled = true;
                                    Entry_Largo.IsEnabled = true;
                                    Entry_Peso.IsEnabled = true;
                                    // Posicionamos el cursor
                                    Entry_Alto.Focus();
                                    // Verificamos el número de unidades de la guía
                                    if (result.CodError > 1)
                                        // Mostramos la cantidad de unidades de la guía
                                        Count_Units.Text = "La guía contiene " + result.CodError + " unidades.";
                                    else
                                        // Mostramos el mensaje que indica que la guía solo contiene una unidad
                                        Count_Units.Text = "La guía contiene una sola unidad.";
                                    // Mostramos el mensaje
                                    Count_Units.IsVisible = true;
                                }
                                else
                                {
                                    // Mostramos el error de Solex
                                    await DisplayAlert("Respuesta de Solex", result.Error, "OK");
                                    // Limpiamos la caja de texto
                                    Entry_CodeBars.Text = "";
                                    // Posicionamos el cursor
                                    Entry_CodeBars.Focus();
                                }
                            }
                            else
                            {
                                // Mostramos el mensaje que devuelve Solex en caso de que algo esté mal
                                // con el usuario que está intentando ingresar
                                await DisplayAlert("Mensaje Informativo", "Ocurrió un error al intentar enviar las medidas", "OK");
                                // Limpiamos la caja de texto
                                Entry_CodeBars.Text = "";
                                // Posicionamos el cursor
                                Entry_CodeBars.Focus();
                            }
                            // Desabilitamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                        });
                    }
                    else
                    {
                        await DisplayAlert("Error de conexión!", "No se encuentra una conexión estable a internet.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error de campos!", "Primero debe digitar el código de barras.", "OK");
                    Entry_CodeBars.Focus();
                }
            };
            Entry_Alto.Completed += (s, e) => Entry_Ancho.Focus();
            Entry_Ancho.Completed += (s, e) => Entry_Largo.Focus();
            Entry_Largo.Completed += (s, e) => Entry_Peso.Focus();
        }

        // Método para verificar la conexión a internet
        private void CheckConnectivity()
        {
            // Validamos que haya conexión a internet
            Constants.hasInternet = CrossConnectivity.Current.IsConnected;

            if (!Constants.hasInternet)
                Labl_Connectivity.Text = Constants.NoInternetText;
            else
                Labl_Connectivity.IsVisible = false;
        }

        public bool CheckInputs()
        {
            string Codigo_Barras = Entry_CodeBars.Text;
            decimal Alto = Convert.ToDecimal(Entry_Alto.Text);
            decimal Ancho = Convert.ToDecimal(Entry_Ancho.Text);
            decimal Largo = Convert.ToDecimal(Entry_Largo.Text);
            decimal Peso = Convert.ToDecimal(Entry_Peso.Text);
            if (String.IsNullOrWhiteSpace(Codigo_Barras))
            {
                DisplayAlert("Mensaje de Alerta", "Primero debe digitar el código de barras", "OK");
                return false;
            }
            if (Alto == 0)
            {
                DisplayAlert("Mensaje de Alerta", "Debe digitar la altura en centímetros", "OK");
                return false;
            }
            if (Ancho == 0)
            {
                DisplayAlert("Mensaje de Alerta", "Debe digitar el ancho en centímetros", "OK");
                return false;
            }
            if (Largo == 0)
            {
                DisplayAlert("Mensaje de Alerta", "Debe digitar el largo en centímetros", "OK");
                return false;
            }
            if (Peso == 0)
            {
                DisplayAlert("Mensaje de Alerta", "Debe digitar el peso en kilos", "OK");
                return false;
            }
            return true;
        }

        // Función de autenticación
        async Task EnviarMedidasAsync(object sender, EventArgs e)
        {
            // Verificamos nuevamente que haya conexión a internet
            CheckConnectivity();
            // Verificamos que se encuentren llenos todos los campos
            if (CheckInputs())
            {
                // Verificamos que exista conexión a internet
                if (Constants.hasInternet)
                {
                    // Creamos un hilo de ejecución para consumir el servicio del login
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Mostramos el spinner de carga
                        ActivitySpinner.IsVisible = true;
                        // Instanciamos el objeto de medidas
                        ImageCubic cubic_image = new ImageCubic
                        {
                            Codigo_Barras = Entry_CodeBars.Text,
                            Alto = Convert.ToDouble(Entry_Alto.Text),
                            Ancho = Convert.ToDouble(Entry_Ancho.Text),
                            Largo = Convert.ToDouble(Entry_Largo.Text),
                            Peso = Convert.ToDouble(Entry_Peso.Text),
                            UsuarioId = Constants.CurrentUser.IdSolex
                        };
                        // Consumimos el servicio de autenticación de Solex
                        var result = await App.RestClient.SaveMedidasCon<ImageCubic>(cubic_image);
                        // Verificamos que el servicio haya devuelto una respuesta
                        if (result != null)
                        {
                            
                        }
                        else
                        {
                            // Mostramos el mensaje que devuelve Solex en caso de que algo esté mal
                            // con el usuario que está intentando ingresar
                            await DisplayAlert("Mensaje de Alerta", "Ocurrió un error al intentar enviar las medidas", "OK");
                        }
                        // Desabilitamos el spinner de carga
                        ActivitySpinner.IsVisible = false;
                    });
                }
                else
                {
                    await DisplayAlert("Error de conexión!", "No se encuentra una conexión estable a internet.", "OK");
                }
            }
        }
    }
}