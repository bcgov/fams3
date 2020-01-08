using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface Address
    {
        [Description("The type of address")]
        string Type { get; }
        [Description("The Address Line 1")]
        string AddressLine1 { get; }
        [Description("The Address Line 2")]
        string AddressLine2 { get; }
        [Description("The Address Line 2")]
        string AddressLine3 { get; }
        [Description("The Address Province or state")]
        string StateProvince { get; }
        [Description("The Address City")]
        string City { get; }
        [Description("The Address Country")]
        string CountryRegion { get; }
        [Description("The Address Zip or Postal Code")]
        string ZipPostalCode { get; }
        [Description("The Address Description")]
        string Description { get; }
        [Description("The Address Effective Date")]
        DateTime? EffectiveDate { get; }
        [Description("The Address End Date")]
        DateTime? EndDate { get; }
    }
}
