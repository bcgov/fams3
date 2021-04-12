namespace DynamicsAdapter.Web.Configuration
{
    public class SearchApiConfiguration
    {
        public string BaseUrl { get; set; }
        // Colon separated list of the profile name of data partners e.g. BCHYDRO:MSDPR:....
        public string AvailableDataPartner { get; set; }
    }
}