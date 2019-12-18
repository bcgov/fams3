using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Core.Person.Contracts
{
    public interface PhoneNumber
    {
        [Description("Supply ")]
        string SuppliedBy { get; }
        [Description("A Date")]
        DateTime? Date { get; }
        [Description("The Date type of the supplied Date")]
        string DateType { get; }
        [Description("The Phone number")]
        string PhoneNumber { get; }
        [Description("The phone number type")]
        string PhoneNumberType { get; }
    }
}
