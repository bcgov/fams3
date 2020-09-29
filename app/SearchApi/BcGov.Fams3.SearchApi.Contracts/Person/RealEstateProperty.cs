using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class RealEstateProperty : PersonalInfo
    {
        [Description("Property ID")]
        public string PID { get; set; }

        [Description("Title Number")]
        public string TitleNumber { get; set; }

        [Description("Legal Description")]
        public string LegalDescription { get; set; }

        [Description("Number Of Owners")]
        public string NumberOfOwners { get; set; }

        [Description("ssg_landtitledistrict")]
        public string LandTitleDistrict { get; set; }

        public Address PropertyAddress { get; set; }
    }
}
