using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface PersonalIdentifier
    {
        [Description("The value of the identifier")]
        string Value { get; }

        [Description("The type of the identifier")]
        PersonalIdentifierType Type { get; }

        [Description("The type code of the identifier directly from data provider")]
        string TypeCode { get; }

        [Description("The issuer of the identifier")]
        string IssuedBy { get; }

        [Description("The description of the identifier")]
        string Description { get; }

        [Description("The notes of the identifier from identifier")]
        string Notes { get; }

        [Description("The related dates information of the identifier")]
        IEnumerable<ReferenceDate> ReferenceDates { get; }
    }
}