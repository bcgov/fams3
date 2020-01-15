using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public interface ReferenceDate
    {
        [Description("The index of this date, for dynamics mapping")]
        int Index { get; }

        [Description("The key of this date, mainly the description of the date")]
        string Key { get; }

        [Description("The date value")]
        DateTime Value { get; }
    }
}
