using System;
using System.Web;
using System.Web.Caching;

namespace WebApplication
{
    // The SSOSessionMap class manages information which maps SSO sessions to application sessions.
    //
    // We record:
    //
    // - The SSO session ID
    // - The application session ID
    // - The SSO session expiry time we last received
    // - A flag which tells us whether to call Sage SSO upon user activity
    // - The UTC time of the last activity on an application session
    //
    // The main operations are:
    //
    // - AddSSOSession: Establish a mapping between an SSO session and an application session, so we can
    //                  lookup an application session given an SSO session ID.
    //
    // - ExtendSSOSession: Update the record of the SSO session expiry time for a given SSO session ID.
    //
    // - EndSSOSession: Clear the mapping for an SSO session.
    //
    // - HasSSOSession: Checks for the existence of an SSO session. This check is used to determine whether
    //                  an application session is "authenticated" - the lack of an SSO session mapping
    //                  indicates that the session is not (or is no longer) authenticated.
    //
    // - RefreshSSOSession: Record user activity on the application. We use the last activity time when
    //                      extending the SSO session in response to Session.ExpiryDue to determine if we
    //                      need to call Sage SSO and to calculate the new SSO session expiry which should
    //                      be requested.
    //
    // - ShouldExtendSSOSession: Uses the last activity time and the session expiry time which was sent in the
    //                           Session.ExpiryDue notification to calculate a new SSO session expiry time and
    //                           return a flag that tells us whether we need to call Sage SSO.
    //
    // - ShouldExtendSSOSessionOnUserActivity: Returns true if we should call Sage SSO to extend the SSO session in response
    //                                         to user activity.
    //
    // - SetShouldExtendSSOSessionOnUserActivity: Sets or clears the extend session on user activity flag.

    // This approach is necessary for ASP.Net since the framework doesn't allow us to change session state items for arbitrary
    // sessions - which is what we need to do when we receive the SSO session notifications. Thus, we need something external
    // to session state that we can correlate with application session state via the SSO session ID (which we put in
    // application session state) and which allows us to, for example, in response to a Session.Ended notification
    // cause an application session to end the next time there's activity on the session.
    //
    // If you're not using ASP.Net and your web application framework DOES allows you to alter session state for abritrary
    // sessions, we recommend that you use that mechanism in preference to this approach.
    //
    // This sample uses the ASP.Net cache which is in-process and, like in-process sessions, gets recycled with the IIS
    // application pool. This is fine for demo purposes, but we recommend using an external cache server for real-world
    // deployments. Alternatively, you can use a database. A cache has convenient semantics for us in this case (because
    // we can set data items to expire at a particular time and we don't have to remove them manually) but the same
    // effect can be achieved using a database which has the general advantage of better fault tolerance
    // (especially for long-lived items).

    public class SSOSessionMap
    {
        public static void AddSSOSession(Guid ssoSessionId, DateTime ssoSessionExpiry, string applicationSessionId)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            HttpContext.Current.Cache.Remove(cacheId);

            HttpContext.Current.Cache.Add(cacheId,
                        new MapEntry()
                        {
                            SSOSessionId = ssoSessionId,
                            ApplicationSessionId = applicationSessionId,
                            ExtendOnUserActivity = false,
                            LastActivity = DateTime.UtcNow,
                            SSOSessionExpiry = ssoSessionExpiry
                        },
                        null,
                        ssoSessionExpiry,
                        Cache.NoSlidingExpiration,
                        CacheItemPriority.Default,
                        null);
        }

        public static bool ExtendSSOSession(Guid ssoSessionId, DateTime ssoSessionExpiry)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            MapEntry mapEntry = (MapEntry)HttpContext.Current.Cache.Get(cacheId);

            if (mapEntry != null)
            {
                mapEntry.SSOSessionExpiry = ssoSessionExpiry;

                HttpContext.Current.Cache.Insert(cacheId,
                              mapEntry,
                              null,
                              ssoSessionExpiry,
                              Cache.NoSlidingExpiration,
                              CacheItemPriority.Default,
                              null);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void EndSSOSession(Guid ssoSessionId)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            HttpContext.Current.Cache.Remove(cacheId);
        }

        public static DateTime GetSSOSessionExpiry(Guid ssoSessionId)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            MapEntry mapEntry = (MapEntry)HttpContext.Current.Cache.Get(cacheId);

            if (mapEntry != null)
            {
                return mapEntry.SSOSessionExpiry;
            }
            else
            {
                throw new Exception("No SSO session with specified ID in the cache.");
            }
        }

        public static bool HasSSOSession(Guid ssoSessionId)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            return HttpContext.Current.Cache.Get(cacheId) != null;
        }

        public static void RefreshSSOSession(Guid ssoSessionId)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            MapEntry mapEntry = (MapEntry)HttpContext.Current.Cache.Get(cacheId);

            if (mapEntry != null)
            {
                mapEntry.LastActivity = DateTime.UtcNow;

                HttpContext.Current.Cache.Insert(cacheId,
                              mapEntry,
                              null,
                              mapEntry.SSOSessionExpiry,
                              Cache.NoSlidingExpiration,
                              CacheItemPriority.Default,
                              null);
            }
            else
            {
                throw new Exception("No SSO session with specified ID in the cache.");
            }
        }

        public static bool ShouldExtendSSOSession(Guid ssoSessionId, DateTime ssoSessionExpiryDue, out DateTime newSSOSessionExpiry)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            MapEntry mapEntry = (MapEntry)HttpContext.Current.Cache.Get(cacheId);

            if (mapEntry != null)
            {
                // Calculate the new SSO session expiry time to request by adding our default application
                // session timeout to the last time we had any activity
                newSSOSessionExpiry = mapEntry.LastActivity + DefaultSessionTimeout;

                // Return true if the application session extends past the SSO session expiry
                return newSSOSessionExpiry > ssoSessionExpiryDue;
            }
            else
            {
                throw new Exception("No SSO session with specified ID in the cache.");
            }
        }

        public static bool ShouldExtendSSOSessionOnUserActivity(Guid ssoSessionId)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            MapEntry mapEntry = (MapEntry)HttpContext.Current.Cache.Get(cacheId);

            if (mapEntry != null)
            {
                return mapEntry.ExtendOnUserActivity;
            }
            else
            {
                return false;
            }
        }

        public static void SetShouldExtendSSOSessionOnUserActivity(Guid ssoSessionId, bool expiryDue)
        {
            string cacheId = MapEntry.GetCacheId(ssoSessionId);

            MapEntry mapEntry = (MapEntry)HttpContext.Current.Cache.Get(cacheId);

            if (mapEntry != null)
            {
                mapEntry.ExtendOnUserActivity = expiryDue;

                HttpContext.Current.Cache.Insert(cacheId,
                              mapEntry,
                              null,
                              mapEntry.SSOSessionExpiry,
                              Cache.NoSlidingExpiration,
                              CacheItemPriority.Default,
                              null);
            }
            else
            {
                throw new Exception("No SSO session with specified ID in the cache.");
            }
        }

        public static TimeSpan DefaultSessionTimeout
        {
            get
            {
                return TimeSpan.FromMinutes(HttpContext.Current.Session.Timeout);
            }
        }

        [Serializable]
        private class MapEntry
        {
            internal Guid SSOSessionId { get; set; }
            internal string ApplicationSessionId { get; set; }
            internal DateTime SSOSessionExpiry { get; set; }
            internal bool ExtendOnUserActivity { get; set; }
            internal DateTime LastActivity { get; set; }

            internal static string GetCacheId(Guid ssoSessionId)
            {
                return string.Format(CachedIdFormat, ssoSessionId.ToString());
            }

            private const string CachedIdFormat = "SSOSessionId_{0}";
        }
    }
}