using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DynamicsAdapter.Web.DynamicService
{
   public interface IDynamicService
   {
       string GetToken();
       JObject GetEntity();
     
       HttpResponseMessage SaveEntity();
       HttpResponseMessage SaveBatch();

   }
}
