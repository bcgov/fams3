using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Register
{
    public class RegisterFailedException : Exception
    {
        public RegisterFailedException()
        {
        }

        public RegisterFailedException(string message)
            : base(message)
        {
        }

        public RegisterFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
