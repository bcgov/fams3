namespace BcGov.Fams3.SearchApi.Core.Configuration
{
    /// <summary>
    /// Represents the RabbitMq Configuration
    /// Settings can be overriden with environment variables
    /// ex: to override RabbitMq.Host, set RABBITMQ__HOST=localhost environment variable.
    /// </summary>
    public class RabbitMqConfiguration
    {

        /// <summary>
        /// Default Constructor, sets the default values.
        /// </summary>
        public RabbitMqConfiguration()
        {
            this.Host = "localhost";
            this.Port = 5672;
            this.Username = "guest";
            this.Password = "guest";
        }

        /// <summary>
        /// RabbitMq Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// RabbitMq Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// RabbitMq Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// RabbitMq Password
        /// </summary>
        public string Password { get; set; }


    }
}

