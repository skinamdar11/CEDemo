using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Serialization;
using CEDataService;
using SageNA.CE.UM.Client;
using SageNA.CE.UMClientInterface;

namespace SageNA.CE.MvcApplicationSignOnDemo.Controllers
{
    public class CEWebDemoController : Controller
    {
        // Example: Action to *manually* initiate login.
        // Please note that this is not required. Hitting any action that has an attribute of [AuthorizeSageId] will automatically redirect to the login process if no valid SSO session exists!
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Login()
        {
            UMClient client = new UMClient();
            return Redirect(client.StartSignOn(this).AbsoluteUri);
        }

        // The action invoked after a successful login
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LoginResult(string id)
        {
            UMClient client = new UMClient();
            UserSignOnResult signOnResult = client.GetSignOnInfo(this, id);
            Session["AuthInfo"] = signOnResult;

            if (signOnResult.IsValid)
            {
                if (signOnResult.OriginalRequestUrl != null && signOnResult.OriginalRequestUrl != string.Empty)
                {
                    return Redirect(signOnResult.OriginalRequestUrl);
                }
                else
                {
                    return RedirectToAction("DemoContent");
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [UMAuthorize]
        public ActionResult DemoContent()
        {
            XmlSerializer serializer = new XmlSerializer(Session["AuthInfo"].GetType());
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, Session["AuthInfo"]);
            var signOnResult = ((UserSignOnResult)Session["AuthInfo"]);
            ViewData["Message"] = string.Format("Welcome, {0} {1}", signOnResult.User.PersonInfo.FirstName, signOnResult.User.PersonInfo.LastName);
            ViewData["PersonInfo"] = sw.ToString();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Error()
        {
            /// TODO: Change to log error, but not display to user
            /// Important!!! The detailled error, i.e. SignOnInfoText and Reason should be written to your local log mechanism used on your server, but
            /// *not* displayed to the end user. It is recommended to show a generic error message based on the reason code (e.g. "Login failed" or "Server unavailable")
            /// with no details to prevent a disclosure vulnerability. You may, however, include a reference that lets support easily locate the corresponding log entry if necessary.
            /// (e.g. "Please contact support and provide reference id <...>").
            var signOnResult = ((UserSignOnResult)Session["AuthInfo"]);
            ViewData["Message"] = signOnResult.SignOnInfoText;
            ViewData["Reason"] = signOnResult.Reason.ToString();
            return View();
        }


        // Example: Action to manually log out, then return to login
        [AcceptVerbs(HttpVerbs.Get)]
        [UMAuthorize]
        public ActionResult Logout()
        {
            UMClient client = new UMClient();
            client.EndSession(this);
            // The above returns the default sign-on Url, but you may redirect to any custom page as needed
            return RedirectToAction("Login");
        }
    }
}
