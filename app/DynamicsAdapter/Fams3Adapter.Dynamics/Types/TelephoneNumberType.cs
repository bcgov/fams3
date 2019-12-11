using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Types
{
    public class TelephoneNumberType : Enumeration
    {
        public static TelephoneNumberType Cell = new TelephoneNumberType(867670000, "Cell");
        public static TelephoneNumberType Work = new TelephoneNumberType(867670002, "Work");
        public static TelephoneNumberType Home = new TelephoneNumberType(867670001, "Home");
        protected TelephoneNumberType(int value, string name) : base(value, name)
        {
        }
    }
}
