using System;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CubingPage : ContentPage
    {
        // Objeto que contiene los datos que se envían a Solex
        private ImageCubic imageCubic = new ImageCubic();

        public CubingPage()
        {
            InitializeComponent();
            InitComponents();
        }

        // Método personalizado que inicializa 
        // los componentes de la página
        void InitComponents()
        {
            // Posiciona el cursor
            Entry_CodeBars.Focus();
            // Seteamos las propiedades de la página
            BackgroundColor = Constants.BackgroundColor;
            Btn_Send.BackgroundColor = Color.FromHex("00549F");
            // Desactivamos el spinner de carga
            ActivitySpinner.IsVisible = false;
            // Desabilitamos todas las entradas 
            // de texto menos la primera
            Entry_Alto.IsEnabled = false;
            Entry_Ancho.IsEnabled = false;
            Entry_Largo.IsEnabled = false;
            Entry_Peso.IsEnabled = false;
            Entry_Unidades.IsEnabled = false;
            Entry_UnidadesA.IsEnabled = false;
            Entry_Volumen.IsEnabled = false;
            // Ocultamos el botón
            ButtonsCubicacion.IsVisible = false;
            // Verificamos la conexión a internet
            CheckConnectivity();
            // Al oprimir enter posiciona el cursor
            // en la siguiente entrada de texto
            Entry_CodeBars.Completed += async (s, e) =>
            {
                // Función que verifica que exista el código de barras
                await VerifyCodeBars();
            };
            // Evento que se genera al cambiar el checkbox
            Checked_All_Units.CheckedChanged += (s, e) =>
            {
                // Posicionamiento del cursor sobre la primera medida
                Entry_Largo.Focus();
            };
            Entry_Largo.Completed += async (s, e) => 
            {
                try
                {
                    if (Convert.ToDouble(Entry_Largo.Text) > 0)
                    {
                        // Esperamos medio segundo
                        await Task.Factory.StartNew(() => {
                            // Habilitamos el siguiente campo
                            Entry_Alto.IsEnabled = true;
                        });
                        await Task.Delay(500);
                        // Posicionamos el cursor
                        Entry_Alto.Focus();
                    }
                    else
                    {
                        var response = await DisplayAlert("Error de campos!", "Primero debe digitar el largo en centímetros.", "OK", "CANCEL");
                        // Verificamos la respuesta
                        if (response)
                            // Posicionamos el cursor
                            Entry_Largo.Focus();
                    }
                }
                catch (Exception er)
                {
                    // Imprimirmos el error
                    Debug.WriteLine(er);
                    // Limpiamos la caja de texto
                    Entry_Largo.Text = "";
                    // Posicionamos el cursor
                    Entry_Largo.Focus();
                }
            };
            Entry_Alto.Completed += async (s, e) => 
            {
                try
                {
                    if (Convert.ToDouble(Entry_Alto.Text) > 0)
                    {
                        // Esperamos medio segundo
                        await Task.Factory.StartNew(() => {
                            // Habilitamos el siguiente campo
                            Entry_Ancho.IsEnabled = true;
                        });
                        await Task.Delay(500);
                        // Posicionamos el cursor
                        Entry_Ancho.Focus();
                    }
                    else
                    {
                        var response = await DisplayAlert("Error de campos!", "Primero debe digitar la altura en centímetros.", "OK", "CANCEL");
                        // Verificamos la respuesta
                        if (response)
                            // Posicionamos el cursor
                            Entry_Alto.Focus();
                    }
                }
                catch (Exception er)
                {
                    // Imprimirmos el error
                    Debug.WriteLine(er);
                    // Limpiamos la caja de texto
                    Entry_Alto.Text = "";
                    // Posicionamos el cursor
                    Entry_Alto.Focus();
                }
            };
            Entry_Ancho.Completed += async (s, e) =>
            {
                try
                {
                    if (Convert.ToDouble(Entry_Ancho.Text) > 0)
                    {
                        // Esperamos medio segundo
                        await Task.Factory.StartNew(() => {
                            // Habilitamos el siguiente campo
                            Entry_Peso.IsEnabled = true;
                        });
                        await Task.Delay(500);
                        // Posicionamos el cursor
                        Entry_Peso.Focus();
                        // Obtenemos el valor de el largo, alto y ancho de la unidad
                        double largo_unidad = Convert.ToDouble(Entry_Largo.Text);
                        double alto_unidad = Convert.ToDouble(Entry_Alto.Text);
                        double ancho_unidad = Convert.ToDouble(Entry_Ancho.Text);
                        // Calculamos el volumen de la unidad en base a su alto y ancho
                        double volumen_unidad = ((largo_unidad * alto_unidad * ancho_unidad) / 1000000) * 400;
                        // Asignamos el valor del volumen
                        imageCubic.Volumen = volumen_unidad;
                        // Asignamos el valor del volumen a la entrada de texto
                        Entry_Volumen.Text = Math.Round(Convert.ToDouble(volumen_unidad), 2).ToString();
                    }
                    else
                    {
                        var response = await DisplayAlert("Error de campos!", "Primero debe digitar el ancho en centímetros.", "OK", "CANCEL");
                        // Verificamos la respuesta
                        if (response)
                            // Posicionamos el cursor
                            Entry_Ancho.Focus();
                    }
                }
                catch (Exception er)
                {  
                    // Imprimirmos el error
                    Debug.WriteLine(er);
                    // Limpiamos la caja de texto
                    Entry_Ancho.Text = "";
                    // Posicionamos el cursor
                    Entry_Ancho.Focus();
                }
            };
            Entry_Peso.Completed += async (s, e) =>
            {
                // Abrimos la vista de la cámara del dispositivo
                await OpenCameraAsync(s, e);
            };
            Btn_ClearRows.Clicked += (s, e) =>
            {
                // Iniicializamos los inputs
                InputsInit();
            };
            Btn_CaptureNewPhoto.Clicked += async (s, e) =>
            {
                // Abrimos la vista de la cámara del dispositivo
                await OpenCameraAsync(s, e);
            };
            Btn_Send.Clicked += async (s, e) =>
            {
                if (CheckInputs())
                    await EnviarMedidasAsync(s, e);
            };
            // Evento que el escaner de código de barras
            Btn_ScannerCodeBars.Clicked += (s, e) =>
            {
                ScanningPage(s, e);
            };
        }

        public void InputsInit()
        {
            // Vaciamos los inputs
            Entry_CodeBars.Text = "";
            Entry_Alto.Text = "";
            Entry_Ancho.Text = "";
            Entry_Largo.Text = "";
            Entry_Peso.Text = "";
            Entry_Volumen.Text = "";
            Entry_Unidades.Text = "1";
            Entry_UnidadesA.Text = "0/0";
            Checked_All_Units.Checked = false;
            // Desabilitamos los campos de medidas
            Entry_Alto.IsEnabled = false;
            Entry_Ancho.IsEnabled = false;
            Entry_Largo.IsEnabled = false;
            Entry_Peso.IsEnabled = false;
            Entry_Unidades.IsEnabled = false;
            // Posicionamos el cursor
            Entry_CodeBars.Focus();
            // Ocultamos el botón de enviar datos
            ButtonsCubicacion.IsVisible = false;
            // Reiniciamos el número de unidades
            imageCubic.Unidades = 1;
            // Cambiamos la imagen
            image_cubic.Source = "camera_icon_gray.png";
        }

        public async Task VerifyCodeBars()
        {
            try
            {
                // Validamos que haya digitado el código de barras
                if (!string.IsNullOrWhiteSpace(Entry_CodeBars.Text))
                {
                    // Verificamos que exista conexión a internet
                    if (Constants.hasInternet)
                    {
                        // Creamos un hilo de ejecución para consumir el servicio del login
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            // Ocultamos el botón de enviar datos
                            Btn_Send.IsVisible = false;
                            // Mostramos el spinner de carga
                            ActivitySpinner.IsVisible = true;
                            // Consumimos el servicio de autenticación de Solex
                            var result = await App.RestClient.GetExistsBarCode(Entry_CodeBars.Text);
                            // Verificamos que el servicio haya devuelto una respuesta
                            if (result != null)
                            {
                                // Verificamos que la respuesta sea afirmativa
                                if (result.Response.IsSuccess)
                                {
                                    // Habilitamos el siguiente campo
                                    Entry_Largo.IsEnabled = true;
                                    // Total de unidades de la guía
                                    Entry_Unidades.Text = result.Unidades.Total.ToString();
                                    // Guardamos las unidades en el objeto
                                    imageCubic.Unidades = result.Unidades.Total;
                                    // Relación del número de unidades auditadas
                                    Entry_UnidadesA.Text = ((result.Unidades.Auditadas <= 0) ? 1 : result.Unidades.Auditadas) + "/" + result.Unidades.Total;
                                    // Posicionamos el cursor
                                    Entry_Largo.Focus();
                                }
                                else
                                    // Mostramos el error de Solex
                                    await DisplayAlert("Respuesta de Solex", result.Response.Error, "OK");
                            }
                            else
                            {
                                // Mostramos el mensaje que devuelve Solex en caso de que algo esté mal
                                // con el usuario que está intentando ingresar
                                await DisplayAlert("Mensaje Informativo", "Ocurrió un error al intentar verificar el código de barras.", "OK");
                            }
                            // Desabilitamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                        });
                    }
                    else
                        await DisplayAlert("Error de conexión!", "No se encuentra una conexión estable a internet.", "OK");
                }
                else
                {
                    await DisplayAlert("Error de campos!", "Primero debe digitar el código de barras.", "OK");
                    Entry_CodeBars.Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                await DisplayAlert("Ocurrió un error!", "Digite o pistolee nuevamente el código de barras de la unidad.", "OK");
                Entry_CodeBars.Text = "";
                Entry_CodeBars.Focus();
            }
        }

        public async Task OpenCameraAsync(object sender, EventArgs e)
        {
            // Verificamos que se encuentren llenos todos los campos
            if (CheckInputs())
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No hay cámara!.", "La cámara no se encuentra disponible.", "OK");

                    return;
                }
                try
                {
                    MediaFile file = await CrossMedia.Current.TakePhotoAsync (
                        new StoreCameraMediaOptions
                        {
                            PhotoSize = PhotoSize.Small,
                            Directory = "ImagesCubing",
                            Name = imageCubic.Codigo_Barras + ".jpg"
                        }
                    );

                    if (file == null)
                        return;

                    var bytes = App.GetImageStreamAsBytes(file.GetStream());
                    string base64 = Convert.ToBase64String(bytes);

                    imageCubic.Imagen = base64;

                    image_cubic.Source = ImageSource.FromStream(() =>
                    {
                        var stream_file = file.GetStream();
                        file.Dispose();
                        return stream_file;
                    });

                    var response_send_solex = await DisplayAlert("Mensaje de confirmación", "¿Desea enviar la información a Solex?", "ACEPTAR", "CANCELAR");

                    if (response_send_solex)
                        await EnviarMedidasAsync(sender, e);

                    // Mostramos el botón de envío de dato
                    Btn_Send.IsVisible = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex);
                    // await DisplayAlert("Ocurrió un error!.", "No se logró acceder a la cámara del dispositivo.", "OK");
                    return;
                }
            }
        }

        // Evento de aparición de la página
        protected override void OnAppearing()
        {
            // Posicionamos el cursor
            Entry_CodeBars.Focus();
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
            // Envolvemos el código en un try catch para poder capturar cualquier tipo de error
            try
            {
                // Instanciamos el objeto de medidas
                imageCubic = new ImageCubic
                {
                    Codigo_Barras = Entry_CodeBars.Text,
                    Alto = Convert.ToDouble(Entry_Alto.Text),
                    Ancho = Convert.ToDouble(Entry_Ancho.Text),
                    Largo = Convert.ToDouble(Entry_Largo.Text),
                    Peso = Convert.ToDouble(Entry_Peso.Text),
                    PesoVolumen = Convert.ToDouble(Entry_Peso.Text),
                    Volumen = Convert.ToDouble(Entry_Volumen.Text),
                    Unidades = Convert.ToInt16(Entry_Unidades.Text),
                    UsuarioId = Convert.ToInt16(Constants.CurrentUser.IdSolex),
                    UsuarioLogin = Constants.CurrentUser.Username
                };
                string TituloAlerta = "Campos vacios!";
                if (string.IsNullOrWhiteSpace(imageCubic.Codigo_Barras))
                {
                    DisplayAlert(TituloAlerta, "Primero debe digitar el código de barras", "OK");
                    return false;
                }
                if (imageCubic.Largo == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el largo en centímetros", "OK");
                    return false;
                }
                if (imageCubic.Alto == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar la altura en centímetros", "OK");
                    return false;
                }
                if (imageCubic.Ancho == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el ancho en centímetros", "OK");
                    return false;
                }
                if (imageCubic.Peso == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el peso en kilos", "OK");
                    return false;
                }
                if (imageCubic.Volumen == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el alto, el ancho y el largo de la unidad para poder calcular su volumen", "OK");
                    return false;
                }
                // Mostramos el botón de enviar a Solex
                ButtonsCubicacion.IsVisible = true;
                // Retornamos la respuesta booleana
                return true;
            }
            catch (Exception er)
            {
                Debug.WriteLine(er);
                return false;
            }
        }

        // Función de autenticación
        async Task EnviarMedidasAsync(object sender, EventArgs e)
        {
            // Envolvemos el código en un try catch para poder capturar cualquier tipo de error
            try
            {
                // Verificamos nuevamente que haya conexión a internet
                CheckConnectivity();
                // Verificamos que exista conexión a internet
                if (Constants.hasInternet)
                {
                    // Creamos un hilo de ejecución para consumir el servicio del login
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Ocultamos el botón de enviar datos
                        Btn_Send.IsVisible = false;
                        // Mostramos el spinner de carga
                        ActivitySpinner.IsVisible = true;
                        // Verificamos si las medidas se van a aplicar a todas las unidades
                        if (Checked_All_Units.Checked && imageCubic.Unidades > 1)
                        {
                            // Mensaje de confirmación
                            var response = await DisplayAlert("Mensaje de Confirmación", "Las medidas se aplicarán a todas las unidades de la guía, ¿está seguro de enviar los datos?.", "ACEPTAR", "CANCELAR");
                            // Verificamos la selección del usuario
                            if (!response)
                            {
                                // Ocultamos el spinner de carga
                                ActivitySpinner.IsVisible = false;
                                // Mostramos el bótón de enviar
                                Btn_Send.IsVisible = true;
                                // Salimos de la función
                                return;
                            }
                        }
                        else
                            imageCubic.Unidades = 1;
                        // Consumimos el servicio de autenticación de Solex
                        var result = await App.RestClient.SaveMedidasCon(imageCubic);
                        // Verificamos que el servicio haya devuelto una respuesta
                        if (result != null)
                        {
                            if (result.IsSuccess)
                                // Iniicializamos los inputs
                                InputsInit();
                            else
                                // Mostramos el error de Solex
                                await DisplayAlert("Ocurrió un error en Solex", result.Error, "OK");
                        }
                        else
                        {
                            // Mostramos el mensaje que devuelve Solex en caso de que algo esté mal
                            // con el usuario que está intentando ingresar
                            await DisplayAlert("Mensaje de Alerta", "Ocurrió un error al intentar enviar las medidas", "OK");
                        }
                        // Desabilitamos el spinner de carga
                        ActivitySpinner.IsVisible = false;
                        // Mostramos nuevamente el botón de envío de datos
                        Btn_Send.IsVisible = true;
                    });
                }
                else
                {
                    await DisplayAlert("Error de conexión!", "No se encuentra una conexión estable a internet.", "OK");
                    return;
                }
            }
            catch (Exception er)
            {
                Debug.WriteLine(er);
                await DisplayAlert("Ocurrió un error!", "No se lograron enviar las medidas.", "OK");
                return;
            }
        }

        // Función para abrir la página del scanner
        async void ScanningPage(object sender, EventArgs e)
        {
            try
            {
                // Se configura el lector de códigos de barras
                var options = new ZXing.Mobile.MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<ZXing.BarcodeFormat>()
                    {
                        ZXing.BarcodeFormat.CODE_128
                    },
                    TryHarder = true,
                    AssumeGS1 = true,
                    AutoRotate = true
                };
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan(options);
                if (result != null)
                {
                    // Capturamos el código de barras
                    string scanResult = result.Text;
                    // Asignamos su valor a la entrada de texto
                    Entry_CodeBars.Text = scanResult;
                    // Verificamos que el código de barras sea válido
                    await VerifyCodeBars();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return;
            }
        }

        // Función para salir de la app
        async void Logout(object sender, EventArgs e)
        {
            // Cerrramos la sesión del usuario desde la BD local
            App.UserDatabase.LoginUser();
            // Verificamos la plataforma y cambiamos la página
            // usando la respectiva animación
            if (Device.RuntimePlatform == Device.Android)
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            else if (Device.RuntimePlatform == Device.iOS)
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }
    }
}