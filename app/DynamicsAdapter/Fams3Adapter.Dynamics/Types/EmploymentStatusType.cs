using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class EmploymentStatusType : Enumeration
    {

        public static EmploymentStatusType Employed = new EmploymentStatusType(867670000, "Employed");
        public static EmploymentStatusType NotEmployed = new EmploymentStatusType(867670001, "Not Employed");
        public static EmploymentStatusType Unknown = new EmploymentStatusType(867670002, "Unknown");
        public static EmploymentStatusType IncomeSource = new EmploymentStatusType(867670003, "Income Source");
        public static EmploymentStatusType SelfEmployed = new EmploymentStatusType(867670005, "Self Employed");
        public static EmploymentStatusType IncomeAssistance = new EmploymentStatusType(867670004, "Income Assistance");

        protected EmploymentStatusType(int value, string name) : base(value, name)
        {
        }
    }
}
