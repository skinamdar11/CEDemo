using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sage.Authorisation;
using System.Configuration;
using System.Net;
using System.IO;
using Sage.Authorisation.Client;

namespace DesktopApplication
{
    public partial class Form1 : Form
    {
        private IAuthorisationResult authResult = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartAuth_Click(object sender, EventArgs e)
        {
            ObtainAuthorisation();
        }

        private void btnUseToken_Click(object sender, EventArgs e)
        {
            if (authResult == null)
            {
                MessageBox.Show("Authorisation needs to be obtained first.");
            }

            HttpWebResponse response = UseAccessToken();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                //Token expired, get another and try again.  This behaviour is
                //defined by the resource server but a 401 is expected if they
                //follow the oauth bearer token specification 
                //http://tools.ietf.org/html/draft-ietf-oauth-v2-bearer-08
            {
                // clear the existing result
                authResult = null;
                ObtainAuthorisation();
                response = UseAccessToken();
            }

            if (response.StatusCode != HttpStatusCode.OK)
                //Some other failure occurred. This behaviour is defined by the
                //resource server.
            {
                MessageBox.Show(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }
            else
                //Success response. This format and behaviour of a successful response is
                //defined by the resource server.
            {
                MessageBox.Show(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }
        }

        private HttpWebResponse UseAccessToken()
        {
            //Call resource server according to oauth bearer token specification.
            //Exact calls to resource server are dependant on resource server 
            //implementation.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ResourceAppRestUrl"]);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + authResult.AccessToken);

            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                return (HttpWebResponse)we.Response;
            }
        }

        private void ObtainAuthorisation()
        {
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string scope = ConfigurationManager.AppSettings["Scope"];

            if (authResult == null)
            {
                using (OAuthClient client = new OAuthClient(clientId))
                {
                    try
                    {
                        //If you have problems connecting try setting the IWebProxy
                        //in the call to client.Authorise below.
                        authResult = client.Authorise(new AuthorisationInfo() { Scope = scope });

                        //Allow tokens to be used if token was returned.
                        btnUseToken.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btnCleanup_Click(object sender, EventArgs e)
        {
            string clientId = ConfigurationManager.AppSettings["ClientId"];

            //Clean out any cached credentials/tokens.
            using (OAuthClient client = new OAuthClient(clientId))
            {
                client.CleanUp();
            }

            authResult = null;
        }

        private void btnStartAuthAttemptAsync_Click(object sender, EventArgs e)
        {
            ObtainAuthorisationAsync();
        }

        private void ObtainAuthorisationAsync()
        {
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string scope = ConfigurationManager.AppSettings["Scope"];

            if (authResult == null)
            {
                OAuthClient client = new OAuthClient(clientId);

                client.AuthoriseCompleted += new OAuthEvents.AuthoriseCompletedHandler(client_AuthoriseCompleted);

                try
                {
                    //If you have problems connecting try setting the IWebProxy
                    //in the call to client.Authorise below.
                    string authId = client.BeginAuthorise(new AuthorisationInfo() { Scope = scope });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void client_AuthoriseCompleted(IOAuth client, IAuthorisationResult authoriseResult)
        {
            if (authoriseResult.Success)
            {
                authResult = authoriseResult;

                //Allow tokens to be used if token was returned.
                btnUseToken.Enabled = true;
            }
            else
            {
                MessageBox.Show(authoriseResult.Error);
            }

            if (client != null)
            {
                client.Dispose();
            }
        }
    }
}
