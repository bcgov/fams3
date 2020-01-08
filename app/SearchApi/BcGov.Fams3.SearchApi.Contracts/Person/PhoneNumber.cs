using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface PhoneNumber
    {
        [Description("Phone Number Description ")]
        string Description { get; }

        [Description("The Phone number")]
        string PhoneNumber { get; }

        [Description("The Extension number")]
        string Extension { get; }

        [Description("The phone number type")]
        string PhoneNumberType { get; }

        [Description("The effective date this phone number")]
        DateTime? EffectiveDate { get; }

        [Description("The end date this phone number")]
        DateTime? EndDate { get; }
    }
}
