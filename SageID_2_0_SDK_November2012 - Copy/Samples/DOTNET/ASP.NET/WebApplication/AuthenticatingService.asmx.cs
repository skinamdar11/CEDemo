namespace WebApplication
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Web;
    using System.Web.Services;
    using System.Xml;
    using System.Xml.Serialization;

    using Sage.Obsidian.Shared.Schema;

    /// <summary>
    /// Summary description for AuthenticatingService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class AuthenticatingService : WebService
    {
        #region Constants and Fields

        private readonly string EncryptionKey = ConfigurationManager.AppSettings["AccessTokenEncryptionKey"];

        private readonly string InitialisationVector = ConfigurationManager.AppSettings["AccessTokenEncryptionInitialisationVector"];

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
            string token = null;
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

        [WebMethod]
        public string HelloWorld()
        {
            // retrieve HTML Authorization header
            string authorizationHeader = HttpContext.Current.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return "Fail - Invalid authorisation header";
            }

            // Authorization scheme must be Bearer
            if (!authorizationHeader.StartsWith("Bearer "))
            {
                return "Fail - Invalid Authorisation scheme specified.";
            }

            // get the token from the header, ignore the first bit i.e. "Bearer "
            string tokenBase64Encoded = authorizationHeader.Substring(7);

            byte[] clearTextToken;
            try
            {
                clearTextToken = DecryptToken(Convert.FromBase64String(tokenBase64Encoded), EncryptionKey, InitialisationVector);
            }
            catch (Exception e)
            {
                return "Fail - Can't decrypt your token. Exception: " + e.Message;
            }

            // deserialize the token to a .net object
            string errorMessage;
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
                return "Token is expired, ValidTo: " + token.Scope.Validity.NotValidAfter;
            }

            // check the token encapsulates the required permission
            bool containsPriv = (from p in token.Scope.Permissions
                                 where 
                                 (p.Resource.StartsWith("SSO/") || p.Resource.StartsWith("/test/resource")) && //Make sure privelege contains your application name
                                 p.Action != string.Empty   //Make sure action is correct for your application e.g "READ", "ACCESS_ACCOUNT", etc...
                                 select p).Any();

            if (!containsPriv)
            {
                return "The token does not contain the required privilege.";
            }

            // do your service work
            return "Success!";
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
                    errorMessage = "Fail - token could not be deserialised. Exception: " + e.Message;
                    return null;
                }
            }
        }

        #endregion
    }
}