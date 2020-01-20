using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface RelatedPerson : Person , BaseMetaData
    {
        [Description("Relationship to the person found")]
        string Relationship { get; }
    }
}
