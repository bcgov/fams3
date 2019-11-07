using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.Services.Dynamics
{
   public interface IDynamicService<T>
   {
       Task<T> Get( string filter, string entity);
       Task<HttpResponseMessage> Save(string filter, string entity, T message);
       Task<HttpResponseMessage> SaveBatch(string filter, string entity, MultipartContent content);

   }

   
}
