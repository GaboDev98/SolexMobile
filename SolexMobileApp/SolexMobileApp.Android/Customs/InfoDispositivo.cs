using Android.OS;
using Xamarin.Forms;
using Android.Content.PM;
using SolexMobileApp.Interfaces;
using SolexMobileApp.Droid.Customs;

[assembly: Dependency(typeof(InfoDispositivo))]
namespace SolexMobileApp.Droid.Customs
{
    public class InfoDispositivo : IDispositivo
    {
        public string IdDispositivo
        {
            get
            {
                Android.Telephony.TelephonyManager mTelephonyMgr;
                mTelephonyMgr = (Android.Telephony.TelephonyManager)Forms.Context.GetSystemService(Android.Content.Context.TelephonyService);

                string deviceId;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    deviceId = mTelephonyMgr.GetImei(0);
                else
                    deviceId = mTelephonyMgr.DeviceId;

                return deviceId;
            }
        }

        public string VersionName
        {
            get
            {
                Android.Content.Context context = Android.App.Application.Context;

                PackageManager manager = context.PackageManager;
                PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);

                return info.VersionName;
            }
        }

        public string RutaDatos => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
    }
}