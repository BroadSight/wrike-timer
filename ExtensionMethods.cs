using System;
using System.Collections.Generic;
using System.Text;

namespace wrike_timer
{
    public static class ExtensionMethods
    {
        public static bool PartsEquals(this Uri uri1, Uri uri2, UriComponents components)
        {
            return uri1.GetComponents(components, UriFormat.Unescaped) == uri2.GetComponents(components, UriFormat.Unescaped);
        }
    }
}
