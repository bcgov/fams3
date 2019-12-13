using System;
using BcGov.Fams3.SearchApi.Core.Person.Enums;

namespace BcGov.Fams3.SearchApi.Core.Person.Contracts
{
    public interface PersonalIdentifier
    {
        string SerialNumber { get; }
        DateTime? EffectiveDate { get; }
        DateTime? ExpirationDate { get; }
        PersonalIdentifierType Type { get; }
        string IssuedBy { get; }
    }
}