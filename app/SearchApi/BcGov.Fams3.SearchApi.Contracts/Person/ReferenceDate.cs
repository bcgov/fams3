using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class ReferenceDate
    {
        [Description("The index of this date, for dynamics mapping")]
        public int Index { get; set; }

        [Description("The key of this date, mainly the description of the date")]
        public string Key { get; set; }

        [Description("The date value")]
        public DateTime Value { get; set; }

    }
}
