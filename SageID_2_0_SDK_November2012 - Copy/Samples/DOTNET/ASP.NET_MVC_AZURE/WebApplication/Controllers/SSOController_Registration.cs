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
        public ActionResult RegisterNew()
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
                        // between a success result and a failure result. RegisterNewResult() illustrates
                        // how to do this.
                        SuccessUri = RegisterNewResultUri,
                        FailureUri = RegisterNewResultUri,

                        CancelAllowed = true,
                        SignOnAfterSuccess = true,

                        // The user will be signed on following a successful registration, so we need to
                        // specify the application session length.
                        SessionLengthMinutes = HttpContext.Session.Timeout,
                        SessionLengthMinutesSpecified = true,
                    };

                    response = client.StartNewUserRegistrationAttempt(request);
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartNewUserRegistrationAttempt(). {1}", ex.GetType().FullName, ex.Message));

                return SignUpPageRedirect;
            }

            // The registration attempt ID is placed in the session to act as a "marker" that
            // a registration attempt is in progress. It's used later to avoid a call to
            // EndNewUserRegistrationAttempt() if no registration attempt is in progress.

            Session["NewUserRegistrationAttempt"] = response.RegistrationAttemptId;

            return Redirect(response.RedirectUri);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult RegisterNewResult(string resultId)
        {
            // Don't do any processing if an attempt was not
            // in progress.
            if (Session["NewUserRegistrationAttempt"] != null)
            {
                Session["NewUserRegistrationAttempt"] = null;

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
                            EndRegistrationAttemptRequest request = new EndRegistrationAttemptRequest();
                            request.ResultId = resultIdGuid;

                            // Make a web service call to SSO to retrieve the result of the
                            // registration attempt.
                            EndRegistrationAttemptResponse response = client.EndNewUserRegistrationAttempt(request);

                            if (response.Item is RegistrationSuccessResult)
                            {
                                RegistrationSuccessResult registrationResult = (RegistrationSuccessResult)response.Item;
                                SuccessResult signOnResult = (SuccessResult)registrationResult.Item;

                                // A web application can specify that a user should be automatically signed-on following
                                // a successful registration. If this is the case, the sign-on is processed
                                // in the same way as a sign-on without registration.
                                SSOSchema.AuthenticationToken authenticationToken = Base64Helper<SSOSchema.AuthenticationToken>.FromBase64Xml(signOnResult.UserAuthenticationToken);
                                Guid userIdentityId = (authenticationToken.Subject.Item as SSOSchema.UserPrincipal).Id;

                                SSOSession.Start(signOnResult.SessionId,
                                                 signOnResult.SessionExpiry,
                                                 signOnResult.EmailAddress,
                                                 signOnResult.DisplayName,
                                                 userIdentityId,
                                                 response.Culture,
                                                 signOnResult.UserAuthenticationToken);

                                Session["Message"] = string.Format("Registration successful. An activation email was sent to {0} and includes the following activation link: {1}",
                                                                   registrationResult.EmailAddress,
                                                                   registrationResult.ActivationLinkUri);
                            }
                            else
                            {
                                RegistrationFailedResult registrationResult = (RegistrationFailedResult)response.Item;

                                if (registrationResult.Reason != RegistrationFailureReason.RegistrationCancelled)
                                {
                                    Session["Message"] = string.Format("Registration unsuccessful. Reason: {0}",
                                                                       registrationResult.Reason.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.EndNewUserRegistrationAttempt(). {1}", ex.GetType().FullName, ex.Message);
                    }
                }
            }

            if (SSOSession.HasSession)
            {
                return AppPageRedirect;
            }
            else
            {
                return SignUpPageRedirect;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult RegisterExisting()
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
                        // between a success result and a failure result. RegisterExistingResult() illustrates
                        // how to do this.
                        SuccessUri = RegisterExistingResultUri,
                        FailureUri = RegisterExistingResultUri,

                        CancelAllowed = true,

                        // The user will be signed on following a successful registration, so we need to
                        // specify the application session length.
                        SessionLengthMinutes = HttpContext.Session.Timeout,
                        SessionLengthMinutesSpecified = true,
                    };

                    response = client.StartExistingUserRegistrationAttempt(request);
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = HttpUtility.HtmlEncode(string.Format("Exception of type {0} raised when calling WebSSOService.StartExistingUserRegistrationAttempt(). {1}", ex.GetType().FullName, ex.Message));

                return SignUpPageRedirect;
            }

            // The registration attempt ID is placed in the session to act as a "marker" that
            // a registration attempt is in progress. It's used later to avoid a call to
            // EndNewUserRegistrationAttempt() if no registration attempt is in progress.

            Session["ExistingUserRegistrationAttempt"] = response.RegistrationAttemptId;

            return Redirect(response.RedirectUri);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult RegisterExistingResult(string resultId)
        {
            // Don't do any processing if an attempt was not
            // in progress.
            if (Session["ExistingUserRegistrationAttempt"] != null)
            {
                Session["ExistingUserRegistrationAttempt"] = null;

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
                            EndRegistrationAttemptRequest request = new EndRegistrationAttemptRequest();
                            request.ResultId = resultIdGuid;

                            // Make a web service call to SSO to retrieve the result of the
                            // registration attempt.
                            EndRegistrationAttemptResponse response = client.EndExistingUserRegistrationAttempt(request);

                            if (response.Item is RegistrationSuccessResult)
                            {
                                // The registration attempt was successful.
                                RegistrationSuccessResult registrationResult = (RegistrationSuccessResult)response.Item;
                                SuccessResult signOnResult = (SuccessResult)registrationResult.Item;

                                // An existing user is signed-on automatically following a successful registration.
                                // The sign-on is processed in the same way as a sign-on without registration.
                                SSOSchema.AuthenticationToken authenticationToken = Base64Helper<SSOSchema.AuthenticationToken>.FromBase64Xml(signOnResult.UserAuthenticationToken);
                                Guid userIdentityId = (authenticationToken.Subject.Item as SSOSchema.UserPrincipal).Id;

                                SSOSession.Start(signOnResult.SessionId,
                                                 signOnResult.SessionExpiry,
                                                 signOnResult.EmailAddress,
                                                 signOnResult.DisplayName,
                                                 userIdentityId,
                                                 response.Culture,
                                                 signOnResult.UserAuthenticationToken);

                                Session["Message"] = "Registration successful.";
                            }
                            else
                            {
                                // The registration attempt was unsuccessful.
                                RegistrationFailedResult registrationResult = (RegistrationFailedResult)response.Item;

                                if (registrationResult.Reason != RegistrationFailureReason.RegistrationCancelled)
                                {
                                    Session["Message"] = string.Format("Registration unsuccessful. Reason: {0}",
                                                                       registrationResult.Reason.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.EndExistingUserRegistrationAttempt(). {1}", ex.GetType().FullName, ex.Message);
                    }
                }
            }

            if (SSOSession.HasSession)
            {
                return AppPageRedirect;
            }
            else
            {
                return SignUpPageRedirect;
            }
        }
    }
}
