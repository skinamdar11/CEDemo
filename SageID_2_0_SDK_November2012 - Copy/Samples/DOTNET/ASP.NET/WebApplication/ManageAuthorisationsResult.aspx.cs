using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageSSO.ServiceReferences;

namespace WebApplication
{
    public partial class ManageAuthorisationsResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Don't do any processing if an attempt was not
                // in progress.
                if (Session["ManageAuthorisationsAttempt"] != null)
                {
                    Session["ManageAuthorisationsAttempt"] = null;

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
                                    EndManageAuthorisationAttemptRequest request = new EndManageAuthorisationAttemptRequest();
                                    request.ResultId = resultId;

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