using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class ApplicantCreatedStatusType : Enumeration
    {

        public static ApplicantCreatedStatusType ReadyToCreate = new ApplicantCreatedStatusType(867670001, "ReadyToCreate");
        protected ApplicantCreatedStatusType(int value, string name) : base(value, name)
        {
        }


    }
}
