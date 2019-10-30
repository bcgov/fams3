using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Services.Dynamics
{
    public class APIException : Exception
    {
        public APIException(string message) : base(message) { }

        public APIException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
