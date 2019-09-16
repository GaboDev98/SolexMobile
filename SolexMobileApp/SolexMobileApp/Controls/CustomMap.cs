using Xamarin.Forms.Maps;
using System.Collections.Generic;

namespace SolexMobileApp.Controls
{
    public class CustomMap : Map
    {
        public List<Position> RouteCoordinates { get; set; }

        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
        }
    }
}
