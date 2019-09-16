using SolexMobileApp.Helpers;

namespace SolexMobileApp.ViewModels
{
    public class LoginViewModel : ObjetoObservable
    {
        public string DeviceId { get; set; }
        public string Version { get; set; }

        public LoginViewModel() {}

        public void GetDeviceId()
        {
            DeviceId = App.ModeloMain.IdMaquina;
            OnPropertyChanged("DeviceId");
        }

        public void GetVersionName()
        {
            Version = App.ModeloMain.VersionName;
            OnPropertyChanged("Version");
        }
    }
}
