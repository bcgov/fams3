using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.People
{
    [Description("Represents a set of information to execute a search on a person")]
    public class Identifier
    {
        [Description("The identification serial number of this identifier.")]
        public string SerialNumber { get; set; }

        [Description("The identification effective date.")]
        public DateTime? EffectiveDate { get; set; }

        [Description("The identification expiration date.")]
        public DateTime? ExpirationDate { get; set; }

        [Description("The identfication type,such as driver license.")]
        public IdentifierTypeEnum Type { get; set; }

        [Description("The identifier is issued by who or which organization.")]
        public string IssuedBy { get; set; }
    }
}
