using System;
using System.Collections.Generic;
using System.Text;

namespace wrike_timer
{
    public static class Constants
    {
        public static class WrikeAuth
        {
            public static readonly Uri AuthUrl     = new Uri("https://www.wrike.com/oauth2/authorize/v4");
            public static readonly Uri RedirectUrl = new Uri("https://localhost:58972/");
            public static readonly Uri TokenUrl    = new Uri("https://www.wrike.com/oauth2/token");
            public static readonly string ApiClientId     = "WTsCagm4";
            public static readonly string ApiClientSecret = "EBXGxL2anOIii8PFim43ffgxYe2nBmSsxrL7pcasAXFAZGe7IxJudOaT6kAQRVWL";
            public static readonly string ApiScope        = "Default, amReadOnlyWorkflow, amReadWriteWorkflow, amReadOnlyTimelogCategory, amReadWriteTimelogCategory, wsReadOnly, wsReadWrite";
            public static readonly string TokenAppPropKey = "wrikeRefreshToken";
        }
    }
}
