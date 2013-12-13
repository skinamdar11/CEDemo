using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CEDataService;
using SageNA.CE.UM.Client;
using SageNA.CE.UMClientInterface;
using System.Net;
using System.Net.Http;

namespace SageNA.CE.MvcApplicationSignOnDemo.Controllers
{
    public class ApiDemoController : ApiController
    {
        UMOAuth _OAuthIF;

        public ApiDemoController()
        {
            _OAuthIF = new UMOAuth();
        }

        /// <summary>
        /// Demonstrates how to use the OAuth internface in the client library
        /// </summary>
        /// <returns>List of tenant Guids</returns>
        public IEnumerable<string> Get()
        {
            UserSignOnResult authResult = _OAuthIF.GetOAuthInfo(HttpContext.Current.Request, false);
            if (authResult.IsValid)
            {
                return authResult.User.AppUserAccessInfo.Select(ai => ai.TenantInfo.TenantId.ToString());
            }
            else
            {
                throw new HttpResponseException(_OAuthIF.GetDefaultErrorResponse(authResult));
            }
        }

        // POST 
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT 
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE 
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}