using System.Linq;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    public partial class TabbedHistoricalListPage : TabbedPage
    {
        protected Stack<Page> TabStack { get; private set; } = new Stack<Page>();

        // Constructor con parámetros
        public TabbedHistoricalListPage(bool loadData = false)
        {
            InitializeComponents(loadData);
        }

        void InitializeComponents(bool data)
        {
            // Título del TabbedPage
            Title = "Histórico de Guías y Ordenes de Cargue";
            if (data)
            {
                Children.Add(new ListHistoricalDeliveredsPage(data));
                Children.Add(new ListHistoricalPickupsPage(data));
            }
            else
            {
                Children.Add(new ListHistoricalDeliveredsPage());
                Children.Add(new ListHistoricalPickupsPage());
            }
        }
    }
}