using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class PersonalIdentifier
    {
        [Description("The value of the identifier")]
        public string Value { get; }

        [Description("The type of the identifier")]
        public PersonalIdentifierType Type { get; }

        [Description("The type code of the identifier directly from data provider")]
        public string TypeCode { get; }

        [Description("The issuer of the identifier")]
        public string IssuedBy { get; }

        [Description("The description of the identifier")]
        public string Description { get; }

        [Description("The description of the identifier")]
        public string Notes { get; }

        [Description("The related dates information of the identifier")]
        public IEnumerable<ReferenceDate> ReferenceDates { get; }
    }
}