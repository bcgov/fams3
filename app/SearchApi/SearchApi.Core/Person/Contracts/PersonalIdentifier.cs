using System;
using SearchApi.Core.Adapters.Models.Enums;

namespace SearchApi.Core.Adapters.Models.Contracts
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