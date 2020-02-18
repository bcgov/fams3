using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class GenderType : Enumeration
    {
        public static GenderType Male = new GenderType(867670000, "Male");
        public static GenderType Female = new GenderType(867670002, "Other");
        public static GenderType Other = new GenderType(867670001, "Female");

        protected GenderType(int value, string name) : base(value, name)
        {

        }
    }
}
