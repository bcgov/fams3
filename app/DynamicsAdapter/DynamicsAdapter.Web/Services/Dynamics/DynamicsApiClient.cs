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

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6InBSTW1LWUVMQVpsYnVRRllnNGNacmppTW02NCJ9.eyJhdWQiOiJodHRwczovL2pzYi1mYW1zLmRldi5qYWcuZ292LmJjLmNhL2FwaS9kYXRhL3Y5LjAvIiwiaXNzIjoiaHR0cDovL3N0czQuZ292LmJjLmNhL2FkZnMvc2VydmljZXMvdHJ1c3QiLCJpYXQiOjE1NzMxNTUyNjgsImV4cCI6MTU3MzE1ODg2OCwidXBuIjoiYWxleC5qb3lldXhAZ292LmJjLmNhIiwidW5pcXVlX25hbWUiOiJJRElSXFxham95ZXV4IiwiYXBwdHlwZSI6IkNvbmZpZGVudGlhbCIsImFwcGlkIjoiM2ExNGQyNzYtNWUzMS00MWI0LWJjOTEtMzJiZTMyODA2OTRmIiwiYXV0aG1ldGhvZCI6InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDphYzpjbGFzc2VzOlBhc3N3b3JkUHJvdGVjdGVkVHJhbnNwb3J0IiwiYXV0aF90aW1lIjoiMjAxOS0xMS0wN1QxOTozNDoyOC4yODZaIiwidmVyIjoiMS4wIiwic2NwIjoib3BlbmlkIn0.I8szg0M0YBn_4UzmuRhFKnAqVAMR7hDPP32Ukrqi9_F5i-q4Wor3_As-frVzPPjHw8QWPYTcFhlz5gfVPiuoLn8byqWofntWtVxV6L2SiTnulwZEes9CndvHcrP3emRV8zPOIw_rm80axbh1RPrfiRmQtdj9Q4zQ91ph3yCMvokDX_ul3bP-CivioTlVvJe49EyT1YCCW3_7Ykj2hZpqPHseBBoloWIFLeroet0grXsIBe41vzGlHPHVKmyQuEISJ-Q-w5ih5_bmYVz4uFU96PJzv0_qD38xFrF_C9p_wBKV-QvnEHvgurKG10XQ6n6_Ua9vBne86-oGRXp9EzsjAw");

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
