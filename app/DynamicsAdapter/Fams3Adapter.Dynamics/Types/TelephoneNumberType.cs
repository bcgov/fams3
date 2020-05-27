using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class TelephoneNumberType : Enumeration
    {
        public readonly TelephoneNumberType Cell = new TelephoneNumberType(867670000, "Cell");
        public readonly TelephoneNumberType Work = new TelephoneNumberType(867670002, "Work");
        public readonly TelephoneNumberType Home = new TelephoneNumberType(867670001, "Home");
        public readonly TelephoneNumberType Other = new TelephoneNumberType(867670003, "Other");
        protected TelephoneNumberType(int value, string name) : base(value, name)
        {

        }
    }
}
