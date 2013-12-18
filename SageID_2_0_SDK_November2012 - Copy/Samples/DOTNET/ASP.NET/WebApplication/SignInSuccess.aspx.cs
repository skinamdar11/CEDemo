using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageSSO.ServiceReferences;
using System.Xml;
using System.Text;
using SSOSchema = Sage.Obsidian.Shared.Schema;

namespace WebApplication
{
    public partial class SignInSuccess : System.Web.UI.Page
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

                                    // Make a web service call to retrieve the result of the
                                    // sign-on attempt.
                                    EndSignOnAttemptResponse response = client.EndSignOnAttempt(request);

                                    SuccessResult result = (SuccessResult)response.Item;

                                    // Extract the user identity ID from the authentication token. This code
                                    // extract shows how to do this using XQuery:

                                    XmlDocument authenticationToken = new XmlDocument();
                                    authenticationToken.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(result.UserAuthenticationToken)));

                                    XmlNamespaceManager nsMgr = new XmlNamespaceManager(authenticationToken.NameTable);
                                    nsMgr.AddNamespace("sso", "http://sso.sage.com");

                                    Guid userIdentityId = new Guid(authenticationToken.SelectSingleNode("sso:AuthenticationToken/sso:Subject/sso:UserPrincipal/sso:Id", nsMgr).InnerText);

                                    // This code shows how to do the same with the optional .NET SSO client
                                    // support library:

                                    //SSOSchema.AuthenticationToken authenticationToken = Base64Helper<SSOSchema.AuthenticationToken>.FromBase64Xml(result.UserAuthenticationToken);
                                    //Guid userIdentityId = (authenticationToken.Subject.Item as SageSSOSchema.UserPrincipal).Id;

                                    // As of Sage ID 1.2, the user IdentityId is available without having to decode the UserAuthenticationToken
                                    // userIdentityId = result.IdentityId;

                                    SSOSession.Start(result.SessionId,
                                                     result.SessionExpiry,
                                                     result.EmailAddress,
                                                     result.DisplayName,
                                                     result.IdentityId,
                                                     response.Culture,
                                                     result.UserAuthenticationToken);
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