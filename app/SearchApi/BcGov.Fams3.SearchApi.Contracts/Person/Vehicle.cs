using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Vehicle : PersonalInfo
    {
        [Description("OwnershipType")]
        public string OwnershipType { get; set; }

        [Description("Vin")]
        public string Vin { get; set; }

        [Description("PlateNumber")]
        public string PlateNumber { get; set; }

        [Description("Owners")]
        public IEnumerable<InvolvedParty> Owners { get; set; }
    }

}
