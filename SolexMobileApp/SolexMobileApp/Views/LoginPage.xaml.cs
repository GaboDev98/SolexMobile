using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using SolexMobileApp.ViewModels;
using SolexMobileApp.Views.Menu;
using SolexMobileApp.Views.Modules;

namespace SolexMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        // Constructor vacío
        public LoginPage()
        {
            InitializeComponent();
            // Inicializzación customizada de componentes
            InitComponents();
            // Definimos la clase de binding context 
            // o construcción de doble sentido
            BindingContext = new LoginViewModel();
        }

        protected override void OnAppearing()
        {
            try
            {
                ((LoginViewModel)BindingContext).GetDeviceId();
                ((LoginViewModel)BindingContext).GetVersionName();
                base.OnAppearing();
            }
            catch(Exception er)
            {
                Debug.WriteLine("Error: " + er.Message);
            }
        }

        // Seteamos algunas propiedades antes de mostrar la página
        void InitComponents()
        {
            // Ocultamos la barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);
            // Evento del botón para iniciar sesión
            Btn_SignIn.Clicked += async (s, e) =>
            {
                await SignInProcedureAsync(s, e);
            };
            // Seteamos las propiedades de la página
            BackgroundColor = Constants.BackgroundColor;
            Lbl_Username.TextColor = Constants.MainTextColor;
            Lbl_Password.TextColor = Constants.MainTextColor;
            Btn_SignIn.BackgroundColor = Color.FromHex("00549F");
            // Desactivamos el spinner de carga
            ActivitySpinner.IsVisible = false;
            // Ajustamos el tamaño del icono del login
            LoginIcon.HeightRequest = Constants.LoginIconHeight;
            // Verificamos la conexión a internet
            CheckConnectivity();
            // Cuando haya digitado el campo de usuario enfocará el de la contraseña
            Entry_Username.Completed += (s, e) => Entry_Password.Focus();
            // Cuando haya digitado el campo de password hará un submit de la pagina
            Entry_Password.Completed += async (s, e) => await SignInProcedureAsync(s, e);
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

        // Función de autenticación
        async Task SignInProcedureAsync(object sender, EventArgs e)
        {
            // Capturamos las credenciales
            User user = new User(Entry_Username.Text, Entry_Password.Text);
            // Validación de campos vacíos
            if (user.CheckInformation())
            {
                // Instanciamos la clase para mapear
                // el request del login
                var request = new RequestAuth
                {
                    // Capturamos los parámetros de credenciales
                    // los cuales van a ser enviados al servicio de login
                    Grant_type = "password",
                    Username = user.Username,
                    Password = user.Password
                };
                // Globalizamos los datos del usuario
                Constants.CurrentUser = user;
                // Globalizamos las credenciales
                Constants.RequestAuth = request;
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
                        // Ocultamos el botón de submit
                        Btn_SignIn.IsVisible = false;
                        // Try catch para excepción de errores
                        try
                        {
                            // Consumimos el servicio de autenticación de Solex
                            var result = await App.RestClient.Login<ResponseAuth>();
                            // Verificamos que el servicio haya devuelto una respuesta
                            if (result != null)
                            {
                                // Verificamos que las credenciales sean correctas
                                if (result.Codigo == 1)
                                {
                                    // Verificamos que el usuario tenga roles asignados
                                    if (result.Roles != null)
                                    {
                                        // Consultamos los datos del usuario en la BD local
                                        var userDB = App.UserDatabase.GetUserByUsername(user.Username);
                                        // Solo en caso de que ya se encuentre registrado en el dispositivo
                                        if (userDB != null)
                                            user.Id = userDB.Id;
                                        // Activamos la propiedad de login
                                        user.IsLogin = true;
                                        // Asignamos el id de solex
                                        user.IdSolex = result.UsuarioId;
                                        // Asignamos la placa al usuario
                                        user.Placa = result.Placa;
                                        // Actualizamos la hora de actualización 
                                        // del registro del usuario
                                        user.Updated_at = DateTime.Now;
                                        // Valor por defecto para los permisos de cubicación
                                        Constants.CurrentUser.IsCubage = false;
                                        // Recorremos los roles uno a uno
                                        foreach (string x in result.Roles)
                                        {
                                            // Chequamos si el usuario tiene el rol de administrador
                                            if (x.Equals("Administrator"))
                                            {
                                                Constants.CurrentUser.IsCubage = true;
                                                Constants.CurrentUser.IsAdmin = true;
                                            }
                                            else
                                            {
                                                // Chequamos si el usuario tiene el rol de cubicación manual
                                                if (x.Equals("Cubicación manual"))
                                                    Constants.CurrentUser.IsCubage = true;
                                            }
                                        }
                                        // Guardamos el dato de los permisos de cubicación
                                        user.IsCubage = Constants.CurrentUser.IsCubage;
                                        // Guardamso el dato del rol de administrador
                                        user.IsAdmin = Constants.CurrentUser.IsAdmin;
                                        // Guardamos los datos del usuario
                                        // en la base de datos local
                                        App.UserDatabase.SaveUser(user);
                                        // Globalizamos el usuario que acaba de ingresar
                                        Constants.CurrentUser = App.UserDatabase.GetLastUserActive();
                                        // Junto con el token que es devuelto desde Solex
                                        App.TokenDatabase.SaveToken(result.Access_token);
                                        // Globalizamos el token de la sesión
                                        if (App.TokenDatabase.GetToken() != null)
                                            // Se obtien el token actual en caso de que no este asignado
                                            Constants.BearerToken = App.TokenDatabase.GetToken().Access_token;
                                        // Si el usuario tiene como único rol el de cubicación
                                        if (Constants.CurrentUser.IsCubage && result.Roles.Length == 1)
                                        {
                                            // Verificamos la plataforma para saber
                                            // de que manera se debe hacer el cambio de página
                                            if (Device.RuntimePlatform == Device.Android)
                                                await Navigation.PushAsync(new CubingPage());
                                            else if (Device.RuntimePlatform == Device.iOS)
                                                await Navigation.PushModalAsync(new NavigationPage(new CubingPage()));
                                        }
                                        else
                                        {
                                            // Verificamos la plataforma para saber
                                            // de que manera se debe hacer el cambio de página
                                            if (Device.RuntimePlatform == Device.Android)
                                                await Navigation.PushAsync(new DashboardPage());
                                            else if (Device.RuntimePlatform == Device.iOS)
                                                await Navigation.PushModalAsync(new NavigationPage(new DashboardPage()));
                                        }
                                    }
                                    else
                                    {
                                        // Mostramos el mensaje informativo
                                        await DisplayAlert("Login", "El usuario no tiene roles asignados!", "OK");
                                    }
                                }
                                else
                                {
                                    // Mensaje informativo del error
                                    await DisplayAlert("Respuesta de Solex", result.Respuesta, "OK");
                                }
                            }
                            else
                            {
                                // Mensaje informativo del error
                                await DisplayAlert("Login", "Ocurrió un error al intentar loguearte!", "OK");
                            } 
                        }
                        catch(Exception er)
                        {
                            // Mensaje informativo del error
                            await DisplayAlert("Login", "Ocurrió un error al intentar loguearte!", "OK");
                            // Impresión del error en consola
                            Debug.WriteLine("Error: " + er.Message);
                        }
                        // Desabilitamos el spinner de carga
                        ActivitySpinner.IsVisible = false;
                        // Mostramos el botón de submit
                        Btn_SignIn.IsVisible = true;
                        // Posicionamiento del cursor
                        Entry_Username.Focus();
                    });
                }
                else
                {
                    // Se busca el usuario 
                    User userDB = App.UserDatabase.GetUserByUsername(user.Username);
                    // Si el usuario existe en el dipositivo, entonces verificamos
                    // que las credenciales de acceso sean correctas
                    if (userDB != null)
                    {
                        if (!App.UserDatabase.CheckCredentialsUserLocal(user.Username, user.Password))
                            await DisplayAlert("Login", "Crendenciales Incorrectas", "OK");
                        else
                        {
                            // Verificamos la plataforma para saber
                            // de que manera se debe hacer el cambio de página
                            if (Device.RuntimePlatform == Device.Android)
                                Application.Current.MainPage = new NavigationPage(new DashboardPage());
                            else if (Device.RuntimePlatform == Device.iOS)
                                await Navigation.PushModalAsync(new NavigationPage(new DashboardPage()));
                        }
                    }
                    // Esto ocurre en dado caso de que no haya conexión a internet  
                    // y el usuario no haya sido autenticado anteriormente en ese dispositivo
                    else
                        await DisplayAlert("Login", "El Usuario no se encuentra registrado en este dispositivo!", "OK");
                }
            }
            // En caso de que alguno de los campos del login esté vacio
            else
            {
                // Mensaje de validación de campos
                await DisplayAlert("Error de campos vacios!", "Por favor digita tus credenciales de acceso, para que puedas ingresar a la aplicación.", "OK");
            }
            // Desabilitamos el spinner de carga
            ActivitySpinner.IsVisible = false;
        }
    }
}