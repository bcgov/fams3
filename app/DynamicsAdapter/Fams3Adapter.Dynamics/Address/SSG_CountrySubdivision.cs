using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Address
{
    public class SSG_CountrySubdivision : DynamicsEntity
    {
        [JsonProperty("ssg_countrysubdivisionid")]
        public Guid CountrySubdivisionId { get; set; }

        //[JsonProperty("ssg_countryid")]
        //public Guid CountryId { get; set; }

        [JsonProperty("ssg_name")]
        public String Name { get; set; }
    }
}
