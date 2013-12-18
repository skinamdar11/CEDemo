using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using OAuthClientWebApp.DataAccess;
using System.Net;
using System.Runtime.Serialization.Json;
using OAuthClientWebApp.DataContracts;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace OAuthClientWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() { TryInitialSetup(); return View("Index"); }

        private void TryInitialSetup()
        {
            if (Session["LoggedInUserId"]  == null)
            {
                Session["LoggedInUserId"] = Guid.NewGuid();
            }
        }

        public ActionResult Error() { return View(); }

        /// <summary>
        /// Starts an authorisation attempt for WebAppA as the resource app
        /// </summary>
        /// <returns></returns>
        public ActionResult StartAuthorisationA()
        {
            return StartAuthorisation(Settings.WebAppAClientId, Settings.WebAppAScope, true);
        }

        /// <summary>
        /// Starts a new authorisation attempt for WebAppB as the resource app
        /// </summary>
        /// <returns></returns>
        public ActionResult StartAuthorisationB()
        {
            return StartAuthorisation(Settings.WebAppBClientId, Settings.WebAppBScope, false);
        }

        /// <summary>
        /// Starts a new authorisation attempt.
        /// </summary>
        public ActionResult StartAuthorisation(string clientId, string scope, bool forWebAppA)
        {
            //Record the fact that the current user is performing an authorisation.
            //We'll need to know this when the Authorisation server notifies the
            //client application of success/failure.
            AuthorisationAttempt attempt = CreateAuthAttempt();
            attempt.ForWebAppA = forWebAppA;
            DataAccessLayer.PersistAuthorisationAttempt(attempt);

            //Construct Uri to authorisation server.
            //Explanations of each parameter can be found in the OAuth specification.
            string startAuthUri = Settings.StartAuthorisationAttemptUrl +
                "?response_type=code" +
                "&client_id=" + HttpUtility.UrlEncode(clientId) +
                "&redirect_uri=" + HttpUtility.UrlEncode(Settings.RedirectUri) +
                "&state=" + HttpUtility.UrlEncode(attempt.AttemptIdentifier) +
                "&scope=" + HttpUtility.UrlEncode(scope);

            return new RedirectResult(startAuthUri);
        }

        /// <summary>
        /// Creates an AuthAttempt structure to hold information
        /// about the current session/auth attempt/user.
        /// </summary>
        private AuthorisationAttempt CreateAuthAttempt()
        {
            AuthorisationAttempt attempt = new AuthorisationAttempt();

            //Construct random string to identify this authorisation attempt
            //The authorisation server will echo this back to us so we can 
            //tie an authorisation attempt with a session/user.
            byte[] buffer = new byte[32];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(buffer);

            string stateIdentifier = Convert.ToBase64String(buffer);

            attempt.AttemptIdentifier = stateIdentifier;
            attempt.UserId = GetLoggedInUserId();

            return attempt;
        }

        /// <summary>
        /// Returns a unique identifier for the logged in user.
        /// </summary>
        private Guid GetLoggedInUserId()
        {
            //Authorisation attempts should be restricted to only being allowed to 
            //be performed by users that are logged in using SageId.
            //This sample doesn't have the concept of a logged-in-user but a real
            //client application would.

            //In a real application this is the point where the unique user identifier
            //would be retrieved.

            if (Session["LoggedInUserId"] == null) { Session["LoggedInUserId"] = Guid.NewGuid(); }

            return (Guid)Session["LoggedInUserId"]; //For illustrative purposes, return a fake user id.
        }

        /// <summary>
        /// Invoked by the SageId calling back into this application with a successful
        /// authorisation attempt.
        /// </summary>
        /// <param name="code">
        /// The authorisation code.
        /// </param>
        /// <param name="state">
        /// The state that was originally supplied to the authorisation server when a
        /// new authorisation attempt was started
        /// </param>
        private ActionResult AuthoriseSuccess(string code, string state)
        {
            //Get the attempt back out of state so we can associate the request with a user.
            AuthorisationAttempt attempt = DataAccessLayer.GetAuthorisationAttempt(state);

            //Clean up 
            DataAccessLayer.RemoveAuthorisationAttempt(state);

            //Now we've got an authorisation code we can try and swap it for
            //an access token/refresh token combo.

            //Construct request (POST) to authorisation server
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Settings.GetAccessTokenUrl);
            request.Method = "POST";

            //Use secret key for authorisation
            string secretKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}"
                , (attempt.ForWebAppA ? Settings.WebAppAClientId : Settings.WebAppBClientId)
                , (attempt.ForWebAppA ? Settings.WebAppASecretKey : Settings.WebAppBSecretKey))));

            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + secretKey);

            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            //Add parameters to body of the request.
            byte[] requestParameters = System.Text.Encoding.UTF8.GetBytes(
                "grant_type=authorization_code" +
                "&code=" + HttpUtility.UrlEncode(code) +
                "&redirect_uri=" + HttpUtility.UrlEncode(Settings.RedirectUri));
            
            request.GetRequestStream().Write(requestParameters, 0, requestParameters.Length);

            HttpWebResponse response;
            try
            {
                //Send request
                response = (HttpWebResponse)request.GetResponse();

                //Read response.
                string responseBody = (new StreamReader(response.GetResponseStream())).ReadToEnd();

                //Deserialize successful response.
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GetAccessTokenSuccess));
                GetAccessTokenSuccess successResponse = (GetAccessTokenSuccess)serializer.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseBody)));

                //Client should persist refresh token here.

                //Tie the authorisation attempt back to the user.
                //Cache the tokens somewhere so that we can use them to make
                //calls against the resource.
                Session["LoggedInUserId"] = attempt.UserId;

                if (attempt.ForWebAppA)
                {
                    Session["WebAppATokens"] = successResponse;
                }
                else
                {
                    Session["WebAppBTokens"] = successResponse;
                }
                
                return Index();
            }
            catch (WebException we)
            {
                //We have to catch WebException because the authorisation server
                //will return a 400 response for an unsuccessful call.

                response = (HttpWebResponse)we.Response;

                string responseBody = (new StreamReader(response.GetResponseStream())).ReadToEnd();

                //Read the failure response
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GetAccessTokenFailure));
                GetAccessTokenFailure failureResponse = (GetAccessTokenFailure)serializer.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseBody)));

                //Show some error message.
                ViewData["Error"] = failureResponse.Error;
                ViewData["ErrorDescription"] = failureResponse.ErrorDescription;

                return View("Error");
            }
            catch (Exception e)
            {
                //Unexpected error.
                ViewData["Error"] = e.Message;
                ViewData["ErrorDescription"] = string.Empty;

                return View("Error");
            }
        }

        /// <summary>
        /// Invoked by the SageId authorisation server when an authorisation attempt failed.
        /// </summary>
        /// <param name="error">
        /// The OAuth defined error code
        /// </param>
        /// <param name="errorDescription">
        /// The SageId defined error description (sub error code).
        /// </param>
        /// <param name="state">
        /// The state that was originally supplied to the authorisation server when a
        /// new authorisation attempt was started
        /// </param>
        private ActionResult AuthoriseFailure(string error, string errorDescription, string state)
        {
            AuthorisationAttempt attempt = DataAccessLayer.GetAuthorisationAttempt(state);

            if (attempt != null)
            {
                ViewData["Error"] = error;
                ViewData["ErrorDescription"] = errorDescription;

                //Attempt finished, remove from persistent storage
                DataAccessLayer.RemoveAuthorisationAttempt(state);
            }
            else
            {
                ViewData["Error"] = "Unknown attempt";
                ViewData["ErrorDescription"] = string.Empty;
            }

            return View("Error");
        }

        public void RefreshTokenImpl(bool forWebAppA)
        {
            GetAccessTokenSuccess tokens;
            if (forWebAppA)
            {
                tokens = (GetAccessTokenSuccess)Session["WebAppATokens"];
            }
            else
            {
                tokens = (GetAccessTokenSuccess)Session["WebAppBTokens"];
            }

            if (tokens == null) { return; }

            //If a refresh token wasn't issued then we can't
            //refresh
            if (string.IsNullOrWhiteSpace(tokens.RefreshToken)) { return; }

            //Construct request (POST) to authorisation server
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Settings.GetAccessTokenUrl);
            request.Method = "POST";

            //Use secret key for authorisation
            string secretKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}"
                , (forWebAppA ? Settings.WebAppAClientId : Settings.WebAppBClientId)
                , (forWebAppA ? Settings.WebAppASecretKey : Settings.WebAppBSecretKey))));

            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + secretKey);

            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            //Add parameters to body of the request.
            byte[] requestParameters = System.Text.Encoding.UTF8.GetBytes(
                "grant_type=refresh_token" +
                "&refresh_token=" + HttpUtility.UrlEncode(tokens.RefreshToken));

            request.GetRequestStream().Write(requestParameters, 0, requestParameters.Length);

            HttpWebResponse response;
            try
            {
                //Send request
                response = (HttpWebResponse)request.GetResponse();

                //Read response.
                string responseBody = (new StreamReader(response.GetResponseStream())).ReadToEnd();

                //Deserialize successful response.
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GetAccessTokenSuccess));
                GetAccessTokenSuccess successResponse = (GetAccessTokenSuccess)serializer.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseBody)));

                //Client should persist new tokens here.

                //Cache the tokens somewhere so that we can use them to make
                //calls against the resource.
                if (forWebAppA)
                {
                    Session["WebAppATokens"] = successResponse;
                }
                else
                {
                    Session["WebAppBTokens"] = successResponse;
                }
            }
            catch (WebException we)
            {
                //We have to catch WebException because the authorisation server
                //will return a 400 response for an unsuccessful call.

                response = (HttpWebResponse)we.Response;

                string responseBody = (new StreamReader(response.GetResponseStream())).ReadToEnd();

                //Read the failure response
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GetAccessTokenFailure));
                GetAccessTokenFailure failureResponse = (GetAccessTokenFailure)serializer.ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseBody)));

                //Show some error message.
                ViewData["Error"] = failureResponse.Error;
                ViewData["ErrorDescription"] = failureResponse.ErrorDescription;
            }
            catch (Exception e)
            {
                //Unexpected error.
                ViewData["Error"] = e.Message;
                ViewData["ErrorDescription"] = string.Empty;
            }
        }

        /// <summary>
        /// Refreshes an access token for a new token using the refresh token.
        /// </summary>
        public ActionResult RefreshTokens()
        {
            RefreshTokenImpl(true);
            RefreshTokenImpl(false);
            return Index();
        }

        /// <summary>
        /// Invoked by the SageID authorisation server at the end of an authorisation attempt.
        /// </summary>
        public ActionResult Authorise(string code, string state, string error, string error_description)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                return AuthoriseFailure(error, error_description, state);
            }
            else
            {
                return AuthoriseSuccess(code, state);
            }
        }

        /// <summary>
        /// Make a call to a resource application using the access token via SOAP
        /// </summary>
        public ActionResult UseAccessTokenSoap()
        {
            //Make a SOAP call to web app a using access token.
            ResourceAppSoap.AuthenticatingServiceSoapClient client = new ResourceAppSoap.AuthenticatingServiceSoapClient(
                "AuthenticatingServiceSoap",
                Settings.ResourceApplicationSoapUrl);

            GetAccessTokenSuccess tokens = (GetAccessTokenSuccess)Session["WebAppATokens"];
            if (tokens == null) { return Index(); }

            using (var scope = new OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                request.Headers["Authorization"] = "Bearer " + tokens.AccessToken;

                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

                try
                {
                    string result =client.HelloWorld();

                    Session["AccessTokenSoapResponse"] = "Call to SOAP made result: " + HttpUtility.HtmlEncode(result);
                }
                catch (Exception e)
                {
                    Session["AccessTokenSoapResponse"] = HttpUtility.HtmlEncode(e.Message);
                }
            }

            return Index();
        }

        /// <summary>
        /// Make a call to a resource application using REST and authorisation using an http header
        /// </summary>
        public ActionResult UseAccessTokenRestHeader()
        {
            GetAccessTokenSuccess tokens = (GetAccessTokenSuccess)Session["WebAppBTokens"];
            if (tokens == null) { return Index(); }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Settings.ResourceApplicationRestUrl);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + tokens.AccessToken);

            HttpWebResponse response;
            try
            {
              response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                response = (HttpWebResponse)we.Response;
            }

            string resultMessage = (new StreamReader(response.GetResponseStream()).ReadToEnd());

            Session["AccessTokenRestHeaderResponse"] = HttpUtility.HtmlEncode(resultMessage);

            return Index();
        }

        /// <summary>
        /// Make a call to a resource application using REST and authorisation using an parameter in the http body.
        /// </summary>
        public ActionResult UseAccessTokenRestBody()
        {
            GetAccessTokenSuccess tokens = (GetAccessTokenSuccess)Session["WebAppBTokens"];
            if (tokens == null) { return Index(); }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Settings.ResourceApplicationRestUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            Stream requestStream = request.GetRequestStream();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("access_token=" + HttpUtility.UrlEncode(tokens.AccessToken));

            requestStream.Write(buffer, 0, buffer.Length);

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                response = (HttpWebResponse)we.Response;
            }

            string resultMessage = (new StreamReader(response.GetResponseStream()).ReadToEnd());

            Session["AccessTokenRestBodyResponse"] = HttpUtility.HtmlEncode(resultMessage);

            return Index();
        }
    }
}
