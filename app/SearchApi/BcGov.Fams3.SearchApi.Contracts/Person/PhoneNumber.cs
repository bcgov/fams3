using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface PhoneNumber : BaseMetaData
    {
        [Description("The Phone number")]
        string PhoneNumber { get; }
        [Description("The type code")]
        string TypeCode { get; }

        [Description("The extension code")]
        string Extension { get; }
        [Description("The type of phone")]
        PhoneTypeCode Type { get; }
    }
}
