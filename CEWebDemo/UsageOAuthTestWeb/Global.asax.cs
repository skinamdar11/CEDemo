using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SageNA.CE.UM.Client;

namespace SageNA.CE.MvcApplicationSignOnDemo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Add user management routes
            UMWebApiConfig.Register(GlobalConfiguration.Configuration);
            UMRouteConfig.RegisterRoutes(RouteTable.Routes);
            // Add UMBundleConfig.RegisterBundles in future version 
            // Add UMBundleConfig.RegisterGlobalFilters in future version
        }
    }
}