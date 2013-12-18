using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageSSO.ServiceReferences;
using System.Xml;
using System.Text;

namespace WebApplication
{
    public partial class SignInFail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Don't do any processing if an attempt was not
                // in progress.
                if (Session["SignOnAttempt"] != null)
                {
                    Session["SignOnAttempt"] = null;

                    // Don't do any processing if there's no
                    // id parameter in the GET request.
                    if (Request.Params["id"] != null)
                    {
                        Guid resultId = Guid.Empty;

                        try
                        {
                            resultId = new Guid(Request.Params["id"]);
                        }
                        catch (Exception)
                        {
                            // Ignore failure to parse GUID
                        }

                        // Don't do any processing if the id parameter is not
                        // a valid, non-zero GUID.
                        if (resultId != Guid.Empty)
                        {
                            try
                            {
                                using (var client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                                {
                                    EndSignOnAttemptRequest request = new EndSignOnAttemptRequest();
                                    request.ResultId = resultId;

                                    // Make a web service call to SSO to retrieve the result of the
                                    // sign-on attempt.
                                    EndSignOnAttemptResponse response = client.EndSignOnAttempt(request);

                                    FailedResult result = (FailedResult)response.Item;

                                    switch (result.Reason)
                                    {
                                        case FailureReason.SignOnCancelled:
                                            // The user clicked the cancel button. No message necessary.
                                            break;
                                        case FailureReason.AccountPasswordReset:
                                            // Note that we DON'T recommend that the activation link is
                                            // displayed to the user. It is displayed here to facilitate testing.
                                            Session["Message"] = string.Format("Password recovery successful. An activation email was sent which includes the following activation link: {0}. ({1})",
                                                                               result.ActivationLinkUri,
                                                                               result.Reason.ToString());
                                            break;
                                        case FailureReason.ValidationFailed:
                                            Session["Message"] = string.Format("Password recovery unsuccessful. ({0})",
                                                                               result.Reason.ToString());
                                            break;
                                        case FailureReason.AccountBlocked:
                                            Session["Message"] = string.Format("Account has been blocked for this application. ({0}) {1}{2}{3}",
                                                                               result.Reason.ToString(),
                                                                               result.IdentityIdSpecified ? " IdentityId=" + result.IdentityId.ToString() : string.Empty,
                                                                               !String.IsNullOrEmpty(result.DisplayName) ? ", DisplayName=" + result.DisplayName : string.Empty,
                                                                               !String.IsNullOrEmpty(result.EmailAddress) ? ", EmailAddress=" + result.EmailAddress : string.Empty);
                                            break;
                                        case FailureReason.AccountExpired:
                                            Session["Message"] = string.Format("Account has expired. ({0}) {1}{2}{3}",
                                                                               result.Reason.ToString(),
                                                                               result.IdentityIdSpecified ? " IdentityId=" + result.IdentityId.ToString() : string.Empty,
                                                                               !String.IsNullOrEmpty(result.DisplayName) ? ", DisplayName=" + result.DisplayName : string.Empty,
                                                                               !String.IsNullOrEmpty(result.EmailAddress) ? ", EmailAddress=" + result.EmailAddress : string.Empty);
                                            break;
                                        case FailureReason.AccountHardLocked:
                                            Session["Message"] = string.Format("Account has been hard locked and must be unlocked by an administrator. ({0}) {1}{2}{3}",
                                                                               result.Reason.ToString(),
                                                                               result.IdentityIdSpecified ? " IdentityId=" + result.IdentityId.ToString() : string.Empty,
                                                                               !String.IsNullOrEmpty(result.DisplayName) ? ", DisplayName=" + result.DisplayName : string.Empty,
                                                                               !String.IsNullOrEmpty(result.EmailAddress) ? ", EmailAddress=" + result.EmailAddress : string.Empty);
                                            break;
                                        case FailureReason.AccountSoftLocked:
                                            Session["Message"] = string.Format("Account has been temporarily locked. ({0}) {1}{2}{3}",
                                                                               result.Reason.ToString(),
                                                                               result.IdentityIdSpecified ? " IdentityId=" + result.IdentityId.ToString() : string.Empty,
                                                                               !String.IsNullOrEmpty(result.DisplayName) ? ", DisplayName=" + result.DisplayName : string.Empty,
                                                                               !String.IsNullOrEmpty(result.EmailAddress) ? ", EmailAddress=" + result.EmailAddress : string.Empty);
                                            break;
                                        case FailureReason.AccountNotActivated:
                                            Session["Message"] = string.Format("Account is not activated. ({0}) {1}{2}{3}",
                                                                               result.Reason.ToString(),
                                                                               result.IdentityIdSpecified ? " IdentityId=" + result.IdentityId.ToString() : string.Empty,
                                                                               !String.IsNullOrEmpty(result.DisplayName) ? ", DisplayName=" + result.DisplayName : string.Empty,
                                                                               !String.IsNullOrEmpty(result.EmailAddress) ? ", EmailAddress=" + result.EmailAddress : string.Empty);
                                            break;
                                        case FailureReason.AccountNotRegistered:
                                            Session["Message"] = string.Format("Account exists but is not registered for this application. ({0}) {1}{2}{3}",
                                                                               result.Reason.ToString(),
                                                                               result.IdentityIdSpecified ? " IdentityId=" + result.IdentityId.ToString() : string.Empty,
                                                                               !String.IsNullOrEmpty(result.DisplayName) ? ", DisplayName=" + result.DisplayName : string.Empty,
                                                                               !String.IsNullOrEmpty(result.EmailAddress) ? ", EmailAddress=" + result.EmailAddress : string.Empty);
                                            break;
                                        case FailureReason.SessionExpired:
                                            Session["Message"] = string.Format("The sign-in page expired. ({0})", result.Reason.ToString());
                                            break;
                                        case FailureReason.UnknownAccount:
                                        case FailureReason.ProtocolViolation:
                                        case FailureReason.SignOnAttemptNotFound:
                                            Session["Message"] = string.Format("An error occured during sign-in. ({0})", result.Reason.ToString());
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.EndSignOnAttempt(). {1}", ex.GetType().FullName, ex.Message);
                            }
                        }
                    }
                }
            }

            if (SSOSession.HasSession)
            {
                Response.Redirect("~");
            }
            else
            {
                Response.Redirect("~/SignIn.aspx");
            }
        }
    }
}