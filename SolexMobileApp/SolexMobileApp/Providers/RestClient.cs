using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using SolexMobileApp.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace SolexMobileApp.Data
{
    public class RestClient
    {
        HttpClient client;

        public RestClient()
        {
            client = new HttpClient();
        }

        void AuthorizationRequest()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.bearerToken);
        }

        public async Task<T> Login<T>()
        {
            try
            {
                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    Constants.LoginUrlTestAuth,
                    new StringContent(
                        JsonConvert.SerializeObject(Constants.requestAuth), 
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

        public async Task<List<ResponseGuia>> GetGuiasByPlaca(string placa)
        {
            AuthorizationRequest();
            List<ResponseGuia> responsePlanilla = new List<ResponseGuia>();
            try
            {
                var uri = new Uri(string.Format(Constants.GetPlanillaUrlTest + placa, string.Empty));

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

        public async Task<List<ResponseOrdenCargue>> GetOrdenesCargueByPlaca(string placa)
        {
            AuthorizationRequest();
            List<ResponseOrdenCargue> responseOCS = new List<ResponseOrdenCargue>();
            try
            {
                var uri = new Uri(string.Format(Constants.GetOrdenesCargueByPlaca + placa, string.Empty));

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

        public async Task<List<ResponseGuiaEstados>> GetEstadosGuia()
        {
            AuthorizationRequest();
            List<ResponseGuiaEstados> responseEstadosGuia = new List<ResponseGuiaEstados>();
            try
            {
                var uri = new Uri(string.Format(Constants.GetEstadosGuiaMobile, string.Empty));

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

        public async Task<T> SaveDetailGuia<T>(RequestGuia request_guia)
        {
            try
            {
                AuthorizationRequest();
                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    Constants.SaveDetalleGuia,
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

        public async Task<T> SaveDetailOrdenCargue<T>()
        {
            try
            {
                AuthorizationRequest();
                RequestOrdenCargue orden_cargue = new RequestOrdenCargue();
                orden_cargue.OrdenCargueNumero = "5646845";
                orden_cargue.UnidadesRecogidas = 1;
                orden_cargue.Observaciones = "Ninguna.";
                string ContentType = "application/json";
                var Result = await client.PostAsync(
                    Constants.SaveDetalleOrdenCargue,
                    new StringContent(
                        JsonConvert.SerializeObject(orden_cargue),
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
    }
}
