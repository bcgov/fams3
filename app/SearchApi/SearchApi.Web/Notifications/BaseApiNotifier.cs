using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.Notifications
{

    public abstract class BaseApiNotifier
    {
        protected bool TryCreateUri(string baseUrl, string path, out Uri uri)
        {
            uri = null;
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            {
                return false;
            }

            return Uri.TryCreate(baseUri, Path.Combine(baseUri.LocalPath, path), out uri);
        }
    }
}
