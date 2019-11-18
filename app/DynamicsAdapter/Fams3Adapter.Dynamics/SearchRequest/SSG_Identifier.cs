using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.SearchRequest
{
        public class SSG_Identifier
        {
            public string SSG_Identification { get; set; }
            public int StatusCode { get; set; }
            public int StateCode { get; set; }
            public DateTime ssg_identificationeffectivedate { get; set; }
        }

}
