using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchRequestOrdered : SearchRequestEvent
    {
        public Person Person { get; set; }
    }
}
