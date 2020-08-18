using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.DataProvider
{

    public class SSG_SearchapiRequestDataProvider
    {
        [JsonProperty("ssg_adaptorname")]
        public string AdaptorName { get; set; }

        [JsonProperty("ssg_searchapirequestdataproviderid")]
        public Guid SearchApiRequestDataProvider { get; set; } 

        [JsonProperty("ssg_SearchAPIRequestId")]
        public SSG_SearchApiRequest SearchAPIRequest { get; set; }


        [JsonProperty("_ssg_searchapirequestid_value")]
        public Guid SearchAPIRequestId { get; set; }

        [JsonProperty("ssg_NumberOfDaysToRetry")]
        public int? NumberOfDaysToRetry { get; set; }

        [JsonProperty("ssg_NumberOfFailures")]
        public int? NumberOfFailures { get; set; }

        [JsonProperty("ssg_DataProviderId")]
        public Guid DataProvider { get; set; }


      
        public int NumberOfRetries;

       
        public int TimeBetweenRetries;
    }

    public class SSG_DataProvider
    {
        [JsonProperty("ssg_NumberOfRetries")]
        public int NumberOfRetries;

        [JsonProperty("ssg_TimeBetweenRetries")]
        public int TimeBetweenRetries;

        [JsonProperty("ssg_dataproviderId")]
        public Guid DataProviderId { get; set; }

        [JsonProperty("ssg_AdaptorName")]
        public string AdaptorName { get; set; }

        [JsonProperty("ssg_NumberOfDaysToRetry")]
        public int NumberOfDaysToRetry { get; set; }

        [JsonProperty("ssg_SearchSpeed")]
        public string  SearchSpeed { get; set; }


    }
}
