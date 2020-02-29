using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.Redis.Configuration
{
    /// <summary>
    /// Represent the Redis configuration
    /// Settings can be overwriten by environment variables
    /// </summary>
    public class RedisConfiguration
    {

        public RedisConfiguration()
        {
            this.Host = "localhost";
            this.Port = 6379;
        }

        /// <summary>
        /// Redis Host
        /// </summary>
        public string Host { get; set; }


        /// <summary>
        /// Redis Port
        /// </summary>
        public int Port { get; set; }

        public string ConnectionString {
            get
            {
                return $"{Host}:{ Port}";
            }
        }
    }
}
