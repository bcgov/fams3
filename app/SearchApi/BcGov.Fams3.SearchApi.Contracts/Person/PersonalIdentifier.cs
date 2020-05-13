using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{

    public class PersonalIdentifier : PersonalInfo
    {
        [Description("The value of the identifier")]
        public string Value { get; set; }

        [Description("The type of the identifier")]
        public PersonalIdentifierType Type { get; set; }

        [Description("The type code of the identifier directly from data provider")]
        public string TypeCode { get; set; }

        [Description("The issuer of the identifier")]
        public string IssuedBy { get; set; }

    }
}