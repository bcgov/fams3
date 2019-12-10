using Fams3Adapter.Dynamics.OptionSets.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Types
{
    //ssg_addresscategorycodes
    public class LocationType : Enumeration
    {
        public static LocationType Residence = new LocationType(867670001, "Residence");
        public static LocationType Mailing = new LocationType(867670000, "Mailing");

        public LocationType(int value, string name) : base(value, name)
        {
        }


    }
}
