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
        public ActionResult SignOutAll()
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

            Session.Abandon();

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

            return SignInPageSessionEndedRedirect;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult SignOut()
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

            Session.Abandon();

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

            return SignInPageSessionEndedRedirect;
        }
    }
}
