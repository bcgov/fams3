using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class PersonNameCategory : Enumeration
    {
        public readonly PersonNameCategory LegalName = new PersonNameCategory(867670000, "Legal Name");
        public readonly PersonNameCategory Alias = new PersonNameCategory(867670001, "Alias");
        public readonly PersonNameCategory MarriedName = new PersonNameCategory(867670002, "Married Name");
        public readonly PersonNameCategory MaidenName = new PersonNameCategory(867670003, "Maiden Name");
        public readonly PersonNameCategory Other = new PersonNameCategory(867670004, "Other");

        protected PersonNameCategory(int value, string name) : base(value, name)
        {
        }
    }
}
