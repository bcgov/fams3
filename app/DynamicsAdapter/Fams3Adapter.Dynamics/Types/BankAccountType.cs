using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class BankAccountType : Enumeration
    {
        public static BankAccountType Chequing = new BankAccountType(867670000, "Chequing");
        public static BankAccountType Savings = new BankAccountType(867670001, "Savings");
        public static BankAccountType Joint = new BankAccountType(867670002, "Joint");
        public static BankAccountType Other = new BankAccountType(867670003, "Other");

        protected BankAccountType(int value, string name) : base(value, name)
        {

        }
    }
}
