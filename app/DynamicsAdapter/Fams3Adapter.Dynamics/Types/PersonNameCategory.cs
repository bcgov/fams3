using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class PersonNameCategory : Enumeration
    {
        public static PersonNameCategory Alberta = new PersonNameCategory(867670000, "Legal Name");
        public static PersonNameCategory BritishColumbia = new PersonNameCategory(867670001, "Alias");
        public static PersonNameCategory Manitoba = new PersonNameCategory(867670002, "Married Name");
        public static PersonNameCategory NewBrunswick = new PersonNameCategory(867670003, "Maiden Name");
  
        protected PersonNameCategory(int value, string name) : base(value, name)
        {
        }
    }
}
