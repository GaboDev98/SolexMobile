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
    public partial class MassiveCubing : ContentPage
    {
        // Objeto que contiene los datos que se envían a Solex
        private MassiveCubic massiveCubic = new MassiveCubic();

        public MassiveCubing()
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
            Unidades_Pistoleadas.BackgroundColor = Color.FromHex("00549F");
            Unidades_Faltantes.BackgroundColor = Color.FromHex("00549F");
            Total_Unidades.BackgroundColor = Color.FromHex("00549F");
            // Desactivamos el spinner de carga
            ActivitySpinner.IsVisible = false;
            // Desabilitamos todas las entradas 
            // de texto menos la primera
            Entry_Alto.IsEnabled = false;
            Entry_Ancho.IsEnabled = false;
            Entry_Largo.IsEnabled = false;
            Entry_Peso.IsEnabled = false;
            // Verificamos la conexión a internet
            CheckConnectivity();
            // Al oprimir enter posiciona el cursor
            // en la siguiente entrada de texto
            Entry_CodeBars.Completed += async (s, e) =>
            {
                // Función que verifica que exista el código de barras
                await VerifyCodeBars();
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
                        massiveCubic.Volumen = volumen_unidad;
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
                // Inicializamos los inputs
                InputsInit();
            };
            Btn_CaptureNewPhoto.Clicked += async (s, e) =>
            {
                // Abrimos la vista de la cámara del dispositivo
                await OpenCameraAsync(s, e);
            };
            // Evento que el escaner de código de barras
            Btn_ScannerCodeBars.Clicked += (s, e) =>
            {
                ScanningPage(s, e);
            };
        }

        public void InputsInit()
        {
            // Se limpia el nombre de la empresa
            Labl_Empresa.Text = "";
            Labl_Empresa.IsVisible = false;
            // Reiniciamos los contadores
            Unidades_Pistoleadas.Text = "0";
            Unidades_Faltantes.Text = "0";
            Total_Unidades.Text = "0";
            // Vaciamos los inputs
            Entry_CodeBars.Text = "";
            Entry_Alto.Text = "";
            Entry_Ancho.Text = "";
            Entry_Largo.Text = "";
            Entry_Peso.Text = "";
            // Desabilitamos los campos de medidas
            Entry_Alto.IsEnabled = false;
            Entry_Ancho.IsEnabled = false;
            Entry_Largo.IsEnabled = false;
            Entry_Peso.IsEnabled = false;
            // Se oculta el spinner
            ActivitySpinner.IsVisible = false;
            // Posicionamos el cursor
            Entry_CodeBars.Focus();
            // Cambiamos la imagen
            image_cubic.Source = "camera_icon_gray.png";
            // Mostramos la imagen de la cámara
            image_cubic.IsVisible = true;
            // Se eliminan todas las lecturas del usuario
            App.UnidadDatabase.DeleteAllUnidades();
        }

        public void UpdateCountsLabel(ResponseGuiaExist result)
        {
            // Obtenemos la cantidad de unidades pistoleadas
            int count_unidades = App.UnidadDatabase.GetUnidadesByIdGuia(massiveCubic.Id_Guia).Count;
            // Se incrementa el contador de unidades
            Unidades_Pistoleadas.Text = count_unidades.ToString();
            // Se descuenta las unidades faltantes
            Unidades_Faltantes.Text = (result.Unidades.Total - count_unidades).ToString();
            // Se actualiza el total de unidades
            Total_Unidades.Text = result.Unidades.Total.ToString();
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
                            // Mostramos el spinner de carga
                            ActivitySpinner.IsVisible = true;
                            // Se oculta la imágen de la cámara
                            image_cubic.IsVisible = false;
                            // Consumimos el servicio de autenticación de Solex
                            var result = await App.RestClient.GetExistsBarCode(Entry_CodeBars.Text);
                            // Verificamos que el servicio haya devuelto una respuesta
                            if (result != null)
                            {
                                // Verificamos que la respuesta sea afirmativa
                                if (result.Response.IsSuccess)
                                {
                                    // Se verifica que exista información de la unidad
                                    if (result.Unidades != null)
                                    {
                                        // Verificamos si ya se pistoleo la unidad
                                        var exist_unidad = App.UnidadDatabase.GetUnidadByIdSolex(result.Unidades.IdUnidad);
                                        // Si la unidad ya fue leída
                                        if (exist_unidad != null)
                                        {
                                            // Mostramos el mensaje de alerta
                                            await DisplayAlert("Mensaje Informativo", "Unidad ya leída.", "OK");
                                            // Limpiamos el nombre de la empresa
                                            Labl_Empresa.Text = "";
                                            // Visualización del label
                                            Labl_Empresa.IsVisible = false;
                                            // Se limpia el valor de la entrada
                                            Entry_CodeBars.Text = "";
                                            // Se posiciona el cursor
                                            Entry_CodeBars.Focus();
                                            // Salimos de la función
                                            return;
                                        }
                                        // Si la unidad ya fue cubicada por sorter
                                        if (result.Unidades.Sorter)
                                        {
                                            // Mostramos el mensaje de alerta
                                            await DisplayAlert("Mensaje Informativo", "La unidad ya fue cubicada por Sorter.", "OK");
                                            // Limpiamos el nombre de la empresa
                                            Labl_Empresa.Text = "";
                                            // Visualización del label
                                            Labl_Empresa.IsVisible = false;
                                            // Se limpia el valor de la entrada
                                            Entry_CodeBars.Text = "";
                                            // Se posiciona el cursor
                                            Entry_CodeBars.Focus();
                                            // Salimos de la función
                                            return;
                                        }
                                        else
                                        {
                                            // Se verifica si la unidad ya fue cubicada
                                            if (result.Unidades.Cubicada)
                                            {
                                                // Mostramos el mensaje de alerta
                                                var response = await DisplayAlert("Mensaje Informativo", "La unidad ya fue cubicada, ¿Desea actualizar su medidas?.", "SI", "NO");
                                                // Si la respuesta es negativa
                                                if (!response)
                                                {
                                                    // Limpiamos el nombre de la empresa
                                                    Labl_Empresa.Text = "";
                                                    // Visualización del label
                                                    Labl_Empresa.IsVisible = false;
                                                    // Se limpia el valor de la entrada
                                                    Entry_CodeBars.Text = "";
                                                    // Se posiciona el cursor
                                                    Entry_CodeBars.Focus();
                                                    // Salimos de la función
                                                    return;
                                                }
                                            }
                                        }
                                        // Se verifica si es la primera lectura de la guía
                                        if (massiveCubic.Id_Guia <= 0)
                                        {
                                            // Se asignan los datos de la guía
                                            massiveCubic.Id_Guia = Convert.ToInt64(result.Unidades.IdGuia);
                                        }
                                        else
                                        {
                                            if (massiveCubic.Id_Guia != result.Unidades.IdGuia)
                                            {
                                                // Mostramos el mensaje de alerta
                                                await DisplayAlert("Mensaje Informativo", "La unidad pertenece a una guía diferente.", "OK");
                                                // Limpiamos el nombre de la empresa
                                                Labl_Empresa.Text = "";
                                                // Visualización del label
                                                Labl_Empresa.IsVisible = false;
                                                // Se limpia el valor de la entrada
                                                Entry_CodeBars.Text = "";
                                                // Se posiciona el cursor
                                                Entry_CodeBars.Focus();
                                                // Salimos de la función
                                                return;
                                            }
                                        }
                                        // Si la empresa no se ha mostrado
                                        if (string.IsNullOrWhiteSpace(Labl_Empresa.Text))
                                        {
                                            // Asignamos el nombre de la empresa
                                            Labl_Empresa.Text = result.Unidades.Empresa;
                                        }
                                        // Si no se ha mostrado
                                        if (!Labl_Empresa.IsVisible)
                                        {
                                            // Visualización del label
                                            Labl_Empresa.IsVisible = true;
                                        }
                                        // Instanciación de la unidad
                                        Unidad unidad = new Unidad
                                        {
                                            IdGuia = massiveCubic.Id_Guia,
                                            NumeroUnidad = result.Unidades.NumeroUnidad,
                                            IdUnidad = result.Unidades.IdUnidad,
                                            ControlledUserId = Constants.CurrentUser.IdSolex
                                        };
                                        // Registro de la unidad
                                        App.UnidadDatabase.SaveUnidad(unidad);
                                        // Se llama a la función que actualiza los contadores
                                        UpdateCountsLabel(result);
                                    }
                                    else
                                    {
                                        // Limpiamos el nombre de la empresa
                                        Labl_Empresa.Text = "";
                                        // Visualización del label
                                        Labl_Empresa.IsVisible = false;
                                        // Mostramos el error de Solex
                                        await DisplayAlert("Respuesta de Solex", "No se pudo obtener infromación de la unidad.", "OK");
                                    }
                                    // Se limpia el valor de la entrada
                                    Entry_CodeBars.Text = "";
                                    // Se posiciona el cursor
                                    Entry_CodeBars.Focus();
                                    // Se habilitada la entrada para empezar a digitar las medidas
                                    Entry_Largo.IsEnabled = true;
                                }
                                else
                                {
                                    // Limpiamos el nombre de la empresa
                                    Labl_Empresa.Text = "";
                                    // Visualización del label
                                    Labl_Empresa.IsVisible = false;
                                    // Se limpia el valor de la entrada
                                    Entry_CodeBars.Text = "";
                                    // Se posiciona el cursor
                                    Entry_CodeBars.Focus();
                                    // Mostramos el error de Solex
                                    await DisplayAlert("Respuesta de Solex", result.Response.Error, "OK");
                                }
                            }
                            else
                            {
                                // Limpiamos el nombre de la empresa
                                Labl_Empresa.Text = "";
                                // Visualización del label
                                Labl_Empresa.IsVisible = false;
                                // Se limpia el valor de la entrada
                                Entry_CodeBars.Text = "";
                                // Se posiciona el cursor
                                Entry_CodeBars.Focus();
                                // Mostramos el mensaje que devuelve Solex en caso de que algo esté mal
                                // con el usuario que está intentando ingresar
                                await DisplayAlert("Mensaje Informativo", "Ocurrió un error al intentar verificar el código de barras.", "OK");
                            }
                            // Ocultamos el spinner de carga
                            ActivitySpinner.IsVisible = false;
                            // Se muestra la imágen de la cámara
                            image_cubic.IsVisible = true;
                        });
                    }
                    else
                        await DisplayAlert("Error de conexión!", "No se encuentra una conexión estable a internet.", "OK");
                }
                else
                {
                    await DisplayAlert("Error de campos!", "Primero debe digitar el código de barras.", "OK");
                    // Se posiciona el cursor
                    Entry_CodeBars.Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                await DisplayAlert("Ocurrió un error!", "Digite o pistolee nuevamente el código de barras de la unidad.", "OK");
                // Se limpia el valor de la entrada
                Entry_CodeBars.Text = "";
                // Se posiciona el cursor
                Entry_CodeBars.Focus();
                // Ocultamos el spinner de carga
                ActivitySpinner.IsVisible = false;
                // Se oculta la imágen de la cámara
                image_cubic.IsVisible = false;
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
                            Name = massiveCubic.Codigo_Barras + ".jpg"
                        }
                    );

                    if (file == null)
                        return;

                    var bytes = App.GetImageStreamAsBytes(file.GetStream());
                    string base64 = Convert.ToBase64String(bytes);

                    massiveCubic.Imagen = base64;

                    image_cubic.Source = ImageSource.FromStream(() =>
                    {
                        var stream_file = file.GetStream();
                        file.Dispose();
                        return stream_file;
                    });

                    var response_send_solex = await DisplayAlert("Mensaje de confirmación", "¿Desea enviar la información a Solex?", "ACEPTAR", "CANCELAR");

                    if (response_send_solex)
                        await EnviarMedidasAsync(sender, e);
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
                massiveCubic = new MassiveCubic
                {
                    Id_Guia = massiveCubic.Id_Guia,
                    Codigo_Barras = Entry_CodeBars.Text,
                    Alto = Convert.ToDouble(Entry_Alto.Text),
                    Ancho = Convert.ToDouble(Entry_Ancho.Text),
                    Largo = Convert.ToDouble(Entry_Largo.Text),
                    Peso = Convert.ToDouble(Entry_Peso.Text),
                    PesoVolumen = Convert.ToDouble(Entry_Peso.Text),
                    Volumen = massiveCubic.Volumen,
                    UsuarioId = Convert.ToInt16(Constants.CurrentUser.IdSolex),
                    UsuarioLogin = Constants.CurrentUser.Username
                };
                string TituloAlerta = "Campos vacios!";
                if (App.UnidadDatabase.GetUnidadesByIdGuia(massiveCubic.Id_Guia).Count <= 0)
                {
                    DisplayAlert(TituloAlerta, "No se ha pistoleado ningún código de barras", "OK");
                    return false;
                }
                if (massiveCubic.Largo == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el largo en centímetros", "OK");
                    return false;
                }
                if (massiveCubic.Alto == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar la altura en centímetros", "OK");
                    return false;
                }
                if (massiveCubic.Ancho == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el ancho en centímetros", "OK");
                    return false;
                }
                if (massiveCubic.Peso == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el peso en kilos", "OK");
                    return false;
                }
                if (massiveCubic.Volumen == 0)
                {
                    DisplayAlert(TituloAlerta, "Debe digitar el alto, el ancho y el largo de la unidad para poder calcular su volumen", "OK");
                    return false;
                }
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
                        // Mostramos el spinner de carga
                        ActivitySpinner.IsVisible = true;
                        // Consultamos las unidades leídas
                        var unidades_leidas = App.UnidadDatabase.GetUnidadesByIdGuia(massiveCubic.Id_Guia);
                        // Contador de posiciones
                        int contador = 0;
                        // Se limpia el campo
                        massiveCubic.IdsNumeros = "";
                        // Variable para los números de las unidades
                        string numeros_unidades = "";
                        // Asignamos las posiciones del array
                        foreach (var unidad in unidades_leidas)
                        {
                            numeros_unidades += unidad.NumeroUnidad.ToString();
                            contador++;
                            if (contador < unidades_leidas.Count)
                                numeros_unidades += ",";
                        }
                        // Se llena el campo
                        massiveCubic.IdsNumeros = numeros_unidades;
                        // Consumimos el servicio de autenticación de Solex
                        var result = await App.RestClient.SaveMassiveUnidades(massiveCubic);
                        // Verificamos que el servicio haya devuelto una respuesta
                        if (result != null)
                        {
                            if (result.IsSuccess)
                            {
                                // Iniicializamos los inputs
                                InputsInit();
                                // Se limpia el objeto principal
                                massiveCubic = new MassiveCubic();
                            }
                            else
                            {
                                // Mostramos el error de Solex
                                await DisplayAlert("Ocurrió un error en Solex", result.Error, "OK");
                            }
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