using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Services.Dynamics.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.Services.Dynamics
{
    public class DynamicService :  IDynamicService<SSG_SearchRequests>
    {

        private readonly AppSettings _settings;
        private readonly DynamicsHttpClient _httpClient;
        private DateTime TokenExpiry;
        private string AccessToken;
       
        public DynamicService(AppSettings settings, DynamicsHttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;

        }
        public  async Task<SSG_SearchRequests> Get(string filter, string entity)
        {
            RefreshToken();
            return await _httpClient.Get<SSG_SearchRequests>($"{_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL}/{entity}?{filter}",
                _settings.DynamicsAPI.Timeout, AccessToken);
        }

        /// <summary>
        /// Get token from dynamics
        /// </summary>
        /// <returns></returns>
        public async  Task<string> GetToken()
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
          
            var httpResponseMessage =  await client.PostAsync(_settings.DynamicsAPI.OAuthUrl, content);
            var _responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
           
            try
            {
                Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(_responseContent);

                TokenExpiry = DateTime.Now;
                return await Task.FromResult(result["access_token"]);
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

        public async Task<HttpResponseMessage> SaveBatch(string filter, string entity, MultipartContent content)
        {
            RefreshToken();
            return await _httpClient.SaveBatch(
                $"{_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL}/{entity}?{filter}", AccessToken,
                content);
        }

        public async Task<HttpResponseMessage> Save(string filter, string entity, SSG_SearchRequests message)
        {
            RefreshToken();
            return await _httpClient.Save(
                $"{_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL}/{entity}?{filter}",
                _settings.DynamicsAPI.Timeout,
                AccessToken, message);
        }
    }
}
