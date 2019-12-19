using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Linq;

namespace Fams3Adapter.Dynamics.Types

{
    
    public class InformationSourceType : Enumeration
    {

        public static InformationSourceType Request = new InformationSourceType(867670000, "Request");
        public static InformationSourceType ICBC = new InformationSourceType(867670001, "ICBC");
        public static InformationSourceType Employer = new InformationSourceType(867670002,  "Employer");
        public static InformationSourceType Other = new InformationSourceType(867670003, "Other");

        protected InformationSourceType(int value, string name) : base(value, name)
        {
        }
    }
    public static class StringExtension
    {
        public static int? SuppliedByID (this string source )
        {
         return  Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        
        }

        public static string SuppliedByValue(this int source)
        {
            return Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Value == source).Name;
         
        }
    }
}
