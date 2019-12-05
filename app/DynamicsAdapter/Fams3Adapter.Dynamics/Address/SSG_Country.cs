using Newtonsoft.Json;
using System;


namespace Fams3Adapter.Dynamics.Address
{
    public class SSG_Country
    {
        [JsonProperty("ssg_countryid")]
        public Guid CountryId { get; set; }
    }
}
