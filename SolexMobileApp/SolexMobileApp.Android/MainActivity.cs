using Android.OS;
using Android.App;
using Com.OneSignal;
using Plugin.Permissions;
using Android.Content.PM;
using Plugin.CurrentActivity;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Content;

namespace SolexMobileApp.Droid
{
    [Activity(Label = "SolexMobileApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity 
    {
        public static Context Activity { get; internal set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            // Plugin XXing para escanear códigos de barras
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            // Inicialización de las actividades
            CrossCurrentActivity.Current.Init(this, bundle);

            // Inicialización de los formularios de Xamarin Forms
            Xamarin.Forms.Forms.Init(this, bundle);

            // Inicializamos el plugin Xamarin.Forms.Maps
            Xamarin.FormsMaps.Init(this, bundle); 

            // Notificaciones push con OneSignal y Firebase
            OneSignal.Current.StartInit("54cea8e1-73e2-48f6-b99a-8b8adbc0f603").EndInit();

            // Método que carga la aplicación
            LoadApplication(new App());

            int requestPermissions = 0;
            string cameraPermission = Android.Manifest.Permission.Camera;
            string externalPermission = Android.Manifest.Permission.WriteExternalStorage;
            string readPhoneState = Android.Manifest.Permission.ReadPhoneState;

            if (!(ContextCompat.CheckSelfPermission(this, cameraPermission) == (int)Permission.Granted)
                || !(ContextCompat.CheckSelfPermission(this, externalPermission) == (int)Permission.Granted)
                || !(ContextCompat.CheckSelfPermission(this, readPhoneState) == (int)Permission.Granted))
            {
                ActivityCompat.RequestPermissions(this, new string[] { cameraPermission, externalPermission, readPhoneState }, requestPermissions);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

