using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using Sage.Authorisation.Server;
using System.Web;
using System.Collections.Specialized;
using Sage.Authorisation;

namespace ClientServerSample.Server
{
    /// <summary>
    /// A simple HTTP REST server that accepts requests from clients to perform an 
    /// authorisation with SageID.
    /// </summary>
    public class Server
    {
        #region Private constants

        /// <summary>
        /// Name of the authorise command. Starts the authorisation process
        /// </summary>
        private const string AUTHORISE_COMMAND = "authorise";
        
        /// <summary>
        /// Name of the finish command, finishes an in-progress authorization
        /// </summary>
        private const string FINISH_AUTHORISE_COMMAND = "finishauthorise";

        /// <summary>
        /// Clean up command. Removes any client credentials and tokens we currently have.
        /// </summary>
        private const string CLEANUP_COMMAND = "cleanup";

        /// <summary>
        /// All supported commands aggregated into a handy list.
        /// </summary>
        private static readonly List<string> COMMANDS = new List<string>() { AUTHORISE_COMMAND, FINISH_AUTHORISE_COMMAND, CLEANUP_COMMAND };

        #endregion

        #region Private members

        /// <summary>
        /// A set of authorisation that are in progress. Keyed on username.
        /// </summary>
        private Dictionary<string, AuthAttempt> _inprogressAuthorisations;

        /// <summary>
        /// Lock to protect _inprogressAuthorisations.
        /// </summary>
        private object _inprogLock;

        #endregion

        #region Private classes

        /// <summary>
        /// Helper class to hold information about an authorisation attempt.
        /// </summary>
        private class AuthAttempt
        {
            /// <summary>
            /// The username of the user that initiated the authorisation attempt
            /// </summary>
            public string Owner { get; set; }

            /// <summary>
            /// The time at which the authorisation was started. Used so we can remove
            /// expired wuthorisation attempts.
            /// </summary>
            public DateTime StartedAtUtc { get; set; }

            /// <summary>
            /// The library object to perform the authorisation for us.
            /// </summary>
            public OAuthClientService AuthService { get; set; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new server that will listen for authorisation requests 
        /// from a client.
        /// </summary>
        public Server()
        {
            Console.WriteLine("Starting Server...");

            _inprogressAuthorisations = new Dictionary<string, AuthAttempt>();
            _inprogLock = new object();

            Timer expiredAuthAttemptsCleaner = new Timer(new TimerCallback(CleanupExpiredAuthorizationAttempts), null, 60000, 60000);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();

            while (true)
            {
                //Block until a request is made
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleRequest), context);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Authorisation attempts can't take longer than a specified amount of time due
        /// to them timing out within SageId.
        /// We'll get rid of any attempts older than 10 minutes.
        /// </summary>
        private void CleanupExpiredAuthorizationAttempts(object state)
        {
            lock (_inprogLock)
            {
                DateTime now = DateTime.UtcNow;
                List<string> attemptsToRemove = new List<string>();

                foreach (KeyValuePair<string, AuthAttempt> kvp in _inprogressAuthorisations)
                {
                    if (kvp.Value.StartedAtUtc < (now - TimeSpan.FromMinutes(10)))
                    {
                        attemptsToRemove.Add(kvp.Key);
                    }
                }

                foreach (string toRemove in attemptsToRemove)
                {
                    _inprogressAuthorisations.Remove(toRemove);
                }
            }
        }

        /// <summary>
        /// Handles an incoming request
        /// </summary>
        private void HandleRequest(object context)
        {
            HttpListenerContext ctx = (HttpListenerContext)context;

            string command = GetCommand(ctx);

            if (command == AUTHORISE_COMMAND)
            {
                HandleAuthorise(ctx);
            }
            else if (command == FINISH_AUTHORISE_COMMAND)
            {
                HandleFinishAuthorise(ctx);
            }
            else if (command == CLEANUP_COMMAND)
            {
                HandleCleanup(ctx);
            }
            else
            {
                SendFailureResponse(ctx, "Unknown command");
            }
        }

        /// <summary>
        /// Handle a request to start an authorisation
        /// </summary>
        private void HandleAuthorise(HttpListenerContext ctx)
        {
            //Check required username parameter is present
            string username;
            if(!TryExtractParameter(ctx.Request.Url, "username", out username))
            {
                SendFailureResponse(ctx, "No username parameter supplied");
            }

            //Normalize
            username = username.ToLower();

            //Try and create an authorisation attempt for this user if they don't already 
            //have an authorisation attempt in flight.
            AuthAttempt attempt = CreateAndAddInProgressAuthorisation(username);
            
            if (attempt == null)
            {
                SendFailureResponse(ctx, string.Format("User '' already has an authorisation in progress", username));
                return;
            }

            OAuthClientService oauthService = attempt.AuthService;

            //Start the authorisation with Sage ID
            try
            {
                IServerAuthorisationResult result = oauthService.StartAuthorisation(new AuthorisationInfo()
                {
                    ResponseType = "code",
                    Scope = Settings.Scope,
                    State = Guid.NewGuid().ToString()
                });

                if (!result.Success)
                {
                    SendFailureResponse(ctx, "Failed to obtain an access token");
                    RetrieveAndRemoveInProgressAuthorisation(username);
                    return;
                }

                if (result.UserAuthorisationRequired)
                {
                    Console.WriteLine(string.Format("User '{0}' is required to interactively grant the authorisation.", username));

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("result=interactive&uri=" + HttpUtility.UrlEncode(result.AuthorisationUri));
                    ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);

                    //Sends the response
                    ctx.Response.Close();
                }
                else
                    //No user interaction required. Access token has been retrieved.
                {
                    //Remove in progress Auth
                    RetrieveAndRemoveInProgressAuthorisation(username);

                    //Use access token here.
                    Console.WriteLine(string.Format("Access token retrieved for user '{0}'", username));

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("result=finished");
                    ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);

                    //Sends the response
                    ctx.Response.Close();
                }

            }
            catch (Exception e)
            {
                SendFailureResponse(ctx, e.Message);
            }
        }

        /// <summary>
        /// Handle a request to finish an authorisation that has been started.
        /// </summary>
        private void HandleFinishAuthorise(HttpListenerContext ctx)
        {
            //Check required parameters are present
            string username;
            if (!TryExtractParameter(ctx.Request.Url, "username", out username))
            {
                SendFailureResponse(ctx, "No username parameter supplied");
            }

            string authorisationCode;
            if (!TryExtractParameter(ctx.Request.Url, "authorisationCode", out authorisationCode))
            {
                SendFailureResponse(ctx, "No authorisationCode parameter supplied");
            }

            string retrieveCredential;
            if (!TryExtractParameter(ctx.Request.Url, "retrieveCredential", out retrieveCredential))
            {
                SendFailureResponse(ctx, "No retrieveCredential parameter supplied");
            }

            //Normalize
            username = username.ToLower();

            AuthAttempt attempt = RetrieveAndRemoveInProgressAuthorisation(username);

            if (attempt == null)
            {
                SendFailureResponse(ctx, string.Format("Attempt for user '{0}' was not started or has timed out", username));
                return;
            }

            OAuthClientService oauthService = attempt.AuthService;

            try
            {
                IServerAuthorisationResult result = oauthService.FinishAuthorisation(authorisationCode, (retrieveCredential.ToLower() == "true"));

                if (!result.Success)
                {
                    SendFailureResponse(ctx, "Could not obtain access token");
                    return;
                }

                //Use access token here.
                Console.WriteLine(string.Format("Access token retrieved for user '{0}'", username));

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("result=finished");
                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);

                ctx.Response.Close();
            }
            catch (Exception e)
            {
                SendFailureResponse(ctx, e.Message);
            }
        }

        private AuthAttempt CreateAndAddInProgressAuthorisation(string username)
        {
            lock (_inprogLock)
            {
                if (_inprogressAuthorisations.ContainsKey(username))
                {
                    return null;
                }
                else
                {
                    OAuthClientService oauthService = new OAuthClientService(Settings.ClientId, username);
                    AuthAttempt attempt = new AuthAttempt()
                    {
                        AuthService = oauthService,
                        Owner = username,
                        StartedAtUtc = DateTime.UtcNow
                    };
                    _inprogressAuthorisations.Add(username, attempt);

                    return attempt;
                }
            }
        }

        private AuthAttempt RetrieveAndRemoveInProgressAuthorisation(string username)
        {
            lock (_inprogLock)
            {
                if (_inprogressAuthorisations.ContainsKey(username))
                {
                    AuthAttempt attempt = _inprogressAuthorisations[username];
                    _inprogressAuthorisations.Remove(username);

                    return attempt;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Cleans up any client credentials and access/refresh tokens for all
        /// users.  
        /// </summary>
        private void HandleCleanup(HttpListenerContext ctx)
        {
            lock (_inprogLock)
            {
                if (_inprogressAuthorisations.Count == 0)
                {
                    OAuthClientService oauthService = new OAuthClientService();
                    oauthService.ClientId = Settings.ClientId;
                    oauthService.Cleanup();

                    ctx.Response.Close();

                    return;
                }
            }
            
            //if we've gotten this far then we couldnt clean up because there 
            //are auths in progress.
            SendFailureResponse(ctx, "Cleanup failed, authorisations in progress");
        }

        /// <summary>
        /// Get the command name from the request
        /// </summary>
        private string GetCommand(HttpListenerContext ctx)
        {
            if (ctx.Request.Url.Segments.Length != 2) { return null; }

            foreach (string command in COMMANDS)
            {
                if (string.Equals(command, ctx.Request.Url.Segments[1], StringComparison.OrdinalIgnoreCase))
                {
                    return command;
                }
            }

            return null;
        }

        /// <summary>
        /// Sends a failure response to the client.
        /// </summary>
        private void SendFailureResponse(HttpListenerContext ctx, string message)
        {
            Console.WriteLine("Failure: " + message);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("result=failure&message=" + HttpUtility.UrlEncode(message));
            ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            ctx.Response.Close();
        }

        /// <summary>
        /// Tries to extract a parameter from a query string
        /// </summary>
        private bool TryExtractParameter(Uri uri, string parameterName, out string parameterValue)
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(uri.Query);

            foreach (string key in nvc.AllKeys)
            {
                if (string.Equals(parameterName, key, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(nvc[key]))
                    {
                        parameterValue = string.Empty;
                        return false;
                    }
                    else
                    {
                        parameterValue = nvc[key];
                        return true;
                    }
                }
            }

            parameterValue = string.Empty;
            return false;
        }

        #endregion
    }
}
