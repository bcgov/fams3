using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.Services.Dynamics
{
   public interface IDynamicService
   {
       Task<string> GetToken();
       Task<JObject> GetEntity();

       Task<HttpResponseMessage> SaveEntity();
       Task<HttpResponseMessage> SaveBatch();

   }

   
}
