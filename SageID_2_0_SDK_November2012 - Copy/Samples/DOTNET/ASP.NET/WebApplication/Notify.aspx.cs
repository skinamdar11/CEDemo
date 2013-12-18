using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web.Caching;
using System.Xml;
using SageSSO.ServiceReferences;

namespace WebApplication
{
    public partial class Notify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.HttpMethod == "POST")
                {
                    byte[] buffer = new byte[Request.InputStream.Length];
                    Request.InputStream.Read(buffer, 0, buffer.Length);

                    // Notification data is supplied in Base64 format, to avoid digital signature encoding issues
                    string data = Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(buffer)));

                    // Load the notification data into an XmlDocument
                    XmlDocument notification = new XmlDocument();
                    notification.LoadXml(data);

                    // Items in the notification XML are in the Sage SSO namespace, so we need to add an
                    // XmlNamespaceManager here. See the schema documentation for Notification for more details on
                    // the notification XML format.
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(notification.NameTable);
                    nsmgr.AddNamespace("sso", "http://sso.sage.com");

                    CheckNotificationValidSignature(notification);

                    CheckNotificationNotExpired(notification, nsmgr);

                    CheckNotificationNotReplayed(notification, nsmgr);

                    // This XPath expression will only return an XmlNode if the notification type node contains "Session.Ended" and there
                    // is a parameter with the name "SessionId".
                    XmlNode sessionIdNode = notification.SelectSingleNode("/sso:Notification[sso:Type=\"Session.Ended\"]/sso:Parameters/sso:Parameter[sso:Name=\"SessionId\"]/sso:Value", nsmgr);

                    if (sessionIdNode != null)
                    {
                        // Remove the SSO session from the session map. The next time a page is loaded for that SSO session ID,
                        // the map is checked and, if the session is not found the application session is ended.
                        SSOSessionMap.EndSSOSession(new Guid(sessionIdNode.InnerText));
                    }

                    // This XPath expression will only return an XmlNode if the notification type node contains "Session.ExpiryDue" and there
                    // is a parameter with the name "SessionId".
                    sessionIdNode = notification.SelectSingleNode("/sso:Notification[sso:Type=\"Session.ExpiryDue\"]/sso:Parameters/sso:Parameter[sso:Name=\"SessionId\"]/sso:Value", nsmgr);

                    if (sessionIdNode != null)
                    {
                        // The session will expire shortly. We need to determine whether we need to
                        // extend the session in Sage SSO and act accordingly.
                        Guid ssoSessionId = new Guid(sessionIdNode.InnerText);

                        if (SSOSessionMap.HasSSOSession(ssoSessionId))
                        {
                            // This XPath expression returns the Timestamp parameter value in the notification.
                            XmlNode ssoSessionExpiryDueNode = notification.SelectSingleNode("/sso:Notification/sso:Parameters/sso:Parameter[sso:Name=\"Timestamp\"]/sso:Value", nsmgr);

                            // The timestamp, like all Sage SSO timestamps, is in XSD schema format and is UTC.
                            DateTime ssoSessionExpiryDue = XmlConvert.ToDateTime(ssoSessionExpiryDueNode.InnerText, XmlDateTimeSerializationMode.RoundtripKind);

                            DateTime newSSOSessionExpiry = DateTime.MinValue;

                            // If there has been some user activity on this session we'll call Sage SSO to extend the
                            // SSO session to match the application session expiry time based on the last activity
                            // and the application session timeout setting.
                            if (SSOSessionMap.ShouldExtendSSOSession(ssoSessionId, ssoSessionExpiryDue, out newSSOSessionExpiry))
                            {
                                using (WebSSOServiceSoapClient client = new WebSSOServiceSoapClient("WebSSOServiceSoapClient"))
                                {
                                    client.Open();

                                    SessionExtendRequest request = new SessionExtendRequest()
                                    {
                                        SessionId = ssoSessionId,
                                        SessionExpiry = newSSOSessionExpiry,
                                        SessionExpirySpecified = true
                                    };

                                    try
                                    {
                                        // Call Sage SSO to extend the SSO session to match our application session
                                        SessionExtendResponse response = client.SessionExtend(request);

                                        // Extend the SSO session in the session map to synchronise with what Sage SSO
                                        // tells us. It's important to observe the session expiry that's returned
                                        // because the Sage SSO session may be nearing its hard timeout and we
                                        // don't want to extend the application session beyond that.
                                        SSOSessionMap.ExtendSSOSession(ssoSessionId, response.SessionExpiry);

                                        // The ExpiryDue flag tells us whether this session is in its expiry period. If
                                        // ExpiryDue returns true at this point either we haven't extended the session far enough
                                        // past the expiry due threshold to cause Sage SSO to unmark the session (perhaps
                                        // because there was some activity right at the start of the application session but
                                        // nothing since) or the SSO session is approaching its hard timeout.
                                        //
                                        // If the flag is false then we'll receive another notification before the session expires,
                                        // if it's true then we WON'T receive another notification before the session expires and
                                        // we must call Sage SSO to extend the session if there's any activity.
                                        SSOSessionMap.SetShouldExtendSSOSessionOnUserActivity(ssoSessionId, response.ExpiryDue);
                                    }
                                    catch (Exception)
                                    {
                                        // There was a problem extending the session on Sage SSO, or some other local problem
                                        // with the SSO session map or application session state. The safest thing to do
                                        // at this point is to end the application session.
                                        SSOSessionMap.EndSSOSession(ssoSessionId);
                                    }
                                }
                            }
                            else
                            {
                                // We're not extending the SSO session right now but we mark the mapped SSO session 
                                // as requiring extension on user activity. If the user makes a call into the
                                // application while the session is marked, we call Sage SSO to extend the session and un-mark
                                // the application session. You can see this code in PageBase.cs in the OnLoad() method.
                                // The application will receive another notification when the Sage SSO session is next due to expire.
                                SSOSessionMap.SetShouldExtendSSOSessionOnUserActivity(ssoSessionId, true);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // We catch all exceptions here so we don't return a 500 Internal Server Error to Sage SSO.
                // In a production system, it would be a good idea to log these.
            }
        }

        private void CheckNotificationValidSignature(XmlDocument notification)
        {
            // Check the signature of the notification using the SSO Root certificate (which should be installed
            // into the LocalMachine / Root certificate store). If the notification wasn't signed by the SSO Root
            // certificate, it didn't come from Sage SSO so in that case we'll ignore it.
            SignedXml signedXml = new SignedXml(notification);

            XmlNodeList nodeList = notification.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

            if (nodeList.Count != 1) throw new Exception("Notification was not signed.");

            signedXml.LoadXml((XmlElement)nodeList[0]);

            if (!signedXml.CheckSignature(SSORootCertificate, true)) throw new Exception("Invalid notification signature.");
        }

        private XmlNode CheckNotificationNotExpired(XmlDocument notification, XmlNamespaceManager nsmgr)
        {
            // We can now check that the Notification has an Issued timestamp, that it is in a valid
            // format and that it is recent.
            XmlNode issuedNode = notification.SelectSingleNode("/sso:Notification/sso:Issued", nsmgr);

            if (issuedNode == null) throw new Exception("Notification contains no Issued element.");

            DateTime issued = DateTime.MinValue;

            if (!DateTime.TryParse(issuedNode.InnerText, out issued)) throw new Exception("Notification issue time is invalid.");

            // Notifications should be delivered within a few seconds of issue, depending on the overall load of
            // your application and Sage SSO. If the notification is more than ten seconds old, discard it.
            if (issued > DateTime.Now + TimeSpan.FromSeconds(10)) throw new Exception("Notification has expired.");

            return issuedNode;
        }

        private void CheckNotificationNotReplayed(XmlDocument notification, XmlNamespaceManager nsmgr)
        {
            // Even given the expiry check, there's a small window of opportunity for a replay attack. It's best to hold a cache of
            // received notification IDs so, if a notification is replayed before it's considered expired, we can detect
            // it. On a load-balanced, multi-node application, it's best to use a caching service such as memcached or velocity
            // to manage this. For the purposes of this application, however, we'll just use the ASP.NET cache.
            XmlNode idNode = notification.SelectSingleNode("/sso:Notification/sso:NotificationId", nsmgr);

            if (idNode == null) throw new Exception("Notification contains no NotificationId element.");

            Guid id = Guid.Empty;

            try
            {
                id = new Guid(idNode.InnerText);
            }
            catch (Exception)
            {
                throw new Exception("NotificationId is invalid.");
            }

            if (IsReplayedId(id)) throw new Exception("Notification has been replayed.");
        }

        private bool IsReplayedId(Guid id)
        {
            bool replayed = false;

            string idKey = string.Format(CachedIdFormat, id.ToString());

            if (Cache.Get(idKey) == null)
            {
                Cache.Add(idKey,                                  // The Notification ID
                          string.Empty,                           // Something that isn't null for the above if statement
                          null,
                          DateTime.Now + TimeSpan.FromMinutes(1), // Hold for one minute - the notification expiry check will
                    // deal with replayed notifications after that time
                          Cache.NoSlidingExpiration,
                          CacheItemPriority.Default,
                          null);
            }
            else
            {
                replayed = true;
            }

            return replayed;
        }

        private static X509Certificate2 SSORootCertificate
        {
            get
            {
                if (_ssoRootCertificate == null)
                {
                    X509FindType findType = (X509FindType)Enum.Parse(typeof(X509FindType), ConfigurationManager.AppSettings["SSORootCertificateFindType"], true);
                    string findValue = ConfigurationManager.AppSettings["SSORootCertificateFindValue"];

                    X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    certStore.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                    try
                    {
                        _ssoRootCertificate = certStore.Certificates.Find(findType, findValue, true)[0];
                    }
                    finally
                    {
                        certStore.Close();
                    }
                }

                return _ssoRootCertificate;
            }
        }

        private static X509Certificate2 _ssoRootCertificate = null;

        private const string CachedIdFormat = "NotificationId_{0}";
    }
}