using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface Name
    {
        [Description("The first name")]
        string FirstName { get; }

        [Description("The last name")]
        string LastName { get; }

        [Description("The middle name")]
        string MiddleName { get; }

        [Description("the type of the names")]
        string Type { get; }

        [Description("the name effective date")]
        DateTime? EffectiveDate { get; }

        [Description("The name end date")]
        DateTime? EndDate { get; }

        [Description("The name description")]
        string Description { get; }
    }
}
