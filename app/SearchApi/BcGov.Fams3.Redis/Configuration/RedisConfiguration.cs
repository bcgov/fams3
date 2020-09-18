namespace BcGov.Fams3.Redis.Configuration
{
    /// <summary>
    /// Represent the Redis configuration
    /// Settings can be overwriten by environment variables
    /// </summary>
    public class FRedisConfiguration
    {

        public FRedisConfiguration()
        {
            this.Host = "localhost";
            this.Port = 6379;
            this.Password = "";
        }

        /// <summary>
        /// Redis Host
        /// </summary>
        public string Host { get; set; }


        /// <summary>
        /// Redis Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Redis Database Password
        /// </summary>

        public string Password { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"{Host}:{ Port}";
            }
        }
    }
}
