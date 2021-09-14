using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SearchRequestAutoCloseStatusCode : Enumeration
    {
        public static SearchRequestAutoCloseStatusCode NoCPMatch =
            new SearchRequestAutoCloseStatusCode(867670000, "No CP Match");
        public static SearchRequestAutoCloseStatusCode CPMissingData =
            new SearchRequestAutoCloseStatusCode(867670001, "CP Missing Data");
        public static SearchRequestAutoCloseStatusCode ReadyToClose =
            new SearchRequestAutoCloseStatusCode(867670004, "Ready To Close");


        protected SearchRequestAutoCloseStatusCode(int value, string name) : base(value, name)
        {
        }

    }
}
