using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageSSO.ServiceReferences;

namespace WebApplication
{
    public partial class ChangePasswordResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Don't do any processing if an attempt was not
                // in progress.
                if (Session["ChangePasswordAttempt"] != null)
                {
                    Session["ChangePasswordAttempt"] = null;

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
                                    EndPasswordChangeAttemptRequest request = new EndPasswordChangeAttemptRequest();
                                    request.ResultId = resultId;

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