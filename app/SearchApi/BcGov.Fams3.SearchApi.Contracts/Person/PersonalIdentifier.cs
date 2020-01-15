using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface PersonalIdentifier
    {
        [Description("The value of the identifier")]
        string Value { get; }

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