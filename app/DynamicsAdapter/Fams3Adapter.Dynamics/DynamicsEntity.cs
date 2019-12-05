using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics
{
    public abstract class DynamicsEntity
    {
        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }
    }
}
