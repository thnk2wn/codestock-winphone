using System;
using System.Windows;
using System.Windows.Resources;

namespace Phone.Common.IO
{
    public class ResourceHelper
    {
        public StreamResourceInfo GetResource(Uri uri)
        {
            var resourceInfo = Application.GetResourceStream(uri);
            return resourceInfo;
        }

        /*
        // Assembly.GetName() throws MethodAccessException currently on WP7. Security / not allowed
        public StreamResourceInfo GetResource(Assembly assembly, string file)
        {
            var uri = GetResourceUri(assembly.GetName().Name, file);
            var info = GetResource(uri);
            return info;
        }
        */

        public StreamResourceInfo GetResource(string assemblyName, string file)
        {
            var uri = GetResourceUri(assemblyName, file);
            var resourceInfo = GetResource(uri);
            return resourceInfo;
        }

        public Uri GetResourceUri(string assemblyName, string file)
        {
            var uri = new Uri(string.Format("{0};component/{1}", assemblyName, file), UriKind.Relative);
            return uri;
        }
    }
}
