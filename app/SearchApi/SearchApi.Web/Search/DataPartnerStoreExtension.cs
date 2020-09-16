using BcGov.Fams3.Redis.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SearchApi.Web.Search
{
    public static class DataPartnerStoreExtension
    {
        public static IEnumerable<DataPartner> GetDataPartnerSection(this string json)
        {
            JObject data = JObject.Parse(json);
            return data.SelectToken(Keys.DATA_PARTNER_JSON_PATH).ToObject<IEnumerable<DataPartner>>();

        }

        public static SearchRequest UpdateDataPartner(this string json, string dataPartner)
        {
            var request = JsonConvert.DeserializeObject<SearchRequest>(json);

            foreach( var partner in request.DataPartners)
            {
                request.DataPartners.ToList().Remove(partner);
                if (partner.Name.ToUpper().Equals(dataPartner.ToUpper()))
                    partner.Completed = true;

                request.DataPartners.ToList().Add(partner);
            }
            
            return request;
        }

        public static bool AllPartnerCompleted(this string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var request = JsonConvert.DeserializeObject<SearchRequest>(json);
                return !request.DataPartners.Any(x => x.Completed == false);
            }
            else return true; // we can't find request, possibly completed and already deleted from redis. requires refactor
            
        }
    }
}
