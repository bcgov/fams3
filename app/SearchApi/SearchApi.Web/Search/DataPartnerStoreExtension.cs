using BcGov.Fams3.Redis.Model;
using BcGov.Fams3.SearchApi.Contracts.Person;
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

        public static SearchRequest UpdateDataPartner(this SearchRequest request, string dataPartner)
        {
            var partner = request.DataPartners?.FirstOrDefault(x => x.Name == dataPartner);
            if(partner != null && partner.SearchSpeed!=SearchSpeedType.Slow)
            {
                partner.Completed = true;
            }
            return request;
        }
        public static SearchRequest ResetDataPartner(this SearchRequest request, string dataPartner)
        {
            var partner = request.DataPartners.ToList().FirstOrDefault(x => x.Name == dataPartner);
            if(partner != null)
                partner.Completed = false;
            return request;
        }
        public static bool AllPartnerCompleted(this SearchRequest request)
        {

            return !request.DataPartners.Any(x => x.Completed == false);
        }

        public static bool AllFastSearchPartnerCompleted(this SearchRequest request)
        {
            return !request.DataPartners.Any(x => x.Completed == false && x.SearchSpeed == SearchSpeedType.Fast);

        }

        public static string DeepSearchKey(this string searchRequestKey, string datapartner)
        {
            return string.Format(Keys.DEEP_SEARCH_REDIS_KEY_FORMAT, searchRequestKey, datapartner);

        }
    }

    //public static class DataPartnerStoreExtension
    //{
    //    public static IEnumerable<DataPartner> GetDataPartnerSection(this string json)
    //    {
    //        JObject data = JObject.Parse(json);
    //        return data.SelectToken(Keys.DATA_PARTNER_JSON_PATH).ToObject<IEnumerable<DataPartner>>();

    //    }

    //    public static SearchRequest UpdateDataPartner(this string json, string dataPartner)
    //    {
    //        var request = JsonConvert.DeserializeObject<SearchRequest>(json);
    //        var partner = request.DataPartners.ToList().FirstOrDefault(x => x.Name == dataPartner);
    //        if (partner == null) return request;
    //        request.DataPartners.ToList().Remove(partner);
    //        partner.Completed = true;
    //        request.DataPartners.ToList().Add(partner);
    //        return request;
    //    }
    //    public static SearchRequest ResetDataPartner(this string json, string dataPartner)
    //    {
    //        var request = JsonConvert.DeserializeObject<SearchRequest>(json);
    //        var partner = request.DataPartners.ToList().FirstOrDefault(x => x.Name == dataPartner);

    //        if (partner == null) return request;
    //        request.DataPartners.ToList().Remove(partner);
    //        partner.Completed = false;
    //        request.DataPartners.ToList().Add(partner);
    //        return request;
    //    }
    //    public static bool AllPartnerCompleted(this string json)
    //    {
    //        if (!string.IsNullOrEmpty(json))
    //        {
    //            var request = JsonConvert.DeserializeObject<SearchRequest>(json);
    //            return !request.DataPartners.Any(x => x.Completed == false);
    //        }
    //        else return true; // we can't find request, possibly completed and already deleted from redis. requires refactor
            
    //    }

    //    public static bool AllFastSearchPartnerCompleted(this string json)
    //    {
    //        if (!string.IsNullOrEmpty(json))
    //        {
    //            var request = JsonConvert.DeserializeObject<SearchRequest>(json);
    //            return !request.DataPartners.Any(x => x.Completed == false && x.SearchSpeed == SearchSpeedType.Fast);
    //        }
    //        else return true; // we can't find request, possibly completed and already deleted from redis. requires refactor

    //    }

    //    public static string DeepSearchKey(this string searchRequestKey, string datapartner)
    //    {
    //         return string.Format(Keys.DEEP_SEARCH_REDIS_KEY_FORMAT, searchRequestKey, datapartner);

    //    }
    //}
}
