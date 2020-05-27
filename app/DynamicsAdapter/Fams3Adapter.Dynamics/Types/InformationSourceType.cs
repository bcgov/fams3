using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fams3Adapter.Dynamics.Types

{
    
    public class InformationSourceType : Enumeration
    {

        public readonly InformationSourceType Request = new InformationSourceType(867670000, "Request");
        public readonly InformationSourceType ICBC = new InformationSourceType(867670001, "ICBC");
        public readonly InformationSourceType Other = new InformationSourceType(867670003, "Other");
        public readonly InformationSourceType BCHydro = new InformationSourceType(867670005, "BCHydro");
        public readonly InformationSourceType MSDPR = new InformationSourceType(867670023, "MSDPR");

        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
   
  }
