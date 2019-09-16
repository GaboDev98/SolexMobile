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
    public partial class ListPickupsPage : ContentPage
    {
        // Lista de ordenes de cargue
        private List<OrdenCargue> listOrdenesItems = new List<OrdenCargue>();
        // Orde de Cargue seleccionada
        private OrdenCargue selected;
        // Variable que controla la actualización 
        // de datos con el servidor
        private bool loadData = false;

        // Constructor con parámetros
        public ListPickupsPage(bool loadData = true)
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

            // Ocultamos el botón por defecto para volver atrás
            NavigationPage.SetHasNavigationBar(this, false);
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
            // Método que valida la conexión a internet
            var connection = CheckNetworkConnection();
            // Validamos que haya conexión a internet
            if ((!loadData) || !connection)
            // if (!connection)
                LoadDataLocal();
            // Si no hay conexión mostramos la data
            // almacenada en el dispositivo
            else
                LoadData();
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
            // Obtenemos las ordenes de cargue de la BD local
            listOrdenesItems = App.OrdenCargueDatabase.GetAllOrdenes(delivereds: false, userId: Constants.CurrentUser.IdSolex);
            // Agregamos la lista al ListView
            ListView1.ItemsSource = listOrdenesItems;
            // Comprobamos la cantidad de regitros
            if (listOrdenesItems.Count == 0)
            {
                // Mostramos el mensaje de cero registros
                Lbl_ListPickups.Text = "Este dispositivo no tiene ordenes de cargue asignadas para el día de hoy.";
                Lbl_ListPickups.IsVisible = true;
            }
            else
                // Ocultamos el mensaje
                Lbl_ListPickups.IsVisible = false;
            // Posicionamos el cursor en el buscador
            Input_Search.Focus();
        }

        public void LoadData()
        {
            // Instanciamos el listado de ordenes de cargue desde el servidor
            List<ResponseOrdenCargue> Items = new List<ResponseOrdenCargue>();
            // Creamos un hilo de ejecución para consumir el servicio de las ordenes de cargue
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Activamos el spinner
                ActivitySpinner.IsVisible = true;
                // Consultamos las ordenes de cargue por número de placa en el servicio de Solex
                Items = await App.RestClient.GetOrdenesCargueByUser(Constants.CurrentUser.IdSolex.ToString());
                // Reseteamos el listado de ordenes de cargue del usuario actual
                App.OrdenCargueDatabase.DeleteAllOrdenes(Constants.CurrentUser.IdSolex);
                // Recorremos el array de objetos json que devuelve el servicio
                foreach (var Item in Items)
                {
                    OrdenCargue orden = new OrdenCargue
                    {
                        OrdenId = Item.OrdenId,
                        Placa = Item.Placa,
                        Latitud = Item.Latitud.ToString(),
                        Longitud = Item.Longitud.ToString(),
                        PlanillaNumero = Item.Planilla,
                        Sucursal = Item.Sucursal,
                        DANE = Item.DANE,
                        Ciudad = Item.Ciudad,
                        Destinatario = Item.Destinatario,
                        DireccionDestino = Item.DireccionDestino,
                        NombreCliente = Item.NombreCliente,
                        TelefonoCliente = Item.TelefonoCliente,
                        UltimoEstadoId = Item.UltimoEstadoId,
                        UnidadesProgramadas = Item.UnidadesProgramadas,
                        UnidadesRecogidas = Item.UnidadesRecogidas,
                        FechaLlegoAlPunto = Item.FechaLlegoAlPunto,
                        FechaAsignacion = Convert.ToDateTime(Item.FechaAsignacion)
                    };
                    if (Item.Latitud != null && Item.Longitud != null)
                    {
                        orden.Latitud = (Item.Latitud).ToString().Replace(",", ".");
                        orden.Longitud = (Item.Latitud).ToString().Replace(",", ".");
                    }
                    if (Item.UltimoEstadoId == Constants.ESTADO_ORDEN_LLEGO_PUNTO)
                    {
                        orden.Arrival = true;
                        orden.Controlled = false;
                        orden.ArrivalDate = Item.FechaLlegoAlPunto;
                        orden.ColorButtonRow = Constants.CODIGO_COLOR_LLEGADA_PUNTO;
                    }
                    else if (Item.UltimoEstadoId == Constants.ESTADO_ORDEN_EXITOSA)
                    {
                        orden.Delivered = true;
                        orden.Controlled = true;
                        orden.DeliveredDate = Item.FechaRecogida;
                        orden.ColorButtonRow = Constants.CODIGO_COLOR_ENTREGA_BLU;
                    } else
                    {
                        orden.Delivered = false;
                        orden.DeliveredDate = null;
                        orden.Controlled = false;
                        orden.ColorButtonRow = Constants.CODIGO_COLOR_NO_ENTREGADA;
                    }
                    // Se asigna el id del usuario que esta controlando la planilla
                    orden.ControlledUserId = Constants.CurrentUser.IdSolex;
                    // Verificamos si ya existe el registro
                    var exist_orden = App.OrdenCargueDatabase.GetOrdenCargueByIdSolex(Item.OrdenId);
                    if (exist_orden != null)
                        // Asignamos el id para pdoer actualizar los datos de la orden de cargue
                        orden.Id = exist_orden.Id;
                    // Por último llamamos al método de la CRUD de orden de cargue
                    // el cual se encarga de insertar o actualizar la orden de cargue según sea el caso
                    App.OrdenCargueDatabase.SaveOrdenCargue(orden);
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
            // Capturamos la respectiva orden de cargue
            selected = (OrdenCargue)e.Item;
            // Navegamos a la página donde se muestra 
            // la información detallada de la respectiva orden de cargue
            Navigation.PushAsync(new DetailOrdenCarguePage(selected));
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
                ListView1.ItemsSource = listOrdenesItems;
            else
                ListView1.ItemsSource = listOrdenesItems.Where(x => x.OrdenId.ToString().Contains(filter));
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