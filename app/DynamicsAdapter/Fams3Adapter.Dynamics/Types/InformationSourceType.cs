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
        public static InformationSourceType Other = new InformationSourceType(867670003, "Other");
        public static InformationSourceType BCHydro = new InformationSourceType(867670005, "BCHydro");
        public static InformationSourceType MSDPR = new InformationSourceType(867670023, "MSDPR");

        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
   
    public class ProviderProfileMap
    {
        public static readonly ReadOnlyDictionary<string, string> ProvideCodeToProfileNameMap = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
        {
            {"ICBC", "ICBC"},
            {"BCHYDRO", "BCHydro"},
             { "MHSD","MSDPR" },
            {"Other","Other" }
        });
    }
}
