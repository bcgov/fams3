using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.Services.Dynamics
{
    public interface IDynamicsApiClient
    {
        Task<T> Get<T>(string endpoint);
        Task<HttpResponseMessage> Save<T>(string endpoint, int timeout, T data);
        Task<HttpResponseMessage> SaveBatch(string endpoint, MultipartContent content);
    }

    public class DynamicsApiClient : IDynamicsApiClient
    {
        private readonly HttpClient _httpClient;
        public DynamicsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> Get<T>(string endpoint)
        {


            _httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            _httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\"");

            using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
            {
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    CancellationToken.None);

                if (!response.IsSuccessStatusCode)
                {
                    var responseData = response.Content == null
                        ? null
                        : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new DynamicsApiException(
                        "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
                        (int)response.StatusCode, responseData,
                        response.Headers.ToDictionary(x => x.Key, x => x.Value), null);
                }


                var stream = await response.Content.ReadAsStreamAsync();

                using (StreamReader sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                }
            }
        }

        public async Task<HttpResponseMessage> Save<T>(string endpoint, int timeout, T data)
        {

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Version = new Version(1, 1),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            _httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");


            return await _httpClient.SendAsync(httpRequestMessage);
        }
        public async Task<HttpResponseMessage> SaveBatch(string endpoint, MultipartContent content)
        {
            
            var request = new HttpRequestMessage(HttpMethod.Patch,
                new Uri(endpoint));

            if (content != null)
            {
                request.Content = content;
            }

            return await _httpClient.SendAsync(request);
        }
    }
}
