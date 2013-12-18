using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;

using Sage.Obsidian.Client.Support;
using SageSSO.ServiceReferences;
using SSOSchema = Sage.Obsidian.Shared.Schema;
using System.Collections.Generic;

namespace WebApplication.Controllers
{
    public partial class SSOController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult ChangePassword()
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
                        // between a success result and a failure result. ChangePasswordResult() illustrates
                        // how to do this.
                        SuccessUri = ChangePasswordResultUri,
                        FailureUri = ChangePasswordResultUri,

                        CancelAllowed = true
                    };

                    response = client.StartPasswordChangeAttempt(request);
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartPasswordChangeAttempt(). {1}", ex.GetType().FullName, ex.Message));

                return SignInPageRedirect;
            }

            // The change password attempt ID is placed in the session to act as a "marker" that
            // a change password attempt is in progress. It's used later to avoid a call to
            // EndChangePasswordAttempt() if no change password attempt is in progress.

            Session["ChangePasswordAttempt"] = response.PasswordChangeAttemptId;

            return Redirect(response.RedirectUri);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ChangePasswordResult(string resultId)
        {
            // Don't do any processing if an attempt was not
            // in progress.
            if (Session["ChangePasswordAttempt"] != null)
            {
                Session["ChangePasswordAttempt"] = null;

                Guid resultIdGuid = Guid.Empty;

                try
                {
                    resultIdGuid = new Guid(resultId);
                }
                catch (Exception)
                {
                    // Ignore failure to parse GUID
                }

                // Don't do any processing if the id parameter is not
                // a valid, non-zero GUID.
                if (resultIdGuid != Guid.Empty)
                {
                    try
                    {
                        using (var client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                        {
                            EndPasswordChangeAttemptRequest request = new EndPasswordChangeAttemptRequest();
                            request.ResultId = resultIdGuid;

                            // Make a web service call to SSO to retrieve the result of the
                            // password change.
                            EndPasswordChangeAttemptResponse response = client.EndPasswordChangeAttempt(request);

                            if (response.Item is PasswordChangeSuccessResult)
                            {
                                // The password change was successful.
                                PasswordChangeSuccessResult result = (PasswordChangeSuccessResult)response.Item;

                                Session["Message"] = "Password change successful.";
                            }
                            else
                            {
                                // The password change was not successful.
                                PasswordChangeFailedResult result = (PasswordChangeFailedResult)response.Item;

                                if (result.Reason != PasswordChangeFailureReason.PasswordChangeCancelled)
                                {
                                    Session["Message"] = string.Format("Password change unsuccessful. Reason: {0}",
                                                                       result.Reason.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.EndPasswordChangeAttempt(). {1}", ex.GetType().FullName, ex.Message);
                    }
                }
            }

            if (SSOSession.HasSession)
            {
                return AppPageRedirect;
            }
            else
            {
                return SignInPageRedirect;
            }
        }
    }
}
