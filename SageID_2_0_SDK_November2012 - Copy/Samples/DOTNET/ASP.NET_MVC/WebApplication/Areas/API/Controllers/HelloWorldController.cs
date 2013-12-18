using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Areas.API.Controllers
{
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Xml;
    using System.Xml.Serialization;

    using Sage.Obsidian.Shared.Schema;

    public class HelloWorldController : Controller
    {
        //
        // GET: /API/HelloWorld/
        public ActionResult Index()
        {
            return Content(HelloWorld());
        }

        #region Constants and Fields

        private readonly string EncryptionKey = ConfigurationManager.AppSettings["AccessTokenEncryptionKey"];

        private readonly string InitialisationVector = ConfigurationManager.AppSettings["AccessTokenEncryptionInitialisationVector"];

        private readonly string APIResource = ConfigurationManager.AppSettings["APIResource"];

        private readonly string APIAction = ConfigurationManager.AppSettings["APIAction"];

        private static X509Certificate2 oauthRootCertificate;

        #endregion

        #region Properties

        private static X509Certificate2 OAuthCertificate
        {
            get
            {
                if (oauthRootCertificate == null)
                {
                    X509FindType findType =
                        (X509FindType)Enum.Parse(typeof(X509FindType), ConfigurationManager.AppSettings["OAuthRootCertificateFindType"], true);
                    string findValue = ConfigurationManager.AppSettings["OAuthRootCertificateFindValue"];

                    X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    certStore.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                    try
                    {
                        oauthRootCertificate = certStore.Certificates.Find(findType, findValue, true)[0];
                    }
                    finally
                    {
                        certStore.Close();
                    }
                }

                return oauthRootCertificate;
            }
        }

        #endregion

        #region Public Methods

        public byte[] DecryptToken(byte[] cipherToken, string key, string iv)
        {
            using (SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create("AES"))
            {
                algorithm.Mode = CipherMode.CBC;
                algorithm.Padding = PaddingMode.PKCS7;

                algorithm.Key = Convert.FromBase64String(key);
                algorithm.IV = Convert.FromBase64String(iv);

                byte[] plainData = null;

                using (ICryptoTransform transform = algorithm.CreateDecryptor())
                {
                    plainData = transform.TransformFinalBlock(cipherToken, 0, cipherToken.Length);
                }

                return plainData;
            }
        }

        public string HelloWorld()
        {
            string errorMessage;
            // retrieve HTML Authorization header
            string authorizationToken = this.GetAuthorizationToken(out errorMessage);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }


            if (string.IsNullOrEmpty(authorizationToken))
            {
                return "Fail - Invalid authorisation header";
            }

            // Authorization scheme must be Bearer
            if (Request.HttpMethod == "GET" &&
                authorizationToken.StartsWith("Bearer ") == false)
            {
                return "Fail - Invalid Authorisation scheme specified.";
            }

            // strip "Bearer " from the token if it was received from the header
            if (authorizationToken.StartsWith("Bearer "))
            {
                authorizationToken = authorizationToken.Substring(7);
            }
            
            byte[] clearTextToken;
            try
            {
                clearTextToken = DecryptToken(Convert.FromBase64String(authorizationToken), EncryptionKey, InitialisationVector);
            }
            catch (Exception e)
            {
                /****************
                 * WARINING
                 * Simply returning exception messages from decryption frameworks can leave you 
                 * vulnerable to security threats, for example the padding oracle attack. http://en.wikipedia.org/wiki/Padding_oracle_attack
                 * 
                 * DO NOT DO THIS IN PRODUCTION SOFTWARE
                 ****************/
                return "Fail - Can't decrypt your token. Exception: " + e.Message;
            }

            // deserialize the token to a .net object
            AuthenticationToken token = GetAuthenticationToken(clearTextToken, out errorMessage);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }

            // confirm that token subject is a valid user
            UserPrincipal subject = token.Subject.Item as UserPrincipal;
            if (subject == null)
            {
                return "Fail - Token subject is not a UserPrinciple";
            }
            // TODO: validate the user is allowed to call this app

            // confirm that the application which was issued the token is valid
            SecretkeyPrincipal secretkeyPrincipal = token.Scope.Bearer.Item as SecretkeyPrincipal;
            X509Principal x509Principal = token.Scope.Bearer.Item as X509Principal;
            if (secretkeyPrincipal == null && x509Principal == null)
            {
                return "Fail - Token bearer is not a valid principle";
            }

            //VALIDATE secretKeyPrincipal or X509Principal here!
            //Make sure that the bearer is correct and expected.

            // confirm the token is still within it's specified validity period
            if (token.Scope.Validity.NotValidBefore > DateTime.UtcNow)
            {
                return "Token is not yet valid, ValidFrom: " + token.Scope.Validity.NotValidBefore;
            }
            if (token.Scope.Validity.NotValidAfter < DateTime.UtcNow)
            {
                HttpContext.Response.StatusCode = 401;
                return "Token is expired, ValidTo: " + token.Scope.Validity.NotValidAfter;
            }

            // check that the token contains the permission that we're expecting
            bool containsPriv = (from   p in token.Scope.Permissions
                                 where  StringComparer.CurrentCultureIgnoreCase.Compare(p.Resource, APIResource) == 0
                                 &&     StringComparer.CurrentCultureIgnoreCase.Compare(p.Action, APIAction) == 0
                                 select p).Any();

            if (!containsPriv)
            {
                return "The token does not contain the required privilege.";
            }

            // do your service work
            return "Success!";
        }

        private string GetAuthorizationToken(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (Request.Headers.AllKeys.Contains("Authorization"))
            {
                return this.HttpContext.Request.Headers["Authorization"];
            }

            if (Request.HttpMethod == "GET")
            {
                errorMessage = "FAIL - HTTP GET requests only support auth tokens in the Authorization header";
                return null;
            }

            if (Request.ContentType != "application/x-www-form-urlencoded")
            {
                errorMessage = "Fail - Tokens in the request body can only be accepted when the ContentType is application/x-www-form-urlencoded";
                return null;
            }
            
            var body = new StreamReader(Request.InputStream).ReadToEnd();
            
            var parameters = HttpUtility.ParseQueryString(body);
            if (!parameters.AllKeys.Contains("access_token"))
            {
                errorMessage = "Fail - access_token parameter missing from body";
                return null;
            }

            return parameters["access_token"];
        }

        #endregion

        #region Methods

        private static void CheckNotificationValidSignature(XmlDocument notification)
        {
            // Check the signature of the notification using the SSO Root certificate. 
            // If the notification wasn't signed by the SSO Root
            // certificate, it didn't come from Sage SSO so in that case we'll ignore it.
            SignedXml signedXml = new SignedXml(notification);

            XmlNodeList nodeList = notification.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

            if (nodeList.Count != 1)
            {
                throw new Exception("Token was not signed.");
            }

            signedXml.LoadXml((XmlElement)nodeList[0]);

            if (!signedXml.CheckSignature(OAuthCertificate, true))
            {
                throw new Exception("Invalid token signature.");
            }
        }

        private static AuthenticationToken GetAuthenticationToken(byte[] clearTextToken, out string errorMessage)
        {
            errorMessage = string.Empty;

            XmlDocument notification = new XmlDocument();
            using (var stream = new MemoryStream(clearTextToken))
            {
                XmlReader reader = XmlReader.Create(stream);

                // Load the notification data into an XmlDocument
                notification.Load(reader);
            }

            try
            {
                CheckNotificationValidSignature(notification);
            }
            catch (Exception e)
            {
                errorMessage = "Fail - Problem with token signature. Exception: " + e.Message;
                return null;
            }

            using (var stream = new MemoryStream(clearTextToken))
            {
                try
                {
                    stream.Position = 0;
                    XmlSerializer serializer = new XmlSerializer(typeof(AuthenticationToken));
                    AuthenticationToken token = serializer.Deserialize(stream) as AuthenticationToken;

                    return token;
                }
                catch (Exception e)
                {
                    errorMessage = "Fail - Deserialisation failed. Exception: " + e.Message;
                    return null;
                }
            }
        }

        #endregion

    }
}
