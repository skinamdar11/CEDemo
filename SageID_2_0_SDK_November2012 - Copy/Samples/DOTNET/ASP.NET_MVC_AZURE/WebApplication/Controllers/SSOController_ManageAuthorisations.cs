using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SageSSO.ServiceReferences;

namespace WebApplication.Controllers
{
    public partial class SSOController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult ManageAuthorisations()
        {
            StartManageAuthorisationAttemptResponse response;
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
                        SuccessUri = ManageAuthorisationsResultUri,
                        FailureUri = ManageAuthorisationsResultUri,

                        CancelAllowed = true
                    };

                    response = client.StartManageAuthorisationAttempt(request);
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartManageAuthorisationAttempt(). {1}", ex.GetType().FullName, ex.Message));

                return SignInPageRedirect;
            }

            // The change password attempt ID is placed in the session to act as a "marker" that
            // a manage authorisations attempt is in progress. It's used later to avoid a call to
            // EndManageAuthorisationsAttempt() if no authorisation management attempt is in progress.

            Session["ManageAuthorisationsAttempt"] = response.ManageAuthorisationAttemptId;

            return Redirect(response.RedirectUri);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ManageAuthorisationsResult(string resultId)
        {
            // Don't do any processing if an attempt was not
            // in progress.
            if (Session["ManageAuthorisationsAttempt"] != null)
            {
                Session["ManageAuthorisationsAttempt"] = null;

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
                            EndManageAuthorisationAttemptRequest request = new EndManageAuthorisationAttemptRequest();
                            request.ResultId = resultIdGuid;

                            // Make a web service call to SSO to retrieve the result of the
                            // authorisation maanagement.
                            EndManageAuthorisationAttemptResponse response = client.EndManageAuthorisationAttempt(request);

                            if (response.Item is ManageAuthorisationSuccessResult)
                            {
                                // The password change was successful.
                                ManageAuthorisationSuccessResult result = (ManageAuthorisationSuccessResult)response.Item;

                                Session["Message"] = "Authorisation management successful.";
                            }
                            else
                            {
                                // The password change was not successful.
                                ManageAuthorisationFailedResult result = (ManageAuthorisationFailedResult)response.Item;

                                if (result.Reason != ManageAuthorisationFailureReason.ManageAuthorisationCancelled)
                                {
                                    Session["Message"] = string.Format("Manage Authorisation failed. Reason: {0}",
                                                                       result.Reason.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.EndManageAuthorisationAttempt(). {1}", ex.GetType().FullName, ex.Message);
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
