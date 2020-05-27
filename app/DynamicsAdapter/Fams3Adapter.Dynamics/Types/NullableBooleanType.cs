using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class NullableBooleanType : Enumeration
    {
        public readonly NullableBooleanType Yes = new NullableBooleanType(867670000, "Yes");
        public readonly NullableBooleanType No = new NullableBooleanType(867670001, "No");

        protected NullableBooleanType(int value, string name) : base(value, name)
        {
        }
    }
}
