using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface Name :BaseMetaData
    {
        [Description("The first name")]
        string FirstName { get; }

        [Description("The last name")]
        string LastName { get; }

        [Description("The middle name")]
        string SecondName { get; }

        [Description("The middle name")]
        string ThirdName { get; }

        [Description("the type of the names")]
        NameType Type { get; }

       

        string TypeCode { get;  }
    }
}
