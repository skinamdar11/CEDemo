using System;
using System.Web;
using SageSSO.ServiceReferences;
using System.Configuration;

namespace WebApplication
{
    [Serializable]
    public class SSOSession
    {
        public Guid SSOSessionId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public Guid IdentityId { get; set; }
        public string AuthenticationToken { get; set; }
        public string Culture { get; set; }

        public DateTime Expiry
        {
            get
            {
                return SSOSessionMap.GetSSOSessionExpiry(SSOSessionId);
            }
        }

        // Get the SSOSession associated with the application session (or null if
        // there isn't one).
        public static SSOSession Current
        {
            get
            {
                SSOSession ssoSession = (SSOSession)HttpContext.Current.Session["SSOSession"];

                if (ssoSession != null)
                {
                    if (!SSOSessionMap.HasSSOSession(ssoSession.SSOSessionId))
                    {
                        ssoSession.End();
                        ssoSession = null;
                    }
                }

                return ssoSession;
            }
        }

        public static bool HasSession
        {
            get
            {
                return Current != null;
            }
        }

        // Check for the existence of an SSO session, record the activity and extend it by calling SSO, if needed. Refresh
        // should be called each time a page is loaded (or if any Ajax service is called). Returns true if the SSO
        // session was successfully refreshed.
        public static bool Refresh()
        {
            bool refreshed = false;

            if (HasSession)
            {
                Guid ssoSessionId = Current.SSOSessionId;

                if (SSOSessionMap.ShouldExtendSSOSessionOnUserActivity(ssoSessionId))
                {
                    // The Sage SSO session associated with this application session has been marked expiry due since
                    // the last time there was activity. Call Sage SSO to extend the session and then clear the
                    // mark so that we don't call SessionExtend again until we receive another Session.ExpiryDue
                    // notification.

                    using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                    {
                        client.Open();

                        SessionExtendRequest request = new SessionExtendRequest()
                        {
                            SessionId = ssoSessionId,
                            SessionExpiry = DateTime.UtcNow + TimeSpan.FromMinutes(HttpContext.Current.Session.Timeout),
                            SessionExpirySpecified = true
                        };

                        try
                        {
                            // Call Sage SSO to extend the session to match our application session.
                            SessionExtendResponse response = client.SessionExtend(request);
                            Current.AuthenticationToken = response.UserAuthenticationToken;

                            // Extend the SSO session in the session map to synchronise with what Sage SSO
                            // tells us. It's important to observe the session expiry that's returned
                            // because the Sage SSO session may be nearing its hard timeout and we
                            // don't want to extend the application session beyond that.
                            SSOSessionMap.ExtendSSOSession(ssoSessionId, response.SessionExpiry);

                            // Unlike when we handle the Session.ExpiryDue notification, we always clear
                            // the expiry due flag here to avoid making continual calls to Sage SSO
                            // if the SSO session is nearing the hard timeout.
                            SSOSessionMap.SetShouldExtendSSOSessionOnUserActivity(ssoSessionId, false);

                            refreshed = true;
                        }
                        catch (Exception)
                        {
                            // There was a problem extending the session on Sage SSO, or some other local problem
                            // with the SSO session map or application session state. The safest thing to do
                            // at this point is to end the application session.
                            Current.End();
                        }
                    }
                }
                else
                {
                    try
                    {
                        // Record the activity in the SSO session map.
                        SSOSessionMap.RefreshSSOSession(ssoSessionId);

                        refreshed = true;
                    }
                    catch (Exception)
                    {
                        // There was a problem refreshing the SSO session in the SSO session map. The safest thing
                        // to do at this point is to end the application session.
                        Current.End();
                    }
                }
            }

            return refreshed;
        }

        // Associate the SSO session with the application session and create a session map entry
        // for it.
        public static void Start(Guid ssoSessionId,
                                 DateTime ssoSessionExpiry,
                                 string emailAddress,
                                 string displayName,
                                 Guid identityId,
                                 string culture,
                                 string authenticationToken)
        {
            SSOSession ssoSession = new SSOSession()
            {
                EmailAddress = emailAddress,
                IdentityId = identityId,
                DisplayName = displayName,
                Culture = culture,
                SSOSessionId = ssoSessionId,
                AuthenticationToken = authenticationToken
            };

            HttpContext.Current.Session["SSOSession"] = ssoSession;

            SSOSessionMap.AddSSOSession(ssoSessionId, ssoSessionExpiry, HttpContext.Current.Session.SessionID);
        }

        // Disassociate the SSO session from the applications session.
        public void End()
        {
            // Note that this removes the SSOSession object from the application session. It does not
            // end the application session (which must be done separately).
            HttpContext.Current.Session["SSOSession"] = null;
        }

    }
}
