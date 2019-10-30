using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.Services.Dynamics
{
    public class DynamicService : IDynamicService
    {

        private readonly AppSettings _settings;
        private DateTime TokenExpiry;
        private string AccessToken;
       
        public DynamicService(AppSettings settings)
        {
            _settings = settings;
           
        }
        public async Task<JObject> Get(string entity)
        {
            RefreshToken();
            using var client = new System.Net.Http.HttpClient
            {
                Timeout = new TimeSpan(0, _settings.DynamicsAPI.Timeout, 0)
            };
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\"");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var strResponse = await client.GetStringAsync(_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL);
            return await  Task.FromResult(JObject.Parse(strResponse));

        }

        /// <summary>
        /// Get token from dynamics
        /// </summary>
        /// <returns></returns>
        public  Task<string> GetToken()
        {
            using var client = new HttpClient
            {
                Timeout = new TimeSpan(0, _settings.DynamicsAPI.Timeout, 0)
            };
            client.DefaultRequestHeaders.Add("client-request-id", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("return-client-request-id", "true");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("resource", _settings.DynamicsAPI.ResourceUrl),
                        new KeyValuePair<string, string>("client_id", _settings.DynamicsAPI.ClientId),
                        new KeyValuePair<string, string>("client_secret", _settings.DynamicsAPI.Secret),
                        new KeyValuePair<string, string>("username", _settings.DynamicsAPI.Username),
                        new KeyValuePair<string, string>("password", _settings.DynamicsAPI.Password),
                        new KeyValuePair<string, string>("scope", "openid"),
                        new KeyValuePair<string, string>("response_mode", "form_post"),
                        new KeyValuePair<string, string>("grant_type", "password")
                     };
            // This will also set the content type of the request
            var content = new FormUrlEncodedContent(pairs);
          
            var httpResponseMessage = client.PostAsync(_settings.DynamicsAPI.OAuthUrl, content).Result;
            var _responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
           
            try
            {
                Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(_responseContent);

                TokenExpiry = DateTime.Now;
                return Task.FromResult(result["access_token"]);
            }
            catch (Exception e)
            {
                throw new APIException(System.Net.HttpStatusCode.BadRequest, e.Message + " " + _responseContent, e);
            }
        }
        /// <summary>
        /// Refresh Token based on set expiry in configuration
        /// </summary>
        private async void RefreshToken()
        {
            if (string.IsNullOrEmpty(AccessToken) || TokenExpiry == DateTime.MinValue || (DateTime.Now - TokenExpiry).TotalMinutes >= _settings.DynamicsAPI.TokenTimeout)
            {
                AccessToken =  await GetToken();
            }
        }

        public async Task<HttpResponseMessage> SaveBatch(string entity, MultipartContent content)
        {
            RefreshToken();
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Patch,
                new Uri(_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL));

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            if (content != null)
            {
                request.Content = content;
            }

            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> Save(string entity, object data)
        {
            RefreshToken();
            using var client = new HttpClient {Timeout = new TimeSpan(0, _settings.DynamicsAPI.Timeout, 0)};

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, _settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL)
            {
                Version = new Version(1, 1),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");


            return await client.SendAsync(httpRequestMessage);
          
        }
    }
}
