using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
   public  interface BaseMetaData
    {
        [Description("The related dates information of the address")]
        IEnumerable<ReferenceDate> ReferenceDates { get; }

        [Description("The description for address")]
        string Description { get; }
    }
}
