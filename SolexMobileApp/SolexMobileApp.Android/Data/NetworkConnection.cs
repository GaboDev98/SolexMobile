using Android.App;
using Android.Net;
using Android.Content;
using SolexMobileApp.Data;
using SolexMobileApp.Droid.Data;

[assembly: Xamarin.Forms.Dependency(typeof(NetworkConnection))]

namespace SolexMobileApp.Droid.Data
{
    public class NetworkConnection : INetworkConnection
    {
        public bool IsConnected { get; set; }

        public void CheckNetworkConnection()
        {
            ConnectivityManager ConnectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            NetworkInfo ActiveNetworkInfo = ConnectivityManager.ActiveNetworkInfo;
            IsConnected = ActiveNetworkInfo != null && ActiveNetworkInfo.IsConnectedOrConnecting;
        }
    }
}