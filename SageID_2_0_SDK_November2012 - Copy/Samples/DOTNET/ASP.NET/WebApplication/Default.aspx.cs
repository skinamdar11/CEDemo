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
    public partial class _Default : ProtectedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SSOSession.HasSession)
            {
                string u = string.Format("{0} ({1})",
                                         SSOSession.Current.DisplayName,
                                         SSOSession.Current.EmailAddress);

                LabelUserInfo.Text = HttpUtility.HtmlEncode(u);

                // Note that it is not recommended to expose the session ID in any way to the
                // user. It's displayed here for testing purposes only.
                LabelSessionId.Text = string.Format("Session ID: {0}", SSOSession.Current.SSOSessionId.ToString());
                LabelSessionExpiry.Text = string.Format("Expires: {0}", SSOSession.Current.Expiry.ToLocalTime().ToString());
            }

            string message = Session["Message"] != null ? Session["Message"].ToString() : null;

            if (!String.IsNullOrEmpty(message))
            {
                LabelMessage.Text = HttpUtility.HtmlEncode(message);
            }

            Session["Message"] = null;
        }

        protected void LinkButtonRefreshPage_Click(object sender, EventArgs e)
        {
        }

        protected void LinkButtonManageAuthorisation_Click(object sender, EventArgs e)
        {
            StartManageAuthorisationAttemptResponse response = null;
            try
            {
                using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                {
                    client.Open();

                    StartManageAuthorisationAttemptRequest request = new StartManageAuthorisationAttemptRequest()
                    {

                        // In this example, SuccessUri and FailureUri are set to the same value. The response
                        // to each End<X>Attempt() contains enough information to distinguish
                        // between a success result and a failure result. ManageAuthorisationsResult() illustrates
                        // how to do this.
                        SuccessUri = ConfigurationManager.AppSettings["ManageAuthorisationsResultUri"],
                        FailureUri = ConfigurationManager.AppSettings["ManageAuthorisationsResultUri"],

                        CancelAllowed = true
                    };

                    response = client.StartManageAuthorisationAttempt(request);
                }
            }
            catch (Exception ex)
            {
                LabelMessage.Text = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartManageAuthorisationAttempt(). {1}", ex.GetType().FullName, ex.Message));
            }

            if (response != null)
            {
                // The change password attempt ID is placed in the session to act as a "marker" that
                // a manage authorisations attempt is in progress. It's used later to avoid a call to
                // EndManageAuthorisationsAttempt() if no authorisation management attempt is in progress.

                Session["ManageAuthorisationsAttempt"] = response.ManageAuthorisationAttemptId;

                Response.Redirect(response.RedirectUri);
            }
        }

        protected void LinkButtonChangePassword_Click(object sender, EventArgs e)
        {
            StartPasswordChangeAttemptResponse response = null;

            try
            {
                using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                {
                    client.Open();

                    StartPasswordChangeAttemptRequest request = new StartPasswordChangeAttemptRequest()
                    {
                        // In this example, SuccessUri and FailureUri are set to the same value. The response
                        // to each End<X>Attempt() contains enough information to distinguish
                        // between a success result and a failure result. ChangePasswordResult.aspx illustrates
                        // how to do this.
                        SuccessUri = ConfigurationManager.AppSettings["ChangePasswordResultUri"],
                        FailureUri = ConfigurationManager.AppSettings["ChangePasswordResultUri"],

                        CancelAllowed = true
                    };
                    
                    response = client.StartPasswordChangeAttempt(request);
                }
            }
            catch (Exception ex)
            {
                LabelMessage.Text = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartPasswordChangeAttempt(). {1}", ex.GetType().FullName, ex.Message));
            }

            if (response != null)
            {
                // The change password attempt ID is placed in the session to act as a "marker" that
                // a change password attempt is in progress. It's used later to avoid a call to
                // EndChangePasswordAttempt() if no change password attempt is in progress.

                Session["ChangePasswordAttempt"] = response.PasswordChangeAttemptId;

                Response.Redirect(response.RedirectUri);
            }
        }
        
        protected void LinkButtonSignOut_Click(object sender, EventArgs e)
        {
            // Always end your local session first. This helps prevent any nasty race
            // conditions which might occur otherwise if, for example, the user clicks
            // a protected link before the web service call to SSO is complete.

            Guid ssoSessionId = Guid.Empty;

            if (SSOSession.HasSession)
            {
                ssoSessionId = SSOSession.Current.SSOSessionId;
                SSOSession.Current.End();
            }

            HttpContext.Current.Session.Abandon();

            if (ssoSessionId != Guid.Empty)
            {
                try
                {
                    // SessionRemoveParticipant() removes this application from the SSO session. If there are other
                    // applications which are participants in the same SSO session, the SSO session will remain
                    // open, otherwise it will be ended.
                    //
                    // SessionRemoveParticpant() is not recommended for applications which are configured to
                    // always automatically sign-on the user if an SSO session already exists.
                    using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                    {
                        client.Open();

                        SessionRemoveParticipantRequest request = new SessionRemoveParticipantRequest()
                        {
                            SessionId = ssoSessionId
                        };

                        SessionRemoveParticipantResponse response = client.SessionRemoveParticipant(request);
                    }
                }
                catch (Exception ex)
                {
                    Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.SessionRemoveParticipant(). {1}", ex.GetType().FullName, ex.Message);
                }
            }

            Response.Redirect("~/SignIn.aspx");
        }

        protected void LinkButtonSignOutAll_Click(object sender, EventArgs e)
        {
            // Always end your local session first. This helps prevent any nasty race
            // conditions which might occur otherwise if, for example, the user clicks
            // a protected link before the web service call to SSO is complete.

            Guid ssoSessionId = Guid.Empty;

            if (SSOSession.HasSession)
            {
                ssoSessionId = SSOSession.Current.SSOSessionId;
                SSOSession.Current.End();
            }

            HttpContext.Current.Session.Abandon();

            if (ssoSessionId != Guid.Empty)
            {
                try
                {
                    // SessionSignOff() ends the SSO session for all participants. A Session.Ended notification
                    // is sent out to all participants (including this one). It is recommended that all
                    // SSO-enabled applications offer a way for the user to end their SSO session.
                    using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                    {
                        client.Open();

                        SessionSignOffRequest request = new SessionSignOffRequest()
                        {
                            SessionId = ssoSessionId
                        };

                        SessionSignOffResponse response = client.SessionSignOff(request);
                    }
                }
                catch (Exception ex)
                {
                    Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.SessionSignOff(). {1}", ex.GetType().FullName, ex.Message);
                }
            }

            Response.Redirect("~/SignIn.aspx");
        }
    }
}