using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class GenderType : Enumeration
    {
        public readonly GenderType Male = new GenderType(867670000, "Male");
        public readonly GenderType Female = new GenderType(867670001, "Female");
        public readonly GenderType Other = new GenderType(867670002, "Other");

        protected GenderType(int value, string name) : base(value, name)
        {

        }
    }
}
