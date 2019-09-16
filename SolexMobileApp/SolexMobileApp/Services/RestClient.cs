using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace SolexMobileApp.Data
{
    public class RestClient
    {
        HttpClient client;
        private string urlBase;

        public RestClient()
        {
            client = new HttpClient();
        }

        void AuthorizationRequest()
        {
            client = new HttpClient();
            var currentSettings = App.SettingsDatabase.GetSettings();
            urlBase = (currentSettings != null) ? currentSettings.UrlSolex : Constants.UrlSolexRC;
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("DeviceId", App.ModeloMain.IdMaquina);
            client.DefaultRequestHeaders.Add("VersionName", App.ModeloMain.VersionName);
            client.DefaultRequestHeaders.Add("UserId", Constants.CurrentUser.IdSolex.ToString());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.BearerToken);
        }

        public async Task<T> Login<T>()
        {
            try
            {
                AuthorizationRequest();

                string ContentType = "application/json";

                var Result = await client.PostAsync(
                    urlBase + Constants.AuthLogin,
                    new StringContent(
                        JsonConvert.SerializeObject(Constants.RequestAuth),
                        Encoding.UTF8,
                        ContentType
                    )
                );
                if (Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JsonResult = Result.Content.ReadAsStringAsync().Result;
                    var ContentResp = JsonConvert.DeserializeObject<T>(JsonResult);
                    return ContentResp;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(T);
        }

        public async Task<List<ResponseGuia>> GetGuiasByUser(string usuario_id, string controladas)
        {
            AuthorizationRequest();

            List<ResponseGuia> responsePlanilla = new List<ResponseGuia>();

            try
            {
                var uri = new Uri(string.Format(urlBase + Constants.GetPlanillaByUser + usuario_id + "/" + controladas, string.Empty));

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    responsePlanilla = JsonConvert.DeserializeObject<List<ResponseGuia>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return responsePlanilla;
        }

        public async Task<List<ResponseOrdenCargue>> GetOrdenesCargueByUser(string usuario_id)
        {
            AuthorizationRequest();

            List<ResponseOrdenCargue> responseOCS = new List<ResponseOrdenCargue>();
            try
            {
                var uri = new Uri(string.Format(urlBase + Constants.GetOrdenesCargueByUser + usuario_id, string.Empty));

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    responseOCS = JsonConvert.DeserializeObject<List<ResponseOrdenCargue>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return responseOCS;
        }

        public async Task<List<ResponseGuiaEstados>> GetEstadosGuiaMobile()
        {
            List<ResponseGuiaEstados> responseEstadosGuia = new List<ResponseGuiaEstados>();
            try
            {
                var uri = new Uri(string.Format(urlBase  + Constants.GetEstadosGuiaMobile, string.Empty));

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    responseEstadosGuia = JsonConvert.DeserializeObject<List<ResponseGuiaEstados>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return responseEstadosGuia;
        }

        public async Task<T> SaveEstadoGuia<T>(RequestEstadoGuia estado_guia)
        {
            try
            {
                AuthorizationRequest();

                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    urlBase + Constants.SaveEstadoGuia,
                    new StringContent(
                        JsonConvert.SerializeObject(estado_guia),
                        Encoding.UTF8,
                        ContentType
                    )
                );

                if (Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JsonResult = Result.Content.ReadAsStringAsync().Result;
                    var ContentResp = JsonConvert.DeserializeObject<T>(JsonResult);
                    return ContentResp;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(T);
        }

        public async Task<T> SaveEstadoOrden<T>(RequestEstadoOrden estado_orden)
        {
            try
            {
                AuthorizationRequest();

                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    urlBase + Constants.SaveEstadoOrden,
                    new StringContent(
                        JsonConvert.SerializeObject(estado_orden),
                        Encoding.UTF8,
                        ContentType
                    )
                );

                if (Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JsonResult = Result.Content.ReadAsStringAsync().Result;
                    var ContentResp = JsonConvert.DeserializeObject<T>(JsonResult);
                    return ContentResp;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(T);
        }

        public async Task<T> SaveDetailGuia<T>(RequestDatosGuia request_guia)
        {
            try
            {
                AuthorizationRequest();

                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    urlBase + Constants.SaveDetalleGuia,
                    new StringContent(
                        JsonConvert.SerializeObject(request_guia),
                        Encoding.UTF8,
                        ContentType
                    )
                );

                if (Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JsonResult = Result.Content.ReadAsStringAsync().Result;
                    var ContentResp = JsonConvert.DeserializeObject<T>(JsonResult);
                    return ContentResp;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(T);
        }

        public async Task<T> SaveDetailOrdenCargue<T>(RequestDatosOrden request_orden)
        {
            try
            {
                AuthorizationRequest();

                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    urlBase + Constants.SaveDetalleOrdenCargue,
                    new StringContent(
                        JsonConvert.SerializeObject(request_orden),
                        Encoding.UTF8,
                        ContentType
                    )
                );

                if (Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JsonResult = Result.Content.ReadAsStringAsync().Result;
                    var ContentResp = JsonConvert.DeserializeObject<T>(JsonResult);
                    return ContentResp;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(T);
        }

        public async Task<ResponseGuiaExist> GetExistsBarCode(string guia_numero)
        {
            AuthorizationRequest();

            ResponseGuiaExist responseGuiaExist = new ResponseGuiaExist();

            try
            {
                var uri = new Uri(string.Format(urlBase + Constants.GetExistsBarCode + guia_numero, string.Empty));

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    responseGuiaExist = JsonConvert.DeserializeObject<ResponseGuiaExist>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return responseGuiaExist;
        }

        public async Task<ResponseSave> SaveMedidasCon(ImageCubic cubic_image)
        {
            AuthorizationRequest();

            ResponseSave responseSave = new ResponseSave();

            try
            {
                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    urlBase + Constants.EnviarMedidasCon,
                    new StringContent(
                        JsonConvert.SerializeObject(cubic_image),
                        Encoding.UTF8,
                        ContentType
                    )
                );

                if (Result.IsSuccessStatusCode)
                {
                    var content = await Result.Content.ReadAsStringAsync();
                    responseSave = JsonConvert.DeserializeObject<ResponseSave>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return responseSave;
        }

        public async Task<ResponseSave> SaveMassiveUnidades(MassiveCubic massive_cubic)
        {
            AuthorizationRequest();

            ResponseSave responseSave = new ResponseSave();

            try
            {
                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    urlBase + Constants.EnviarMedidasMasivasCon,
                    new StringContent(
                        JsonConvert.SerializeObject(massive_cubic),
                        Encoding.UTF8,
                        ContentType
                    )
                );

                if (Result.IsSuccessStatusCode)
                {
                    var content = await Result.Content.ReadAsStringAsync();
                    responseSave = JsonConvert.DeserializeObject<ResponseSave>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return responseSave;
        }
    }
}
