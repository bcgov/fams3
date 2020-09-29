using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Vehicle
{
    public class VehicleEntity : DynamicsEntity
    {
        [JsonProperty("ssg_ownershiptype")]
        public string OwnershipType { get; set; }

        [JsonProperty("ssg_vin")]
        public string Vin { get; set; }

        [JsonProperty("ssg_licenseplate")]
        public string PlateNumber { get; set; }

        [JsonProperty("ssg_name")]
        public string Discription { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_year")]
        public string Year { get; set; }

        [JsonProperty("ssg_make")]
        public string Make { get; set; }

        [JsonProperty("ssg_model")]
        public string Model { get; set; }

        [JsonProperty("ssg_vehiclecolour")]
        public string Color { get; set; }

        [JsonProperty("ssg_vehicletype")]
        public string Type { get; set; }

        [JsonProperty("ssg_leasingcompany")]
        public string LeasingCom { get; set; }

        [JsonProperty("ssg_lessee")]
        public string Lessee { get; set; }

        [JsonProperty("ssg_leasingcompanyaddress")]
        public string LeasingComAddr { get; set; }
    }

    public class SSG_Asset_Vehicle : VehicleEntity
    {
        [JsonProperty("ssg_asset_vehicleid")]
        public Guid VehicleId { get; set; }

        [JsonProperty("ssg_ssg_asset_vehicle_ssg_assetowner_Vehicle")]
        public SSG_AssetOwner[] SSG_AssetOwners { get; set; }

        public bool IsDuplicated { get; set; }
    }
}
