using System.Collections.Generic;

namespace SearchApi.Web.Configuration
{
    /// <summary>
    /// Represents the searchApi settings
    /// </summary>
    public class SearchApiOptions
    {
        public List<WebHookNotification> WebHooks { get; set; } = new List<WebHookNotification>();

        public SearchApiOptions AddWebHook(string name, string uri, string apiKey=null)
        {
            WebHooks.Add(new WebHookNotification()
            {
                Name = name,
                Uri = uri,
                ApiKey = apiKey
            });
            return this;
        }

    }

    /// <summary>
    /// Represents the webHook
    /// </summary>
    public class WebHookNotification
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string ApiKey { get; set; }
    }

}