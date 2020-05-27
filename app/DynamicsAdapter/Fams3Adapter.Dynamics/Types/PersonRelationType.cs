using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class PersonRelationType : Enumeration
    {
        public readonly PersonRelationType Spouse = new PersonRelationType(867670000, "Spouse");
        public readonly PersonRelationType AuntUncle = new PersonRelationType(867670002, "Aunt/Uncle");
        public readonly PersonRelationType Parent = new PersonRelationType(867670001, "Parent");
        public readonly PersonRelationType Child = new PersonRelationType(867670003, "Child");
        public readonly PersonRelationType Sibling = new PersonRelationType(867670004, "Sibling");
        public readonly PersonRelationType Cousin = new PersonRelationType(867670005, "Cousin");
        public readonly PersonRelationType Friend = new PersonRelationType(867670006, "Friend");
        protected PersonRelationType(int value, string name) : base(value, name)
        {

        }
    }

}
