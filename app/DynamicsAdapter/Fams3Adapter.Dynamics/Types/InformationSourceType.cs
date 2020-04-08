using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Linq;

namespace Fams3Adapter.Dynamics.Types

{
    
    public class InformationSourceType : Enumeration
    {

        public static InformationSourceType Request = new InformationSourceType(867670000, "Request");
        public static InformationSourceType ICBC = new InformationSourceType(867670001, "ICBC");
        public static InformationSourceType Other = new InformationSourceType(867670003, "Other");
        public static InformationSourceType BCHydro = new InformationSourceType(867670005, "BCHydro");

        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
   
}
