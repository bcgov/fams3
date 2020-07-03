using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class IncomeAssistanceStatusType : Enumeration
    {
        public static IncomeAssistanceStatusType Active = new IncomeAssistanceStatusType(867670000, "Active");
        public static IncomeAssistanceStatusType Closed = new IncomeAssistanceStatusType(867670001, "Closed");
        public static IncomeAssistanceStatusType Unknown = new IncomeAssistanceStatusType(867670002, "Unknown");

        protected IncomeAssistanceStatusType(int value, string name) : base(value, name)
        {
        }
    }
}
