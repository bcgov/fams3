using System.Collections.Generic;

namespace DynamicsAdapter.Web.Configuration
{
    public class AppSettings
    {
        public DynamicsAPIConfig DynamicsAPI { get; set; }
    }

    public class DynamicsAPIConfig
    {
        /// <summary>
        /// Http Call timeout in minutes
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// ADFS Auth URL
        /// </summary>
        public string OAuthUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// token time out in minutes
        /// </summary>
        public int TokenTimeout { get; set; }
        public List<EndPoint> EndPoints { get; set; }


    }

    public class EndPoint
    {
        public string Entity { get; set; }
        public string URL { get; set; }
    }


}
