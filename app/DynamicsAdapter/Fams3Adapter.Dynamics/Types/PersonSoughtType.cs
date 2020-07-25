using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class PersonSoughtType : Enumeration
    {
        public static PersonSoughtType P = new PersonSoughtType(867670000, "P");
        public static PersonSoughtType R = new PersonSoughtType(867670001, "R");
        protected PersonSoughtType(int value, string name) : base(value, name)
        {
        }


    }
}
