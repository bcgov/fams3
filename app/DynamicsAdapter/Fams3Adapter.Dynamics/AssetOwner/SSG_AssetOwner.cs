using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Vehicle;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.AssetOwner
{
    public class AssetOwnerEntity
    {
        [JsonProperty("ssg_type")]
        public string Type { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_organizationname")]
        public string OrganizationName { get; set; }

        [JsonProperty("ssg_lastname")]
        public string LastName { get; set; }

        [JsonProperty("ssg_firstname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_thirdgivenname")]
        public string OtherName { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_Vehicle")]
        public SSG_Asset_Vehicle Vehicle { get; set; }

        [JsonProperty("ssg_OtherAsset")]
        public SSG_Asset_Other OtherAsset { get; set; }
    }

    public class SSG_AssetOwner : AssetOwnerEntity
    {
        [JsonProperty("ssg_assetownerid")]
        public Guid AssetOwnerId { get; set; }
    }
}
