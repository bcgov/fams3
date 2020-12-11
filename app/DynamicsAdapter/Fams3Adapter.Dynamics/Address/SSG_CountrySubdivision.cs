using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Address
{
    public class SSG_CountrySubdivision : DynamicsEntity
    {
        [JsonProperty("ssg_countrysubdivisionid")]
        public Guid CountrySubdivisionId { get; set; }

        [JsonProperty("ssg_name")]
        public String Name { get; set; }

        [JsonProperty("ssg_provincecodewithoutcountry")]
        public String ProvinceCode { get; set; }

    }
}
