using System;

namespace DynamicsAdapter.Web.SearchAgency.Exceptions
{
    public class AgencyRequestException : Exception
    {
        public AgencyRequestException()
        {
        }

        public AgencyRequestException(string message)
            : base(message)
        {
        }

        public AgencyRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
