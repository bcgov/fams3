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

        [JsonProperty("ssg_allretriesflag")]
        public int? AllRetriesDone { get; set; }

        [JsonProperty("ssg_numberofdaystoretry")]
        public int? NumberOfDaysToRetry { get; set; }

        [JsonProperty("ssg_numberoffailures")]
        public int? NumberOfFailures { get; set; }
      
        public int NumberOfRetries { get; set; }

       
        public int TimeBetweenRetries { get; set; }
        public int SearchSpeed { get; set; }
    }

    public class SSG_DataProvider
    {
        [JsonProperty("ssg_NumberOfRetries")]
        public int NumberOfRetries { get; set; }

        [JsonProperty("ssg_TimeBetweenRetries")]
        public int TimeBetweenRetries { get; set; }

        [JsonProperty("ssg_dataproviderId")]
        public Guid DataProviderId { get; set; }

        [JsonProperty("ssg_AdaptorName")]
        public string AdaptorName { get; set; }

        [JsonProperty("ssg_NumberOfDaysToRetry")]
        public int NumberOfDaysToRetry { get; set; }

        [JsonProperty("ssg_SearchSpeed")]
        public int  SearchSpeed { get; set; }


    }
}
