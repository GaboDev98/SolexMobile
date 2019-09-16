using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListDeliveredsPage : ContentPage
    {
        // Lista de guías
        private List<Guia> listPlanillaItems = new List<Guia>();
        // Guia seleccionada
        private Guia selected;
        // Variable que controla la actualización 
        // de datos con el servidor
        private bool loadData = false;

        // Constructor con parámetros
        public ListDeliveredsPage(bool loadData = false)
        {
            InitializeComponent();
            // Definitimos la variable 
            // de carga de datos del servidor
            this.loadData = loadData;
            InitComponents();
            InitSearchBar();
        }

        // Seteamos algunas propiedades antes de mostrar la página
        public void InitComponents()
        {
            // Seteamos las algunas propiedades de la página
            BackgroundColor = Constants.BackgroundColor;

            // Agregamos el evento de click al ListView
            ListView1.ItemTapped += ListView1_ItemTapped;
        }

        // Método para válidara que exista o no conexión a internet
        private bool CheckNetworkConnection()
        {
            // Validamos que haya conexión a internet
            Constants.hasInternet = CrossConnectivity.Current.IsConnected;

            // Si hay conexión actualizamos la data
            // desde el servicio de Solex
            if (Constants.hasInternet)
                return true;
            else
                return false;
        }

        // Cuando la página haya terminado de cargar
        // actualizamos los registros del listView
        protected override void OnAppearing()
        {
            try
            {
                // Método que valida la conexión a internet
                var connection = CheckNetworkConnection();
                // Validamos que haya conexión a internet
                if (!loadData || !connection)
                // if (!connection)
                    LoadDataLocal();
                // Si no hay conexión mostramos la data
                // almacenada en el dispositivo
                else
                    LoadData();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RefreshList(object sender, EventArgs e)
        {
            // Activamos la función de refresh en el ListView
            ListView1.BeginRefresh();
            // Validamos que haya conexión a internet
            if (CheckNetworkConnection())
                LoadData();
            else
                LoadDataLocal();
            // Desactivamos la función de refresh
            ListView1.EndRefresh();
        }

        public void LoadDataLocal()
        {
            // Obtenemos las guías de la BD local
            listPlanillaItems = App.GuiaDatabase.GetAllGuias(controlled: false, userId: Constants.CurrentUser.IdSolex);
            // Agregamos la lista al ListView
            ListView1.ItemsSource = listPlanillaItems;
            // Comprobamos la cantidad de regitros
            if (listPlanillaItems.Count == 0)
            {
                // Actualizamos el contenido del mensaje en pantalla
                Lbl_ListNotDelivereds.Text = "Este dispositivo no tiene guías asignadas para el día de hoy.";
                // Mostramos el mensaje de cero registros
                Lbl_ListNotDelivereds.IsVisible = true;
            }
            else
            {
                // Ocultamos el mensaje
                Lbl_ListNotDelivereds.IsVisible = false;
            }
            // Posicionamos el cursor en el buscador
            Input_Search.Focus();
        }

        public void LoadData()
        {
            // Instanciamos el listado de guías desde el servidor
            List<ResponseGuia> Items = new List<ResponseGuia>();
            // Creamos un hilo de ejecución para consumir el servicio de las entregas
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Activamos el spinner
                ActivitySpinner.IsVisible = true;
                // Consultamos las guías por número de placa en el servicio de Solex
                Items = await App.RestClient.GetGuiasByUser(Constants.CurrentUser.IdSolex.ToString(), "0");
                // Reseteamos el listado de entregas del usuario actual
                App.GuiaDatabase.DeleteAllGuias(Constants.CurrentUser.IdSolex, false);
                // Recorremos el array de objetos json que devuelve el servicio
                foreach (var Item in Items)
                {
                    // Creamos el objeto guía de la BD local
                    Guia guia = new Guia
                    {
                        IdGuia = Item.GuiaId,
                        Placa = Item.Placa,
                        PlanillaNumero = Item.Planilla,
                        Sucursal = Item.Sucursal,
                        GuiaNumero = Item.Guia,
                        DANE = Item.DANE,
                        Ciudad = Item.Ciudad,
                        Destinatario = Item.Destinatario,
                        DireccionDestino = Item.DireccionDestino,
                        TelefonoCliente = Item.TelefonoCliente,
                        Unidades = Item.Unidades,
                        UltimoEstadoId = Item.UltimoEstadoId,
                        FechaAsignacion = Convert.ToDateTime(Item.FechaAsignacion),
                        FechaEntrega = Item.FechaEntrega,
                        FechaLlegoAlPunto = Item.FechaLlegoAlPunto,
                        Receives = Item.RecibeNombre,
                        Receives_Doc = Item.RecibeDocumento
                    };
                    if (Item.Latitud != null && Item.Longitud != null)
                    {
                        guia.Latitud = (Item.Latitud).ToString().Replace(",", ".");
                        guia.Longitud = (Item.Latitud).ToString().Replace(",", ".");
                    }
                    if (Item.UltimoEstadoId == Constants.ESTADO_GUIA_LLEGO_PUNTO)
                    {
                        guia.Arrival = true;
                        guia.Controlled = false;
                        guia.ArrivalDate = Item.FechaLlegoAlPunto;
                        guia.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                    }
                    else if (Item.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_BLU)
                    {
                        guia.Delivered = true;
                        guia.Controlled = true;
                        guia.DeliveredDate = Item.FechaEntrega;
                        guia.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_BLU;
                    }
                    else if (Item.UltimoEstadoId == Constants.ESTADO_GUIA_ENTREGA_PARCIAL)
                    {
                        guia.Delivered = true;
                        guia.Controlled = true;
                        guia.DeliveredDate = Item.FechaEntrega;
                        guia.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_PARCIAL;
                    }
                    else if (Item.UltimoEstadoId == Constants.ESTADO_GUIA_NO_ENTREGADA)
                    {
                        guia.Delivered = false;
                        guia.Controlled = true;
                        guia.ColorButtonRow = Constants.CODIGO_COLOR_NO_ENTREGADA;
                    }
                    else
                    {
                        guia.Arrival = false;
                        guia.Controlled = false;
                        guia.ColorButtonRow = Constants.CODIGO_COLOR_NO_ENTREGADA;
                    }
                    // Se asigna el id del usuario que esta controlando la planilla
                    guia.ControlledUserId = Constants.CurrentUser.IdSolex;
                    // Verificamos si ya existe el registro
                    var exist_guia = App.GuiaDatabase.GetGuiaByIdSolex(Item.GuiaId);
                    if (exist_guia != null)
                        // Asignamos el id para pdoer actualizar los datos de la guía
                        guia.Id = exist_guia.Id;
                    // Por último llamamos al método de la CRUD de guía,
                    // el cual se encarga de insertar o actualizar la guía según sea el caso
                    App.GuiaDatabase.SaveGuia(guia);
                }
                // Actualizamos los datos del ListView
                LoadDataLocal();
                // Ocultamos el spinner
                ActivitySpinner.IsVisible = false;
                // Posicionamos el cursor en el buscador
                Input_Search.Focus();
            });
        }

        // Método que captura el evento de seleccionar una celada de la lista
        private void ListView1_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Variable que permite navegar a la siguiente página 
            // y volver sin necesidad de preguntar al servicio
            loadData = false;
            // Capturamos la respectiva guía
            selected = (Guia)e.Item;
            // Navegamos a la página donde se muestra 
            // la información detallada de la respectiva guía
            Navigation.PushAsync(new DetailGuiaPage(selected));
        }

        // Método que inicializa la barra de busqueda
        void InitSearchBar()
        {
            // Capturamos el evento de cambio en el valor del Entry
            Input_Search.TextChanged += (s, e) => FilterItemPlanilla(Input_Search.Text);
            // Evento de presionar el Entry
            Input_Search.SearchButtonPressed += (s, e) => FilterItemPlanilla(Input_Search.Text);
        }

        private void FilterItemPlanilla(string filter)
        {
            // Activamos la función de refresh en el ListView
            ListView1.BeginRefresh();
            // Verificamos que el campo de búsqueda
            // no esté completamente vacío
            if (string.IsNullOrWhiteSpace(filter))
                ListView1.ItemsSource = listPlanillaItems;
            else
                ListView1.ItemsSource = listPlanillaItems.Where(x => x.GuiaNumero.Contains(filter));
            // Desactivamos la función de refresh
            ListView1.EndRefresh();
        }

        // Función o método que nos redirige al listado principal
        async void MainListOpen(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new TabbedListPage(false));
            }
            catch (Exception er)
            {
                Debug.WriteLine(er);
            }
        }
    }
}