using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.Services.Dynamics
{
    public class DynamicsHttpClient
    {
        public async Task<T> Get<T>(string endpoint, int timeout, string accessToken)
        {
            using var client = new HttpClient
            {
                Timeout = new TimeSpan(0, timeout, 0)
            };
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\"");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var strResponse =
                await client.GetStringAsync(endpoint);
            return await Task.FromResult(JsonConvert.DeserializeObject<T>(strResponse));
        }

        public async Task<HttpResponseMessage> Save<T>(string endpoint, int timeout, string accessToken, T data)
        {
            using var client = new HttpClient { Timeout = new TimeSpan(0, timeout, 0) };

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Version = new Version(1, 1),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");


            return await client.SendAsync(httpRequestMessage);
        }
        public async Task<HttpResponseMessage> SaveBatch(string endpoint, string accessToken, MultipartContent content)
        {
            
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Patch,
                new Uri(endpoint));

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (content != null)
            {
                request.Content = content;
            }

            return await client.SendAsync(request);
        }
    }
}
