using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageSSO.ServiceReferences;
using SSOSchema = Sage.Obsidian.Shared.Schema;
using Sage.Obsidian.Client.Support;

namespace WebApplication
{
    public partial class SignUpExistingResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Don't do any processing if an attempt was not
                // in progress.
                if (Session["ExistingUserRegistrationAttempt"] != null)
                {
                    Session["ExistingUserRegistrationAttempt"] = null;

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
                                    EndRegistrationAttemptRequest request = new EndRegistrationAttemptRequest();
                                    request.ResultId = resultId;

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