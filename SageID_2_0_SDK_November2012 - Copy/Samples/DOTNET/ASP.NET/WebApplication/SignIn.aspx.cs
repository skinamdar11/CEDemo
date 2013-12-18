using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageSSO.ServiceReferences;
using System.Configuration;

namespace WebApplication
{
    public partial class SignIn : PublicPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // This code is responsible for displaying the status message in the page
                // footer.

                if (Request.Params["sessionEnded"] != null)
                {
                    Session["Message"] = "Session ended.";

                    Response.Redirect("~/SignIn.aspx");
                }

                string message = Session["Message"] != null ? Session["Message"].ToString() : null;

                if (!String.IsNullOrEmpty(message))
                {
                    LabelMessage.Text = HttpUtility.HtmlEncode(message);
                }

                Session["Message"] = null;
            }
        }

        protected void LinkButtonSignIn_Click(object sender, EventArgs e)
        {
            StartSignOnAttemptResponse response = null;

            try
            {
                using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                {
                    client.Open();

                    StartSignOnAttemptRequest request = new StartSignOnAttemptRequest()
                    {
                        // SuccessUri is the URI to which you want the user to be redirected after a successful sign-on.
                        SuccessUri = ConfigurationManager.AppSettings["SignInSuccessUri"],

                        // FailureUri is the URI to which you want the user to be redirected after an unsuccessful sign-on. Note
                        // that SuccessUri and FailureUri can be the same if you prefer. The response from the call to EndSignOnAttempt
                        // provides enough information to distinguish between success and failure.
                        FailureUri = ConfigurationManager.AppSettings["SignInFailUri"],

                        // CancelAllowed lets you specify whether or not the cancel button is displayed. If the user cancels the sign-on by
                        // clicking the cancel button, EndSignOnAttempt gives a specific failure code.
                        CancelAllowed = true,

                        // Set SessionLengthMinutes to the length of your application session.
                        SessionLengthMinutes = HttpContext.Current.Session.Timeout,
                        SessionLengthMinutesSpecified = true,

                        // You may pass a small amount of arbitrary state data to StartSignOnAttempt, which will be returned
                        // unmodified to your application when EndSignOnAttempt is called. This is optional and the
                        // facility is provided to support web frameworks which do not automatically establish application
                        // sessions. Most of the WebSSOService APIs support the State parameter.
                        // State = "this is arbitary state",

                        // Optionally set culture for this attempt
                        // Culture = "fr-fr",

                        // Optionally set TemplateName and CaptchaTheme.
                        // TemplateName = "other",
                        // CaptchaTheme = CaptchaTheme.White
                    };

                    response = client.StartSignOnAttempt(request);
                }
            }
            catch (Exception ex)
            {
                LabelMessage.Text = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartSignOnAttempt(). {1}", ex.GetType().FullName, ex.Message));
            }

            if (response != null)
            {
                // The sign-on attempt ID is placed in the session to act as a "marker" that
                // a sign-on attempt is in progress. It's used later to avoid a call to
                // EndSignOnAttempt() if no sign-on attempt is in progress.

                Session["SignOnAttempt"] = response.SignOnAttemptId;

                Response.Redirect(response.RedirectUri);
            }
        }

        protected void LinkButtonSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SignUp.aspx");
        }
    }
}