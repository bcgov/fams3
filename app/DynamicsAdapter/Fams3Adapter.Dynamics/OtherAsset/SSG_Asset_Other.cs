using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.OtherAsset
{
    public class AssetOtherEntity : DynamicsEntity
    {
        [JsonProperty("ssg_otherassettype")]
        public string TypeDescription { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_assetdescription")]
        public string AssetDescription { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_Person")]
        public virtual SSG_Person Person { get; set; }
    }

    public class SSG_Asset_Other : AssetOtherEntity
    {
        [JsonProperty("ssg_asset_otherid")]
        public Guid AssetOtherId { get; set; }
    }
}
