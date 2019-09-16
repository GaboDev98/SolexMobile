using Xamarin.Forms;

namespace SolexMobileApp.Models
{
    public class Constants
    {
        public static bool IsDev = true;

        public static string AppName = "Solex Mobile";

        public static Color BackgroundColor = Color.White; // Color.FromRgb(58, 153, 215)

        public static Color MainTextColor = Color.Black;

        public static int LoginIconHeight = 110;

        public static string IdDevice { get; set;  }

        public static string VersionName { get; set; }

        // ---------- Login -------------------

        public static RequestAuth RequestAuth { get; set; }

        public static string BearerToken { get; set; }

        public static User CurrentUser { get; set; }

        public static Settings CurrentSettings { get; set; }

        public static string UrlSolexProd = "http://solex.blulogistics.net/Solex/Services/SolexMobileApi.svc";

        public static string UrlSolexRC = "http://solex.blulogistics.net/SolexRC/Services/SolexMobileApi.svc";

        public static string UrlSolexTest = "http://testsolex.blulogistics.net/Solex/Services/SolexMobileApi.svc";

        public static string UrlSolex2 = "http://testsolex.blulogistics.net/Solex2/Services/SolexMobileApi.svc";

        public static string UrlSolexPre = "http://testsolex.blulogistics.net/SolexPre/Services/SolexMobileApi.svc";

        public static string UrlSolex2VPN = "http://10.5.0.92/Solex2/Services/SolexMobileApi.svc";

        public static string Auth = "/Auth";

        public static string AuthLogin = "/AuthLogin";

        public static string GetPlanillaByUser = "/GetPlanillasByUser/";

        public static string GetOrdenesCargueByUser = "/GetOrdenesCargueByUser/";

        public static string GetEstadosGuiaMobile = "/GetEstadosGuiaMobile";

        public static string SaveEstadoGuia = "/SaveEstadoGuia";

        public static string SaveEstadoOrden = "/SaveEstadoOrden";

        public static string SaveDetalleGuia = "/SaveDetalleGuia";

        public static string SaveDetalleOrdenCargue = "/SaveDetalleOrdenCargue";

        public static string GetExistsBarCode = "/GetExistsBarCode/";

        public static string EnviarMedidasCon = "/SaveMedidasCon";

        public static string EnviarMedidasMasivasCon = "/SaveMassiveMedidasCon";

        public static string UploadImageCubic = "/UploadImageCubic";

        public static string NoInternetText = "El dispositivo no cuenta con conexión a internet en estos momentos!";

        public static bool hasInternet = false;

        // ---------- Constantes de la lógica de negocio de Solex -------------------

        public static decimal ESTADO_GUIA_LLEGO_PUNTO = -5;

        public static decimal ESTADO_GUIA_ENTREGA_BLU = 1;

        public static decimal ESTADO_GUIA_ENTREGA_PARCIAL = 20;

        public static decimal ESTADO_GUIA_NO_ENTREGADA = 1078;

        public static decimal ESTADO_ORDEN_LLEGO_PUNTO = 3;

        public static decimal ESTADO_ORDEN_EXITOSA = 101;

        public static decimal ESTADO_ORDEN_NO_EXITOSA = 102;

        // ---------- Constantes para los códigos de colores -------------------

        public static string CODIGO_COLOR_LETRA_NORMAL = "#FFFFFF";

        public static string CODIGO_COLOR_BOTON_AZUL= "#00549F";

        public static string CODIGO_COLOR_BOTON_AZUL_PRESSED = "#043763";

        public static string CODIGO_COLOR_ENTREGA_PARCIAL = "#FF9200";

        public static string CODIGO_COLOR_LLEGADA_PUNTO = "#FFFC00";

        public static string CODIGO_COLOR_ENTREGA_BLU = "#009F1C";

        public static string CODIGO_COLOR_NO_ENTREGADA = "#FF5252";

        public static string CODIGO_COLOR_ORDEN_EXITOSA = "#009F1C";

        public static string CODIGO_COLOR_ORDEN_NO_EXITOSA = "#FF5252";
    }
}
