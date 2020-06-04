using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class EmploymentRecordType : Enumeration
    {

        public static EmploymentRecordType Employment = new EmploymentRecordType(867670000, "Employment");
        public static EmploymentRecordType IncomeAssistance = new EmploymentRecordType(867670001, "Income Assistance");

        protected EmploymentRecordType(int value, string name) : base(value, name)
        {
        }
    }
}
