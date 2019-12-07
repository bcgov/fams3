using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics
{
    public abstract class DynamicsEntity
    {
        [JsonProperty(Keys.DYNAMICS_STATUS_CODE_FIELD)]
        public int StateCode { get; set; }

        [JsonProperty(Keys.DYNAMICS_STATUS_CODE_FIELD)]
        public int StatusCode { get; set; }
    }
}
