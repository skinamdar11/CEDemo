using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
    // ProtectedPage is the base class for those pages which should not be accessed
    // without a valid SSO session. ProtectedPage performs two important functions:
    //
    // - It checks whether a valid SSO session exists and, if there isn't one, it
    //   redirects to the sign-in page.
    //
    // - It ensures that the page is not cached by the browser so that a user
    //   without a valid SSO session can't back-click to a protected page in the
    //   browser cache.

    public class ProtectedPage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            // This is a simple check to see if there is an SSOSession object
            // in the application session. It is used later on to determine if
            // a "your session has ended" message should be displayed to the
            // user.
            bool hadSession = HttpContext.Current.Session["SSOSession"] != null;

            // SSOSession.Refresh() checks for the existence of a valid SSOSession and
            // extends the SSO session if necessary.
            if (!SSOSession.Refresh())
            {
                // If SSOSession.Refresh() returns false the SSOSession object has already
                // been removed from the application session but the application session needs
                // to be ended as well.
                HttpContext.Current.Session.Abandon();

                // We can't simply write a message to the session here because it's about to be abandoned and
                // a new session ID will be generated for the next call. Therefore, we're going to use a 
                // query string instead to signal that the user's session ended.
                Response.Redirect(string.Format("~/SignIn.aspx{0}", hadSession ? "?sessionEnded=true" : string.Empty));
            }
            else
            {
                // Prevent this page from being cached in the browser.
                HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
                HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.Cache.SetNoStore();

                base.OnLoad(e);
            }
        }
    }
}