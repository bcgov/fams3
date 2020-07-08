using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Duplicate
{
    public class SSG_DuplicateDetectionConfig
    {
        [JsonProperty("ssg_entity")]
        public string EntityName { get; set; }

        [JsonProperty("ssg_fields")]
        public string DuplicateFields { get; set; }

        public string[] DuplicateFieldList { get; set; }
    }
}
