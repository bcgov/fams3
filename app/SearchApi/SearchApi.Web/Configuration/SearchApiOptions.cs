using System.Collections.Generic;

namespace SearchApi.Web.Configuration
{
    /// <summary>
    /// Represents the searchApi settings
    /// </summary>
    public class SearchApiOptions
    {
        public List<WebHookNotification> WebHooks { get; set; } = new List<WebHookNotification>();
        public string ApiKeyForDynadaptor { get; set; }
        public int Timeout { get; set; } = 2;

        public SearchApiOptions AddWebHook(string name, string uri)
        {
            WebHooks.Add(new WebHookNotification()
            {
                Name = name,
                Uri = uri,
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
    }

}