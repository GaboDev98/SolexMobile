using System.IO;
using Xamarin.Forms;
using SolexMobileApp.Data;
using SolexMobileApp.Models;
using System.Collections.Generic;
using SolexMobileApp.Controllers;

namespace SolexMobileApp
{
    public partial class App : Application
    {
        static RestClient restClient;
        static RestService restService;
        private static Label labelScreen;
        static UserDatabaseController userDatabase;
        static GuiaDatabaseController guiaDatabase;
        static TokenDatabaseController tokenDatabase;
        static UnidadDatabaseController unidadDatabase;
        static SettingsDatabaseController settingsDatabase;
        static OrdenCargueDatabaseController ordenDatabase;
        static ImageGuiaDatabaseController imageGuiaDatabase;
        static EstadoGuiaDatabaseController estadoGuiaDatabase;
        static DetalleUnidadesDatabaseController detalleUnidadesDatabase;

        public static MainModelo ModeloMain { get; set; }

        public App()
        {
            // Inicializamos los componentes
            InitializeComponent();
            // Actualizamos la lista de estados desde Solex
            GetEstadosGuia();
            // Obtenemos el último usuario activo
            // en la aplicación, verificamos que no haya 
            // una sesión activa, guardamos el usuario actual
            Constants.CurrentUser = UserDatabase.GetLastUserActive();
            // Configuración actual
            Constants.CurrentSettings = SettingsDatabase.GetSettings();
            // Clase global de atributos de la aplicación
            ModeloMain = new MainModelo();
            // Verificamos que la sesión no se encuentre activa
            if ((Constants.CurrentUser != null) && (Constants.CurrentUser.IsLogin))
            {
                // Obtenemos el token del usuario actual
                Constants.BearerToken = TokenDatabase.GetToken().Access_token;
                // Redirigimos al dashboard
                MainPage = new NavigationPage(new Views.Menu.DashboardPage());
            }
            else
                MainPage = new NavigationPage(new Views.LoginPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            // Se actualiza el id del dispositivo
            Constants.IdDevice = ModeloMain.IdMaquina;
            // Se actualiza la versión de la app
            Constants.VersionName = ModeloMain.VersionName;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static UserDatabaseController UserDatabase
        {
            get
            {
                if(userDatabase == null)
                {
                    userDatabase = new UserDatabaseController();
                }
                return userDatabase;
            }
        }

        public static TokenDatabaseController TokenDatabase
        {
            get
            {
                if (tokenDatabase == null)
                {
                    tokenDatabase = new TokenDatabaseController();
                }
                return tokenDatabase;
            }
        }

        public static SettingsDatabaseController SettingsDatabase
        {
            get
            {
                if (settingsDatabase == null)
                {
                    settingsDatabase = new SettingsDatabaseController();
                }
                return settingsDatabase;
            }
        }

        public static GuiaDatabaseController GuiaDatabase
        {
            get
            {
                if (guiaDatabase == null)
                {
                    guiaDatabase = new GuiaDatabaseController();
                }
                return guiaDatabase;
            }
        }

        public static UnidadDatabaseController UnidadDatabase
        {
            get
            {
                if (unidadDatabase == null)
                {
                    unidadDatabase = new UnidadDatabaseController();
                }
                return unidadDatabase;
            }
        }

        public static DetalleUnidadesDatabaseController DetalleUnidadesDatabase
        {
            get
            {
                if (detalleUnidadesDatabase == null)
                {
                    detalleUnidadesDatabase = new DetalleUnidadesDatabaseController();
                }
                return detalleUnidadesDatabase;
            }
        }

        public static OrdenCargueDatabaseController OrdenCargueDatabase
        {
            get
            {
                if (ordenDatabase == null)
                {
                    ordenDatabase = new OrdenCargueDatabaseController();
                }
                return ordenDatabase;
            }
        }

        public static ImageGuiaDatabaseController ImageGuiaDatabase
        {
            get
            {
                if (imageGuiaDatabase == null)
                {
                    imageGuiaDatabase = new ImageGuiaDatabaseController();
                }
                return imageGuiaDatabase;
            }
        }

        public static EstadoGuiaDatabaseController EstadoGuiaDatabase
        {
            get
            {
                if (estadoGuiaDatabase == null)
                {
                    estadoGuiaDatabase = new EstadoGuiaDatabaseController();
                }
                return estadoGuiaDatabase;
            }
        }

        public static RestService RestService
        {
            get
            {
                if (restService == null)
                {
                    restService = new RestService();
                }
                return restService;
            }
        }

        public static RestClient RestClient
        {
            get
            {
                if (restClient == null)
                {
                    restClient = new RestClient();
                }
                return restClient;
            }
        }

        // ------------ Internet Connection -------------------------
        public static void StartCheckIfInternet(Label label, Page page)
        {
            labelScreen = label;
            label.Text = Constants.NoInternetText;
        }

        public void GetEstadosGuia()
        {
            // Instanciamos el listado de estados desde el servidor
            List<ResponseGuiaEstados> Items = new List<ResponseGuiaEstados>();
            // Creamos un hilo de ejecución para consumir el servicio de los estados
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Eliminamos todos los registros de la BD
                App.EstadoGuiaDatabase.DeleteAllEstados();
                // Consultamos las guías por número de placa en el servicio de Solex
                Items = await RestClient.GetEstadosGuiaMobile();
                // Recorremos el array de objetos json que devuelve el servicio
                foreach (var Item in Items)
                {
                    Estado estado = new Estado
                    {
                        IdSolex = Item.Id,
                        Nombre = Item.Nombre,
                        Tipo = "Guía"
                    };
                    // Por último llamamos al método de la CRUD de los estados,
                    // el cual se encarga de insertar o actualizar el estado según sea el caso
                    EstadoGuiaDatabase.SaveEstado(estado);
                }
            });
        }

        public static byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
