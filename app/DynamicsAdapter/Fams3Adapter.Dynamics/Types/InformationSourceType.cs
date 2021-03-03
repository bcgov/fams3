using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fams3Adapter.Dynamics.Types

{
    
    public class InformationSourceType : Enumeration
    {

        public static InformationSourceType Request = new InformationSourceType(867670000, "Request");
        public static InformationSourceType ICBC = new InformationSourceType(867670001, "ICBC");
        public static InformationSourceType BCHydro = new InformationSourceType(867670005, "BCHydro");
        public static InformationSourceType MSDPR = new InformationSourceType(867670023, "MSDPR");
        public static InformationSourceType WorkSafeBC = new InformationSourceType(867670032, "WORKSAFEBC");
        public static InformationSourceType JCA = new InformationSourceType(867670019, "JCA");
        public static InformationSourceType HCIM = new InformationSourceType(867670016, "HCIM");
        public static InformationSourceType CORNET = new InformationSourceType(867670009, "CORNET");

        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
   
  }
