using System;
using System.ComponentModel;
using BcGov.Fams3.SearchApi.Core.Person.Enums;

namespace BcGov.Fams3.SearchApi.Core.Person.Contracts
{
    public interface PersonalIdentifier
    {
        [Description("The serial number of the identifier")]
        string SerialNumber { get; }
        [Description("The effective date of the identifier")]
        DateTime? EffectiveDate { get; }
        [Description("The expiration date of the identifier")]
        DateTime? ExpirationDate { get; }
        [Description("The type of the identifier")]
        PersonalIdentifierType Type { get; }
        [Description("The issuer of the identifier")]
        string IssuedBy { get; }
    }
}