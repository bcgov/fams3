using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class RelatedPersonPersonType : Enumeration
    {
        public static RelatedPersonPersonType Relation = new RelatedPersonPersonType(867670000, "Relation");
        public static RelatedPersonPersonType Applicant = new RelatedPersonPersonType(867670001, "Applicant");
        protected RelatedPersonPersonType(int value, string name) : base(value, name)
        {

        }
    }

}
