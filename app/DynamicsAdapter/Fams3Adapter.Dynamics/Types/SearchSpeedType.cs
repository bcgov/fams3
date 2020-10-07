using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class SearchSpeedType : Enumeration
    {
     

        public static SearchSpeedType Fast = new SearchSpeedType(867670000, "Fast");
        public static SearchSpeedType Slow = new SearchSpeedType(867670001, "Slow");
     

        protected SearchSpeedType(int value, string name) : base(value, name)
        {

        }
    }
}
