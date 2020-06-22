using System;

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

    }
}
