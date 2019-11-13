using System;

namespace DynamicsAdapter.Web.Services.Dynamics.Model
{
    public class SSG_SearchApiRequest
    {
        public Guid SSG_SearchApiRequestId { get; set; }
        public string SSG_PersonGivenName { get; set; }
        public string SSG_PersonSurname { get; set; }
        public string SSG_PersonMiddleName { get; set; }
        public string SSG_PersonThirdGivenName { get; set; }
        public DateTime SSG_PersonBirthDate { get; set; }
        public int StatusCode { get; set; }

    }
}
