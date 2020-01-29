using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Types
{
    public class NullableBooleanType : Enumeration
    {
        public static NullableBooleanType Yes = new NullableBooleanType(867670000, "Yes");
        public static NullableBooleanType No = new NullableBooleanType(867670001, "No");

        protected NullableBooleanType(int value, string name) : base(value, name)
        {
        }
    }
}
