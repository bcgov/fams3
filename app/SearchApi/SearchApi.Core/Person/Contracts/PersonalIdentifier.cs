using System;
using SearchApi.Core.Person.Enums;

namespace SearchApi.Core.Person.Contracts
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