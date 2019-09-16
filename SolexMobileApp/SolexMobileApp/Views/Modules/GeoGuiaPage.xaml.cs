using System;
using Xamarin.Forms;
using Plugin.Geolocator;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using SolexMobileApp.Controls;
using Plugin.Permissions.Abstractions;

namespace SolexMobileApp.Views.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoGuiaPage : ContentPage
    {
        // Guia y ubicación actual
        private Guia guia;
        public Plugin.Geolocator.Abstractions.Position position;
        // Creamos la variable que contendrá el mapa
        public CustomMap mapa = new CustomMap();

        // Constructor de la clase
        public GeoGuiaPage(Guia guia)
        {
            InitializeComponent();
            // Obtenemos la guia
            this.guia = guia;
            // Inicializamos los componentes
            InitComponets();
        }

        // Función que se encarca de inicializar
        // los componentes principales de la clase
        void InitComponets()
        {
            // Mostramos en el título el número de la guía
            Title = guia.GuiaNumero;
        }

        // Esté método se activa asincrónicamente al momento que aparece la página
        protected override async void OnAppearing()
        {
            // Seteamos las propiedades del mapa
            mapa.MapType = MapType.Street;
            mapa.IsShowingUser = true;
            mapa.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                new Position(4.702, -74.041),
                Distance.FromMiles(5.0))
            );

            // Agregamos el mapa al contenido de la página
            Content = new StackLayout
            {
                Children = {
                    mapa
                }
            };

            // Marcamos la dirección de la guía
            if (guia.Latitud != null && guia.Longitud != null)
                mapa.RouteCoordinates.Add(new Position(Convert.ToDouble(guia.Latitud), Convert.ToDouble(guia.Longitud)));

            //  MyMap.MoveToRegion(
            //  MapSpan.FromCenterAndRadius(
            //  new Xamarin.Forms.Maps.Position(4.702613, -74.041655), Distance.FromMiles(5)));

            // Activamos el método del BroadcastReceiver Location
            // para estar atentos al cambio de posición en el GPS
            await StartListening();

            // Ocultamos el spinner de carga
            ActivitySpinner.IsVisible = false;
        }

        // Esté método se activa al desaparcer la página
        protected override async void OnDisappearing()
        {
            // Desactivamos el tracking de la ubicación
            await CrossGeolocator.Current.StopListeningAsync();
        }

        // Método que inicializa el escuchador o tracking
        // de la ubicación del dispositivo
        async Task StartListening()
        {
            // Si ya se encuentra activo el escuchador
            // salimos de la función
            if (CrossGeolocator.Current.IsListening)
                return;

            // Validamos que existan los permisos necesarios
            // para obtener la ubicación actual por medio del GPS o de la red
            var hasPermission = await Utils.Utils.CheckPermissions(Permission.Location);
            if (!hasPermission)
                return;

            // Esta lógica se ejecutará en segundo plano automáticamente en iOS, sin embargo, 
            // para Android y UWP debe poner lógica en los servicios en segundo plano. 
            // De lo contrario, si se mata tu aplicación, se eliminarán las actualizaciones de la ubicación.
            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = true,
                DeferralDistanceMeters = 1,
                DeferralTime = TimeSpan.FromSeconds(1),
                ListenForSignificantChanges = true,
                PauseLocationUpdatesAutomatically = false
            });

            // Agregamos el método correspondiente al evento de cambio de ubicación
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
        }

        // Metodo que captura la posición actual y la actualiza en pantalla
        private void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Obtenemos la posición actual
                var currentPosition = e.Position;
                // Actualizamos la variable global de posición
                position = currentPosition;
                // Actualizamos la posición actual en el mapa
                mapa.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        new Position(
                        position.Latitude,
                        position.Longitude),
                        Distance.FromMiles(0.3)
                    )
                );
            });
        }
    }
}