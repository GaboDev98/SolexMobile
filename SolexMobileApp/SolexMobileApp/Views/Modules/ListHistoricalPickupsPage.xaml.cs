using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;
using SolexMobileApp.Models;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListHistoricalPickupsPage : ContentPage
    {
        // Lista de ordenes de cargue
        private List<OrdenCargue> listPlanillaItems = new List<OrdenCargue>();
        // Orden seleccionada
        private OrdenCargue selected;
        // Variable que controa la actualización 
        // de datos con el servidor
        private bool loadData = false;

        // Constructor con parámetros
        public ListHistoricalPickupsPage(bool loadData = false)
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

            // Mostramos el listado de tareas realizadas
            RefreshList();
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

        private void RefreshList()
        {
            // Activamos la función de refresh en el ListView
            ListView1.BeginRefresh();
            // Función que devuelve el listado de ordenes de cargue
            LoadDataLocal();
            // Desactivamos la función de refresh
            ListView1.EndRefresh();
        }

        public void LoadDataLocal()
        {
            // Obtenemos las ordenes de la BD local
            listPlanillaItems = App.OrdenCargueDatabase.GetAllOrdenes(true, userId: Constants.CurrentUser.IdSolex);
            // Agregamos la lista al ListView
            ListView1.ItemsSource = listPlanillaItems;
            // Verificamos el número de registros
            if (listPlanillaItems.Count == 0)
            {
                Lbl_ListHistoricalPickups.Text = "Este dispositivo no registra tareas realizadas.";
                // Mostramos el mensaje de cero registros
                Lbl_ListHistoricalPickups.IsVisible = true;
            }
            // Ocultamos el spinner de carga
            ActivitySpinner.IsVisible = false;
            // Posicionamos el cursor en el buscador
            Input_Search.Focus();
        }

        // Método que captura el evento de seleccionar una celada de la lista
        private void ListView1_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Capturamos la respectiva orden
            selected = (OrdenCargue)e.Item;
            // Navegamos a la página donde se muestra 
            // la información detallada de la respectiva orden
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
                ListView1.ItemsSource = listPlanillaItems;
            else
                ListView1.ItemsSource = listPlanillaItems.Where(x => x.OrdenId.ToString().Contains(filter));
            // Desactivamos la función de refresh
            ListView1.EndRefresh();
        }
    }
}