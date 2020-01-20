using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface Address : BaseMetaData
    {
        [Description("The type of address")]
        AddressType Type { get; }

        [Description("The type code of the address directly from data provider")]
        string TypeCode { get; }

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

    }
}
