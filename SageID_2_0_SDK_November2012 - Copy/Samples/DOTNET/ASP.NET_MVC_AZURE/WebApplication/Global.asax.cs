using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Configuration;

namespace WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

           

            #region *** SSO controller ***

            // Successful sign-on
            routes.MapRoute(
                "SignOnSuccess",
                "SSO/SignOnSuccess/{resultId}",
                new { controller = "SSO", action = "SignOnSuccessful", resultId = "" }
            );

            // Failed sign-on
            routes.MapRoute(
                "SignOnFail",
                "SSO/SignOnFail/{resultId}",
                new { controller = "SSO", action = "SignOnFailed", resultId = "" }
            );

            // Sign out
            routes.MapRoute(
                "SignOut",
                "SSO/SignOut",
                new { controller = "SSO", action = "SignOut"}
            );

            // Sign out all
            routes.MapRoute(
                "SignOutAll",
                "SSO/SignOutAll",
                new { controller = "SSO", action = "SignOutAll"}
            );

            // Init sign-on
            routes.MapRoute(
                "SignOnInit",
                "SSO/SignOnInit",
                new { controller = "SSO", action = "SignOnInit" }
            );

            // Change password
            routes.MapRoute(
                "ChangePassword",
                "SSO/ChangePassword",
                new { controller = "SSO", action = "ChangePassword" }
            );

            // Change password result
            routes.MapRoute(
                "ChangePasswordResult",
                "SSO/ChangePasswordResult/{resultId}",
                new { controller = "SSO", action = "ChangePasswordResult", resultId = "" }
            );

            // Register new
            routes.MapRoute(
                "RegisterNew",
                "SSO/RegisterNew",
                new { controller = "SSO", action = "RegisterNew" }
            );

            // Register new result
            routes.MapRoute(
                "RegisterNewResult",
                "SSO/RegisterNewResult/{resultId}",
                new { controller = "SSO", action = "RegisterNewResult", resultId = "" }
            );

            // Register existing
            routes.MapRoute(
                "RegisterExisting",
                "SSO/RegisterExisting",
                new { controller = "SSO", action = "RegisterExisting" }
            );

            // Register existing result
            routes.MapRoute(
                "RegisterExistingResult",
                "SSO/RegisterExistingResult/{resultId}",
                new { controller = "SSO", action = "RegisterExistingResult", resultId = "" }
            );

            // SSO notifications
            routes.MapRoute(
                "SSONotify",
                "SSO/Notify",
                new { controller = "SSO", action = "SSONotify" }
            );

            // Activation
            routes.MapRoute(
                "Activation",
                "SSO/Activation/{resultId}",
                new { controller = "SSO", action = "Activation", resultId = "" }
            );

            #endregion

            #region *** Home controller ***

            // Sign-in landing page
            routes.MapRoute(
                "SignIn",
                "SignIn",
                new { controller = "Home", action = "SignIn" }
            );

            // Sign-in landing page
            routes.MapRoute(
                "SignUp",
                "SignUp",
                new { controller = "Home", action = "SignUp" }
            );

            // Main application
            routes.MapRoute(
                "App",
                "",
                new { controller = "Home", action = "App" }
            );

            #endregion
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            // This code ensures that any session cookie that has been set for web SSO is
            // removed before the incoming request is bound to a session. This helps improve
            // concurrency for requests coming from a single client. It only affects the notify
            // URL and only if the client is erroneously sending cookie data.
            //
            // Note that the cookie isn't cleared from the client - it just means that a
            // request doesn't become bound to a session.

            if (_ssoNotifyUriRegex.IsMatch(Request.Url.AbsoluteUri) &&
                Request.Cookies[ConfigurationManager.AppSettings["SessionCookieName"]] != null)
            {
                Request.Cookies.Remove(ConfigurationManager.AppSettings["SessionCookieName"]);
            }
        }

        // Matches http[s]://<dnsName>|<ipAddress>[:port]/SSO/Notify
        private static readonly Regex _ssoNotifyUriRegex = new Regex(@"^http.?://[a-zA-Z0-9\-]*(\.[a-zA-Z0-9\-]*)*(:\d+)?/SSO/Notify$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
    }
}