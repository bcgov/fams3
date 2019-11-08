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
    public class DynamicService :  IDynamicService<SSG_SearchRequest>
    {

        private readonly AppSettings _settings;
        private readonly DynamicsApiClient _httpClient;

        public DynamicService(AppSettings settings, DynamicsApiClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;

        }
        public  async Task<SSG_SearchRequest> Get(string filter, string entity)
        {
            return await _httpClient.Get<SSG_SearchRequest>($"{_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL}/{entity}?{filter}");
        }

        public async Task<HttpResponseMessage> SaveBatch(string filter, string entity, MultipartContent content)
        {
            return await _httpClient.SaveBatch(
                $"{_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL}/{entity}?{filter}",
                content);
        }

        public async Task<HttpResponseMessage> Save(string filter, string entity, SSG_SearchRequest message)
        {
            return await _httpClient.Save(
                $"{_settings.DynamicsAPI.EndPoints.Single(x => x.Entity == entity).URL}/{entity}?{filter}",
                _settings.DynamicsAPI.Timeout, message);
        }
    }
}
