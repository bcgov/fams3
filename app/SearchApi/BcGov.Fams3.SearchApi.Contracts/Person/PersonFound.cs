using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class PersonFound : Person
    {
        public PersonalIdentifier SourcePersonalIdentifier { get; set; }
    }
}
