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
    public partial class SignUp : PublicPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string message = Session["Message"] != null ? Session["Message"].ToString() : null;

                if (!String.IsNullOrEmpty(message))
                {
                    LabelMessage.Text = HttpUtility.HtmlEncode(message);
                }

                Session["Message"] = null;
            }
        }

        protected void LinkButtonNewUser_Click(object sender, EventArgs e)
        {
            StartRegistrationAttemptResponse response = null;

            try
            {
                using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                {
                    client.Open();

                    StartNewUserRegistrationAttemptRequest request = new StartNewUserRegistrationAttemptRequest()
                    {
                        // In this example, SuccessUri and FailureUri are set to the same value. The response
                        // to each End<X>Attempt() contains enough information to distinguish
                        // between a success result and a failure result. SignUpNewResult.aspx illustrates
                        // how to do this.
                        SuccessUri = ConfigurationManager.AppSettings["SignUpNewResultUri"],
                        FailureUri = ConfigurationManager.AppSettings["SignUpNewResultUri"],

                        CancelAllowed = true,
                        SignOnAfterSuccess = true,

                        // The user will be signed on following a successful registration, so we need to
                        // specify the application session length.
                        SessionLengthMinutes = HttpContext.Current.Session.Timeout,
                        SessionLengthMinutesSpecified = true,
                    };

                    response = client.StartNewUserRegistrationAttempt(request);
                }
            }
            catch (Exception ex)
            {
                LabelMessage.Text = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartNewUserRegistrationAttempt(). {1}", ex.GetType().FullName, ex.Message));
            }

            if (response != null)
            {
                // The registration attempt ID is placed in the session to act as a "marker" that
                // a registration attempt is in progress. It's used later to avoid a call to
                // EndNewUserRegistrationAttempt() if no registration attempt is in progress.

                Session["NewUserRegistrationAttempt"] = response.RegistrationAttemptId;

                Response.Redirect(response.RedirectUri);
            }
        }

        protected void LinkButtonExistingUser_Click(object sender, EventArgs e)
        {
            StartRegistrationAttemptResponse response = null;

            try
            {
                using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                {
                    client.Open();

                    StartExistingUserRegistrationAttemptRequest request = new StartExistingUserRegistrationAttemptRequest()
                    {
                        // In this example, SuccessUri and FailureUri are set to the same value. The response
                        // to each End<X>Attempt() contains enough information to distinguish
                        // between a success result and a failure result. SignUpExistingResult.aspx illustrates
                        // how to do this.
                        SuccessUri = ConfigurationManager.AppSettings["SignUpExistingResultUri"],
                        FailureUri = ConfigurationManager.AppSettings["SignUpExistingResultUri"],

                        // The user will be signed on following a successful registration, so we need to
                        // specify the application session length.
                        SessionLengthMinutes = HttpContext.Current.Session.Timeout,
                        SessionLengthMinutesSpecified = true,

                        CancelAllowed = true                        
                    };

                    response = client.StartExistingUserRegistrationAttempt(request);
                }
            }
            catch (Exception ex)
            {
                LabelMessage.Text = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartExistingUserRegistrationAttempt(). {1}", ex.GetType().FullName, ex.Message));
            }

            if (response != null)
            {
                // The registration attempt ID is placed in the session to act as a "marker" that
                // a registration attempt is in progress. It's used later to avoid a call to
                // EndExistingUserRegistrationAttempt() if no registration attempt is in progress.

                Session["ExistingUserRegistrationAttempt"] = response.RegistrationAttemptId;

                Response.Redirect(response.RedirectUri);
            }
        }

        protected void LinkButtonBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SignIn.aspx");
        }
    }
}