using System;
using System.Web;

namespace WebApplication
{
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return SSOSession.Refresh();
        }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            // This is a simple check to see if there is an SSOSession object
            // in the application session. It is used later on to determine if
            // a "your session has ended" message should be displayed to the
            // user.
            bool hadSession = HttpContext.Current.Session["SSOSession"] != null;

 	        base.OnAuthorization(filterContext);

            if (filterContext.Result is System.Web.Mvc.HttpUnauthorizedResult)
            {
                if (hadSession)
                {
                    filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary() { { "controller", "Home" }, { "action", "SignIn" }, { "sessionEnded", "true" } });
                }
                else
                {
                    filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary() { { "controller", "Home" }, { "action", "SignIn" } });
                }                
            }
        }
    }
}