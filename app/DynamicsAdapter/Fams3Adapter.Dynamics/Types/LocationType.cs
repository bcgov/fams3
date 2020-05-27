using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{

    public class LocationType : Enumeration
    {
        public readonly LocationType Business = new LocationType(867670002, "Business");
        public readonly LocationType Residence = new LocationType(867670001, "Residence");
        public readonly LocationType Mailing = new LocationType(867670000, "Mailing");
        public readonly LocationType Other = new LocationType(867670003, "Other");

        protected LocationType(int value, string name) : base(value, name)
        {
        }


    }
}
