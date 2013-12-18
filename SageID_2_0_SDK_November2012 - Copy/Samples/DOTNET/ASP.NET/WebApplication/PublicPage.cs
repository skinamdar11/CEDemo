using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
    // PublicPage is the base class for those pages which can be accessed
    // without an SSO session but which should redirect the user to their
    // home page if a valid SSO session exists. For example, if a valid SSO
    // session exists there's no need to sign in the user again. SignIn.aspx
    // is therefore a "public" page.

    public class PublicPage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            if (SSOSession.HasSession)
            {
                Response.Redirect("~");
            }
            else
            {
                base.OnLoad(e);
            }
        }
    }
}