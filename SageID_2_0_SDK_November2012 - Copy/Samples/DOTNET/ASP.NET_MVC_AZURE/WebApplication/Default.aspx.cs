using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace WebAdmin.Application
{
    public partial class _Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            // Change the current path so that the Routing handler can correctly interpret
            // the request, then restore the original path so that the OutputCache module
            // can correctly process the response (if caching is enabled).

            /*string originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
            HttpContext.Current.RewritePath(originalPath, false);*/

            //The above redirection code doesn't work on Azure, the latest
            //version of MVC3 doesn't event generate the above code snippet
            //when creating a new project. Instead, do an old-school redirect.
            Response.Redirect("~/SignIn");
        }
    }
}
