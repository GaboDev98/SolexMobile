using System.Linq;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SolexMobileApp.Views.Modules
{
    public partial class TabbedListPage : TabbedPage
    {
        protected Stack<Page> TabStack { get; private set; } = new Stack<Page>();

        // Constructor con parámetros
        public TabbedListPage(bool loadData = false)
        {
            InitializeComponents(loadData);
        }

        void InitializeComponents(bool data)
        {
            // Título del TabbedPage
            Title = "Guías y Ordenes de Cargue";
            if (data)
            {
                Children.Add(new ListDeliveredsPage(data));
                Children.Add(new ListPickupsPage(data));
            }
            else
            {
                Children.Add(new ListDeliveredsPage());
                Children.Add(new ListPickupsPage());
            }
        }
    }
}