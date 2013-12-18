using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BasicREST
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an HTTP POST request to the Sage Id WebSSOService WebStartSignonAttempt API
            //
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://services.sso.staging.services.sage.com/sso/webssoservice/web/webstartsignonattempt");
            request.Method = "POST";

            // You may need to manually set a proxy and/or proxy credentials - if so, uncomment the following code and
            // modify as necessary
            //
            //WebProxy proxy = new WebProxy("ncl-bluecoat:80");
            //proxy.UseDefaultCredentials = true;
            //request.Proxy = proxy;

            // You must specify your client certificate for each request
            //
            X509Certificate2 clientCert = new X509Certificate2("YourClientCertificate.pfx", "YourClientCertificatePassword");
            request.ClientCertificates.Add(clientCert);

            // The following POST data should match the example on the API help page (https://services.sso.staging.services.sage.com/sso/webssoservice/web/help/operations/webstartsignonattempt#request-xml)
            //
            string postData = "<StartSignOnAttemptRequest><SuccessUri xmlns=\"http://sso.sage.com\">http://example.sage.com/success</SuccessUri><FailureUri xmlns=\"http://sso.sage.com\">http://example.sage.com/failure</FailureUri><CancelAllowed xmlns=\"http://sso.sage.com\">true</CancelAllowed><State xmlns=\"http://sso.sage.com\">Some state</State></StartSignOnAttemptRequest>";

            // The data should use UTF-8 encoding
            //
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);

            // You must set both the content type and content length
            //
            request.ContentType = "text/xml";
            request.ContentLength = postBytes.Length;

            // Write the POST data and close the stream
            //
            request.GetRequestStream().Write(postBytes, 0, postBytes.Length);
            request.GetRequestStream().Close();

            string responseData = null;
            HttpWebResponse response = null;

            try
            {
                // Get the response and read the response data
                //
                response = (HttpWebResponse)request.GetResponse();
                responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();                

                // The response data will contain a URL which can be pasted into a browser to get a Sage Id sign-on 
                // page for this attempt
                //
                Console.WriteLine(responseData);
            }
            catch (WebException ex)
            {
                // A WebException contains additional response information that should be examined
                //
                HandleWebException(ex);
            }
            catch (Exception ex)
            {
                // A generic problem has occurred
                //
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (response != null)
                {
                    // Always ensure that the response stream is closed
                    //
                    response.GetResponseStream().Close();
                }
            }

            Console.ReadLine();
        }

        private static void HandleWebException(WebException ex)
        {
            HttpWebResponse response = null;

            try
            {
                // Get the response from the exception and read the response data
                //
                response = (HttpWebResponse)ex.Response;
                string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // The response data will contain further information on the cause of the failure
                //
                Console.WriteLine(responseData);
            }
            finally
            {
                // Always ensure that the response stream is closed
                //
                response.GetResponseStream().Close();
            }            
        }
    }
}
