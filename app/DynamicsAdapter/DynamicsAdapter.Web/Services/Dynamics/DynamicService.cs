using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public Task<JObject> GetEntity()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetToken()
        {
            using var client = new HttpClient
            {
                Timeout = new TimeSpan(0, int.Parse(_settings.DynamicsAPI.Timeout), 0)
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
        private async void RefreshToken()
        {
            if (String.IsNullOrEmpty(AccessToken) || TokenExpiry == DateTime.MinValue || (DateTime.Now - TokenExpiry).TotalMinutes >= double.Parse(_settings.DynamicsAPI.TokenTimeout))
            {
                AccessToken =  await GetToken();
            }
        }
        public Task<HttpResponseMessage> SaveBatch()
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> SaveEntity()
        {
            throw new NotImplementedException();
        }
    }
}
