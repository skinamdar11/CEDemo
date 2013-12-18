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
        public ActionResult Activation(string resultId)
        {
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
                        ActivationLinkContextRequest request = new ActivationLinkContextRequest();
                        request.ActivationId = resultIdGuid;

                        // Make a web service call to SSO to get the context which is associated with
                        // this ID.
                        ActivationLinkContextResponse response = client.ActivationLinkContext(request);

                        // For convenience, we'll load the context items into a dictionary
                        Dictionary<string, string> contextItems = new Dictionary<string, string>();
                        foreach (ActivationLinkContextItem item in response.Items)
                        {
                            contextItems.Add(item.Name, item.Value);
                        }

                        if (contextItems["Handler"] == "WebSSO/Registration")
                        {
                            // The user clicked on an account activation link

                            if (contextItems["Result"] == "Activated")
                            {
                                // The account was activated
                                Session["Message"] = string.Format("Account activated for user identity {0}", contextItems["IdentityId"]);
                            }
                            else
                            {
                                // The activation link was expired (either too old or already used)
                                Session["Message"] = string.Format("The activation link was expired.");
                            }
                        }
                        else if (contextItems["Handler"] == "WebSSO/PasswordActivation")
                        {
                            // The user clicked on a password activation link

                            if (contextItems["Result"] == "PasswordChanged")
                            {
                                // The password was changed
                                Session["Message"] = string.Format("Password changed for user identity {0}", contextItems["IdentityId"]);
                            }
                            else
                            {
                                // The activation link was expired (either too old or already used)
                                Session["Message"] = string.Format("The activation link was expired.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session["Message"] = string.Format("Exception of type {0} raised when calling WebSSOService.ActivationLinkContext(). {1}", ex.GetType().FullName, ex.Message);
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
