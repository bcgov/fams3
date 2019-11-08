using System;

namespace DynamicsAdapter.Web.Services.Dynamics.Model
{
    public class SSG_SearchRequest
    {
        public Guid SSG_SearchRequestId { get; set; }
        public string SSG_PersonGivenName { get; set; }
        public string SSG_PersonSurname { get; set; }
        public DateTime SSG_PersonBirthDate { get; set; }
        public int StatusCode { get; set; }

    }
}
