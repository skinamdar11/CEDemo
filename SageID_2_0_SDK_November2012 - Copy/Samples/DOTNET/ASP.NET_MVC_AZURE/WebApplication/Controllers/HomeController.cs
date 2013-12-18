using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SignIn()
        {
            string message = Session["Message"] != null ? Session["Message"].ToString() : null;

            if (!String.IsNullOrEmpty(message))
            {
                ViewData["Message"] = HttpUtility.HtmlEncode(message);
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["sessionEnded"]))
                {
                    ViewData["Message"] = "Session ended.";
                }
            }

            Session["Message"] = null;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SignUp()
        {
            // This code is responsible for displaying the status message in the page
            // footer.

            string message = Session["Message"] != null ? Session["Message"].ToString() : null;

            if (!String.IsNullOrEmpty(message))
            {
                ViewData["Message"] = HttpUtility.HtmlEncode(message);
            }

            Session["Message"] = null;

            return View();
        }

        // Note the use of the [Authorize] attribute here. This is the default action for this application and should not be available to
        // unauthenticated users. The [Authorize] attribute checks for the existence of an SSO session for this user and redirects the
        // user to the sign-in page if no SSO session exists. All actions in your application which requre an authenticated session
        // should do this.
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult App()
        {
            string message = Session["Message"] != null ? Session["Message"].ToString() : null;

            if (!String.IsNullOrEmpty(message))
            {
                ViewData["Message"] = HttpUtility.HtmlEncode(message);
            }

            Session["Message"] = null;

            string u = string.Format("{0} ({1})",
                         SSOSession.Current.DisplayName,
                         SSOSession.Current.EmailAddress);

            ViewData["UserInfo"] = HttpUtility.HtmlEncode(u);

            // Note that it is not recommended to expose the session ID in any way to the
            // user. It's displayed here for testing purposes only.
            ViewData["SessionId"] = string.Format("Session ID: {0}", SSOSession.Current.SSOSessionId.ToString());
            ViewData["SessionExpiry"] = string.Format("Expires: {0}", SSOSession.Current.Expiry.ToLocalTime().ToString());

            return View();
        }
    }
}
