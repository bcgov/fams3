using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class OtherAsset : PersonalInfo
    {
        [Description("Asset Type Description")]
        public string TypeDescription { get; set; }

        [Description("Reference Description")]
        public string ReferenceDescription { get; set; }

        [Description("Reference Value")]
        public string ReferenceValue { get; set; }

        [Description("Owners")]
        public IEnumerable<AssetOwner> Owners { get; set; }
    }
}
