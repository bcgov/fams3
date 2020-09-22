using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class IncomeAssistanceClassType : Enumeration
    {
        public static IncomeAssistanceClassType Unemployable = new IncomeAssistanceClassType(867670000, "01 - Unemployable or Disability Level I");
        public static IncomeAssistanceClassType Employable = new IncomeAssistanceClassType(867670001, "03 - Employable");
        public static IncomeAssistanceClassType LongTermCare = new IncomeAssistanceClassType(867670002, "05 - Long Term Care");
        public static IncomeAssistanceClassType MedicalOnly = new IncomeAssistanceClassType(867670003, "08 - Medical Only");

        protected IncomeAssistanceClassType(int value, string name) : base(value, name)
        {
        }
    }
}
