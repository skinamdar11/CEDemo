using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace OAuthClientWebApp
{
    public static class Settings
    {
        public static string WebAppAClientId { get { return ConfigurationManager.AppSettings["WebAppAClientId"]; } }
        public static string WebAppBClientId { get { return ConfigurationManager.AppSettings["WebAppBClientId"]; } }
        public static string WebAppASecretKey { get { return ConfigurationManager.AppSettings["WebAppASecretKey"]; } }
        public static string WebAppBSecretKey { get { return ConfigurationManager.AppSettings["WebAppBSecretKey"]; } }
        public static string RedirectUri { get { return ConfigurationManager.AppSettings["RedirectUri"]; } }
        public static string StartAuthorisationAttemptUrl { get { return ConfigurationManager.AppSettings["StartAuthorisationAttemptUrl"]; } }
        public static string GetAccessTokenUrl { get { return ConfigurationManager.AppSettings["GetAccessTokenUrl"]; } }
        public static string ResourceApplicationSoapUrl { get { return ConfigurationManager.AppSettings["ResourceApplicationSoapUrl"]; } }
        public static string ResourceApplicationRestUrl { get { return ConfigurationManager.AppSettings["ResourceApplicationRestUrl"]; } }
        public static string WebAppAScope { get { return ConfigurationManager.AppSettings["WebAppAScope"]; } }
        public static string WebAppBScope { get { return ConfigurationManager.AppSettings["WebAppBScope"]; } }

    }
}