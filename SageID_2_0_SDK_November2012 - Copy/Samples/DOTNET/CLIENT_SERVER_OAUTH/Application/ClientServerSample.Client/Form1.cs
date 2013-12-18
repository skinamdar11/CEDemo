using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.IO;
using Sage.Authorisation.Client;
using Sage.Authorisation;

namespace ClientServerSample.Client
{
    public partial class Form1 : Form
    {
        #region Private constants

        /// <summary>
        /// The base address of the server sample
        /// </summary>
        private const string SERVER_BASE_ADDRESS = "http://127.0.0.1:8080/";

        /// <summary>
        /// The uri format pattern for starting an authorisation.
        /// </summary>
        private const string AUTHORISE_URI_FORMAT = SERVER_BASE_ADDRESS + "authorise?username={0}";

        /// <summary>
        /// The uri format pattern for finishing an authorisation
        /// </summary>
        private const string FINISH_AUTHORISE_URI_FORMAT = SERVER_BASE_ADDRESS + "finishauthorise?username={0}&authorisationCode={1}&retrieveCredential={2}";

        /// <summary>
        /// The uri format pattern for performing a cleanup.
        /// </summary>
        private const string CLEANUP_URI_FORMAT = SERVER_BASE_ADDRESS + "cleanup";

        #endregion

        #region Constructors

        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Starts an authorisation using a server component.
        /// </summary>
        private void btnStartAuthorisation_Click(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(
                AUTHORISE_URI_FORMAT,
                HttpUtility.UrlEncode(Environment.UserName)));

            try
            {
                string responseMessage = GetResponseMessage(request);

                string result;
                if (!TryExtractParameter(responseMessage, "result", out result))
                {
                    MessageBox.Show("Expected parameter 'result' was not recieved");
                    return;
                }

                result = result.ToLower();

                if (result == "interactive")
                    //User interaction required.
                {
                    string authorisationUri;
                    if (!TryExtractParameter(responseMessage, "uri", out authorisationUri))
                    {
                        MessageBox.Show("Expected parameter 'uri' was not recieved");
                    }

                    //User a thin Oauth client to perform the UI actions only.
                    OAuthClientThin thinClient = new OAuthClientThin();
                    IAuthorisationResultThin authResult = thinClient.Authorise(authorisationUri, new WebProxySettings()
                    {
                         AutoDetect = true
                    });

                    if (!authResult.Success)
                        //Authorisation failed
                    {
                        MessageBox.Show("Could not obtain authorisation from the user");
                    }
                    else
                        //Authorisation succeeded
                    {
                        //Pass authorisation code back to server to finish off authorisation process.
                        HttpWebRequest finishRequest = (HttpWebRequest)WebRequest.Create(string.Format(
                            FINISH_AUTHORISE_URI_FORMAT,
                            HttpUtility.UrlEncode(Environment.UserName),
                            HttpUtility.UrlEncode(authResult.AuthorisationCode),
                            authResult.RefreshCredential.ToString()));

                        responseMessage = GetResponseMessage(finishRequest);

                        TryExtractParameter(responseMessage, "result", out result);

                        if (result == "finished")
                        {
                            MessageBox.Show("Server obtained tokens from authorisation code");
                        }
                        else if (result == "failure")
                        {
                            string errorMessage;
                            TryExtractParameter(responseMessage, "message", out errorMessage);
                            MessageBox.Show(string.Format("Server experienced a failure during an authorisation attempt. '{0}'", errorMessage));
                        }
                        else
                        {
                            MessageBox.Show("Unknown response from server");
                        }
                        
                    }
                }
                else if (result == "finished")
                    //No user interaction required
                {
                    MessageBox.Show("Server has successfully obtained an access token without requiring user intervention.");
                }
                else if (result == "failure")
                {
                    string errorMessage;
                    TryExtractParameter(responseMessage, "message", out errorMessage);
                    MessageBox.Show(string.Format("Server experienced a failure during an authorisation attempt. '{0}'", errorMessage));
                }
                else
                {
                    MessageBox.Show("Unknown result retrieved from server.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Invokes the cleanup functionality on the server to remove client credentials and 
        /// access/refresh tokens.
        /// </summary>
        private void btnCleanup_Click(object sender, EventArgs e)
        {
            //Pass authorisation code back to server to finish off authorisation process.
            HttpWebRequest finishRequest = (HttpWebRequest)WebRequest.Create(CLEANUP_URI_FORMAT);

            string responseMessage = GetResponseMessage(finishRequest);

            if (string.IsNullOrEmpty(responseMessage))
            {
                MessageBox.Show("Cleanup succeeded.");
            }
            else
            {
                string errorMessage;
                TryExtractParameter(responseMessage, "message", out errorMessage);
                MessageBox.Show(string.Format("Cleanup failed'{0}'", errorMessage));
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Executes the http request and extracts the returned response body.
        /// </summary>
        private string GetResponseMessage(HttpWebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Tries to extract a named parameter from a query string.
        /// </summary>
        private bool TryExtractParameter(String queryString, string parameterName, out string parameterValue)
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(queryString);

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
